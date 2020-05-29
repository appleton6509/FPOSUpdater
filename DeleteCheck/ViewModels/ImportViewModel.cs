using FPOSDB.Context;
using FPOSDB.DTO;
using FPOSDB.Parameters;
using FPOSPriceUpdater.Helper;
using log4net;
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
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
        private bool _isImportDone = false;
        public bool IsImportDone
        {
            get
            {
                return _isImportDone;
            }
            set
            {
                if (value != _isImportDone)
                    _isImportDone = value;
                RaisePropertyChange(nameof(IsImportDone));
            }
        }
        public ICommand ClickImport { get; set; }
        public ICommand ClickViewFailedItems { get; set; }
        public ICommand ClickViewIgnoredItems { get; set; }
        public ICommand ClickViewImportedItems { get; set; }

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
        private static string FAILED_ITEMS_FILENAME
        {
            get
            {
                return "FailedItems.txt";
            }
        }
        private static string IGNORED_ITEMS_FILENAME
        {
            get
            {
                return "IgnoredItems.txt";
            }
        }
        private static string IMPORTED_ITEMS_FILENAME
        {
            get
            {
                return "ImportedItems.txt";
            }
        }

        public ImportViewModel()
        {
            ClickImport = new RelayCommand<object>(ClickImportCommand);
            ClickViewFailedItems = new RelayCommand<object>(ClickViewFailedItemsCommand);
            ClickViewIgnoredItems = new RelayCommand<object>(ClickViewIgnoredItemsCommand);
            ClickViewImportedItems = new RelayCommand<object>(ClickViewImportedItemsCommand);
        }

        private void ClickViewFailedItemsCommand(object obj)
        {
            System.Diagnostics.Process.Start(FAILED_ITEMS_FILENAME);
        }
        private void ClickViewImportedItemsCommand(object obj)
        {
            System.Diagnostics.Process.Start(IMPORTED_ITEMS_FILENAME);
        }
        private void ClickViewIgnoredItemsCommand(object obj)
        {
            System.Diagnostics.Process.Start(IGNORED_ITEMS_FILENAME);
        }
        private void ImportResultsToFile(ImportResult result)
        {
            Serializer.ToFile(result.Ignored, IGNORED_ITEMS_FILENAME);
            Serializer.ToFile(result.Failed, FAILED_ITEMS_FILENAME);
            Serializer.ToFile(result.Imported, IMPORTED_ITEMS_FILENAME);
        }

        private void ClickImportCommand(object obj = null)
        {
            _log.Info("***** Import is starting *****");
            IsImportDone = false;

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
                    ImportResult results = db.UpdateItemPrices(items);

                    var ignoredItems = results.DistinctIgnoredCount;
                    var importedItems = results.DistinctImportedCount;
                    var failedItems = results.DistinctFailedCount;
                    var processedItems = failedItems + importedItems + ignoredItems;
                    var csvItems = items.Select(x => x.ItemName).Distinct().Count();

                    string message = "Import Complete" + Environment.NewLine;
                    message += "Total Items In File:  " + csvItems + Environment.NewLine;
                    message += "Total Processed:  " + processedItems + Environment.NewLine;
                    message += "Total Ignored:  " + ignoredItems + Environment.NewLine;
                    message += "Total Imported:  " + importedItems + Environment.NewLine;
                    message += "Total Failed:  " + failedItems + Environment.NewLine;

                    IsImportDone = true;

                    ImportStatus = message;
                    ImportResultsToFile(results);
                    _log.Info(message);
                }
                catch (Exception ex)
                {
                    ImportStatus =
                        "Unable to read import file." + Environment.NewLine +
                                ex.Message;
                    _log.Error("Import failed", ex);
                }
                finally
                {
                    IsNotImporting = true;
                    _log.Info("***** Import is ended *****");
                }
            });
        }
        public bool IsImportReady()
        {
            ImportStatus = String.Empty;

            if (String.IsNullOrEmpty(ImportPath))
            {
                ImportStatus = "Failed!" + Environment.NewLine + "Import file is missing or invalid";
                _log.Error("Import did not start as import path is null or empty");
                return false;
            }

            DBService db = new DBService(ConnectionString.GetString());
            if (!db.IsConnectionValid())
            {
                ImportStatus = "Failed!" +
                    Environment.NewLine + "Check your database connection.";
                _log.Error("Import did not start as database connection is invalid");
                return false;
            }
            return true;
        }
    }
}
