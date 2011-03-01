using System;
using System.Collections.Generic;
using EcoManager.Data.Management;
using EcoManager.Data.Proxies;
using NHibernate.Validator.Constraints;

namespace EcoManager.Data.Entities
{
    public class Dataset : BindableObject
    {
        public virtual int Id { get; set; }
        public virtual DatasetGroup Group { get; set; }
        public virtual DatasetProgram Program { get; set; }

        private string title;
        [Length(1024), NotEmpty]
        public virtual String Title
        {
            get { return title; }
            set 
            { 
                title = value;
                base.RaisePropertyChanged("Title");
            }
        }

        public virtual String Description { get; set; }
        public virtual String SampleDescription { get; set; }
        public virtual DateTime TimeRangeBegin { get; set; }
        public virtual DateTime? TimeRangeEnd { get; set; }

        public virtual IList<TableInfo> Tables { get; set; }

        public virtual IList<Dataset> Parents { get; set; }
        public virtual IList<Dataset> Children { get; set; }

        public Dataset()
        {
            Tables = new List<TableInfo>();
            Children = new List<Dataset>();
            Parents = new List<Dataset>();
        }

        public override string ToString()
        {
            return string.Format("{0}", Title);
        }
    }
}
