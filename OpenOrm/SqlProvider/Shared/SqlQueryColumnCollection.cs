using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenOrm.SqlProvider.Shared
{
    public class SqlQueryColumnCollection
    {
        List<SqlQueryColumn> Parameters { get; set; }

        public SqlQueryColumnCollection()
        {
            Parameters = new List<SqlQueryColumn>();
        }

        public string ToColumnNames()
        {
            return string.Join(",", Parameters.Select(x => "[" + x.ColumnName + "]"));
        }

        public string ToParameterNames()
        {
            return string.Join(",", Parameters.Select(x => x.ParameterName));
        }

        public string ToColumnsWithType()
        {
            List<string> result = new List<string>();

            foreach (SqlQueryColumn c in Parameters)
            {
                result.Add($"");
            }

            return string.Join(",", result);
        }

        public string ToValues()
        {
            return string.Join(",", Parameters.Select(x => x.ParameterValueFormattedString));
        }

        public List<SqlParameterItem> ToParameters()
        {
            List<SqlParameterItem> result = new List<SqlParameterItem>();
            foreach (SqlQueryColumn c in Parameters)
            {
                result.Add(new SqlParameterItem
                {
                    Name = c.ParameterName,
                    Value = c.ParameterValue
                });
            }
            return result;
        }

        //public SqlCommandBuilder Build()
        //{
        //    SqlCommandBuilder scb = new SqlCommandBuilder();

        //    scb.

        //    return scb;
        //}

        //public List<SqlParameter> ToSqlParameters()
        //{
        //    List<SqlParameter> result = new List<SqlParameter>();

        //    foreach (SqlQueryColumn c in Parameters)
        //    {
        //        result.Add(new SqlParameter
        //        {
        //            ParameterName = c.ParameterName,
        //            Value = c.ParameterValue == null && c.DefaultValue != null ? c.DefaultValue : c.ParameterValue,
        //            SqlDbType = c.SqlDbType,
        //            Direction = System.Data.ParameterDirection.Input,
        //            IsNullable = c.IsNullable,

        //        });
        //    }

        //    return result;
        //}
    }
}
