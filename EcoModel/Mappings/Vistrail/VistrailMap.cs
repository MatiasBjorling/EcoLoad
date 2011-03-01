using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using EcoManager.Data.Entities.Vistrail;

namespace EcoManager.Data.Mappings.Vistrail
{
    public class VistrailMap : ClassMap<Entities.Vistrail.Vistrail>
    {
        public VistrailMap()
        {
            Id(x => x.Id)
                .Column("id");
            Map(x => x.EntityType)
                .Column("entity_type");
            Map(x => x.Version)
                .Column("version");
            Map(x => x.Name)
                .Column("name");
            Map(x => x.LastModified)
                .Column("last_modified");
        }
    }
}
