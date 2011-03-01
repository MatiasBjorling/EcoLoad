using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using EcoManager.Data.Entities;

namespace EcoManager.Forms.ViewModel
{
    public class DatasetViewModel : INotifyPropertyChanged
    {
        readonly ObservableCollection<DatasetViewModel> children;
        readonly DatasetViewModel parent;
        readonly Dataset dataset;

        bool isExpanded;
        bool isSelected;

        public DatasetViewModel(Dataset dataset)
            : this(dataset, null)
        {
        }

        private DatasetViewModel(Dataset dataset, DatasetViewModel parent)
        {
            this.dataset = dataset;
            this.parent = parent;

            children = new ObservableCollection<DatasetViewModel>(
                    (from child in dataset.Children
                     select new DatasetViewModel(child, this))
                     .ToList());
        }

        public Dataset Dataset
        {
            get { return dataset; }
        }
        

        public ObservableCollection<DatasetViewModel> Children
        {
            get { return children; }
        }

        public string Title
        {
            get { return dataset.Title; }
            set
            {
                dataset.Title = value;
                OnPropertyChanged("Title");
            }
        }

        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                if (value != isExpanded)
                {
                    isExpanded = value;
                    OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (isExpanded && parent != null)
                    parent.IsExpanded = true;
            }
        }

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (value != isSelected)
                {
                    isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        public DatasetViewModel Parent
        {
            get { return parent; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
