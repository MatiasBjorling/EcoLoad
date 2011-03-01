using System;


namespace EcoManager.Data.Entities
{
    public class TemporalInterval : TemporalBase 
    {
        public virtual DateTime TimeBegin { get; set; }
        public virtual DateTime? TimeEnd { get; set; }
    }
}
