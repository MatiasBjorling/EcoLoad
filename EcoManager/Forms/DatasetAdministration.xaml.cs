using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using EcoManager.Data.Enums;
using EcoManager.Data.Management;
using EcoManager.Data.Manipulation;
using EcoManager.Forms.ViewModel;
using EcoManager.Shared.Tools;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace EcoManager.Forms
{
    /// <summary>
    /// Interaction logic for DatasetAdministration.xaml
    /// </summary>
    public partial class DatasetAdministration : UserControl
    {
        private DatasetAdministrationViewModel Model;

        public DatasetAdministration()
        {
            InitializeComponent();
        }

        public void RefreshTables()
        {
            if (datasetView.SelectedItem != null)
            {
                Model.UpdateTableStructure(((DatasetViewModel) datasetView.SelectedItem).Dataset);
            }
        }

        //public Object PublishDatasets(Dataset ds, TreeViewItem tvi)
        //{
        //    tvi.Header = ds.Title;

        //    if (ds.Children.Count > 0)
        //    {
        //        foreach (Dataset dss in ds.Children)
        //        {
        //            Object o = PublishDatasets(dss, new TreeViewItem());

        //            tvi.Items.Add(o);
        //        }
        //    }
        //    else
        //        return ds;
            
        //    return tvi;
        //}

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Model != null)
                return;

            Model = new DatasetAdministrationViewModel(this);
            DataContext = Model;
        }

        private void datasetView_SelectedItemChanged_1(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
                Model.UpdateTableStructure(((DatasetViewModel) e.NewValue).Dataset);
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (datasetView.SelectedItem is DatasetViewModel)
            {
                DatasetViewModel dsvm = datasetView.SelectedItem as DatasetViewModel;
                Model.StoreDataset(dsvm.Dataset);
            }
                
        }

        //private void btnNewDataset_Click(object sender, RoutedEventArgs e)
        //{
        //    if (datasetView.SelectedItem is DatasetViewModel)
        //    {
        //        DatasetViewModel parentDataset = datasetView.SelectedItem as DatasetViewModel;

        //        DatasetViewModel newDatasetVM = new DatasetViewModel(new Dataset());
        //        newDatasetVM.Title = "New dataset";
        //        newDatasetVM.Dataset.TimeRangeBegin = DateTime.Now;
        //        newDatasetVM.Dataset.Parents.Add(parentDataset.Dataset);

        //        using (UnitOfWork.Start())
        //        {
        //            UnitOfWork.CurrentSession.Save(newDatasetVM.Dataset);
        //        }
        //        parentDataset.Children.Add(newDatasetVM);
                
        //    } 
        //    else
        //    {
        //        MessageBox.Show("You have to select an existing dataset to use as root for a new dataset.");
        //        return;
        //    }
        //}

        private void ShowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DataViewer window = new DataViewer();
            window.Show();

            window.Load(Convert.ToInt32(e.Parameter));
        }

        private void ReplaceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainWindow mv = VisualHelp.FindVisualParentRoot(this) as MainWindow;

            if (mv != null)
                mv.AddImportTab(((DatasetViewModel)datasetView.SelectedItem).Dataset, Convert.ToInt32(e.Parameter));
        }

        private void VisualizeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TableManager tm = new TableManager();
            tm.CreateRawTable((int)e.Parameter);

            Window w = new VistrailOptions((int)e.Parameter);
            
            w.ShowDialog();

        }

        private void btnNewTable_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mv = VisualHelp.FindVisualParentRoot(this) as MainWindow;

            if (mv != null) 
                mv.AddImportTab(((DatasetViewModel)datasetView.SelectedItem).Dataset);
        }

        private void btnNewDataset_Click(object sender, RoutedEventArgs e)
        {
            if (datasetView.SelectedItem == null)
            {
                MessageBox.Show("Select an existing entry first.");
                return;
            }

            var dsvmParent = (DatasetViewModel)datasetView.SelectedItem;

            DatasetViewModel dsvm = new DatasetViewModel(Model.GetNewDataset(dsvmParent.Dataset));
            
            dsvmParent.Children.Add(dsvm);
            dsvmParent.IsExpanded = true;
            dsvmParent.IsSelected = false;
            dsvm.IsSelected = true;

        }

        private void btnDeleteDataset_Click(object sender, RoutedEventArgs e)
        {
            if (datasetView.SelectedItem == null)
            {
                MessageBox.Show("Select an existing entry first.");
                return;
            }

            if (MessageBox.Show("Everything under this tree will be deleted. Including all data. Are you sure?", "Delete?",
                            MessageBoxButton.YesNo, MessageBoxImage.Asterisk, MessageBoxResult.No) == MessageBoxResult.No)
                return;

            var dsvm = (DatasetViewModel)datasetView.SelectedItem;

            var parent = dsvm.Parent;
            if (parent != null)
            {
                parent.IsSelected = true;
                parent.Children.Remove(dsvm);    
            }
            
            using (UnitOfWork.Start())
            {
                UnitOfWork.CurrentSession.Delete(dsvm.Dataset);
            }
        }

        private void OnListViewItemPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }


        private void ExportCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            
            saveFileDialog.Filter = "CSV File(*.csv)|*.*";

            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

            TableManager tm = new TableManager();
            tm.CreateRawTable((int)e.Parameter);

            string path = ConfigurationManager.AppSettings["CMDSQLPath"];

            string savePath = saveFileDialog.FileName;
            if (!savePath.Contains(".csv"))
                savePath += ".csv";

            //make sure we backup to a unique name
            
            
            FileInfo fInfo = new FileInfo(savePath);

            Process p = new Process();
            p.StartInfo.FileName = "sqlcmd";
            p.StartInfo.Arguments =
                "-S (local)\\SQL2008  -d DMU -E -Q \"SET NOCOUNT ON; SELECT * FROM tmpVizStorage\"  -s\";\" -w 999 -W -o " + ConfigurationManager.AppSettings["TMPPath"] + "\\" + 
                fInfo.Name;
            p.StartInfo.WorkingDirectory = path;
            p.StartInfo.UseShellExecute = true;
            p.Start();

            p.WaitForExit();

            if (File.Exists(ConfigurationManager.AppSettings["TMPPath"] + "\\" + fInfo.Name))
            File.Move(ConfigurationManager.AppSettings["TMPPath"] + "\\" + fInfo.Name, savePath);
            else
            {
                Logger.Message("An error has occured.");
                return;
            }

            Logger.Message("The data has been exported.");
        }

        private void AppendCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainWindow mv = VisualHelp.FindVisualParentRoot(this) as MainWindow;

            if (mv != null)
                mv.AddImportTab(((DatasetViewModel)datasetView.SelectedItem).Dataset, Convert.ToInt32(e.Parameter), ImportType.Append);
        }
    }
}
