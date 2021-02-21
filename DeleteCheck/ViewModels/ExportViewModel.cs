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
            Export.UpdateStatus(StatusMessage.Create(TransferStatus.Started, "Exporting..."));
            
            Task.Run(() =>
            {
                try
                {
                    DBService db = new DBService(ConnectionString.GetString());
                    List<ItemPriceDTO> items = new List<ItemPriceDTO>();
                    items = db.GetAllItemPrices();
                    string path = $"{Export.Path}\\export.csv";
                    Serializer.ToCsv(items, path);
                    int itemsexported = items.Select(x => x.ItemName).Distinct().Count();
                    Export.UpdateStatus(StatusMessage.Create(TransferStatus.Success, $"{itemsexported} items exported {Environment.NewLine} {path}"));
                }
                catch (SqlException ea)
                {
                    Export.UpdateStatus(StatusMessage.Create(TransferStatus.Failed, "An error occured with the database.", ea));
                }
                catch (IOException ez)
                {
                    Export.UpdateStatus(StatusMessage.Create(TransferStatus.Failed, "Unable to write to export file. ", ez));
                }
                catch (Exception ex)
                {
                    Export.UpdateStatus(StatusMessage.Create(TransferStatus.Failed, "Unable to export.", ex));
                }
                Export.UpdateStatus(StatusMessage.Create(TransferStatus.Stopped));
            });
        }

        public void Visibility(bool isVisible)
        {
            if (!isVisible)
                Export.UpdateStatus(StatusMessage.Create(TransferStatus.Reset));
        }
    }
}
