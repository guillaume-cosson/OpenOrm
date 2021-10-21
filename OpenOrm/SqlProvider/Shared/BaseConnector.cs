using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace OpenOrm.SqlProvider.Shared
{
    public class BaseConnector
    {
        public ArrayList Parameters { get; set; }

        public BaseConnector()
        {
            Parameters = new ArrayList();
        }

        public int GetParamSize(object value)
        {
            if (value is string)
            {
                return value.ToString().Length;
            }
            else
            {
                return 0;
            }
        }
    }
}
