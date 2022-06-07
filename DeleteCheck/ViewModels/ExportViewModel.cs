using FPOSDB.Context;
using FPOSDB.DTO;
using FPOSUpdater.BusinessLogic;
using FPOSUpdater.Helper;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FPOSUpdater.ViewModels
{
    public class ExportViewModel : BaseViewModel
    {
        public Export Export { get; set; } = new Export(LogManager.GetLogger(typeof(Export)));

        public ICommand ClickExport { get; set; }

        public ICommand ClickExportButtons { get; set; }
        public ExportViewModel()
        {
            Export.FullPath = $"{Export.Path}\\export.csv";
            ClickExport = new RelayCommand<object>(ClickExportCommand);
            ClickExportButtons = new RelayCommand<object>(ClickExportButtonsCommand);
        }

        private void ClickExportCommand(object obj = null)
        {
            if (!Export.IsReady())
                return;
            Task.Run(() =>
            {
                try
                {
                    Export.FullPath = $"{Export.Path}\\exportItems.csv";
                    UpdateExportStatusToStarted();
                    List<ItemPrice> items = GetItemPricesFromDb();
                    ExportItemsToCsv(items);
                    UpdateExportStatusToSuccess(items.Select(x => x.ItemName).Distinct().Count());
                }
                catch (SqlException ea)
                {
                    UpdateExportStatusToFailed("An error occured with the database.", ea);
                }
                catch (IOException ez)
                {
                    UpdateExportStatusToFailed("Unable to write to export file. ", ez);
                }
                catch (Exception ex)
                {
                    UpdateExportStatusToFailed("Unable to export.", ex);
                }
                UpdateExportStatusToStopped();


            });
        }

        private void ClickExportButtonsCommand(object obj = null)
        {
            if (!Export.IsReady())
                return;
            Task.Run(() =>
            {
                try
                {
                    Export.FullPath = $"{Export.Path}\\exportButtons.csv";
                    UpdateExportStatusToStarted();
                    var buttons = GetButtonsTextFromDb();
                    ExportButtonsToCsv(buttons);
                    UpdateExportStatusToSuccess(buttons.Select(x => x.DisplayName).Distinct().Count());
                }
                catch (SqlException ea)
                {
                    UpdateExportStatusToFailed("An error occured with the database.", ea);
                }
                catch (IOException ez)
                {
                    UpdateExportStatusToFailed("Unable to write to export file. ", ez);
                }
                catch (Exception ex)
                {
                    UpdateExportStatusToFailed("Unable to export.", ex);
                }
                UpdateExportStatusToStopped();


            });
        }

        public List<ItemPrice> GetItemPricesFromDb()
        {
            DBService db = new DBService(ConnectionString.GetString());
            var items = db.GetAllItemPrices();
            return items;
        }

        public List<Button> GetButtonsTextFromDb()
        {
            DBService db = new DBService(ConnectionString.GetString());
            var buttons = db.GetAllButtons();
            return buttons;
        }

        public void UpdateExportStatusToStarted()
        {
            Export.UpdateStatus(StatusMessage.Create(TransferStatus.Started, "Exporting..."));
        }
        public void UpdateExportStatusToStopped()
        {
            Export.UpdateStatus(StatusMessage.Create(TransferStatus.Stopped));
        }
        public void UpdateExportStatusToSuccess(int numberOfItemsExported)
        {
            Export.UpdateStatus(StatusMessage.Create(TransferStatus.Success, $"{numberOfItemsExported} items exported {Environment.NewLine} {Export.FullPath}"));
        }
        public void UpdateExportStatusToFailed(string errorMessage, Exception e)
        {
            Export.UpdateStatus(StatusMessage.Create(TransferStatus.Failed, errorMessage, e));
        }

        public void ExportItemsToCsv(List<ItemPrice> items)
        {
             Serializer.ExportDataToCsv(items, Export.FullPath);
        }

        public void ExportButtonsToCsv(List<Button> buttons)
        {
            Serializer.ExportDataToCsv(buttons, Export.FullPath);
        }

        public void Visibility(bool isVisible)
        {
            if (!isVisible)
                Export.UpdateStatus(StatusMessage.Create(TransferStatus.Reset));
        }
    }
}
