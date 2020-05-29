using FPOSDB.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPOSDB.Parameters
{
    public class ImportResult
    {
        public int DistinctImportedCount { get { return this.Imported.Select(x => x.ItemName).Distinct().Count(); } }
        public int DistinctFailedCount { get { return this.Failed.Select(x => x.ItemName).Distinct().Count(); } }
        public int DistinctIgnoredCount { get { return this.Ignored.Select(x => x.ItemName).Distinct().Count(); } }
        public List<ItemPriceDTO> Imported { get; set; } = new List<ItemPriceDTO>();
        public List<ItemPriceDTO> Failed { get; set; } = new List<ItemPriceDTO>();
        public List<ItemPriceDTO> Ignored { get; set; } = new List<ItemPriceDTO>();

    }
}
