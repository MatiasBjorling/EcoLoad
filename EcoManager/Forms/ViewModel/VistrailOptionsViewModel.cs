using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using EcoManager.Data.Entities;
using EcoManager.Data.Entities.Vistrail;
using EcoManager.Data.Management;
using EcoManager.Data.Proxies;
using NHibernate.Criterion;

namespace EcoManager.Forms.ViewModel
{
    class VistrailOptionsViewModel : BindableObject
    {
        public Window UControl { get; set; }

        private UnitOfWorkImpl uowImpl;

        private ObservableCollection<ActionAnnotation> actionAnnotations;
        public ObservableCollection<ActionAnnotation> ActionAnnotations
        {
            get { return actionAnnotations; }
            set { actionAnnotations = value; base.RaisePropertyChanged("ActionAnnotations"); }
        }

        public VistrailOptionsViewModel(Window uControl)
        {
            UControl = uControl;
        }

        public void UpdateActions(int id)
        {
            using (UnitOfWork.StartMysql())
            {
                if (id == 1) // Cassiope
                {
                    ActionAnnotations = new ObservableCollection<ActionAnnotation>(UnitOfWork.CurrentSession
                                                                        .CreateCriteria<ActionAnnotation>()
                                                                        .Add(Restrictions.Eq("VistrailParent", 8))
                                                                        .Add(Restrictions.Eq("Akey", "__tag__"))
                                                                        .List<ActionAnnotation>());
                } else if (id == 2) // Snow
                {
                    ActionAnnotations = new ObservableCollection<ActionAnnotation>(UnitOfWork.CurrentSession
                                                                        .CreateCriteria<ActionAnnotation>()
                                                                        .Add(Restrictions.Eq("VistrailParent", 9))
                                                                        .Add(Restrictions.Eq("Akey", "__tag__"))
                                                                        .List<ActionAnnotation>());
                }
            }
        }
    }
}
