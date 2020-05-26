using FPOSPriceUpdater.Views;
using System.Windows.Controls;

namespace FPOSPriceUpdater.Helper
{
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
    }
}
