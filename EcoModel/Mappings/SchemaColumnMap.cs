using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EcoManager.Data.Entities;
using FluentNHibernate.Mapping;

namespace EcoManager.Data.Mappings
{
    public class SchemaColumnMap : ClassMap<SchemaColumn>
    {
        public SchemaColumnMap()
        {
            Id(x => x.Id);
            References(x => x.Schema)
                .Column("SchemaId");
            Map(x => x.Name);
            Map(x => x.OrigName);
            Map(x => x.ColOrder);
            Map(x => x.Type);
            Map(x => x.GroupId);
            Map(x => x.SpatialGeoType);
            Map(x => x.TemporalEndingType);
            Map(x => x.TemporalType);
            Map(x => x.DateFormat);
            Map(x => x.ValidBegin);
            Map(x => x.ValidEnd);
        }
    }
}
