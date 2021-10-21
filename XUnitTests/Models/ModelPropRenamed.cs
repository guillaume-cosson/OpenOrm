using OpenOrm;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTests.Models
{
    public class ModelPropRenamed
    {
        [DbPrimaryKey, DbAutoIncrement]
        public long Id { get; set; }
        [DbColumnName("Renamed")]
        public string Str { get; set; }
    }
}
