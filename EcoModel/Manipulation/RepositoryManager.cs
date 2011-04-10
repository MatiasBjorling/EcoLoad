using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Xml;
using EcoManager.Data.Entities;
using EcoManager.Data.Entities.NonRel;
using EcoManager.Data.Entities.NonRel.ImportExport;
using EcoManager.Data.Enums;
using EcoManager.Data.Factories;
using EcoManager.Data.Management;
using EcoManager.Shared.Tools;
using GisSharpBlog.NetTopologySuite.Geometries;
using NHibernate;

namespace EcoManager.Data.Manipulation
{
    public class RepositoryManager
    {
        private class SpatialColumnStructure
        {
            private int latitudeCol = -1;
            public int LatitudeCol
            {
                get { return latitudeCol; }
                set { latitudeCol = value; }
            }

            private int longtitudeCol = -1;
            public int LongtitudeCol
            {
                get { return longtitudeCol; }
                set { longtitudeCol = value; }
            }

            public int GroupId { get; set; }
        }
        private class TemporalColumnStructure
        {
            private int timeBeginCol = -1;
            public int TimeBeginCol
            {
                get { return timeBeginCol; }
                set { timeBeginCol = value; }
            }

            private int timeEndCol = -1;
            public int TimeEndCol
            {
                get { return timeEndCol; }
                set { timeEndCol = value; }
            }

            public String DateFormat { get; set; }

            public TimeTypes TimeType { get; set; }

            public int GroupId { get; set; }
        }

        private DatasetHolder DataHolder { get; set; }

        private UnitOfWorkImpl uowImpl;

        // Ongoing repository information
        public TableInfo TableInformation { get; set; }
        public SchemaInfo SchemaInformation { get; set; }

        public RepositoryManager(DatasetHolder data)
        {
            DataHolder = data;
        }

        public void CreateDataSetTableMetaInformation(WorkEntity workEntity)
        {
            if (uowImpl == null)
                uowImpl = UnitOfWork.Start();

            if (SchemaInformation == null)
                return;

            if (DataHolder.ImportType != ImportType.Append)
            {
                TableInfo ti = new TableInfo();

                ti.TableDescription = DataHolder.TableDescription;
                ti.ValidBegin = DateTime.Now;
                ti.Schema = SchemaInformation;
                ti.Dataset = workEntity.WorkingDataset;

                if (DataHolder.ParentTable != null)
                    ti.Parents.Add(DataHolder.ParentTable);

                UnitOfWork.CurrentSession.Save(ti);
                TableInformation = ti;
            }

            if (workEntity != null)
                workEntity.Completed = true;

        }

        public void CreateDataSetSchemaMetaInformation(WorkEntity workEntity)
        {
            if (uowImpl == null)
                uowImpl = UnitOfWork.Start();

            if (workEntity == null)
                return;

            if (DataHolder.ImportType == ImportType.Append)
            {
                TableInfo ti = UnitOfWork.CurrentSession.Get<TableInfo>(DataHolder.ParentTable.Id);

                TableInformation = ti;
                SchemaInformation = ti.Schema;

                DataHolder.Data = new DataSet();
                DataHolder.Data.ReadXml(new XmlNodeReader(ti.Storage));

                for (int i = 0; i< DataHolder.Columns.Count; i++)
                {
                    ImportColumn ic = DataHolder.Columns[i];

                    bool found = false;
                    foreach (SchemaColumn sc in SchemaInformation.Columns.Where(sc => sc.OrigName == ic.OrigName))
                        found = true;
                    
                    if (!found)
                    {
                        SchemaColumn sc = new SchemaColumn();
                        sc.Name = ic.Name;
                        sc.OrigName = ic.OrigName;
                        sc.Schema = SchemaInformation;
                        sc.Type = ic.StorageType;
                        sc.ValidBegin = DateTime.Now;
                        sc.ColOrder = i+1; // The _UniqueRow is already added.
                        if (!String.IsNullOrEmpty(ic.DateFormat))
                            sc.DateFormat = ic.DateFormat;

                        UnitOfWork.CurrentSession.Save(sc);
                        SchemaInformation.Columns.Add(sc);

                        DataHolder.Data.Tables[0].Columns.Add(sc.OrigName);
                    }
                    // Loop though the new added stuff.
                }
            }
            else
            {
                SchemaInfo schema = new SchemaInfo();
                schema.ValidBegin = DateTime.Now;
                //schema.SchemaParent = latestSchema;
                schema.Name = "Import Demo";

                UnitOfWork.CurrentSession.Save(schema);
                UnitOfWork.CurrentSession.Flush();

                for (int i = 0; i < DataHolder.Columns.Count;i++ )
                {
                    ImportColumn ic = DataHolder.Columns[i];

                    SchemaColumn sc = new SchemaColumn();
                    sc.Name = ic.Name;
                    sc.OrigName = ic.OrigName;
                    sc.Schema = schema;
                    sc.Type = ic.StorageType;
                    sc.ValidBegin = DateTime.Now;
                    sc.ColOrder = i;
                    if (!String.IsNullOrEmpty(ic.DateFormat))
                        sc.DateFormat = ic.DateFormat;
                
                    UnitOfWork.CurrentSession.Save(sc);
                    schema.Columns.Add(sc);
                }

                SchemaInformation = schema;
            }

            workEntity.Completed = true;
        }

