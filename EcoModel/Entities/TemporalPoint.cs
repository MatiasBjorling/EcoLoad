using System;

namespace EcoManager.Data.Entities
{
    public class TemporalPoint : TemporalBase
    {
        public virtual DateTime? Point { get; set; }
    }
}
