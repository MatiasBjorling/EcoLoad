using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EcoManager.Data.Entities
{
    public class TableInfo
    {
        public virtual int Id { get; set; }
        public virtual SchemaInfo Schema { get; set; }
        public virtual Dataset Dataset { get; set; }
        public virtual string TableDescription { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime ValidBegin { get; set; }
        public virtual DateTime? ValidEnd { get; set; }
        public virtual XmlDocument Storage { get; set; }

        public virtual IList<TableInfo> Parents { get; set; }
        public virtual IList<TableInfo> Children { get; set; }

        public TableInfo()
        {
            Parents = new List<TableInfo>();
            Children = new List<TableInfo>();
            Created = DateTime.Now;
        }
    }
}
