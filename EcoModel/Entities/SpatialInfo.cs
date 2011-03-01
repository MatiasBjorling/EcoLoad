using System;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace EcoManager.Data.Entities
{
    public class SpatialInfo
    {
        public virtual TableInfo Table { get; set; }
        public virtual int RowNr { get; set; }
        public virtual int SpatialGroup { get; set; }
        public virtual Point Location { get; set; }

        public override bool Equals(object obj)
        {
            SpatialInfo si = obj as SpatialInfo;
            if (si == null)
                return false;
            
            string s1 = String.Format("{0}{1}{2}", Table.Id, RowNr, SpatialGroup);
            string s2 = String.Format("{0}{1}{2}", si.Table.Id, si.RowNr, si.SpatialGroup);

            return s1.Equals(s2);
        }

        public override int GetHashCode()
        {
            return 1;
        }
    }
}
