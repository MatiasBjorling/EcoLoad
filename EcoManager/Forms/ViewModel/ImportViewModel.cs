using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using EcoManager.Data.Entities;
using EcoManager.Data.Entities.NonRel;
using EcoManager.Data.Entities.NonRel.ImportExport;
using EcoManager.Data.Management;
using EcoManager.Data.Manipulation;
using EcoManager.Data.Proxies;
using EcoManager.Shared.Tools;
using LumenWorks.Framework.IO.Csv;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace EcoManager.Forms.ViewModel
{
    public class ImportViewModel : BindableObject, IDisposable
    {
        public UserControl UControl { get; set; }

        private UnitOfWorkImpl uowImpl;


        private string importPath = String.Empty;
        public string ImportPath
        {
            get { return importPath; }
            set { importPath = value; }
        }

        // Sent to workEntities
        public DatasetHolder Data { get; set; }

        // Table Preview and loading
        public Dataset WorkingDataset { get; set; }
        private DataView importDataView;

        private char charDelimiter = ';';
        public char CharDelimiter
        {
            get { return charDelimiter; }
            set { 
                    charDelimiter = value;
                    base.RaisePropertyChanged("CharDelimiter");
                }
        }

        private List<WorkEntity> WorkEntities;


        public ImportViewModel(Dataset dataset)
        {
            Data = new DatasetHolder();
            WorkingDataset = dataset;
        }

        public DataView GridData
        {
            get
            {
                // Cached
                if (importDataView != null)
                    return importDataView;

                DataSet ds = new DataSet("CsvDataSet");
                try
                {

                    using (CachedCsvReader csv = new CachedCsvReader(new StreamReader(importPath), true, CharDelimiter))
                    {
                        DataTable dt = new DataTable("Table");

                        csv.DefaultParseErrorAction = ParseErrorAction.RaiseEvent;
                        csv.ParseError += new EventHandler<ParseErrorEventArgs>(csv_ParseError);
                        string[] headers = csv.GetFieldHeaders();
                        using (UnitOfWork.Start())
                        {
                            SchemaInfo si = null;
                            if (Data.ParentTable != null)
                                si = UnitOfWork.CurrentSession.Get<SchemaInfo>(Data.ParentTable.Schema.Id);

                            for (int i = 0; i < headers.Length; i++)
                            {
                                dt.Columns.Add(headers[i]);

                                ImportColumn ic = new ImportColumn(i, headers[i]);

                                if (Data.ParentTable != null)
                                {
                                    // This is very ugly! Refactor ImportColumn to SchemaColumn.
                                    foreach (SchemaColumn sc in
                                        si.Columns.Where(sc => sc.OrigName == ic.Name))
                                    {
                                        ic.StorageType = sc.Type;
                                        ic.GroupId = sc.GroupId;
                                        ic.SpatialGeoType = sc.SpatialGeoType;
                                        ic.TemporalType = sc.TemporalType;
                                        ic.TemporalEndingType = sc.TemporalEndingType;
                                        ic.DateFormat = sc.DateFormat;
                                    }

                                }

                                Data.Columns.Add(ic);
                            }
                        }

                        DataRow dataRow;
                        foreach (object o in csv)
                        {
                            dataRow = dt.NewRow();
                            if (o != null)
                                dataRow.ItemArray = (string[]) o;

                            dt.Rows.Add(dataRow);

//#if (DEBUG)
//{	
//                        if (dt.Rows.Count > 100)
//                           break;
//}
//#endif

                        }
                        ds.Tables.Add(dt);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Message("Could not load file. Error: " + ex);
                }

                importDataView = ds.Tables[0].DefaultView;

                Data.StorageTable = ds;

                return importDataView;
            }
        }

        void csv_ParseError(object sender, ParseErrorEventArgs e)
        {
            // if the error is that a field is missing, then skip to next line
            if (e.Error is MissingFieldException)
            {
                Logger.Error("--MISSING FIELD ERROR OCCURRED");
                e.Action = ParseErrorAction.AdvanceToNextLine;
            }

        }

        public void SetParentTableFromId(int tableId)
        {
            using (UnitOfWork.Start())
            {
                Data.ParentTable = UnitOfWork.CurrentSession
                        .CreateCriteria<TableInfo>()
                        .CreateAlias("Schema", "schema")
                        .SetFetchMode("schema", FetchMode.Join)
                        .Add(Restrictions.Eq("Id", tableId))
                        .SetProjection(Projections.ProjectionList()
                            .Add(Projections.Property("Id"), "Id")
                            .Add(Projections.Property("TableDescription"), "TableDescription")
                            .Add(Projections.Property("Schema"), "Schema")
                            .Add(Projections.Property("ValidBegin"), "ValidBegin")
                            .Add(Projections.Property("ValidEnd"), "ValidEnd"))
                            .SetResultTransformer(Transformers.AliasToBean(typeof(TableInfo)))
                        .UniqueResult<TableInfo>();
            }
        }
        
        public List<WorkEntity> LoadDataset()
        {
            RepositoryManager rm = new RepositoryManager(Data);

            // Initialize working structure
            WorkEntities = new List<WorkEntity>();

            // Create meta data for schema of table
            WorkEntity workEntity = new WorkEntity();
            workEntity.Name = "Creating meta information for dataset schema";
            workEntity.WorkingDataset = WorkingDataset;
            workEntity.Execute += new WorkEntity.DynamicFunc(rm.CreateDataSetSchemaMetaInformation);
            WorkEntities.Add(workEntity);

            // Create meta data for table
            workEntity = new WorkEntity();
            workEntity.Name = "Creating meta information for dataset.";
            workEntity.WorkingDataset = WorkingDataset;
            workEntity.Execute += new WorkEntity.DynamicFunc(rm.CreateDataSetTableMetaInformation);
            WorkEntities.Add(workEntity);


            // Load the data into the database
            workEntity = new WorkEntity();
            workEntity.Name = "Loading dataset into database";
            workEntity.WorkingDataset = WorkingDataset;
            workEntity.Execute += new WorkEntity.DynamicFunc(rm.LoadData);
            WorkEntities.Add(workEntity);

            // Load the spatial data into the database
            workEntity = new WorkEntity();
            workEntity.Name = "Loading spatial columns into database";
            workEntity.WorkingDataset = WorkingDataset;
            workEntity.Execute += new WorkEntity.DynamicFunc(rm.CreateSpatialSupportData);
            WorkEntities.Add(workEntity);

            // Load the temporal data into the database
            workEntity = new WorkEntity();
            workEntity.Name = "Loading temporal columns into database";
            workEntity.WorkingDataset = WorkingDataset;
            workEntity.Execute += new WorkEntity.DynamicFunc(rm.CreateTemporalSupportData);
            WorkEntities.Add(workEntity);


            // Do any clean up and commit
            workEntity = new WorkEntity();
            workEntity.Name = "Commit changes to database.";
            workEntity.WorkingDataset = WorkingDataset;
            workEntity.Execute += new WorkEntity.DynamicFunc(rm.FinalizeWorkFlow);
            workEntity.CommitEntity = true;
            WorkEntities.Add(workEntity);

            // Write any history information
            //workEntity = new WorkEntity();
            //workEntity.Name = "Store history and provenance information";
            //workEntity.Execute += new WorkEntity.DynamicFunc(rm.CreateDataSetTableMetaInformation);

            return WorkEntities;
        }

        public void Dispose()
        {
            
        }
    }
}
