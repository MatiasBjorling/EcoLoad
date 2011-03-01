using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EcoManager.Data.Entities;
using FluentNHibernate.Mapping;

namespace EcoManager.Data.Mappings
{
    public class TemporalPointMap : ClassMap<TemporalPoint>
    {
        public TemporalPointMap()
        {
            CompositeId()
               .KeyReference(x => x.Table, new string[] { "TableId" })
               .KeyProperty(x => x.RowNr)
               .KeyProperty(x => x.TemporalGroup);
            Map(x => x.Point)
                .Column("TimePoint");
        }
    }
}
