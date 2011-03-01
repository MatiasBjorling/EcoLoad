using System;

namespace EcoManager.Data.Entities
{
    public class DatasetProgram
    {
        public virtual int Id { get; set; }
        public virtual String Description { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", Description);
        }
    }
}
