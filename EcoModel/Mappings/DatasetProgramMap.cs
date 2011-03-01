using EcoManager.Data.Entities;
using FluentNHibernate.Mapping;

namespace EcoManager.Data.Mappings
{
    public class DatasetProgramMap : ClassMap<DatasetProgram>
    {
        public DatasetProgramMap()
        {
            Id(x => x.Id);
            Map(x => x.Description);
        }
    }
}
