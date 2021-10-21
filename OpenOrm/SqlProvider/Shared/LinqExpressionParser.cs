using OpenOrm.Schema;
using OpenOrm.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;

[assembly: InternalsVisibleTo("OpenOrm.SqlServer")]
[assembly: InternalsVisibleTo("OpenOrm.SQLite")]
[assembly: InternalsVisibleTo("OpenOrm.MySql")]

namespace OpenOrm.SqlProvider.Shared
{
    public abstract class LinqExpressionParser
    {
        public string Sql { get; set; }
        public List<SqlParameterItem> Parameters { get; set; }
        public bool WithBrackets { get; set; }

        internal enum ParserOptions
        {
            EMPTY,
            NO_PARAMETER
        }

        internal LinqExpressionParser()
        {
            Parameters = new List<SqlParameterItem>();
        }

        internal void BuildFromExpression(Expression exp, TableDefinition td, bool withBrackets = true)
        {
            WithBrackets = withBrackets;

            ParserOptions options = ParserOptions.EMPTY;
            if (exp is ConstantExpression) options = ParserOptions.NO_PARAMETER;

            Sql = RecurseExp(exp, td, options);
        }

        internal string RecurseExp(Expression exp, TableDefinition td, ParserOptions options = ParserOptions.EMPTY)
        {
            string result = "";
            //if (exp is BinaryExpression)
            //{
            //    result += "(";
            //    BinaryExpression bexp = (BinaryExpression)exp;

            //    //LEFT
            //    if (bexp.Left is BinaryExpression)
            //    {
            //        result += RecurseExp(bexp.Left, td);
            //    }
            //    else if (bexp.Left is UnaryExpression)
            //    {
            //        UnaryExpression uexp = (UnaryExpression)bexp.Left;
            //        if (uexp.Operand is MemberExpression)
            //        {
            //            MemberExpression mexp = (MemberExpression)uexp.Operand;
            //            if (mexp.Member is PropertyInfo)
            //            {
            //                PropertyInfo pi = ((PropertyInfo)mexp.Member);
            //                if (WithBrackets)
            //                {
            //                    result += $"[{td.GetColumnNameFor(pi)}]";
            //                }
            //                else
            //                {
            //                    result += td.GetColumnNameFor(pi);
            //                }

            //                if (pi.PropertyType == typeof(bool))
            //                {
            //                    if (uexp.NodeType == ExpressionType.Not)
            //                    {
            //                        result += " = 0";
            //                    }
            //                    else
            //                    {
            //                        result += " = 1";
            //                    }
            //                }
            //            }
            //            else if (mexp.Member is FieldInfo)
            //            {
            //                FieldInfo fi = ((FieldInfo)mexp.Member);
            //                var value = GetValue(mexp);
            //                string paramName = $"@p{Parameters.Count}";

            //                if (value is bool)
            //                {
            //                    Parameters.Add(new SqlParameterItem { Name = paramName, Value = (bool)value ? "1" : "0", SqlDbType = OpenOrmTools.ToSqlDbType(fi) });
            //                }
            //                else
            //                {
            //                    Parameters.Add(new SqlParameterItem { Name = paramName, Value = value, SqlDbType = OpenOrmTools.ToSqlDbType(fi) });
            //                }
            //                result += NodeTypeToString(bexp.Left.NodeType) + paramName;
            //            }
            //        }
            //        else if (uexp.Operand is MethodCallExpression)
            //        {
            //            result += ResolveMethodCallExpression(uexp.Operand, td, uexp.NodeType);
            //        }
            //    }
            //    else if (bexp.Left is MemberExpression)
            //    {
            //        MemberExpression mexp = (MemberExpression)bexp.Left;
            //        if (mexp.Member is PropertyInfo)
            //        {
            //            PropertyInfo pi = ((PropertyInfo)mexp.Member);
            //            if (WithBrackets)
            //            {
            //                result += $"[{td.GetColumnNameFor(pi)}]";
            //            }
            //            else
            //            {
            //                result += td.GetColumnNameFor(pi);
            //            }

            //            if (pi.PropertyType == typeof(bool) && bexp.NodeType != ExpressionType.Equal && bexp.NodeType != ExpressionType.NotEqual)
            //            {
            //                result += " = 1";
            //            }
            //        }
            //        else if (mexp.Member is FieldInfo)
            //        {
            //            object value = GetValue(mexp);
            //            if (value is string)
            //            {
            //                //value = prefix + (string)value + postfix;
            //                result += $"'{value.ToString().Replace("'", "''")}'";
            //            }
            //            else if (value is DateTime)
            //            {
            //                result += $"'{value}'";
            //            }
            //            else
            //            {
            //                result += $"{value}";
            //            }
            //        }
            //    }
            //    else if (bexp.Left is ConstantExpression)
            //    {
            //        var value = ((ConstantExpression)bexp.Left).Value;

            //        if (value is bool)
            //        {
            //            result += (bool)value ? "(1=1)" : "(1=0)";
            //        }
            //    }
            //    else if (bexp.Left is MethodCallExpression)
            //    {
            //        result += ResolveMethodCallExpression(bexp.Left, td, bexp.NodeType);

            //        if(/*((MethodCallExpression)bexp.Left).Method.Name == "IsNullOrEmpty" &&*/ bexp.NodeType != ExpressionType.AndAlso && bexp.NodeType != ExpressionType.OrElse)
            //        {
            //            return result + " )";
            //        }
            //        //var methodCall = (MethodCallExpression)bexp.Left;
            //        //// LIKE queries:
            //        //if (methodCall.Method == typeof(string).GetMethod("Contains", new[] { typeof(string) }))
            //        //{
            //        //    string paramName = $"@p{Parameters.Count}";
            //        //    result += GetContainsSqlString(RecurseExp(methodCall.Object, td), paramName, ExpressionType.Equal);
            //        //    Parameters.Add(new SqlParameterItem { Name = paramName, Value = GetContainsFormatedValue(RecurseExp(methodCall.Arguments[0], td)).Replace("('", "").Replace("')", ""), SqlDbType = System.Data.SqlDbType.NVarChar });
            //        //}
            //        //else if (methodCall.Method == typeof(string).GetMethod("StartsWith", new[] { typeof(string) }))
            //        //{
            //        //    string paramName = $"@p{Parameters.Count}";
            //        //    result += GetStartsWithSqlString(RecurseExp(methodCall.Object, td), paramName, ExpressionType.Equal);
            //        //    Parameters.Add(new SqlParameterItem { Name = paramName, Value = GetStartsWithFormatedValue(RecurseExp(methodCall.Arguments[0], td)).Replace("('", "").Replace("')", ""), SqlDbType = System.Data.SqlDbType.NVarChar });
            //        //}
            //        //else if (methodCall.Method == typeof(string).GetMethod("EndsWith", new[] { typeof(string) }))
            //        //{
            //        //    string paramName = $"@p{Parameters.Count}";
            //        //    result += GetEndsWithSqlString(RecurseExp(methodCall.Object, td), paramName, ExpressionType.Equal);
            //        //    Parameters.Add(new SqlParameterItem { Name = paramName, Value = GetEndsWithFormatedValue(RecurseExp(methodCall.Arguments[0], td)).Replace("('", "").Replace("')", ""), SqlDbType = System.Data.SqlDbType.NVarChar });
            //        //}
            //        //else if (methodCall.Method.Name == "IsNullOrEmpty")
            //        //{
            //        //    Expression property;
            //        //    if (methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 2)
            //        //    {
            //        //        property = methodCall.Arguments[1];
            //        //    }
            //        //    else if (!methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 1)
            //        //    {
            //        //        property = methodCall.Arguments[0];
            //        //    }
            //        //    else
            //        //    {
            //        //        throw new Exception("Unsupported method call: " + methodCall.Method.Name);
            //        //    }

            //        //    //result += $" NULLIF(REPLACE(ISNULL({RecurseExp(property, td)}, ''), ' ', ''), '') IS NULL) ";
            //        //    result += GetIsNullOrEmptySqlString(RecurseExp(property, td), ExpressionType.Equal) + ") ";
            //        //    return result;
            //        //}
            //        //// IN queries:
            //        //else if (methodCall.Method.Name == "Contains")
            //        //{
            //        //    Expression collection;
            //        //    Expression property;
            //        //    if (methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 2)
            //        //    {
            //        //        collection = methodCall.Arguments[0];
            //        //        property = methodCall.Arguments[1];
            //        //    }
            //        //    else if (!methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 1)
            //        //    {
            //        //        collection = methodCall.Object;
            //        //        property = methodCall.Arguments[0];
            //        //    }
            //        //    else
            //        //    {
            //        //        throw new Exception("Unsupported method call: " + methodCall.Method.Name);
            //        //    }

            //        //    object rawValue = GetValue(collection);
            //        //    List<string> parameters = new List<string>();
            //        //    foreach (object item in rawValue.GetEnumerable())
            //        //    {
            //        //        string paramName = $"@p{Parameters.Count}";
            //        //        parameters.Add(paramName);
            //        //        Parameters.Add(new SqlParameterItem { Name = paramName, Value = item, SqlDbType = OpenOrmTools.ToSqlDbType(item.GetType()) });
            //        //    }

            //        //    //result += RecurseExp(property, td) + $" IN ({string.Join(",", parameters)})";
            //        //    result += GetInQuerySqlString(RecurseExp(property, td), string.Join(",", parameters), ExpressionType.Equal);
            //        //}
            //        //else if (methodCall.Method.Name == "ToLower" || methodCall.Method.Name == "ToUpper")
            //        //{
            //        //    if (methodCall.Object != null)
            //        //    {
            //        //        if (methodCall.Method.Name == "ToLower") result += $"lower({RecurseExp(methodCall.Object, td)})";
            //        //        else result += $"upper({RecurseExp(methodCall.Object, td)})";
            //        //    }
            //        //    else
            //        //    {
            //        //        throw new Exception("Unsupported method call: " + methodCall.Method.Name);
            //        //    }
            //        //}
            //        //else
            //        //{
            //        //    throw new Exception("Unsupported method call: " + methodCall.Method.Name);
            //        //}
            //    }


            //    //OPERATOR
            //    result += NodeTypeToString(bexp.NodeType);


            //    //RIGHT
            //    if (bexp.Right is BinaryExpression)
            //    {
            //        result += RecurseExp(bexp.Right, td);
            //    }
            //    else if (bexp.Right is UnaryExpression)
            //    {
            //        UnaryExpression uexp = (UnaryExpression)bexp.Right;
            //        if (uexp.Operand is MemberExpression)
            //        {
            //            MemberExpression mexp = (MemberExpression)uexp.Operand;
            //            if (mexp.Member is PropertyInfo)
            //            {
            //                PropertyInfo pi = ((PropertyInfo)mexp.Member);
            //                if (WithBrackets)
            //                {
            //                    result += $"[{td.GetColumnNameFor(pi)}]";
            //                }
            //                else
            //                {
            //                    result += td.GetColumnNameFor(pi);
            //                }
            //                if (pi.PropertyType == typeof(bool))
            //                {
            //                    if (uexp.NodeType == ExpressionType.Not)
            //                    {
            //                        result += " = 0";
            //                    }
            //                    else
            //                    {
            //                        result += " = 1";
            //                    }
            //                }
            //            }
            //            else if (mexp.Member is FieldInfo)
            //            {
            //                FieldInfo fi = ((FieldInfo)mexp.Member);
            //                var value = GetValue(mexp);
            //                string paramName = $"@p{Parameters.Count}";

            //                if (value is bool)
            //                {
            //                    Parameters.Add(new SqlParameterItem { Name = paramName, Value = (bool)value ? "1" : "0", SqlDbType = OpenOrmTools.ToSqlDbType(fi) });
            //                }
            //                else
            //                {
            //                    Parameters.Add(new SqlParameterItem { Name = paramName, Value = value, SqlDbType = OpenOrmTools.ToSqlDbType(fi) });
            //                }
            //                result += NodeTypeToString(bexp.Right.NodeType) + paramName;
            //            }
            //        }
            //        else if(uexp is Expression)
            //        {
            //            //Expression mexp = uexp.Operand;
            //            result += RecurseExp(uexp, td);
            //        }
            //    }
            //    else if (bexp.Right is MemberExpression)
            //    {
            //        MemberExpression mexp = (MemberExpression)bexp.Right;
            //        if (mexp.Member is PropertyInfo)
            //        {
            //            //result += NodeTypeToString(bexp.NodeType);
            //            PropertyInfo pi = ((PropertyInfo)mexp.Member);


            //            if (mexp.NodeType == ExpressionType.MemberAccess && pi.PropertyType == typeof(bool))
            //            {
            //                if (WithBrackets)
            //                {
            //                    result += $"[{td.GetColumnNameFor(pi)}] = 1";
            //                }
            //                else
            //                {
            //                    result += $"{td.GetColumnNameFor(pi)} = 1";
            //                }
            //            }
            //            else if (mexp.Expression.NodeType == ExpressionType.MemberAccess || mexp.Expression.NodeType == ExpressionType.Constant)
            //            {
            //                var value = GetValue(mexp);
            //                string paramName = $"@p{Parameters.Count}";

            //                if (value is bool)
            //                {
            //                    Parameters.Add(new SqlParameterItem { Name = paramName, Value = (bool)value ? "1" : "0", SqlDbType = OpenOrmTools.ToSqlDbType(pi) });
            //                }
            //                else
            //                {
            //                    Parameters.Add(new SqlParameterItem { Name = paramName, Value = value, SqlDbType = OpenOrmTools.ToSqlDbType(pi) });
            //                }
            //                result += paramName;
            //            }
            //            else
            //            {
            //                if (WithBrackets)
            //                {
            //                    result += $"[{td.GetColumnNameFor(pi)}]";
            //                }
            //                else
            //                {
            //                    result += td.GetColumnNameFor(pi);
            //                }
            //            }
            //            //else
            //            //{
            //            //    string paramName = $"@p{Parameters.Count}";
            //            //    result += paramName;
            //            //    Parameters.Add(new SqlParameterItem { Name = paramName, Value = GetValue(mexp), SqlDbType = OpenOrmTools.ToSqlDbType(pi) });
            //            //}
            //            //else if (pi.PropertyType == typeof(DateTime))
            //            //{
            //            //    result += $"'{GetValue(mexp)}'";

            //            //}
            //            //else if (pi.PropertyType == typeof(string))
            //            //{
            //            //    result += $"'{GetValue(mexp)}'";
            //            //}
            //            //else
            //            //{
            //            //    result += $"{GetValue(mexp)}";
            //            //}
            //        }
            //        else if (mexp.Member is FieldInfo)
            //        {
            //            FieldInfo fi = ((FieldInfo)mexp.Member);
            //            var value = GetValue(mexp);
            //            string paramName = $"@p{Parameters.Count}";

            //            if (value is bool)
            //            {
            //                Parameters.Add(new SqlParameterItem { Name = paramName, Value = (bool)value ? "1" : "0", SqlDbType = OpenOrmTools.ToSqlDbType(fi) });
            //            }
            //            else
            //            {
            //                Parameters.Add(new SqlParameterItem { Name = paramName, Value = value, SqlDbType = OpenOrmTools.ToSqlDbType(fi) });
            //            }
            //            result += /*NodeTypeToString(bexp.NodeType) +*/ paramName;
            //        }
            //    }
            //    else if (bexp.Right is ConstantExpression)
            //    {
            //        string paramName = $"@p{Parameters.Count}";
            //        var value = ((ConstantExpression)bexp.Right).Value;

            //        if (value is bool)
            //        {
            //            Parameters.Add(new SqlParameterItem { Name = paramName, Value = (bool)value ? 1 : 0, SqlDbType = OpenOrmTools.ToSqlDbType(((ConstantExpression)bexp.Right).Type) });
            //        }
            //        else
            //        {
            //            Parameters.Add(new SqlParameterItem { Name = paramName, Value = value, SqlDbType = OpenOrmTools.ToSqlDbType(((ConstantExpression)bexp.Right).Type) });
            //        }
            //        result += /*NodeTypeToString(bexp.NodeType) +*/ paramName;
            //    }
            //    else if (bexp.Right is MethodCallExpression)
            //    {
            //        result += ResolveMethodCallExpression(bexp.Right, td, bexp.NodeType);
            //        //var methodCall = (MethodCallExpression)bexp.Right;
            //        //// LIKE queries:
            //        //if (methodCall.Method == typeof(string).GetMethod("Contains", new[] { typeof(string) }))
            //        //{
            //        //    string paramName = $"@p{Parameters.Count}";
            //        //    result += GetContainsSqlString(RecurseExp(methodCall.Object, td), paramName, ExpressionType.Equal);
            //        //    Parameters.Add(new SqlParameterItem { Name = paramName, Value = GetContainsFormatedValue(RecurseExp(methodCall.Arguments[0], td)), SqlDbType = System.Data.SqlDbType.NVarChar });
            //        //}
            //        //else if (methodCall.Method == typeof(string).GetMethod("StartsWith", new[] { typeof(string) }))
            //        //{
            //        //    string paramName = $"@p{Parameters.Count}";
            //        //    result += GetStartsWithSqlString(RecurseExp(methodCall.Object, td), paramName, ExpressionType.Equal);
            //        //    Parameters.Add(new SqlParameterItem { Name = paramName, Value = GetStartsWithFormatedValue(RecurseExp(methodCall.Arguments[0], td)), SqlDbType = System.Data.SqlDbType.NVarChar });
            //        //}
            //        //else if (methodCall.Method == typeof(string).GetMethod("EndsWith", new[] { typeof(string) }))
            //        //{
            //        //    string paramName = $"@p{Parameters.Count}";
            //        //    result += GetEndsWithSqlString(RecurseExp(methodCall.Object, td), paramName, ExpressionType.Equal);
            //        //    Parameters.Add(new SqlParameterItem { Name = paramName, Value = GetEndsWithFormatedValue(RecurseExp(methodCall.Arguments[0], td)), SqlDbType = System.Data.SqlDbType.NVarChar });
            //        //}
            //        //else if (methodCall.Method.Name == "IsNullOrEmpty")
            //        //{
            //        //    Expression property;
            //        //    if (methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 2)
            //        //    {
            //        //        property = methodCall.Arguments[1];
            //        //    }
            //        //    else if (!methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 1)
            //        //    {
            //        //        property = methodCall.Arguments[0];
            //        //    }
            //        //    else
            //        //    {
            //        //        throw new Exception("Unsupported method call: " + methodCall.Method.Name);
            //        //    }

            //        //    //result += $" NULLIF(REPLACE(ISNULL({RecurseExp(property, td)}, ''), ' ', ''), '') IS NULL ";
            //        //    result += GetIsNullOrEmptySqlString(RecurseExp(property, td), ExpressionType.Equal);
            //        //}
            //        //// IN queries:
            //        //else if (methodCall.Method.Name == "Contains")
            //        //{
            //        //    Expression collection;
            //        //    Expression property;
            //        //    if (methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 2)
            //        //    {
            //        //        collection = methodCall.Arguments[0];
            //        //        property = methodCall.Arguments[1];
            //        //    }
            //        //    else if (!methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 1)
            //        //    {
            //        //        collection = methodCall.Object;
            //        //        property = methodCall.Arguments[0];
            //        //    }
            //        //    else
            //        //    {
            //        //        throw new Exception("Unsupported method call: " + methodCall.Method.Name);
            //        //    }

            //        //    object rawValue = GetValue(collection);
            //        //    List<string> parameters = new List<string>();
            //        //    foreach (object item in rawValue.GetEnumerable())
            //        //    {
            //        //        string paramName = $"@p{Parameters.Count}";
            //        //        parameters.Add(paramName);
            //        //        Parameters.Add(new SqlParameterItem { Name = paramName, Value = item, SqlDbType = OpenOrmTools.ToSqlDbType(item.GetType()) });
            //        //    }

            //        //    //result += RecurseExp(property, td) + $" IN ({string.Join(",", parameters)})";
            //        //    result += GetInQuerySqlString(RecurseExp(property, td), string.Join(",", parameters), ExpressionType.Equal);
            //        //}
            //        //else if (methodCall.Method.Name == "ToLower" || methodCall.Method.Name == "ToUpper")
            //        //{
            //        //    if (methodCall.Object != null)
            //        //    {
            //        //        if (methodCall.Method.Name == "ToLower") result += $"{RecurseExp(methodCall.Object, td)}";
            //        //        else result += $"{RecurseExp(methodCall.Object, td)}";
            //        //    }
            //        //    else
            //        //    {
            //        //        throw new Exception("Unsupported method call: " + methodCall.Method.Name);
            //        //    }
            //        //}
            //        //else
            //        //{
            //        //    throw new Exception("Unsupported method call: " + methodCall.Method.Name);
            //        //}
            //    }

            //    result += ")";
            //}
            if (exp is BinaryExpression)
            {
                BinaryExpression bexp = (BinaryExpression)exp;
                //result += $"({RecurseExp(bexp.Left, td)} {NodeTypeToString(bexp.NodeType)} {RecurseExp(bexp.Right, td)})";
                result += "(";
                result += RecurseExp(bexp.Left, td);

                if (bexp.Left is MethodCallExpression && (((MethodCallExpression)bexp.Left)).Method.Name.EqualsOr("IsNullOrEmpty", "Contains", "StartsWith", "EndsWith") && bexp.NodeType != ExpressionType.AndAlso && bexp.NodeType != ExpressionType.OrElse)
                {
                    return result + " )";
                }

                result += NodeTypeToString(bexp.NodeType);
                result += RecurseExp(bexp.Right, td);
                result += ")";
            }
            else if (exp is UnaryExpression)
            {
                UnaryExpression uexp = (UnaryExpression)exp;
                if (uexp.Operand is MemberExpression)
                {
                    MemberExpression mexp = (MemberExpression)uexp.Operand;
                    if (mexp.Member is PropertyInfo)
                    {
                        PropertyInfo pi = ((PropertyInfo)mexp.Member);
                        if (WithBrackets)
                        {
                            result += $"[{td.GetColumnNameFor(pi)}]";
                        }
                        else
                        {
                            result += td.GetColumnNameFor(pi);
                        }
                        if (pi.PropertyType == typeof(bool))
                        {
                            if (uexp.NodeType == ExpressionType.Not)
                            {
                                result += " = 0";
                            }
                            else
                            {
                                result += " = 1";
                            }
                        }
                    }
                    else if (mexp.Member is FieldInfo)
                    {
                        FieldInfo fi = ((FieldInfo)mexp.Member);
                        var value = GetValue(mexp);
                        string paramName = $"@p{Parameters.Count}";

                        if (value is bool)
                        {
                            Parameters.Add(new SqlParameterItem { Name = paramName, Value = (bool)value ? "1" : "0", SqlDbType = OpenOrmTools.ToSqlDbType(fi) });
                        }
                        else
                        {
                            Parameters.Add(new SqlParameterItem { Name = paramName, Value = value, SqlDbType = OpenOrmTools.ToSqlDbType(fi) });
                        }
                        result += NodeTypeToString(exp.NodeType) + paramName;
                    }
                }
                else if(uexp.Operand is MethodCallExpression)
                {
                    result += ResolveMethodCallExpression(uexp.Operand, td, uexp.NodeType);
                    //var methodCall = (MethodCallExpression)uexp.Operand;
                    //// LIKE queries:
                    //if (methodCall.Method == typeof(string).GetMethod("Contains", new[] { typeof(string) }))
                    //{
                    //    string paramName = $"@p{Parameters.Count}";
                    //    result += GetContainsSqlString(RecurseExp(methodCall.Object, td), paramName, ExpressionType.Equal);
                    //    Parameters.Add(new SqlParameterItem { Name = paramName, Value = GetContainsFormatedValue(RecurseExp(methodCall.Arguments[0], td)), SqlDbType = System.Data.SqlDbType.NVarChar });
                    //}
                    //else if (methodCall.Method == typeof(string).GetMethod("StartsWith", new[] { typeof(string) }))
                    //{
                    //    string paramName = $"@p{Parameters.Count}";
                    //    result += GetStartsWithSqlString(RecurseExp(methodCall.Object, td), paramName, ExpressionType.Equal);
                    //    Parameters.Add(new SqlParameterItem { Name = paramName, Value = GetStartsWithFormatedValue(RecurseExp(methodCall.Arguments[0], td)), SqlDbType = System.Data.SqlDbType.NVarChar });
                    //}
                    //else if (methodCall.Method == typeof(string).GetMethod("EndsWith", new[] { typeof(string) }))
                    //{
                    //    string paramName = $"@p{Parameters.Count}";
                    //    result += GetEndsWithSqlString(RecurseExp(methodCall.Object, td), paramName, ExpressionType.Equal);
                    //    Parameters.Add(new SqlParameterItem { Name = paramName, Value = GetEndsWithFormatedValue(RecurseExp(methodCall.Arguments[0], td)), SqlDbType = System.Data.SqlDbType.NVarChar });
                    //}
                    //else if (methodCall.Method.Name == "IsNullOrEmpty")
                    //{
                    //    Expression property;
                    //    if (methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 2)
                    //    {
                    //        property = methodCall.Arguments[1];
                    //    }
                    //    else if (!methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 1)
                    //    {
                    //        property = methodCall.Arguments[0];
                    //    }
                    //    else
                    //    {
                    //        throw new Exception("Unsupported method call: " + methodCall.Method.Name);
                    //    }

                    //    //result += $" NULLIF(REPLACE(ISNULL({RecurseExp(property, td)}, ''), ' ', ''), '') IS NULL ";
                    //    result += GetIsNullOrEmptySqlString(RecurseExp(property, td), ExpressionType.Equal);
                    //}
                    //// IN queries:
                    //else if (methodCall.Method.Name == "Contains")
                    //{
                    //    Expression collection;
                    //    Expression property;
                    //    if (methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 2)
                    //    {
                    //        collection = methodCall.Arguments[0];
                    //        property = methodCall.Arguments[1];
                    //    }
                    //    else if (!methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 1)
                    //    {
                    //        collection = methodCall.Object;
                    //        property = methodCall.Arguments[0];
                    //    }
                    //    else
                    //    {
                    //        throw new Exception("Unsupported method call: " + methodCall.Method.Name);
                    //    }

                    //    object rawValue = GetValue(collection);
                    //    List<string> parameters = new List<string>();
                    //    foreach (object item in rawValue.GetEnumerable())
                    //    {
                    //        string paramName = $"@p{Parameters.Count}";
                    //        parameters.Add(paramName);
                    //        Parameters.Add(new SqlParameterItem { Name = paramName, Value = item, SqlDbType = OpenOrmTools.ToSqlDbType(item.GetType()) });
                    //    }

                    //    //result += RecurseExp(property, td) + $" IN ({string.Join(",", parameters)})";
                    //    result += GetInQuerySqlString(RecurseExp(property, td), string.Join(",", parameters), ExpressionType.Equal);
                    //}
                    //else
                    //{
                    //    throw new Exception("Unsupported method call: " + methodCall.Method.Name);
                    //}
                }
            }
            else if (exp is MemberExpression)
            {
                MemberExpression mexp = (MemberExpression)exp;
                //if (mexp.Member is PropertyInfo)
                //{
                //    PropertyInfo pi = ((PropertyInfo)mexp.Member);
                //    if (WithBrackets)
                //    {
                //        result += $"[{td.GetColumnNameFor(pi)}]";
                //    }
                //    else
                //    {
                //        result += td.GetColumnNameFor(pi);
                //    }
                //    if (pi.PropertyType == typeof(bool))
                //    {
                //        result += " = 1";
                //    }
                //}
                //else if (mexp.Member is FieldInfo)
                //{
                //    FieldInfo fi = ((FieldInfo)mexp.Member);
                //    var value = GetValue(mexp);
                //    string paramName = $"@p{Parameters.Count}";

                //    if (value is bool)
                //    {
                //        Parameters.Add(new SqlParameterItem { Name = paramName, Value = (bool)value ? "1" : "0", SqlDbType = OpenOrmTools.ToSqlDbType(fi) });
                //    }
                //    else
                //    {
                //        Parameters.Add(new SqlParameterItem { Name = paramName, Value = value, SqlDbType = OpenOrmTools.ToSqlDbType(fi) });
                //    }
                //    result += NodeTypeToString(exp.NodeType) + paramName;
                //}

                if (mexp.Member is PropertyInfo)
                {
                    //result += NodeTypeToString(bexp.NodeType);
                    PropertyInfo pi = ((PropertyInfo)mexp.Member);


                    if (mexp.NodeType == ExpressionType.MemberAccess && pi.PropertyType == typeof(bool))
                    {
                        if (WithBrackets)
                        {
                            result += $"[{td.GetColumnNameFor(pi)}] = 1";
                        }
                        else
                        {
                            result += $"{td.GetColumnNameFor(pi)} = 1";
                        }
                    }
                    else if ((mexp.NodeType == ExpressionType.MemberAccess && mexp.Member.Name == "Now") || (mexp.Expression.NodeType == ExpressionType.MemberAccess || mexp.Expression.NodeType == ExpressionType.Constant))
                    {
                        var value = GetValue(mexp);
                        string paramName = $"@p{Parameters.Count}";

                        if (value is bool)
                        {
                            Parameters.Add(new SqlParameterItem { Name = paramName, Value = (bool)value ? "1" : "0", SqlDbType = OpenOrmTools.ToSqlDbType(pi) });
                        }
                        else
                        {
                            Parameters.Add(new SqlParameterItem { Name = paramName, Value = value, SqlDbType = OpenOrmTools.ToSqlDbType(pi) });
                        }
                        result += paramName;
                    }
                    else
                    {
                        if (WithBrackets)
                        {
                            result += $"[{td.GetColumnNameFor(pi)}]";
                        }
                        else
                        {
                            result += td.GetColumnNameFor(pi);
                        }
                    }
                    //else
                    //{
                    //    string paramName = $"@p{Parameters.Count}";
                    //    result += paramName;
                    //    Parameters.Add(new SqlParameterItem { Name = paramName, Value = GetValue(mexp), SqlDbType = OpenOrmTools.ToSqlDbType(pi) });
                    //}
                    //else if (pi.PropertyType == typeof(DateTime))
                    //{
                    //    result += $"'{GetValue(mexp)}'";

                    //}
                    //else if (pi.PropertyType == typeof(string))
                    //{
                    //    result += $"'{GetValue(mexp)}'";
                    //}
                    //else
                    //{
                    //    result += $"{GetValue(mexp)}";
                    //}
                }
                else if (mexp.Member is FieldInfo)
                {
                    FieldInfo fi = ((FieldInfo)mexp.Member);
                    var value = GetValue(mexp);
                    string paramName = $"@p{Parameters.Count}";

                    if (value is bool)
                    {
                        Parameters.Add(new SqlParameterItem { Name = paramName, Value = (bool)value ? "1" : "0", SqlDbType = OpenOrmTools.ToSqlDbType(fi) });
                    }
                    else
                    {
                        Parameters.Add(new SqlParameterItem { Name = paramName, Value = value, SqlDbType = OpenOrmTools.ToSqlDbType(fi) });
                    }
                    result += /*NodeTypeToString(bexp.NodeType) +*/ paramName;
                }
            }
            else if (exp is ConstantExpression)
            {
                var value = ((ConstantExpression)exp).Value;

                if(options.HasFlag(ParserOptions.NO_PARAMETER))
                {
                    if (value is bool)
                    {
                        result += (bool)value ? "(1=1)" : "(1=0)";
                    }
                    else if (value is string)
                    {
                        result += value.ToString().Replace("'", "''");
                    }
                    else
                    {
                        result += value.ToString();
                    }
                }
                else
                {
                    string paramName = $"@p{Parameters.Count}";

                    if (value is bool)
                    {
                        Parameters.Add(new SqlParameterItem { Name = paramName, Value = (bool)value ? 1 : 0, SqlDbType = OpenOrmTools.ToSqlDbType(((ConstantExpression)exp).Type) });
                    }
                    else
                    {
                        Parameters.Add(new SqlParameterItem { Name = paramName, Value = value, SqlDbType = OpenOrmTools.ToSqlDbType(((ConstantExpression)exp).Type) });
                    }
                    result += /*NodeTypeToString(bexp.NodeType) +*/ paramName;
                }
            }
            else if (exp is MethodCallExpression)
            {
                result += ResolveMethodCallExpression(exp, td, exp.NodeType);
                //var methodCall = (MethodCallExpression)exp;
                //// LIKE queries:
                //if (methodCall.Method == typeof(string).GetMethod("Contains", new[] { typeof(string) }))
                //{
                //    string paramName = $"@p{Parameters.Count}";
                //    result += GetContainsSqlString(RecurseExp(methodCall.Object, td), paramName, ExpressionType.Equal);
                //    Parameters.Add(new SqlParameterItem { Name = paramName, Value = GetContainsFormatedValue(RecurseExp(methodCall.Arguments[0], td)), SqlDbType = System.Data.SqlDbType.NVarChar });
                //}
                //else if (methodCall.Method == typeof(string).GetMethod("StartsWith", new[] { typeof(string) }))
                //{
                //    string paramName = $"@p{Parameters.Count}";
                //    result += GetStartsWithSqlString(RecurseExp(methodCall.Object, td), paramName, ExpressionType.Equal);
                //    Parameters.Add(new SqlParameterItem { Name = paramName, Value = GetStartsWithFormatedValue(RecurseExp(methodCall.Arguments[0], td)), SqlDbType = System.Data.SqlDbType.NVarChar });
                //}
                //else if (methodCall.Method == typeof(string).GetMethod("EndsWith", new[] { typeof(string) }))
                //{
                //    string paramName = $"@p{Parameters.Count}";
                //    result += GetEndsWithSqlString(RecurseExp(methodCall.Object, td), paramName, ExpressionType.Equal);
                //    Parameters.Add(new SqlParameterItem { Name = paramName, Value = GetEndsWithFormatedValue(RecurseExp(methodCall.Arguments[0], td)), SqlDbType = System.Data.SqlDbType.NVarChar });
                //}
                //else if (methodCall.Method.Name == "IsNullOrEmpty")
                //{
                //    Expression property;
                //    if (methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 2)
                //    {
                //        property = methodCall.Arguments[1];
                //    }
                //    else if (!methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 1)
                //    {
                //        property = methodCall.Arguments[0];
                //    }
                //    else
                //    {
                //        throw new Exception("Unsupported method call: " + methodCall.Method.Name);
                //    }

                //    //result += $" NULLIF(REPLACE(ISNULL({RecurseExp(property, td)}, ''), ' ', ''), '') IS NULL ";
                //    result += GetIsNullOrEmptySqlString(RecurseExp(property, td), ExpressionType.Equal);
                //}
                //// IN queries:
                //else if (methodCall.Method.Name == "Contains")
                //{
                //    Expression collection;
                //    Expression property;
                //    if (methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 2)
                //    {
                //        collection = methodCall.Arguments[0];
                //        property = methodCall.Arguments[1];
                //    }
                //    else if (!methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 1)
                //    {
                //        collection = methodCall.Object;
                //        property = methodCall.Arguments[0];
                //    }
                //    else
                //    {
                //        throw new Exception("Unsupported method call: " + methodCall.Method.Name);
                //    }

                //    object rawValue = GetValue(collection);
                //    List<string> parameters = new List<string>();
                //    foreach (object item in rawValue.GetEnumerable())
                //    {
                //        string paramName = $"@p{Parameters.Count}";
                //        parameters.Add(paramName);
                //        Parameters.Add(new SqlParameterItem { Name = paramName, Value = item, SqlDbType = OpenOrmTools.ToSqlDbType(item.GetType()) });
                //    }

                //    //result += RecurseExp(property, td) + $" IN ({string.Join(",", parameters)})";
                //    result += GetInQuerySqlString(RecurseExp(property, td), string.Join(",", parameters), ExpressionType.Equal);
                //}
                //else
                //{
                //    throw new Exception("Unsupported method call: " + methodCall.Method.Name);
                //}
            }

            return result;
        }

