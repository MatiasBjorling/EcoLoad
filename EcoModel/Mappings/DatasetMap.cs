using EcoManager.Data.Entities;
using FluentNHibernate.Mapping;

namespace EcoManager.Data.Mappings
{
    public class DatasetMap : ClassMap<Dataset>
    {
        public DatasetMap()
        {
            Id( x => x.Id);

            References(x => x.Group, "GroupId");
            References(x => x.Program, "ProgramId");
            
            HasMany(x => x.Tables)
                .KeyColumn("DatasetId")
                .Cascade.DeleteOrphan();

            HasManyToMany(x => x.Parents)
                .Table("DatasetParent")
                .ParentKeyColumn("Id")
                .ChildKeyColumn("Parent");

            HasManyToMany(x => x.Children)
                .Table("DatasetParent")
                .ParentKeyColumn("Parent")
                .ChildKeyColumn("Id")
                .FetchType.Join();

            Map(x => x.Title);
            Map(x => x.Description);
            Map(x => x.SampleDescription);
            Map(x => x.TimeRangeBegin);
            Map(x => x.TimeRangeEnd);
        }
    }
}
