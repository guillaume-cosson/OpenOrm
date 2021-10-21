using OpenOrm;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTests.Models
{
    public class ModelInt : DbModel
    {
        #region Properties
        [DbPrimaryKey, DbAutoIncrement]
        public long Id { get; set; }
        public int Integer_test { get; set; }
        #endregion
    }
}
