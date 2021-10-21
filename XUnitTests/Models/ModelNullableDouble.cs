using OpenOrm;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTests.Models
{
    public class ModelNullableDouble : DbModel
    {
        [DbPrimaryKey, DbAutoIncrement]
        public long Id { get; set; }
        public double? NullableDouble_test { get; set; }
    }
}
