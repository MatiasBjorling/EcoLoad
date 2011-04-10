using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using EcoManager.Data.Entities;
using EcoManager.Data.Enums;
using EcoManager.Forms;

namespace EcoManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            datasetAdmin.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
        }

        public void AddImportTab(Dataset dataset)
        {
            AddImportTab(dataset, 0, ImportType.New);
        }

        public void AddImportTab(Dataset dataset, int tableId)
        {
            AddImportTab(dataset, tableId, ImportType.Replace);
        }

        public void AddImportTab(Dataset dataset, int tableId, ImportType importType)
        {
            TabItem t = new TabItem();
            t.Header = "Import into " + dataset.Title;
            t.Content = new Import(dataset, tableId, importType);

            tabControl.Items.Add(t);
            tabControl.SelectedItem = t;
        }


        public void RemoveImportTab(FrameworkElement fe)
        {
            foreach (TabItem ti in tabControl.Items.Cast<TabItem>().Where(ti => ti.Content == fe))
            {
                tabControl.Items.Remove(ti);
                break;
            }

            datasetAdmin.RefreshTables();
        }
    }
}
