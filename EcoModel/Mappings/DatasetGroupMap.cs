using EcoManager.Data.Entities;
using FluentNHibernate.Mapping;

namespace EcoManager.Data.Mappings
{
    public class DatasetGroupMap : ClassMap<DatasetGroup>
    {
        public DatasetGroupMap()
        {
            Id(x => x.Id);
            Map(x => x.Title);
            Map(x => x.Description);
            HasMany(x => x.Datasets)
                .KeyColumn("Id")
                .Cascade.All();
        }
    }
}
