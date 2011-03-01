using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EcoManager.Data.Entities.NonRel.ImportExport
{
    public class WorkEntity : INotifyPropertyChanged 
    {

        public delegate void DynamicFunc(WorkEntity workEntity);
        public DynamicFunc Execute;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool CommitEntity;

        public Dataset WorkingDataset { get; set; }

        private string name;
        public String Name
        {
            get { return name; }
            set
            {
                name = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        private bool completed;
        public Boolean Completed
        {
            get { return completed; }
            set { 
                completed = value; 
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Completed"));
            }
        }

        private List<string> errors;
        public List<String> Errors
        {
            get
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("HasErrors"));
                return errors;
            }
            set { errors = value; }
        }

        public bool HasErrors
        {
            get { return errors.Count != 0; }
        }

        public WorkEntity()
        {
            Errors = new List<string>();
        }
    }
}
