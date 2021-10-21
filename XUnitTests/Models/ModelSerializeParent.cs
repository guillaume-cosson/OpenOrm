using OpenOrm;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTests.Models
{
    public class ModelSerializeParent
    {
        [DbPrimaryKey, DbAutoIncrement]
        public long Id { get; set; }

        public string str { get; set; }
        public List<string> lst { get; set; }
        [DbSerialize]
        public List<string> lst_serialize { get; set; }
        [DbSerialize]
        public ObjectSerialize obj_serialize { get; set; }
        [DbSerialize]
        public List<ObjectSerialize> lstobj_serialize { get; set; }
    }
}
