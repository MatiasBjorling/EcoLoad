using System;
using System.Collections.Generic;

namespace EcoManager.Data.Entities
{
    public class DatasetGroup
    {
        public virtual int Id { get; set; }
        public virtual String Title { get; set; }
        public virtual String Description { get; set; }
        public virtual IList<Dataset> Datasets { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", Title);
        }
    }
}
