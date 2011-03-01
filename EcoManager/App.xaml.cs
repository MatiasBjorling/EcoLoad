using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using EcoManager.Data.Entities;
using EcoManager.Data.Management;
using EcoManager.Shared.Tools;

namespace EcoManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Logger.Info("Starting DMU...");
            using(UnitOfWork.Start())
            {
                
            }
            
        }
    }
}
