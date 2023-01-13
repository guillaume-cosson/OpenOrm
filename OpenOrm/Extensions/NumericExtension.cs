using System;
using System.Collections.Generic;
using System.Text;

namespace OpenOrm.Extensions
{
    public static partial class Extension
    {
        #region Convert
        public static int ToInt(this long l)
        {
            return Convert.ToInt32(l);
        }
        public static int ToInt(this long? l)
        {
            return Convert.ToInt32(l.HasValue ? l.Value : 0);
        }
        public static int ToInt(this decimal d)
        {
            return Convert.ToInt32(d);
        }
        public static int ToInt(this decimal? d)
        {
            return Convert.ToInt32(d.HasValue ? d.Value : 0);
        }
        public static int ToInt(this double d)
        {
            return Convert.ToInt32(d);
        }
        public static int ToInt(this double? d)
        {
            return Convert.ToInt32(d.HasValue ? d.Value : 0);
        }



        public static long ToInt64(this int i)
        {
            return Convert.ToInt64(i);
        }
        public static long ToInt64(this int? i)
        {
            return Convert.ToInt64(i.HasValue ? i.Value : 0);
        }
        public static long ToInt64(this decimal d)
        {
            return Convert.ToInt64(d);
        }
        public static long ToInt64(this decimal? d)
        {
            return Convert.ToInt64(d.HasValue ? d.Value : 0);
        }
        public static long ToInt64(this double d)
        {
            return Convert.ToInt64(d);
        }
        public static long ToInt64(this double? d)
        {
            return Convert.ToInt64(d.HasValue ? d.Value : 0);
        }



        public static double ToDouble(this int d)
        {
            return Convert.ToDouble(d);
        }
        public static double ToDouble(this int? d)
        {
            return Convert.ToDouble(d.HasValue ? d.Value : 0);
        }
        public static double ToDouble(this long d)
        {
            return Convert.ToDouble(d);
        }
        public static double ToDouble(this long? d)
        {
            return Convert.ToDouble(d.HasValue ? d.Value : 0);
        }
        public static double ToDouble(this decimal d)
        {
            return Convert.ToDouble(d);
        }
        public static double ToDouble(this decimal? d)
        {
            return Convert.ToDouble(d.HasValue ? d.Value : 0);
        }



        public static decimal ToDecimal(this int d)
        {
            return Convert.ToDecimal(d);
        }
        public static decimal ToDecimal(this int? d)
        {
            return Convert.ToDecimal(d.HasValue ? d.Value : 0);
        }
        public static decimal ToDecimal(this long d)
        {
            return Convert.ToDecimal(d);
        }
        public static decimal ToDecimal(this long? d)
        {
            return Convert.ToDecimal(d.HasValue ? d.Value : 0);
        }
        public static decimal ToDecimal(this double d)
        {
            return Convert.ToDecimal(d);
        }
        public static decimal ToDecimal(this double? d)
        {
            return Convert.ToDecimal(d.HasValue ? d.Value : 0);
        }
        #endregion

        #region Limit
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
        #endregion

        #region Clamp
        public static int Clamp(this int d, int max)
        {
            if (d > max) return max;
            return d;
        }
        public static int Clamp(this int d, int min, int max)
        {
            if (d < min) return min;
            if (d > max) return max;
            return d;
        }
        public static long Clamp(this long d, long max)
        {
            if (d > max) return max;
            return d;
        }
        public static long Clamp(this long d, long min, long max)
        {
            if (d < min) return min;
            if (d > max) return max;
            return d;
        }
        public static decimal Clamp(this decimal d, decimal max)
        {
            if (d > max) return max;
            return d;
        }
        public static decimal Clamp(this decimal d, decimal min, decimal max)
        {
            if (d < min) return min;
            if (d > max) return max;
            return d;
        }
        public static double Clamp(this double d, double max)
        {
            if (d > max) return max;
            return d;
        }
        public static double Clamp(this double d, double min, double max)
        {
            if (d < min) return min;
            if (d > max) return max;
            return d;
        }
        public static float Clamp(this float d, float max)
        {
            if (d > max) return max;
            return d;
        }
        public static float Clamp(this float d, float min, float max)
        {
            if (d < min) return min;
            if (d > max) return max;
            return d;
        }
        #endregion

        #region FromPercentage
        public static double FromPercentage(this int value, int percentage)
        {
            return Math.Floor(((double)percentage / 100.0d) * (double)value);
        }

        public static double FromPercentage(this int value, double percentage)
        {
            return Math.Floor((percentage / 100.0d) * (double)value);
        }

        public static double FromPercentage(this int value, decimal percentage)
        {
            return Math.Floor(((double)percentage / 100.0d) * value);
        }


        public static double FromPercentage(this double value, int percentage)
        {
            return Math.Floor(((double)percentage / 100.0d) * (double)value);
        }

        public static double FromPercentage(this double value, double percentage)
        {
            return Math.Floor((percentage / 100.0d) * (double)value);
        }

        public static double FromPercentage(this double value, decimal percentage)
        {
            return Math.Floor(((double)percentage / 100.0d) * value);
        }


        public static decimal FromPercentage(this decimal value, int percentage)
        {
            return Math.Floor(((decimal)percentage / 100.0m) * value);
        }

        public static double FromPercentage(this decimal value, double percentage)
        {
            return Math.Floor((percentage / 100.0d) * (double)value);
        }

        public static decimal FromPercentage(this decimal value, decimal percentage)
        {
            return Math.Floor((percentage / 100.0m) * value);
        }
        #endregion

        #region In
        public static bool In(this int value, params int[] inValues)
        {
            for (int i = 0; i < inValues.Length; i++)
            {
                if (inValues[i] == value) return true;
            }
            return false;
        }

        public static bool In(this long value, params long[] inValues)
        {
            for (int i = 0; i < inValues.Length; i++)
            {
                if (inValues[i] == value) return true;
            }
            return false;
        }
        #endregion
    }
}
