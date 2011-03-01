using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EcoManager.Data.Entities;
using EcoManager.Data.Entities.NonRel;
using FluentNHibernate.Mapping;

namespace EcoManager.Data.Mappings
{
    public class SpatialInfoMap : ClassMap<SpatialInfo>
    {
        public SpatialInfoMap()
        {
            //Id(x => x.Table);
            //Id(x => x.RowNr);
            //Id(x => x.SpatialGroup);
            CompositeId()
                .KeyReference(x => x.Table, new string[] { "TableId" })
                .KeyProperty(x => x.RowNr)
                .KeyProperty(x => x.SpatialGroup);
            Map(x => x.Location)
                .CustomType(typeof(NHibernate.Spatial.Type.GeometryType))
                .LazyLoad();
        }
    }
}
