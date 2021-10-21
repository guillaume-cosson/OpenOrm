using OpenOrm.Schema;
using OpenOrm.SqlProvider.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace OpenOrm.SqlProvider.SqlServer
{
    public class SqlServerExpressionParser : LinqExpressionParser
    {
        public string SqlWithParameters
        {
            get
            {
                string tmp = Sql.ToString();
                foreach (SqlParameterItem p in Parameters)
                {
                    tmp = tmp.Replace(p.Name, SqlServerTools.FormatValueToString(p.Value));
                }
                return tmp;
            }
        }

        private SqlServerExpressionParser()
        {
            Parameters = new List<SqlParameterItem>();
        }

        public static SqlServerExpressionParser Build<T>(Expression<Func<T, bool>> predicate, TableDefinition td)
        {
            SqlServerExpressionParser sb = new SqlServerExpressionParser();
            sb.BuildFromExpression(predicate.Body, td);
            return sb;
        }

        public static string BuildSqlWhereFromExpression<T>(Expression<Func<T, bool>> predicate, TableDefinition td, out List<SqlParameterItem> parameters)
        {
            SqlServerExpressionParser sb = new SqlServerExpressionParser();
            sb.BuildFromExpression(predicate.Body, td);
            parameters = sb.Parameters;
            return sb.Sql;
        }

        public static string BuildSqlWhereFromExpressionWithParameters<T>(Expression<Func<T, bool>> predicate, TableDefinition td)
        {
            SqlServerExpressionParser sb = new SqlServerExpressionParser();
            sb.BuildFromExpression(predicate.Body, td);
            return sb.SqlWithParameters;
        }

        internal override string GetContainsSqlString(string sql, string param_name, ExpressionType exptype)
        {
            if (exptype == ExpressionType.Not)
            {
                return $" CHARINDEX ({param_name}, {sql} COLLATE Latin1_General_CS_AS) <= 0 "; //case sensitive
            }
            else
            {
                return $" CHARINDEX ({param_name}, {sql} COLLATE Latin1_General_CS_AS) > 0 "; //case sensitive
            }
        }

        internal override string GetStartsWithSqlString(string sql, string param_name, ExpressionType exptype)
        {
            if (exptype == ExpressionType.Not)
            {
                return $" (LEFT({sql}, LEN({param_name})) <> {param_name}) ";
            }
            else
            {
                return $" (LEFT({sql}, LEN({param_name})) = {param_name}) ";
            }
        }

        internal override string GetEndsWithSqlString(string sql, string param_name, ExpressionType exptype)
        {
            if (exptype == ExpressionType.Not)
            {
                return $" (RIGHT({sql}, LEN({param_name})) <> {param_name}) ";
            }
            else
            {
                return $" (RIGHT({sql}, LEN({param_name})) = {param_name}) ";
            }
        }

        internal override string GetIsNullOrEmptySqlString(string sql, ExpressionType exptype)
        {
            if(exptype == ExpressionType.Not)
            {
                return $" NULLIF(REPLACE(ISNULL({sql}, ''), ' ', ''), '') IS NOT NULL ";
            }
            else
            {
                return $" NULLIF(REPLACE(ISNULL({sql}, ''), ' ', ''), '') IS NULL ";
            }
        }

        internal override string GetInQuerySqlString(string sql, string parameters, ExpressionType exptype)
        {
            if (exptype == ExpressionType.Not)
            {
                return sql + $" NOT IN ({parameters})";
            }
            else
            {
                return sql + $" IN ({parameters})";
            }
        }

        internal override string GetContainsFormatedValue(string value)
        {
            return value;
        }

        internal override string GetStartsWithFormatedValue(string value)
        {
            return value;
        }

        internal override string GetEndsWithFormatedValue(string value)
        {
            return value;
        }

        internal override string GetToUpperMethod(string value)
        {
            return $" UPPER({value})";
        }

        internal override string GetToLowerMethod(string value)
        {
            return $" LOWER({value})";
        }



        #region Tools
        internal override string NodeTypeToString(ExpressionType nodeType)
        {
            switch (nodeType)
            {
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Equal:
                    return " = ";
                case ExpressionType.GreaterThan:
                    return " > ";
                case ExpressionType.GreaterThanOrEqual:
                    return " >= ";
                case ExpressionType.LessThan:
                    return " < ";
                case ExpressionType.LessThanOrEqual:
                    return " <= ";
                case ExpressionType.NotEqual:
                    return " <> ";
                case ExpressionType.OrElse:
                    return " OR ";
                case ExpressionType.Not:
                    return " NOT ";
                case ExpressionType.Add:
                    return " + ";
                case ExpressionType.And:
                    return " & ";
                case ExpressionType.Divide:
                    return " / ";
                case ExpressionType.ExclusiveOr:
                    return " ^ ";
                case ExpressionType.Modulo:
                    return " % ";
                case ExpressionType.Multiply:
                    return " * ";
                case ExpressionType.Negate:
                    return " - ";
                case ExpressionType.Or:
                    return " | ";
                case ExpressionType.Subtract:
                    return " - ";
                default: return "";
            }
            throw new Exception($"Unsupported node type: {nodeType}");
        }
        #endregion
    }



    public static class ExpressionExtentions
    {
        public static string ToSqlWhere<T>(this Expression<Func<T, bool>> predicate, TableDefinition td, out List<SqlParameterItem> parameters)
        {
            SqlServerExpressionParser sb = SqlServerExpressionParser.Build(predicate, td);
            parameters = sb.Parameters;
            return sb.Sql;
        }

        public static string ToSqlWhereWithParameters<T>(this Expression<Func<T, bool>> predicate, TableDefinition td)
        {
            SqlServerExpressionParser sb = SqlServerExpressionParser.Build(predicate, td);
            return sb.SqlWithParameters;
        }
    }
}
