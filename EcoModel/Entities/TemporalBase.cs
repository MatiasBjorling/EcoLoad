using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EcoManager.Data.Entities
{
    public class TemporalBase
    {
        public virtual TableInfo Table { get; set; }
        public virtual int RowNr { get; set; }
        public virtual int TemporalGroup { get; set; }

        public override bool Equals(object obj)
        {
            SpatialInfo si = obj as SpatialInfo;
            if (si == null)
                return false;

            string s1 = String.Format("{0}{1}{2}", Table.Id, RowNr, TemporalGroup);
            string s2 = String.Format("{0}{1}{2}", si.Table.Id, si.RowNr, si.SpatialGroup);

            return s1.Equals(s2);
        }

        public override int GetHashCode()
        {
            return 1;
        }
    }
}
