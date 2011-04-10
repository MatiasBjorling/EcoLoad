using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using EcoManager.Data.Entities;
using EcoManager.Data.Enums;
using EcoManager.Forms.ViewModel;
using EcoManager.Shared.Tools;
using LumenWorks.Framework.IO.Csv;
using ComboBox = System.Windows.Controls.ComboBox;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;

namespace EcoManager.Forms
{
    /// <summary>
    /// Interaction logic for Import.xaml
    /// </summary>
    public partial class Import : UserControl
    {
        private string[] headers;

        private readonly ImportViewModel model;

        private ImportType importType = ImportType.New;

        public Import()
        {
            InitializeComponent();
        }

        public Import(Dataset dataset) : this()
        {
            model = new ImportViewModel(dataset);
            model.UControl = this;
            DataContext = model;
        }

        public Import(Dataset dataset, int tableId) : this(dataset)
        {
            model.SetParentTableFromId(tableId);
            this.importType = ImportType.Replace;
        }

        public Import(Dataset dataset, int tableId, ImportType importType) : this(dataset)
        {
            model.SetParentTableFromId(tableId);
            this.importType = importType;
        }


        private void btnChooseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            model.ImportPath = openFileDialog.FileName;
            txtFilePath.Text = model.ImportPath;
            
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(model.ImportPath))
            {
                MessageBox.Show("File is not found or is not selected.");
                return;
            }
                
            dgPreview.ItemsSource = model.GridData;

            DataTemplate dtemplate = (DataTemplate)dgPreview.FindResource("ColumnHeaderDataTemplate");
            for (int i = 0; i < dgPreview.Columns.Count;i++ )
            {
                DataGridColumn dgc = dgPreview.Columns[i];
                dgc.Header = dtemplate.LoadContent();
                
                // Set options for ColumnName TextBox
                Grid gridColumn = (Grid)dgc.Header;
                gridColumn.DataContext = model.Data.Columns[i];

                // Set options for ColumnName TextBox
                TextBox txtHeaderColumnName = VisualHelp.FindVisualChildByName<TextBox>((DependencyObject)dgc.Header,
                                                                             "txtColumnName");

                // Set options for Datatype Combobox
                ComboBox cmbHeaderDataType = VisualHelp.FindVisualChildByName<ComboBox>((DependencyObject)dgc.Header, "cmbDataType");
                cmbHeaderDataType.SelectionChanged += new SelectionChangedEventHandler(headerDataType_SelectionChanged);

                // Set options for The temporal subtype Combobox
                ComboBox cmbHeaderTemporalSubType = VisualHelp.FindVisualChildByName<ComboBox>((DependencyObject)dgc.Header, "cmbTemporalSubType");
                cmbHeaderTemporalSubType.SelectionChanged += new SelectionChangedEventHandler(headerTemporalSubType_SelectionChanged);

            }
        }

        /// <summary>
        /// Show the type of temporal part of combo box if Interval is chosen in the TemporalSubType Combobox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void headerTemporalSubType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;

            if (cmb == null)
                return;

            var doc = VisualHelp.FindVisualParentRoot(cmb);
            Grid gridTemporalPartOf = VisualHelp.FindVisualChildByName<Grid>(doc, "TemporalSubPartOfGroup");

            if (gridTemporalPartOf == null)
                return;

            switch ((TimeTypes)e.AddedItems[0])
            {
                case TimeTypes.Point:
                case TimeTypes.Length:
                    gridTemporalPartOf.Visibility = Visibility.Collapsed;
                    break;
                case TimeTypes.Interval:
                    gridTemporalPartOf.Visibility = Visibility.Visible;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Show custom boxes for the given datatype.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void headerDataType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;

            if (cmb == null)
                return;

            var doc = VisualHelp.FindVisualParentRoot(cmb);
            Grid gridTemporal = VisualHelp.FindVisualChildByName<Grid>(doc, "TemporalGroup");
            Grid gridSpatial = VisualHelp.FindVisualChildByName<Grid>(doc, "SpatialGroup");

            if (gridTemporal == null || gridSpatial == null)
                return;


            switch ((StorageTypes)e.AddedItems[0])
            {
                case StorageTypes.Text:
                case StorageTypes.Integer:
                case StorageTypes.Float:
                    gridTemporal.Visibility = Visibility.Collapsed;
                    gridSpatial.Visibility = Visibility.Collapsed;
                    break;
                case StorageTypes.Spatial:
                    gridTemporal.Visibility = Visibility.Collapsed;
                    gridSpatial.Visibility = Visibility.Visible;
                    break;
                case StorageTypes.Temporal:
                    gridTemporal.Visibility = Visibility.Visible;
                    gridSpatial.Visibility = Visibility.Collapsed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            if (!model.Data.IsValid)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string s in model.Data.GetErrors)
                {
                    sb.AppendFormat("{0}\n", s);
                }
                MessageBox.Show(sb.ToString());
                return;
            }

            model.Data.ImportType = importType;

            ImportExportProgress progresser = new ImportExportProgress();
            progresser.Execute(model.LoadDataset());
            Window w = new Window {Content = progresser};
            w.Width = progresser.WindowSize.Width;
            w.Height = progresser.WindowSize.Height;
            w.Name = "Import";
            w.ShowDialog();

            CloseTab();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseTab();
        }

        private void CloseTab()
        {
            MainWindow mv = VisualHelp.FindVisualParentRoot(this) as MainWindow;

            if (mv != null)
                mv.RemoveImportTab(this);
        }
    }
}
