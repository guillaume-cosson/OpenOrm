using OpenOrm.CoreTools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace OpenOrm.SqlProvider.SQLite
{
    public static class SQLiteTools
    {
        
        public static string FormatValueToString(object value)
        {
            if (value != null)
            {
                string formattedValue = "NULL";
                string ptype = value.GetType().Name.ToLower();
                switch (ptype)
                {
                    case "string":
                        formattedValue = $"\"{value.ToString().Replace("\"", "'")}\"";
                        break;
                    case "char":
                        //string s = string.Concat("", value);
                        //formattedValue = $"\"{s}\"";
                        break;
                    case "long":
                    case "int64":
                    case "int":
                    case "int32":
                    case "int16":
                    case "short":
                        formattedValue = $"{value}";
                        break;
                    case "decimal":
                    case "double":
                    case "float":
                    case "single":
                        string decsep = CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator;
                        formattedValue = value.ToString().Replace(".", decsep).Replace(",", decsep);
                        break;
                    case "datetime":
                        formattedValue = $"\"{(DateTime)value:dd/MM/yyyy HH:mm:ss}\"";
                        break;
                    case "bool":
                    case "boolean":
                        string bv = (bool)value ? "1" : "0";
                        formattedValue = $"{bv}";
                        break;
                    case "object":
                        formattedValue = $"\"{value}\"";
                        break;
                }

                return formattedValue;
            }
            else
            {
                return "NULL";
            }
        }


        public static string ToStringType(SqlDbType dbtype)
        {
            switch (dbtype)
            {
                default:
                case SqlDbType.NVarChar:
                    return $"TEXT";
                case SqlDbType.Int:
                    return "INTEGER";
                case SqlDbType.BigInt:
                    return "INTEGER";
                case SqlDbType.DateTime:
                    return "TEXT";
                case SqlDbType.Bit:
                    return "INTEGER";
                case SqlDbType.Binary:
                    return "TEXT";
                case SqlDbType.NChar:
                    return "TEXT";
                case SqlDbType.Decimal:
                    return "REAL";
                case SqlDbType.Float:
                    return "REAL";
                case SqlDbType.Timestamp:
                    return "TEXT";
            }
        }

        public static string ToStringType(PropertyInfo pi)
        {
            return ToStringType(OpenOrmTools.ToSqlDbType(pi));
        }

        public static string ToStringType(FieldInfo fi)
        {
            return ToStringType(OpenOrmTools.ToSqlDbType(fi));
        }

        public static string ToStringType(Type type)
        {
            return ToStringType(OpenOrmTools.ToSqlDbType(type));
        }

        public static string ToStringType<T>()
        {
            return ToStringType(typeof(T));
        }

        public static SqlDbType DetectParameterType(object value)
        {
            if (value is string)
                return SqlDbType.NVarChar;
            else if (value is int)
                return SqlDbType.Int;
            else if (value is long)
                return SqlDbType.BigInt;
            else if (value is bool)
                return SqlDbType.Bit;
            else if (value is DateTime)
                return SqlDbType.DateTime;
            else if (value is Array[])
                return SqlDbType.Binary;
            else if (value is char)
                return SqlDbType.NChar;
            else if (value is decimal)
                return SqlDbType.Decimal;
            else if (value is float)
                return SqlDbType.Float;
            else if (value is TimeSpan)
                return SqlDbType.Timestamp;
            else
                return SqlDbType.NVarChar;
        }
    }
}
