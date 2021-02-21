using FPOSDB.DTO;
using FPOSDB.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPOSPriceUpdater.BusinessLogic
{
    public class ResultMessage 
    {
        private readonly List<ItemPriceDTO> csvItems;
        private readonly ImportResult result;

        private ResultMessage(List<ItemPriceDTO> csvItems, ImportResult results) 
        {
            this.csvItems = csvItems;
            this.result = results;
        }
        public static ResultMessage Create(List<ItemPriceDTO> csvItems, ImportResult results)
        {
            return new ResultMessage(csvItems, results);
        }
        public String Generate()
        {
            var ignoredItems = result.DistinctIgnoredCount;
            var importedItems = result.DistinctImportedCount;
            var failedItems = result.DistinctFailedCount;
            var processedItems = failedItems + importedItems + ignoredItems;
            var csvItemCount = csvItems.Select(x => x.ItemName).Distinct().Count();

            string message = "Import Complete" + Environment.NewLine;
            message += "Total Items In File:  " + csvItems + Environment.NewLine;
            message += "Total Processed:  " + processedItems + Environment.NewLine;
            message += "Total Ignored:  " + ignoredItems + Environment.NewLine;
            message += "Total Imported:  " + importedItems + Environment.NewLine;
            message += "Total Failed:  " + failedItems + Environment.NewLine;

            return message;
        }
    }
}
