using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using EcoManager.Data.Entities;
using EcoManager.Data.Management;

namespace EcoManager.Forms.ViewModel
{
    public class DataViewerViewModel
    {
        public Window HostWindow { get; set; }

        public DataSet DataSetHolder = new DataSet();

        public DataViewerViewModel(Window hostWindow)
        {
            HostWindow = hostWindow;
        }

        public void Load(int tableId)
        {
            using (UnitOfWork.Start())
            {
                TableInfo ti = UnitOfWork.CurrentSession.Get<TableInfo>(tableId);
                if (ti != null)
                {
                    DataSetHolder.ReadXml(new XmlNodeReader(ti.Storage));

                    foreach (SchemaColumn sc in ti.Schema.Columns)
                        DataSetHolder.Tables[0].Columns[sc.ColOrder].ColumnName = sc.Name;
                } 
                else
                {
                    DataSetHolder.Clear();
                    MessageBox.Show("No information was found at the database id. The database might be inconsistent.");
                }
            }
        }
    }
}