        public void LoadData(WorkEntity workEntity)
        {
            if (TableInformation == null)
                return;

            if (DataHolder.ImportType == ImportType.Append)
            {
                // Retrieve the current data set
                // Merge the new dataset with the current data set.
                // Store in the same storage element.

                int lastUniqueId = Int32.Parse(DataHolder.Data.Tables[0].Rows[DataHolder.Data.Tables[0].Rows.Count - 1]["_UniqueRowId"].ToString());

                DataView storageView = DataHolder.StorageTable.Tables[0].DefaultView;
 
               
                foreach (DataRowView drv in storageView)
                {
                    DataRow newRow = DataHolder.Data.Tables[0].NewRow();

                    for (int i=0;i<DataHolder.Data.Tables[0].Columns.Count;i++)
                    {
                        if (DataHolder.StorageTable.Tables[0].Columns.Contains(DataHolder.Data.Tables[0].Columns[i].ColumnName))
                            newRow[DataHolder.Data.Tables[0].Columns[i].ColumnName] =
                                drv[DataHolder.Data.Tables[0].Columns[i].ColumnName];

                    }

                    newRow["_UniqueRowId"] = ++lastUniqueId;
                    DataHolder.Data.Tables[0].Rows.Add(newRow);
                }

                XmlDataDocument xmlDataDocument = new XmlDataDocument(DataHolder.Data);

                TableInformation.Storage = xmlDataDocument;
                UnitOfWork.CurrentSession.SaveOrUpdate(TableInformation);
                
            } 
            else
            {
                // Create unique key.
                DataHolder.StorageTable.Tables[0].Columns.Add("_UniqueRowId");
                int i = 0;
                foreach (DataRow dr in DataHolder.StorageTable.Tables[0].Rows)
                    dr["_UniqueRowId"] = i++;

                XmlDataDocument xmlDataDocument = new XmlDataDocument(DataHolder.StorageTable);

                TableInformation.Storage = xmlDataDocument;
                UnitOfWork.CurrentSession.SaveOrUpdate(TableInformation);
            }
            

            if (workEntity != null)
                workEntity.Completed = true;
        }

        

        public void CreateSpatialSupportData(WorkEntity workEntity)
        {
            if (SchemaInformation == null)
                return;

            if (TableInformation == null)
                return;

            Dictionary<int, SpatialColumnStructure> spatialStructures = GetSpatialStructures();

            if (spatialStructures == null)
                return;


            if (DataHolder.ImportType == ImportType.Append)
            {
                
            } else
            {
                foreach (SpatialColumnStructure c in spatialStructures.Values)
                {
                    int i = 0;
                    foreach (DataRowView dr in DataHolder.StorageTable.Tables[0].AsDataView())
                    {
                        SpatialInfo si = new SpatialInfo
                        {
                            Table = TableInformation,
                            RowNr = i++,
                            SpatialGroup = c.GroupId
                        };

                        Double latitude, longtitude;
                        if (Double.TryParse(dr[c.LatitudeCol].ToString(), out latitude) && Double.TryParse(dr[c.LongtitudeCol].ToString(), out longtitude))
                        {
                            if ((latitude >= -90 && latitude <= 90) || (longtitude >= -90 && longtitude <= 90))
                            {
                                si.Location = new Point(latitude, longtitude) { SRID = 4326 }; // WGS format.
                                UnitOfWork.CurrentSession.Save(si);
                            }
                            else
                            {
                                workEntity.Errors.Add(string.Format("Could not store geographic location. Lat: {0} Long: {1}. Coordinates too small or large. Must be within -90 and 90 degrees.", dr[c.LatitudeCol], dr[c.LongtitudeCol]));
                            }
                        }
                        else
                        {
                            workEntity.Errors.Add("Could not store geographic location. Lat: " + dr[c.LatitudeCol] + " Long: " + dr[c.LongtitudeCol]);
                        }
                    }
                }
            }
            

            if (workEntity != null)
                workEntity.Completed = true;
        }

