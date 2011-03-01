using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EcoManager.Data.Proxies;
using NHibernate.Validator.Constraints;

namespace EcoManager.Data.Entities.NonRel
{
    public class DatasetHolder : BindableObject
    {
        public List<ImportColumn> Columns { get; set; }
        public Dataset DatasetParent { get; set; }

        private string tableDescription = String.Empty;
        [Length(1, 255, Message = "Please enter a description first.")]
        public string TableDescription
        {
            get { return tableDescription; }
            set { 
                tableDescription = value;
                base.RaisePropertyChanged("TableDescription");
            }
        }

        [NotNull(Message = "Please load a dataset first.")]
        public DataSet StorageTable { get; set; }

        // Used for data provenance
        public TableInfo ParentTable { get; set; }

        public DatasetHolder()
        {
            Columns = new List<ImportColumn>();
        }
    }
}
