using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EcoManager.Forms.ViewModel
{
    /// <summary>
    /// Interaction logic for DataViewer.xaml
    /// </summary>
    public partial class DataViewer : Window
    {
        private DataViewerViewModel Model;

        public DataViewer()
        {
            InitializeComponent();
            Model = new DataViewerViewModel(this);
        }

        public void Load(int tableId)
        {
            Model.Load(tableId);
            dataGrid1.ItemsSource = Model.DataHolder.Tables[0].DefaultView;
        }
    }
}
