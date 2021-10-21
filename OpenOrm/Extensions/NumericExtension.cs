using System;
using System.Collections.Generic;
using System.Text;

namespace OpenOrm.Extensions
{
    public static partial class Extension
    {
        public static int ToInt(this long l)
        {
            return Convert.ToInt32(l);
        }
        public static int ToInt(this decimal d)
        {
            return Convert.ToInt32(d);
        }
        public static int ToInt(this double d)
        {
            return Convert.ToInt32(d);
        }

        public static long ToInt64(this int i)
        {
            return Convert.ToInt64(i);
        }
        public static long ToInt64(this decimal d)
        {
            return Convert.ToInt64(d);
        }
        public static long ToInt64(this double d)
        {
            return Convert.ToInt64(d);
        }

        public static double ToDouble(this int d)
        {
            return Convert.ToDouble(d);
        }
        public static double ToDouble(this long d)
        {
            return Convert.ToDouble(d);
        }
        public static double ToDouble(this decimal d)
        {
            return Convert.ToDouble(d);
        }

        public static decimal ToDecimal(this int d)
        {
            return Convert.ToDecimal(d);
        }
        public static decimal ToDecimal(this long d)
        {
            return Convert.ToDecimal(d);
        }
        public static decimal ToDecimal(this double d)
        {
            return Convert.ToDecimal(d);
        }

        public static int Limit(this int d, int limit)
        {
            if (d > limit) return limit;
            return d;
        }
        public static int Limit(this int d, int min, int max)
        {
            if (d < min) return min;
            if (d > max) return max;
            return d;
        }
        public static long Limit(this long d, long limit)
        {
            if (d > limit) return limit;
            return d;
        }
        public static long Limit(this long d, long min, long max)
        {
            if (d < min) return min;
            if (d > max) return max;
            return d;
        }
        public static decimal Limit(this decimal d, decimal limit)
        {
            if (d > limit) return limit;
            return d;
        }
        public static decimal Limit(this decimal d, decimal min, decimal max)
        {
            if (d < min) return min;
            if (d > max) return max;
            return d;
        }
        public static double Limit(this double d, double limit)
        {
            if (d > limit) return limit;
            return d;
        }
        public static double Limit(this double d, double min, double max)
        {
            if (d < min) return min;
            if (d > max) return max;
            return d;
        }
        public static float Limit(this float d, float limit)
        {
            if (d > limit) return limit;
            return d;
        }
        public static float Limit(this float d, float min, float max)
        {
            if (d < min) return min;
            if (d > max) return max;
            return d;
        }

    }
}