        internal string ResolveMethodCallExpression(Expression exp, TableDefinition td, ExpressionType exptype)
        {
            string result = "";
            var methodCall = (MethodCallExpression)exp;
            // LIKE queries:
            if (methodCall.Method == typeof(string).GetMethod("Contains", new[] { typeof(string) }))
            {
                string paramName = $"@p{Parameters.Count}";
                result += GetContainsSqlString(RecurseExp(methodCall.Object, td), paramName, exptype);
                Parameters.Add(new SqlParameterItem { Name = paramName, Value = GetContainsFormatedValue(RecurseExp(methodCall.Arguments[0], td, ParserOptions.NO_PARAMETER)), SqlDbType = System.Data.SqlDbType.NVarChar });
            }
            else if (methodCall.Method == typeof(string).GetMethod("StartsWith", new[] { typeof(string) }))
            {
                string paramName = $"@p{Parameters.Count}";
                result += GetStartsWithSqlString(RecurseExp(methodCall.Object, td), paramName, exptype);
                Parameters.Add(new SqlParameterItem { Name = paramName, Value = GetStartsWithFormatedValue(RecurseExp(methodCall.Arguments[0], td, ParserOptions.NO_PARAMETER)), SqlDbType = System.Data.SqlDbType.NVarChar });
            }
            else if (methodCall.Method == typeof(string).GetMethod("EndsWith", new[] { typeof(string) }))
            {
                string paramName = $"@p{Parameters.Count}";
                result += GetEndsWithSqlString(RecurseExp(methodCall.Object, td), paramName, exptype);
                Parameters.Add(new SqlParameterItem { Name = paramName, Value = GetEndsWithFormatedValue(RecurseExp(methodCall.Arguments[0], td, ParserOptions.NO_PARAMETER)), SqlDbType = System.Data.SqlDbType.NVarChar });
            }
            else if (methodCall.Method.Name == "IsNullOrEmpty")
            {
                Expression property;
                if (methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 2)
                {
                    property = methodCall.Arguments[1];
                }
                else if (!methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 1)
                {
                    property = methodCall.Arguments[0];
                }
                else
                {
                    throw new Exception("Unsupported method call: " + methodCall.Method.Name);
                }

                //result += $" NULLIF(REPLACE(ISNULL({RecurseExp(property, td)}, ''), ' ', ''), '') IS NULL ";
                result += GetIsNullOrEmptySqlString(RecurseExp(property, td), exptype);
            }
            // IN queries:
            else if (methodCall.Method.Name == "Contains")
            {
                Expression collection;
                Expression property;
                if (methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 2)
                {
                    collection = methodCall.Arguments[0];
                    property = methodCall.Arguments[1];
                }
                else if (!methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 1)
                {
                    collection = methodCall.Object;
                    property = methodCall.Arguments[0];
                }
                else
                {
                    throw new Exception("Unsupported method call: " + methodCall.Method.Name);
                }

                object rawValue = GetValue(collection);
                List<string> parameters = new List<string>();
                foreach (object item in rawValue.GetEnumerable())
                {
                    string paramName = $"@p{Parameters.Count}";
                    parameters.Add(paramName);
                    Parameters.Add(new SqlParameterItem { Name = paramName, Value = item, SqlDbType = OpenOrmTools.ToSqlDbType(item.GetType()) });
                }

                //result += RecurseExp(property, td) + $" IN ({string.Join(",", parameters)})";
                if(parameters.Any())
                {
                    result += GetInQuerySqlString(RecurseExp(property, td), string.Join(",", parameters), exptype);
                }
            }
            else if (methodCall.Method.Name == "ToLower" || methodCall.Method.Name == "ToUpper")
            {
                if (methodCall.Object != null)
                {
                    if (methodCall.Method.Name == "ToLower") result += GetToLowerMethod($"{RecurseExp(methodCall.Object, td)}");
                    else result += GetToUpperMethod($"{RecurseExp(methodCall.Object, td)}");
                }
                else
                {
                    throw new Exception("Unsupported method call: " + methodCall.Method.Name);
                }
            }
            else
            {
                throw new Exception("Unsupported method call: " + methodCall.Method.Name);
            }

            return result;
        }

