using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EcoManager.Data.Entities;
using EcoManager.Data.Types;
using FluentNHibernate.Mapping;

namespace EcoManager.Data.Mappings
{
    public class TableInfoMap : ClassMap<TableInfo>
    {
        public TableInfoMap()
        {
            Id(x => x.Id);
            References(x => x.Schema)
                .Column("SchemaId");

            References(x => x.Dataset)
                .Column("DatasetId");

            HasManyToMany(x => x.Parents)
                .Table("TableInfoParent")
                .ParentKeyColumn("Id")
                .ChildKeyColumn("Parent");

            HasManyToMany(x => x.Children)
                .Table("TableInfoParent")
                .ParentKeyColumn("Parent")
                .ChildKeyColumn("Id")
                .FetchType.Join();

            Map(x => x.TableDescription);
            Map(x => x.Created);
            Map(x => x.ValidBegin);
            Map(x => x.ValidEnd);
            Map(x => x.Storage)
                .LazyLoad()
                .CustomType(typeof (XmlType));

        }
    }
}
