using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using EcoManager.Data.Entities.NonRel.ImportExport;
using EcoManager.Data.Management;
using EcoManager.Shared.Tools;

namespace EcoManager.Forms.ViewModel
{
    public class ImportExportProgressViewModel
    {
        public UserControl UControl { get; set; }

        private List<WorkEntity> workEntities;
        public List<WorkEntity> WorkEntities
        {
            get { return workEntities; }
            set { workEntities = value; }
        }

        private BackgroundWorker bgw;

        public delegate void FinishedExecutingEventHandler(object sender);

        public event FinishedExecutingEventHandler FinishedExecuting;

        public ImportExportProgressViewModel()
        {
            bgw = new BackgroundWorker();
            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
        }

        public void ExecuteBegin()
        {
            bgw.RunWorkerAsync();
        }

        void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FinishedExecuting(this);
        }

        void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            if (WorkEntities == null)
                return;

            foreach (WorkEntity we in WorkEntities)
            {
                if (!we.CommitEntity)
                    we.Execute(we);
            }

            bool HaveErrors = false;
            int numErrors = 0;
            foreach (WorkEntity we in WorkEntities.Where(we => we.HasErrors))
                numErrors = + we.Errors.Count;

            if (numErrors > 0)
            {
                if (MessageBox.Show(
                    "The import had " + numErrors + " errors when importing. Do you want to commit the changes eitherway?",
                    "Errors", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    workEntities.Find(we => we.CommitEntity).Execute(workEntities.Find(we => we.CommitEntity));
                } else
                {
                    UnitOfWork.CurrentSession.Transaction.Rollback();
                }
            } 
            else
            {
                workEntities.Find(we => we.CommitEntity).Execute(workEntities.Find(we => we.CommitEntity));
            }
        }
    }
}
