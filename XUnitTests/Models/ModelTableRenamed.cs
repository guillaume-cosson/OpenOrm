using OpenOrm;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTests.Models
{
    [DbModel("Renamed")]
    public class ModelTableRenamed
    {
        [DbPrimaryKey, DbAutoIncrement]
        public long Id { get; set; }
        public string Str { get; set; }
    }
}