        //Formatting methods
        internal abstract string GetContainsSqlString(string sql, string param_name, ExpressionType exptype);
        internal abstract string GetStartsWithSqlString(string sql, string param_name, ExpressionType exptype);
        internal abstract string GetEndsWithSqlString(string sql, string param_name, ExpressionType exptype);
        internal abstract string GetIsNullOrEmptySqlString(string sql, ExpressionType exptype);
        internal abstract string GetInQuerySqlString(string sql, string parameters, ExpressionType exptype);
        internal abstract string GetContainsFormatedValue(string value);
        internal abstract string GetStartsWithFormatedValue(string value);
        internal abstract string GetEndsWithFormatedValue(string value);
        internal abstract string GetToUpperMethod(string value);
        internal abstract string GetToLowerMethod(string value);

        #region Tools
        private static object GetValue(Expression member)
        {
            // source: http://stackoverflow.com/a/2616980/291955
            var objectMember = Expression.Convert(member, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            return getter();
        }

        internal abstract string NodeTypeToString(ExpressionType nodeType);
        
        #endregion

    }

    public static class ExpressionExtentions
    {
        //public static string ToSqlWhere<T>(this Expression<Func<T, bool>> predicate, TableDefinition td, out List<SqlParameterItem> parameters)
        //{
        //    LinqExpressionParser parser = LinqExpressionParser.Build(predicate, td);
        //    parameters = parser.Parameters;
        //    return parser.Sql;
        //}

        //public static string ToSqlWhereWithParameters<T>(this Expression<Func<T, bool>> predicate, TableDefinition td)
        //{
        //    SqlServerExpressionParser sb = SqlServerExpressionParser.Build(predicate, td);
        //    return sb.SqlWithParameters;
        //}
    }
}
