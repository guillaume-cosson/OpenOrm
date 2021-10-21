using OpenOrm;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTests.Models
{
    public class Read
    {
        [DbPrimaryKey, DbAutoIncrement]
        public long Id { get; set; }
        public long int64 { get; set; }
        public int Order { get; set; }
        public string Group { get; set; }
        public string Where { get; set; }
        public string Select { get; set; }
        public string Drop { get; set; }
        public string Primary { get; set; }
        public string Key { get; set; }
        public string PrimaryKey { get; set; }
        public string With { get; set; }
    }
}
