using FPOSDB.Context;
using FPOSDB.DTO;
using FPOSPriceUpdater.Enum;
using FPOSPriceUpdater.Helper;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FPOSPriceUpdater.ViewModels
{
    public class ExportViewModel : BaseViewModel
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

        private void ClickExportCommand(object obj = null)
        {
            if (!IsExportReady())
                return;

            SetExportStatus(Enum.TransferStatus.Started, "Exporting...");
            Task.Run(() =>
            {
                DBService db = new DBService(ConnectionString.GetString());
                List<ItemPriceDTO> items = new List<ItemPriceDTO>();

                int itemsexported = 9;
                string path = $"{ExportPath}\\export.csv";
                try
                {
                    items = db.GetAllItemPrices();
                    Serializer.ToCsv(items, path);
                    itemsexported = items.Select(x => x.ItemName).Distinct().Count();
                    SetExportStatus(Enum.TransferStatus.Success, $"{itemsexported} items exported {Environment.NewLine} {path}");
                }
                catch (Exception ex)
                {
                    SetExportStatus(Enum.TransferStatus.Failed, "Unable to write to export file. ", ex);
                }
                SetExportStatus(Enum.TransferStatus.Stopped, "");
            });
        }
        
        public bool IsExportReady()
        {
            if (String.IsNullOrEmpty(ExportPath))
            {
                SetExportStatus(Enum.TransferStatus.Failed, "Failed!" + Environment.NewLine + "Export path is missing or invalid");
                return false;
            }

            DBService db = new DBService(ConnectionString.GetString());
            if (!db.IsConnectionValid())
            {
                SetExportStatus(Enum.TransferStatus.Failed, "Failed!" + Environment.NewLine + "Check your database connection.");
                return false;
            }
            return true;
        }

        public void Visibility(bool isVisible)
        {
            if (!isVisible)
                SetExportStatus(TransferStatus.Reset, "");
        }

        private void SetExportStatus(TransferStatus status,string ExportMessage, Exception ex = null)
        {
            switch (status)
            {
                case TransferStatus.Started:
                    IsNotExporting = false;
                    ExportStatus = ExportMessage;
                    _log.Info("***** Export Started *****");
                    break;
                case TransferStatus.Failed:
                    IsNotExporting = true;
                    ExportStatus = ExportMessage;
                    _log.Error(ExportMessage, ex);
                    _log.Error("***** Export Failed *****");
                    break;
                case TransferStatus.Stopped:
                    IsNotExporting = true;
                    _log.Info("***** Export Stopped *****");
                    break;
                case TransferStatus.Success:
                    IsNotExporting = true;
                    ExportStatus = ExportMessage;
                    _log.Info(ExportMessage);
                    _log.Info("***** Export Successful *****");
                    break;
                case TransferStatus.Reset:
                    IsNotExporting = true;
                    ExportStatus = ExportMessage;
                    _log.Info(ExportMessage);
                    _log.Info("***** Export Successful *****");
                    break;
            }
        }
    }
}
