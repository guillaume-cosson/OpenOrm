using OpenOrm;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTests.Models
{
    public class ModelKeywordColName
    {
        [DbPrimaryKey, DbAutoIncrement]
        public long Id { get; set; }
        public string Order { get; set; }
        public string Where { get; set; }
        public string Create { get; set; }
        public string Update { get; set; }
        public string Delete { get; set; }
        public string Declare { get; set; }
        public string With { get; set; }
        public string Constraint { get; set; }
        public string NULL { get; set; }
        public string Primary { get; set; }
        public string Inner { get; set; }
    }
}
