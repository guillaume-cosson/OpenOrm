using OpenOrm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Models
{
    public class GlobalConfig
    {
        [DbPrimaryKey, DbAutoIncrement]
        public long Id { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
