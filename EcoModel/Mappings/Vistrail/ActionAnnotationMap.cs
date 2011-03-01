using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EcoManager.Data.Entities.Vistrail;
using FluentNHibernate.Mapping;

namespace EcoManager.Data.Mappings.Vistrail
{
    public class ActionAnnotationMap : ClassMap<ActionAnnotation>
    {
        public ActionAnnotationMap()
        {
            Table("action_annotation");
            Id(x => x.Id)
                .Column("id");
            Map(x => x.Akey)
                .Column("akey");
            Map(x => x.Value)
                .Column("value");
            Map(x => x.ActionId)
                .Column("action_id");
            Map(x => x.Date)
                .Column("Date");
            Map(x => x.VistrailParent)
                .Column("parent_id");

        }
    }
}
