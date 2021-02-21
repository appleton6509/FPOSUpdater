using FPOSPriceUpdater.Views;
using System.Windows;
using System.Windows.Controls;

namespace FPOSPriceUpdater.Helper
{
    public enum PageView
    {
        Connection,
        Import,
        Export
    }
    public static class Navigator
    {
        static Page _connection;
        static Page _import;
        static Page _export;

        public static Page ConnectionView
        {
            get
            {
                if (_connection == null)
                    _connection = new ConnectionView();
                return _connection;
            }
        }
        public static Page ImportView
        {
            get
            {
                if (_import == null)
                    _import = new ImportView();
                return _import;
            }
        }
        public static Page ExportView
        {
            get
            {
                if (_export == null)
                    _export = new ExportView();
                return _export;
            }
        }
        public static void ChangeView(PageView page, Frame frame)
        {
            ConnectionView.Visibility = Visibility.Hidden;
            ImportView.Visibility = Visibility.Hidden;
            ExportView.Visibility = Visibility.Hidden;
            Page displayPage;

            switch (page)
            {
                case PageView.Import:
                    ImportView.Visibility = Visibility.Visible;
                    displayPage = ImportView;
                    break;
                case PageView.Connection:
                    ConnectionView.Visibility = Visibility.Visible;
                    displayPage = ConnectionView;
                    break;
                case PageView.Export:
                    ExportView.Visibility = Visibility.Visible;
                    displayPage = ExportView;
                    break;
                default:
                    ConnectionView.Visibility = Visibility.Visible;
                    displayPage = ConnectionView;
                    break;
            }
            frame.Content = displayPage;
        }
    }
}
