using OpenOrm;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTests.Models
{
    public class ModelStringNotNull : DbModel
    {
        [DbPrimaryKey, DbAutoIncrement]
        public long Id { get; set; }
        [DbNotNull("default-string!")]
        public string DefaultString_test { get; set; }
    }
}
