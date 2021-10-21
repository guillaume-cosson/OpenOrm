using System;
using System.Collections.Generic;
using System.Text;

namespace OpenOrm.Extensions
{
    public static partial class Extension
    {
        public static string GetMessage(this Exception ex)
        {
            string message = ex.Message;
            if (ex.InnerException != null)
            {
                message += Environment.NewLine + GetMessage(ex.InnerException);
            }
            return message;
        }
    }
}
