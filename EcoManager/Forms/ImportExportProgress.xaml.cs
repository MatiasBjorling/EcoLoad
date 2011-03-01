using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using EcoManager.Data.Entities.NonRel.ImportExport;
using EcoManager.Forms.ViewModel;
using EcoManager.Shared.Tools;

namespace EcoManager.Forms
{
    /// <summary>
    /// Interaction logic for ImportExportProgress.xaml
    /// </summary>
    public partial class ImportExportProgress : UserControl
    {
        private ImportExportProgressViewModel Model;

        public Size WindowSize = new Size(350,250);

        public ImportExportProgress()
        {
            InitializeComponent();
            Model = new ImportExportProgressViewModel();
            Model.UControl = this;
            DataContext = Model;

            Model.FinishedExecuting += new ImportExportProgressViewModel.FinishedExecutingEventHandler(Model_FinishedExecuting);
        }

        void Model_FinishedExecuting(object sender)
        {
            btnClose.IsEnabled = true;
            lblFinished.Visibility = Visibility.Visible;
        }

        public void Execute(List<WorkEntity> workEntities)
        {
            Model.WorkEntities = workEntities;
            Model.ExecuteBegin();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Window parent = this.Parent as Window;
            if (parent != null)
                parent.Close();
            else
            {
                Logger.Error("Not able to close window because the user control parent is not a window.");
            }
        }
    }
}
