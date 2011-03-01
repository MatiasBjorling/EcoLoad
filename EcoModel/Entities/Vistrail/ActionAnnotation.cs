using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EcoManager.Data.Entities.Vistrail
{
    public class ActionAnnotation
    {
        public virtual int Id { get; set; }
        public virtual string Akey { get; set; }
        public virtual string Value { get; set; }
        public virtual int ActionId { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual int VistrailParent { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
