using OpenOrm;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTests.Models
{
    public class ModelMultiplePK
    {
        [DbPrimaryKey, DbAutoIncrement]
        public long Id { get; set; }
        [DbPrimaryKey, DbSize(10)]
        public string SecondKey { get; set; }
        public string value3 { get; set; }
    }
}
