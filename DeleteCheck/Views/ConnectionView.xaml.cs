using FPOSUpdater.ViewModels;
using System.Windows;
using System.Windows.Controls;


namespace FPOSUpdater.Views
{
    /// <summary>
    /// Interaction logic for Connection.xaml
    /// </summary>
    public partial class ConnectionView : Page
    {
        ConnectionViewModel ViewModel = new ConnectionViewModel();

        public ConnectionView()
        {
            InitializeComponent();
            this.DataContext = ViewModel;
            ViewModel.TestConnection();
        }


        private void BtnTestConnection_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.TestConnection();
        }
    }
}
