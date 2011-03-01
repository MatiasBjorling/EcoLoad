using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EcoManager.Data.Entities;
using FluentNHibernate.Mapping;

namespace EcoManager.Data.Mappings
{
    public class TemporalIntervalMap : ClassMap<TemporalInterval>
    {
        public TemporalIntervalMap()
        {
            CompositeId()
               .KeyReference(x => x.Table, new string[] { "TableId" })
               .KeyProperty(x => x.RowNr)
               .KeyProperty(x => x.TemporalGroup);
            Map(x => x.TimeBegin);
            Map(x => x.TimeEnd);

        }
    }
}
