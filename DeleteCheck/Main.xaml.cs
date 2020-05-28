using FPOSDB.Context;
using FPOSPriceUpdater.Helper;
using log4net;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;


namespace FPOSPriceUpdater
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Main()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.Configure();
            CreateConnectionString();
            MainFrame.Content = Navigator.ConnectionView;
            log.Info("App Started");
        }

        private void CreateConnectionString()
        {
            var Instance = "LOCALHOST\\CESSQL";
            var DBName = "FPOS5";
            ConnectionString.CreateString(Instance, DBName);
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = Navigator.ConnectionView;
        }

        private void btnExportPrices_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = Navigator.ExportView;
        }

        private void btnImportPrices_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = Navigator.ImportView;
        }
    }
}
