using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using EcoManager.Forms.ViewModel;

namespace EcoManager.Forms
{
    /// <summary>
    /// Interaction logic for VistrailOptions.xaml
    /// </summary>
    public partial class VistrailOptions : Window
    {
        private VistrailOptionsViewModel Model;

        public VistrailOptions(int tableId)
        {
            InitializeComponent();
            Model = new VistrailOptionsViewModel(this);
            DataContext = Model;
            cmbDataSource.SelectedIndex = 0;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            Process p = new Process();
            p.StartInfo.Arguments = "vistrails.py -c 0 -t localhost -r 3306 -f vistrails -u root ";
            if (cmbDataSource.SelectedIndex == 0)
                p.StartInfo.Arguments += "8:";
            else 
                p.StartInfo.Arguments += "9:";

            if (cmbWorkflows.SelectedItem == null)
            {    
                MessageBox.Show("No workflow selected.");
                return;
            }

            

            p.StartInfo.Arguments += cmbWorkflows.SelectedItem;
            MessageBox.Show(p.StartInfo.Arguments);
            p.StartInfo.FileName = "C:\\Program Files (x86)\\VisTrails\\vistrails\\Python26\\python";
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.WorkingDirectory = "C:\\Program Files (x86)\\VisTrails\\vistrails\\";
            p.Start();

        }

        private void cmbDataSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String text = ((ComboBoxItem) e.AddedItems[0]).Content.ToString();
            if (text.Equals("Cassiope"))
            {
                Model.UpdateActions(1);
            } 
            else
            {
                Model.UpdateActions(2);
            }
        }
    }
}