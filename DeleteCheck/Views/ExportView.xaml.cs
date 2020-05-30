using FPOSPriceUpdater.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FPOSPriceUpdater.Views
{
    /// <summary>
    /// Interaction logic for ExportView.xaml
    /// </summary>
    public partial class ExportView : Page
    {
        ExportViewModel ViewModel = new ExportViewModel();

        public ExportView()
        {
            InitializeComponent();
            this.DataContext = ViewModel;
            this.IsVisibleChanged += ExportView_IsVisibleChanged;
        }

        private void ExportView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel.Visibility((bool)e.NewValue);
        }
    }
}
