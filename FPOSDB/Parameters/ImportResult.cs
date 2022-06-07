using FPOSDB.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPOSDB.Parameters
{
    public interface IImportResult<T> where T : IModel
    {
        int DistinctFailedCount { get; }
        int DistinctIgnoredCount { get; }
        int DistinctImportedCount { get; }
        List<T> Failed { get; set; }
        List<T> Ignored { get; set; }
        List<T> Imported { get; set; }
    }

    public class ImportResult<T> : IImportResult<T> where T : IModel
    {
        public int DistinctImportedCount { get { return this.Imported.Select(x => x.DisplayName).Distinct().Count(); } }
        public int DistinctFailedCount { get { return this.Failed.Select(x => x.DisplayName).Distinct().Count(); } }
        public int DistinctIgnoredCount { get { return this.Ignored.Select(x => x.DisplayName).Distinct().Count(); } }
        public List<T> Imported { get; set; } = new List<T>();
        public List<T> Failed { get; set; } = new List<T>();
        public List<T> Ignored { get; set; } = new List<T>();

    }
}
