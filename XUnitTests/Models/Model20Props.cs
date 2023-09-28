using OpenOrm;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTests.Models
{
    [DbModel]
    public class Model20Props
    {
        [DbPrimaryKey, DbAutoIncrement]
        public long Id { get; set; }




        [DbDecimalSize(8, 2)]
        public decimal dec1 { get; set; }
        [DbDecimalSize(12, 2)]
        public decimal dec2 { get; set; }
        [DbDecimalSize(16, 2), DbNotNull(2.5)]
        public decimal dec3 { get; set; }
        public decimal dec4 { get; set; }



        public string str1 { get; set; }
        [DbSize(256)]
        public string str2 { get; set; }
        [DbNotNull("defval")]
        public string str3 { get; set; }
        public string str4 { get; set; }



        [DbDecimalSize(8, 2)]
        public double dbl1 { get; set; }
        public double dbl2 { get; set; }
        public double dbl3 { get; set; }
        public double dbl4 { get; set; }

        

        public DateTime dt1 { get; set; }
        public DateTime dt2 { get; set; }
        public DateTime dt3 { get; set; }
        public DateTime dt4 { get; set; }



        [DbNotNull(1)]
        public long lng1 { get; set; }
        [DbNotNull(2)]
        public long lng2 { get; set; }
        [DbNotNull(3)]
        public long lng3 { get; set; }
        [DbNotNull(4)]
        public long lng4 { get; set; }

        public bool testbool { get; set; }

    }
}