        public void CreateTemporalSupportData(WorkEntity workEntity)
        {
            if (SchemaInformation == null)
                return;

            if (TableInformation == null)
                return;

            if (DataHolder.ImportType == ImportType.Append)
            {
                UnitOfWork.CurrentSession.CreateSQLQuery("DELETE FROM TemporalPoint WHERE TableId = " + TableInformation.Id).ExecuteUpdate();
                UnitOfWork.CurrentSession.CreateSQLQuery("DELETE FROM TemporalLength WHERE TableId = " + TableInformation.Id).ExecuteUpdate();
                UnitOfWork.CurrentSession.CreateSQLQuery("DELETE FROM TemporalInterval WHERE TableId = " + TableInformation.Id).ExecuteUpdate();


            }

            Dictionary<int, TemporalColumnStructure> temporalStructures = GetTemporalStructures();

            if (temporalStructures == null)
                return;

            CultureInfo provider = CultureInfo.InvariantCulture;


            foreach (TemporalColumnStructure c in temporalStructures.Values)
            {
                int i = 0;
                DataSet ds = DataHolder.StorageTable;
                if (DataHolder.ImportType == ImportType.Append)
                    ds = DataHolder.Data;
                foreach (DataRowView dr in ds.Tables[0].AsDataView())
                {
                    //if (i > 1000)
                    //    break;

                    TemporalBase tb = TemporalFactory.GetNewTemporal(c.TimeType);
                    tb.Table = TableInformation;
                    tb.RowNr = i++;
                    tb.TemporalGroup = c.GroupId;

                    DateTime begin;
                    bool parsed = false;
                    if (String.IsNullOrEmpty(c.DateFormat))
                        parsed = DateTime.TryParse(dr[c.TimeBeginCol].ToString(), null, DateTimeStyles.None, out begin);
                    else
                        parsed = DateTime.TryParseExact(dr[c.TimeBeginCol].ToString(), c.DateFormat, provider, DateTimeStyles.None, out begin);

                    if (parsed)
                    {
                        switch (c.TimeType)
                        {
                            case TimeTypes.Point:

                                ((TemporalPoint)tb).Point = begin;
                                break;
                            case TimeTypes.Interval:
                                ((TemporalInterval)tb).TimeBegin = begin;
                                ((TemporalInterval)tb).TimeEnd =
                                    String.IsNullOrEmpty(c.DateFormat) ?
                                        Convert.ToDateTime(dr[c.TimeEndCol], null) :
                                        DateTime.ParseExact(dr[c.TimeEndCol].ToString(), c.DateFormat, provider);
                                break;
                            case TimeTypes.Length:
                                ((TemporalLength)tb).Length = begin;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        UnitOfWork.CurrentSession.Save(tb);
                    }


                }

                if (c.GroupId == 0)
                {
                    UnitOfWork.CurrentSession.Flush();

                    if (c.TimeType == TimeTypes.Point)
                    {
                        var o = UnitOfWork.CurrentSession.CreateSQLQuery("SELECT MIN(TimePoint) as Minimum, MAX(TimePoint) as Maximum FROM TemporalPoint WHERE tableId = " +
                                                                    TableInformation.Id + " AND TemporalGroup = 0")
                                                                    .AddScalar("Minimum", NHibernateUtil.DateTime2)
                                                                    .AddScalar("Maximum", NHibernateUtil.DateTime2).UniqueResult<object[]>();

                        TableInformation.ValidBegin = (DateTime)o[0]; ;
                        TableInformation.ValidEnd = (DateTime)o[1]; ;
                        UnitOfWork.CurrentSession.Update(TableInformation);
                    }
                    else if (c.TimeType == TimeTypes.Interval)
                    {
                        var o = UnitOfWork.CurrentSession.CreateSQLQuery("SELECT MIN(TimeBegin) as Minimum, MAX(TimeEnd) as Maximum FROM TemporalPoint WHERE tableId = " +
                                                                    TableInformation.Id + " AND TemporalGroup = 0")
                                                                    .AddScalar("Minimum", NHibernateUtil.DateTime2)
                                                                    .AddScalar("Maximum", NHibernateUtil.DateTime2).UniqueResult<object[]>();

                        TableInformation.ValidBegin = (DateTime)o[0]; ;
                        TableInformation.ValidEnd = (DateTime)o[1]; ;
                        UnitOfWork.CurrentSession.Update(TableInformation);
                    }
                }
            }
            



            if (workEntity != null)
                workEntity.Completed = true;
        }

        public void FinalizeWorkFlow(WorkEntity workEntity)
        {
            UnitOfWork.CurrentSession.Transaction.Commit();

            if (workEntity != null)
                workEntity.Completed = true;

            uowImpl.Dispose();
        }

        private Dictionary<int, TemporalColumnStructure> GetTemporalStructures()
        {
            Dictionary<int, TemporalColumnStructure> temporalGroups = new Dictionary<int, TemporalColumnStructure>(4);

            foreach (ImportColumn c in DataHolder.Columns.Where(c=>c.StorageType == StorageTypes.Temporal))
            {
                
                TemporalColumnStructure curGroup;

                // Fetch the group
                if (!temporalGroups.ContainsKey(c.GroupId))
                {
                    curGroup = new TemporalColumnStructure { GroupId = c.GroupId };
                    temporalGroups.Add(c.GroupId, curGroup);
                }

                curGroup = temporalGroups[c.GroupId];

                curGroup.TimeType = c.TemporalType;
                curGroup.DateFormat = c.DateFormat;

                switch (c.TemporalType)
                {
                    case TimeTypes.Length:
                    case TimeTypes.Point:
                        curGroup.TimeBeginCol = c.ColumnNr;
                        break;
                    case TimeTypes.Interval:
                        switch (c.TemporalEndingType)
                        {
                            case TimeEndings.Beginning:
                                curGroup.TimeBeginCol = c.ColumnNr;
                                break;
                            case TimeEndings.Ending:
                                curGroup.TimeEndCol = c.ColumnNr;
                                break;
                            default:
                                return null;
                        }
                        break;
                    default:
                        return null;
                }
            }

            // Check for validity
            if (temporalGroups.Values.Any(c=>c.TimeBeginCol == -1))
            {
                Logger.Error("Missing either column for time instance.");
                return null;
            }

            if (temporalGroups.Values.Any(c => c.TimeType == TimeTypes.Interval && ( c.TimeBeginCol == -1 || c.TimeEndCol == -1)))
            {
                Logger.Error("Missing either start or end column for time interval.");
                return null;
            }
            
            return temporalGroups;
        }


        private Dictionary<int, SpatialColumnStructure> GetSpatialStructures()
        {
            // Find Spatial Groups
            Dictionary<int, SpatialColumnStructure> spatialGroups = new Dictionary<int, SpatialColumnStructure>(4);
            foreach (ImportColumn c in DataHolder.Columns.Where(c => c.StorageType == StorageTypes.Spatial))
            {
                SpatialColumnStructure curGroup;

                // Fetch the group
                if (!spatialGroups.ContainsKey(c.GroupId))
                {
                    curGroup = new SpatialColumnStructure { GroupId = c.GroupId };
                    spatialGroups.Add(c.GroupId, curGroup);
                }

                curGroup = spatialGroups[c.GroupId];

                if (c.SpatialGeoType == GeographyNames.Latitude)
                    curGroup.LatitudeCol = c.ColumnNr;

                if (c.SpatialGeoType == GeographyNames.Longitude)
                    curGroup.LongtitudeCol = c.ColumnNr;
            }

            // Check for validity
            if (spatialGroups.Values.Any(c => c.LatitudeCol == -1 || c.LongtitudeCol == -1))
            {
                Logger.Error("Missing either latitude or longtitude coordinates.");
                return null;
            }

            return spatialGroups;
        }
    }
}
