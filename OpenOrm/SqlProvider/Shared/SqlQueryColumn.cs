using System;
using System.Data;
using System.Reflection;

namespace OpenOrm.SqlProvider.Shared
{
    public class SqlQueryColumn
    {
        public string ColumnName { get; set; }
        public string ParameterName { get; set; }
        public object ParameterValue { get; set; }
        public Type ParameterType { get; set; }
        public SqlDbType SqlDbType { get; set; }
        public string SqlDbTypeString { get; set; }
        public string ParameterValueFormattedString { get; set; }
        public PropertyInfo Property { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsNullable { get; set; }
        public object DefaultValue { get; set; }
    }
}
