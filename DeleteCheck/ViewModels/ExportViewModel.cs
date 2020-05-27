using FPOSDB.Context;
using FPOSDB.DTO;
using FPOSPriceUpdater.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FPOSPriceUpdater.ViewModels
{
    public class ExportViewModel : BaseViewModel
    {
        private string _exportPath;
        public string ExportPath
        {
            get
            {
                return _exportPath;
            }
            set
            {
                if (value != _exportPath)
                    _exportPath = value;
            }
        }
        private bool _isNotExporting = true;
        public bool IsNotExporting
        {
            get
            {
                return _isNotExporting;
            }
            set
            {
                if (value != _isNotExporting)
                    _isNotExporting = value;
                RaisePropertyChange(nameof(IsNotExporting));
            }
        }
        public ICommand ClickExport { get; set; }
        private string _exportStatus;
        public string ExportStatus
        {
            get
            {
                return _exportStatus;
            }
            set
            {
                if (value != _exportStatus)
                    _exportStatus = value;
                RaisePropertyChange(nameof(ExportStatus));
            }
        }
        public ExportViewModel()
        {
            ClickExport = new RelayCommand<object>(ClickExportCommand);
        }

        public bool IsExportReady()
        {
            ExportStatus = String.Empty;

            if (String.IsNullOrEmpty(ExportPath))
            {
                ExportStatus = "Failed!" + Environment.NewLine + "Export path is missing or invalid";
                return false;
            }

            DBService db = new DBService(ConnectionString.GetString());
            if (!db.IsConnectionValid())
            {
                ExportStatus = "Failed!" +
                    Environment.NewLine + "Check your database connection.";
                return false;
            }
            return true;
        }
        private void ClickExportCommand(object obj = null)
        {
            if (!IsExportReady())
                return;
            IsNotExporting = false;

            ExportStatus = "Exporting...";
            DBService db = new DBService(ConnectionString.GetString());
            List<ItemPriceDTO> items = db.GetAllItemPrices();
            string path = $"{ExportPath}\\export.csv";

            Task.Run(() =>
            {
                try
                {
                    Serializer.ToCsv(items, path);
                    ExportStatus =
                        items.Select(x => x.ItemName).Distinct().Count() + " items exported" +
                        Environment.NewLine + Environment.NewLine +
                        path;
                }
                catch (Exception)
                {
                    ExportStatus =
                        "Failed!" + Environment.NewLine +
                        "Unable to write to export file.";
                }
            });
            IsNotExporting = true;
        }
    }
}
