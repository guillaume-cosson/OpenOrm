using OpenOrm;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTests.Models
{
    public class ModelPrivate
    {
        [DbPrimaryKey, DbAutoIncrement]
        public long Id { get; set; }
        public double Double_public { get; set; }
        private double Double_private { get; set; }

        public ModelPrivate()
        {

        }

        public ModelPrivate(double a, double b)
        {
            Double_public = a;
            Double_private = b;
        }
    }
}
