using OpenOrm;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTests.Models
{
    public class ModelStringUnique : DbModel
    {
        [DbPrimaryKey, DbAutoIncrement]
        public long Id { get; set; }
        [DbUnique, DbSize(20)]
        public string String_unique { get; set; }
    }
}
