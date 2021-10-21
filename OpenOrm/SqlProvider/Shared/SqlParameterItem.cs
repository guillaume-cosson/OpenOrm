using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OpenOrm.SqlProvider.Shared
{
    public struct SqlParameterItem
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public SqlDbType SqlDbType { get; set; }

        public SqlParameterItem(string name, object value, SqlDbType type)
        {
            Name = name;
            Value = value;
            SqlDbType = type;
        }

        public SqlParameterItem(string name, object value)
        {
            Name = name;
            Value = value;
            SqlDbType = OpenOrmTools.ToSqlDbType(value.GetType());
        }

        public override string ToString()
        {
            return $"{Name} | {Value} | {SqlDbType}";
        }

        //public string FormattedValue
        //{
        //    get
        //    {
        //        if (Value is string)
        //        {
        //            return $"'{Value.ToString().Replace("'", "''")}'";
        //        }
        //        else if (Value is DateTime)
        //        {
        //            return $"'{(DateTime)Value:dd/MM/yyyy HH:mm:ss}'";
        //        }
        //        else
        //        {
        //            return Value.ToString();
        //        }
        //    }
        //}

        //public string GetSqlType()
        //{
        //    if(Value is string)
        //    {
        //        return "NVARCHAR(MAX)";
        //    }
        //    else if(Value is bool)
        //    {
        //        return "BIT";
        //    }
        //    else if(Value is decimal || Value is float || Value is double)
        //    {
        //        return "DECIMAL(10,2)";
        //    }
        //    else if(Value is DateTime)
        //    {
        //        return "DATETIME";
        //    }

        //    return "NVARCHAR(MAX)";
        //}
    }
}
