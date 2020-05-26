using FPOSDB.Context;
using FPOSDB.DTO;
using FPOSPriceUpdater.Helper;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FPOSPriceUpdater.ViewModels
{
    public class ImportViewModel : BaseViewModel
    {
        private string _importPath;
        public string ImportPath
        {
            get
            {
                return _importPath;
            }
            set
            {
                if (value != _importPath)
                    _importPath = value;
            }
        }
        private bool _isNotImporting = true;
        public bool IsNotImporting
        {
            get
            {
                return _isNotImporting;
            }
            set
            {
                if (value != _isNotImporting)
                    _isNotImporting = value;
                RaisePropertyChange(nameof(IsNotImporting));
            }
        }
        private bool _isPartialImport = false;
        public bool IsPartialImport
        {
            get
            {
                return _isPartialImport;
            }
            set
            {
                if (value != _isPartialImport)
                    _isPartialImport = value;
                RaisePropertyChange(nameof(IsPartialImport));
            }
        }
        public ICommand ClickImport { get; set; }
        public ICommand ClickViewFailedImports { get; set; }
        private string _importStatus;
        public string ImportStatus
        {
            get
            {
                return _importStatus;
            }
            set
            {
                if (value != _importStatus)
                    _importStatus = value;
                RaisePropertyChange(nameof(ImportStatus));
            }
        }
        private static string PARTIAL_ITEMS_PATH { 
            get
            {
                return Path.GetTempPath() + "FPOSImportErrors.txt";
            }
        }

        public ImportViewModel()
        {
            ClickImport = new RelayCommand<object>(ClickImportCommand);
            ClickViewFailedImports = new RelayCommand<object>(ClickViewFailedImportsCommand);
        }

        private void ClickViewFailedImportsCommand(object obj)
        {
            System.Diagnostics.Process.Start(PARTIAL_ITEMS_PATH);
        }

        private void ClickImportCommand(object obj = null)
        {
            IsPartialImport = false;

            if (!IsImportReady())
                return;

            IsNotImporting = false;
            ImportStatus = "Importing...";

            DBService db = new DBService(ConnectionString.GetString());
            Task.Run(() =>
            {
                try
                {
                    List<ItemPriceDTO> items = Serializer.FromCSV(ImportPath);
                    List<ItemPriceDTO> itemsNotUpdated = db.UpdateItemPrices(items);
                    var totalItems = items.Count();
                    int totalUpdated;
                    if (itemsNotUpdated.Count > 0)
                    {
                        IsPartialImport = true;
                        totalUpdated = items.Count() - itemsNotUpdated.Count();
                        Serializer.ToFile(itemsNotUpdated, PARTIAL_ITEMS_PATH);
                    } else
                    {
                        totalUpdated = items.Count();
                    }
                    ImportStatus =
                        "Success!" + Environment.NewLine + Environment.NewLine +
                        "(" + totalUpdated + " of " + totalItems + ")" + " items imported";
                    IsNotImporting = true;
                }
                catch (Exception)
                {
                    ImportStatus =
                        "Failed!" + Environment.NewLine +
                        "Unable to read import file.";
                }
            });
        }
        public bool IsImportReady()
        {
            ImportStatus = String.Empty;

            if (String.IsNullOrEmpty(ImportPath))
            {
                ImportStatus = "Failed!" + Environment.NewLine + "Import file is missing or invalid";
                return false;
            }

            DBService db = new DBService(ConnectionString.GetString());
            if (!db.IsConnectionValid())
            {
                ImportStatus = "Failed!" +
                    Environment.NewLine + "Check your database connection.";
                return false;
            }
            return true;
        }
    }
}
