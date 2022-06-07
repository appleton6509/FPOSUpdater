using FPOSDB.DTO;
using FPOSDB.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPOSUpdater.BusinessLogic
{
    public class ResultMessage<T> where T : BaseModel<T>
    {
        private readonly List<T> csvItems;
        private readonly IImportResult<T> result;

        private ResultMessage(List<T> csvItems, IImportResult<T> results) 
        {
            this.csvItems = csvItems;
            this.result = results;
        }
        public static ResultMessage<T> Create(List<T> csvItems, IImportResult<T> results)
        {
            return new ResultMessage<T>(csvItems, results);
        }
        public string Generate()
        {
            var ignoredItems = result.DistinctIgnoredCount;
            var importedItems = result.DistinctImportedCount;
            var failedItems = result.DistinctFailedCount;
            var processedItems = failedItems + importedItems + ignoredItems;
            var csvItemCount = csvItems.Select(x => x.DisplayName).Distinct().Count();

            string message = "Import Complete" + Environment.NewLine;
            message += "Total In File:  " + csvItemCount + Environment.NewLine;
            message += "Total Processed:  " + processedItems + Environment.NewLine;
            message += "Total Ignored:  " + ignoredItems + Environment.NewLine;
            message += "Total Imported:  " + importedItems + Environment.NewLine;
            message += "Total Failed:  " + failedItems + Environment.NewLine;

            return message;
        }
    }
}
