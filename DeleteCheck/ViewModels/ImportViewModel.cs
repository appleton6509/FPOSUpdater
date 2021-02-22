using FPOSDB.Context;
using FPOSDB.DTO;
using FPOSDB.Parameters;
using FPOSPriceUpdater.BusinessLogic;
using FPOSPriceUpdater.Helper;
using log4net;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FPOSPriceUpdater.ViewModels
{
    public class ImportViewModel : BaseViewModel
    {
        public Import Import { get; set; } = new Import(LogManager.GetLogger(typeof(Import)));
        public ICommand ClickImport { get; set; }
        public ICommand ClickViewFailedItems { get; set; }
        public ICommand ClickViewIgnoredItems { get; set; }
        public ICommand ClickViewImportedItems { get; set; }

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
            Serializer.WriteToFile(result.Ignored, IGNORED_ITEMS_FILENAME);
            Serializer.WriteToFile(result.Failed, FAILED_ITEMS_FILENAME);
            Serializer.WriteToFile(result.Imported, IMPORTED_ITEMS_FILENAME);
        }

        private void ClickImportCommand(object obj = null)
        {
            if (!Import.IsReady())
                return;

            Import.UpdateStatus(StatusMessage.Create(TransferStatus.Started, "Importing..."));
            Task.Run(() =>
            {
                try
                {
                    DBService db = new DBService(ConnectionString.GetString());
                    List<ItemPriceDTO> items = Serializer.FromCSV(Import.Path);
                    ImportResult results = db.UpdateItemPrices(items);
                    ResultMessage message = ResultMessage.Create(items, results);
                    ImportResultsToFile(results);
                    Import.UpdateStatus(StatusMessage.Create(TransferStatus.Success, message.Generate()));
                }
                catch (Exception ex)
                {
                    Import.UpdateStatus(StatusMessage.Create(TransferStatus.Failed, "Unable to read import file.", ex));
                }

                Import.UpdateStatus(StatusMessage.Create(TransferStatus.Stopped));

            });
        }

    }
}
