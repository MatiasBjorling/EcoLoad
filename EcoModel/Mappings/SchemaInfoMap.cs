using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EcoManager.Data.Entities;
using FluentNHibernate.Mapping;

namespace EcoManager.Data.Mappings
{
    public class SchemaInfoMap : ClassMap<SchemaInfo>
    {
        public SchemaInfoMap()
        {
            Id(x => x.Id);
            References(x => x.SchemaParent)
                .Column("ParentId");
            Map(x => x.Name);
            Map(x => x.ValidBegin);
            Map(x => x.ValidEnd);

            HasMany(x => x.Columns)
                .KeyColumn("SchemaId");

            HasMany(x => x.Tables)
                .Inverse()
                .ForeignKeyConstraintName("SchemaId")
                .KeyColumn("SchemaId");
        }
    }
}
