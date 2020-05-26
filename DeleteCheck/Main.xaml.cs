using FPOSDB.Context;
using FPOSPriceUpdater.Helper;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;


namespace FPOSPriceUpdater
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window, INotifyPropertyChanged
    {
        private string _version;
        public string Version
        {
            get
            {
                return this._version;
            }
            set
            {
                if (value != _version)
                {
                    this._version = "v" + value;
                    RaisePropertyChange(nameof(Version));
                }
            }
        }

        public Main()
        {
            InitializeComponent();
            CreateVersion();
            CreateConnectionString();
            MainFrame.Content = Navigator.ConnectionView;
        }

        private void CreateConnectionString()
        {
            var Instance = "LOCALHOST\\CESSQL";
            var DBName = "FPOSEMPTY";
            ConnectionString.CreateString(Instance, DBName);
        }
        private void CreateVersion()
        {
            try
            {
                Version = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            catch (Exception)
            {
                Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }

            this.Title = "FPOS Price Updater " + Version;
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

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void RaisePropertyChange(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
                return;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
