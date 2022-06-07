using FPOSUpdater.ViewModels;
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

namespace FPOSUpdater.Views
{
    /// <summary>
    /// Interaction logic for ImportView.xaml
    /// </summary>
    public partial class ImportView : Page
    {
        public ImportView()
        {
            InitializeComponent();
            this.DataContext = new ImportViewModel();
        }
    }
}
