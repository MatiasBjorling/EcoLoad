using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EcoManager.Data.Entities.Vistrail
{
    public class Vistrail
    {
        public virtual int Id { get; set; }
        public virtual string EntityType { get; set; }
        public virtual string Version { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime LastModified { get; set; }
    }
}
