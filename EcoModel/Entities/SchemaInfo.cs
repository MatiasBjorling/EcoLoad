using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EcoManager.Data.Entities
{
    public class SchemaInfo
    {
        public virtual int Id { get; set;}
        public virtual SchemaInfo SchemaParent { get; set; }
        public virtual String Name { get; set; }
        public virtual DateTime ValidBegin { get; set; }
        public virtual DateTime? ValidEnd { get; set; }

        public virtual IList<SchemaColumn> Columns { get; set; }
        public virtual IList<TableInfo> Tables { get; set; }

        public SchemaInfo()
        {
            Columns = new List<SchemaColumn>();
            Tables = new List<TableInfo>();
        }
    }
}
