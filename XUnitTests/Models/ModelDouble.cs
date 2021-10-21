using OpenOrm;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTests.Models
{
    public class ModelDouble : DbModel
    {
        [DbPrimaryKey, DbAutoIncrement]
        public long Id { get; set; }
        public double Double_test { get; set; }
    }
}
