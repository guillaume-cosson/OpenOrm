using System;
using System.Collections.Generic;
using System.Text;

namespace OpenOrm
{
    public static partial class OpenOrmTools
    {
        public static string Coalesce(params string[] p)
        {
            if (p == null || p.Length == 0) return string.Empty;
            for (int i = 0; i < p.Length; i++)
            {
                if (!string.IsNullOrEmpty(p[i])) return p[i];
            }
            return string.Empty;
        }
    }
}
