using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EcoManager.Data.Entities;
using EcoManager.Forms;
using EcoManager.Shared.Tools;

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
            TabItem t = new TabItem();
            t.Header = "Import into " + dataset.Title;
            t.Content = new Import(dataset);
            
            tabControl.Items.Add(t);
            tabControl.SelectedItem = t;
        }

        public void AddImportTab(Dataset dataset, int tableId)
        {
            TabItem t = new TabItem();
            t.Header = "Import into " + dataset.Title;
            t.Content = new Import(dataset, tableId);

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
