using FPOSDB.Context;
using FPOSDB.DTO;
using FPOSPriceUpdater.BusinessLogic;
using FPOSPriceUpdater.Helper;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FPOSPriceUpdater.ViewModels
{
    public class ExportViewModel : BaseViewModel
    {
        public Export Export { get; set; } = new Export(LogManager.GetLogger(typeof(Export)));
    
        public ICommand ClickExport { get; set; }

        public ExportViewModel()
        {
            Export.FullPath = $"{Export.Path}\\export.csv";
            ClickExport = new RelayCommand<object>(ClickExportCommand);
        }

        private void ClickExportCommand(object obj = null)
        {
            if (!Export.IsReady())
                return;
            Task.Run(() =>
            {
                try
                {
                    UpdateExportStatusToStarted();
                    List<ItemPriceDTO> items = GetItemPricesFromDb();
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

        public List<ItemPriceDTO> GetItemPricesFromDb()
        {
            DBService db = new DBService(ConnectionString.GetString());
            List<ItemPriceDTO> items = new List<ItemPriceDTO>();
            items = db.GetAllItemPrices();
            return items;
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

        public void ExportItemsToCsv(List<ItemPriceDTO> items)
        {
             Serializer.ExportDataToCsv(items, Export.FullPath);
        }

        public void Visibility(bool isVisible)
        {
            if (!isVisible)
                Export.UpdateStatus(StatusMessage.Create(TransferStatus.Reset));
        }
    }
}
