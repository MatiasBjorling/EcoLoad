using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EcoManager.Data.Entities;
using EcoManager.Data.Enums;
using EcoManager.Data.Management;
using EcoManager.Shared.Tools;
using NHibernate.Criterion;

namespace EcoManager.Data.Manipulation
{
    public class TableManager
    {
        public string CreateRawTable(int tableId)
        {
            using (UnitOfWork.Start())
            {

                TableInfo ti = UnitOfWork.CurrentSession.Get<TableInfo>(tableId);

                List<TableInfo> tables = new List<TableInfo>() {ti};

                var si = UnitOfWork.CurrentSession.CreateCriteria<SchemaInfo>()
                    .CreateAlias("Tables", "tableInfo")
                    .Add(Restrictions.Eq("tableInfo.Id", tableId))
                    .UniqueResult<SchemaInfo>();

                StringBuilder selects = new StringBuilder();
                StringBuilder joins = new StringBuilder();
                StringBuilder wheres = new StringBuilder();

                selects.Append("SELECT ");
                joins.Append("FROM tableInfo t ");

                string crossApply = "CROSS APPLY storage.nodes('/CsvDataSet/Table') as R(nref) ";

                // Let's create the query))
                foreach (SchemaColumn sc in si.Columns)
                {


                    if (sc.Type == StorageTypes.Text)
                    {
                        selects.Append("nref.value('" + sc.OrigName + "[1]', 'NVARCHAR(MAX)') '" + sc.Name + "', ");

                    }

                    if (sc.Type == StorageTypes.Float)
                    {
                        selects.Append("nref.value('" + sc.OrigName + "[1]', 'float') '" + sc.Name + "', ");
                    }

                    if (sc.Type == StorageTypes.Integer)
                    {
                        selects.Append("nref.value('" + sc.OrigName + "[1]', 'int') '" + sc.Name + "', ");
                    }
                }

                selects.Append("nref.value('_UniqueRowId[1]', 'int') '_UniqueRowId'");

                string finalSQL = string.Format("{0} into #temp1 {1} {2}  WHERE t.id = {3}", selects, joins, crossApply, tableId);

                UnitOfWork.CurrentSession.CreateSQLQuery("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tmpVizStorage]') AND type in (N'U')) DROP TABLE [dbo].[tmpVizStorage]").ExecuteUpdate();
                UnitOfWork.CurrentSession.CreateSQLQuery(finalSQL).ExecuteUpdate();


                selects.Clear();
                wheres.Clear();
                joins.Clear();

                selects.Append("SELECT t1.* ");
                

                foreach (SchemaColumn sc in si.Columns)
                {
                    if (sc.Type == StorageTypes.Temporal)
                    {
                        switch (sc.TemporalType)
                        {
                            case TimeTypes.Point:
                                joins.AppendFormat("JOIN TemporalPoint tp{0} ON t1._UniqueRowId = tp{0}.RowNr ", sc.GroupId);
                                selects.AppendFormat(", tp{0}.TimePoint AS '{1}'", sc.GroupId, sc.Name);
                                wheres.AppendFormat("tp{0}.tableId = {1} AND tp{0}.TemporalGroup = {0}", sc.GroupId, tableId);
                                break;
                            case TimeTypes.Length:
                                joins.AppendFormat("JOIN TemporalLength tl{0} ON t1._UniqueRowId = tp{0}.RowNr ", sc.GroupId);
                                selects.AppendFormat(", tp{0}.TimeLength AS '{1}'", sc.GroupId, sc.Name);
                                wheres.AppendFormat("tp{0}.tableId = {1} AND tp{0}.TemporalGroup = {0}", sc.GroupId, tableId);
                                break;
                            case TimeTypes.Interval:
                                Logger.Message("Does not support tables with intervals.");
                                break;
                        }
                    }
                }

                string secondfinalSQL = string.Format("{0} into tmpVizStorage FROM #temp1 t1 {1}  WHERE {2}", selects, joins, wheres);

                UnitOfWork.CurrentSession.CreateSQLQuery(secondfinalSQL).ExecuteUpdate();
            }
            


            return "tmpVizStorage";
        }
    }
}
