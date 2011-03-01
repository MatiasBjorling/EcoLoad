using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using EcoManager.Data.Entities;
using EcoManager.Data.Entities.Vistrail;
using EcoManager.Data.Management;
using EcoManager.Data.Proxies;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace EcoManager.Forms.ViewModel
{
    public class DatasetAdministrationViewModel : BindableObject, IDisposable
    {
        public UserControl UControl { get; set; }

        private UnitOfWorkImpl uowImpl;

        private BindingList<DatasetGroup> datasetGroups;
        public BindingList<DatasetGroup> DatasetGroups
        {
            get { return datasetGroups; }
            set { datasetGroups = value; base.RaisePropertyChanged("DatasetGroups"); }
        }

        private BindingList<DatasetProgram> datasetPrograms;
        public BindingList<DatasetProgram> DatasetPrograms
        {
            get { return datasetPrograms; }
            set { datasetPrograms = value; base.RaisePropertyChanged("DatasetPrograms"); }
        }

        private ObservableCollection<DatasetViewModel> datasets;
        public ObservableCollection<DatasetViewModel> Datasets
        {
            get { return datasets; }
            set { datasets = value; base.RaisePropertyChanged("Datasets"); }
        }

        private ObservableCollection<TableInfo> tables;
        public ObservableCollection<TableInfo> Tables
        {
            get { return tables; }
            set { tables = value; base.RaisePropertyChanged("Tables"); }
        }

        public DatasetAdministrationViewModel(UserControl uControl)
        {
            UControl = uControl;
      
            using (UnitOfWork.Start())
            {
                DatasetPrograms =
                new BindingList<DatasetProgram>(UnitOfWork.CurrentSession.CreateCriteria<DatasetProgram>().List<DatasetProgram>());
                DatasetGroups =
                    new BindingList<DatasetGroup>(UnitOfWork.CurrentSession.CreateCriteria<DatasetGroup>().List<DatasetGroup>());

                ObservableCollection<Dataset> DatasetsTmp =
                    new ObservableCollection<Dataset>(UnitOfWork.CurrentSession
                        .CreateCriteria<Dataset>()
                        .SetResultTransformer(new DistinctRootEntityResultTransformer())
                        .List<Dataset>()
                        .Where(x=>x.Parents.Count == 0));

                Datasets = new ObservableCollection<DatasetViewModel>(
                    (from element in DatasetsTmp
                     select new DatasetViewModel(element) {IsExpanded = true})
                     .ToList());
            }           
        }

        /// <summary>
        /// Retrieves the latest version of the tables associated to a given dataset.
        /// </summary>
        /// <param name="dataset"></param>
        public void UpdateTableStructure(Dataset dataset)
        {
            //using (UnitOfWork.StartMysql())
            //{
            //    var larlar = UnitOfWork.CurrentSession.CreateCriteria<Vistrail>().List<Vistrail>();
            //}

            using (UnitOfWork.Start())
            {
                Tables = new ObservableCollection<TableInfo>(
                    UnitOfWork.CurrentSession
                        .CreateCriteria<TableInfo>()
                        .Add(Restrictions.Eq("Dataset", dataset))
                        .SetProjection(Projections.ProjectionList()
                            .Add(Projections.Property("Id"), "Id")
                            .Add(Projections.Property("TableDescription"), "TableDescription")
                            .Add(Projections.Property("ValidBegin"), "ValidBegin")
                            .Add(Projections.Property("ValidEnd"), "ValidEnd"))
                            .SetResultTransformer(Transformers.AliasToBean(typeof(TableInfo)))
                            .AddOrder(new Order("Id", false))
                        .List<TableInfo>());
            }
        }

        public void StoreDataset(Dataset dataset)
        {
            using (UnitOfWork.Start())
            {
                UnitOfWork.CurrentSession.SaveOrUpdate(dataset);
            }
        }

        public Dataset GetNewDataset(Dataset dataset)
        {
            Dataset ds = new Dataset();
            ds.Title = "Change title";
            ds.TimeRangeBegin = DateTime.Now;
            ds.Parents.Add(dataset);

            using (UnitOfWork.Start())
            {
                UnitOfWork.CurrentSession.Save(ds);
            }

            return ds;
        }

        public void Dispose()
        {
          
        }
    }
}
