using OpenOrm.Configuration;
//using OpenOrm.SqlProvider.MySql;
//using OpenOrm.SqlProvider.SQLite;
//using OpenOrm.SqlProvider.SqlServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenOrm.SqlProvider.Shared
{
    public class SqlQuery : IDisposable
    {
        #region Properties
        SqlResult SR;
        OpenOrmDbConnection _cnx;
        ISqlConnector CurrentConnector;

        public delegate void QueryDelegate(string command);
        public static event QueryDelegate OnSqlQuery;

        private string _storedProcedure;
        public string StoredProcedure
        {
            get { return _storedProcedure; }
            set { _storedProcedure = value; ClearParameters(); }
        }

        private string _sql;
        public string Sql
        {
            get { return _sql; }
            set { _sql = value; ClearParameters(); }
        }

        private bool _autoDispose;
        public bool AutoDispose
        {
            get { return _autoDispose; }
            set { _autoDispose = value; }
        }

        private SqlQueryType _queryType;
        public SqlQueryType QueryType
        {
            get { return _queryType; }
            set { _queryType = value; }
        }
        #endregion

        #region Constructors
        public SqlQuery()
        {
            Parameters = new List<SqlParameterItem>();
        }

        public SqlQuery(OpenOrmDbConnection cnx)
        {
            Parameters = new List<SqlParameterItem>();
            _cnx = cnx;
            CurrentConnector = cnx.Configuration.ConnectorProvider; //GetConnector(cnx.Configuration);
        }

        /// <summary>
        /// Création de l'objet SqlQuery
        /// </summary>
        /// <param name="sSql"></param>
        /// <param name="eType"></param>
        /// <param name="bAutoDispose"></param>
        public SqlQuery(OpenOrmDbConnection cnx, string sSql, SqlQueryType eType = SqlQueryType.Sql, bool bAutoDispose = false)
        {
            Parameters = new List<SqlParameterItem>();
            CurrentConnector = cnx.Configuration.ConnectorProvider; //GetConnector(cnx.Configuration);
            AutoDispose = bAutoDispose;
            QueryType = eType;
            _cnx = cnx;
            Sql = sSql;

            //if (eType == SqlQueryType.Sql)
            //    Sql = sSql;
            //else if (eType == SqlQueryType.StoredProcedure)
            //    StoredProcedure = sSql;
            //else if (eType == SqlQueryType.Auto)
            //{
            //    if (
            //        (sSql.ToLower().Contains("select") && sSql.ToLower().Contains("from"))
            //        || (sSql.ToLower().Contains("where") && (sSql.ToLower().Contains("=") || sSql.ToLower().Contains("<") || sSql.ToLower().Contains(">") || sSql.ToLower().Contains("between") || sSql.ToLower().Contains("in") || sSql.ToLower().Contains("exists")))
            //        || (sSql.ToLower().Contains("update") && sSql.ToLower().Contains("where"))
            //        || (sSql.ToLower().Contains("delete") && sSql.ToLower().Contains("where"))
            //    )
            //    {
            //        QueryType = SqlQueryType.Sql;
            //        Sql = sSql;
            //    }
            //    else
            //    {
            //        QueryType = SqlQueryType.StoredProcedure;
            //        StoredProcedure = sSql;
            //    }
            //}
        }
        #endregion

        #region Parameters
        public List<SqlParameterItem> Parameters { get; set; }

        public void AddParameter(SqlParameterItem p)
        {
            Parameters.Add(p);
            //CurrentConnector.AddParameter(p.Name, p.Value);
        }

        public void AddParameter(string paramName, object value, SqlDbType type)
        {
            Parameters.Add(new SqlParameterItem { Name = paramName, Value = value, SqlDbType = type });
            //CurrentConnector.AddParameter(paramName, value, type);
        }

        public void AddParameter(string paramName, object value)
        {
            Parameters.Add(new SqlParameterItem { Name = paramName, Value = value, SqlDbType = OpenOrmTools.ToSqlDbType(value.GetType()) });
            //CurrentConnector.AddParameter(paramName, value, type);
        }

        //public void AddParameter(string paramName, object value)
        //{
        //    SqlDbType type = DetectParameterType(value);
        //    Parameters.Add(new SqlParameterItem { Name = paramName, Value = value, SqlDbType = type });
        //    //CurrentConnector.AddParameter(paramName, value, type);
        //}

        /// <summary>
        /// Vide la liste des paramètres précédemment ajoutés
        /// </summary>
        public void ClearParameters()
        {
            CurrentConnector.Parameters = new ArrayList();
            Parameters = new List<SqlParameterItem>();
        }
        #endregion

        #region Execute
        public void ExecuteSql(OpenOrmDbConnection cnx, string command, List<SqlParameterItem> parameters = null)
        {
            if (CurrentConnector == null) CurrentConnector = cnx.Configuration.ConnectorProvider; //GetConnector(cnx.Configuration);

            if (parameters != null && parameters.Count > 0)
            {
                foreach (SqlParameterItem p in parameters)
                {
                    if (!Parameters.Any(x => x.Name == p.Name))
                    {
                        AddParameter(p);
                    }
                }
            }

            if (Parameters != null && Parameters.Count > 0)
            {
                foreach (SqlParameterItem p in Parameters)
                {
                    CurrentConnector.AddParameter(p.Name, p.Value, p.SqlDbType);
                }
            }

            //if (cnx.Configuration.PrintSqlQueries) System.Diagnostics.Debug.WriteLine(GetSql(cnx.Configuration.Connector, command));

            OnSqlQuery?.Invoke(command);

            CurrentConnector.ExecuteSql(cnx, command);

            ClearParameters();

            if (AutoDispose)
                Dispose();
        }

        public async Task ExecuteSqlAsync(OpenOrmDbConnection cnx, string command, List<SqlParameterItem> parameters = null)
        {
            if (CurrentConnector == null) CurrentConnector = cnx.Configuration.ConnectorProvider; //GetConnector(cnx.Configuration);

            if (parameters != null && parameters.Count > 0)
            {
                foreach (SqlParameterItem p in parameters)
                {
                    if (!Parameters.Any(x => x.Name == p.Name))
                    {
                        AddParameter(p);
                    }
                }
            }

            if (Parameters != null && Parameters.Count > 0)
            {
                foreach (SqlParameterItem p in Parameters)
                {
                    CurrentConnector.AddParameter(p.Name, p.Value, p.SqlDbType);
                }
            }

			//if (cnx.Configuration.PrintSqlQueries) System.Diagnostics.Debug.WriteLine(GetSql(cnx.Configuration.Connector, command));

			OnSqlQuery?.Invoke(command);

			await CurrentConnector.ExecuteSqlAsync(cnx, command);

            ClearParameters();

            if (AutoDispose)
                Dispose();
        }



        public void ExecuteStoredProcedure(OpenOrmDbConnection cnx, string command, List<SqlParameterItem> parameters = null)
        {
            if (CurrentConnector == null) CurrentConnector = cnx.Configuration.ConnectorProvider; //GetConnector(cnx.Configuration);

            if (parameters != null && parameters.Count > 0)
            {
                foreach (SqlParameterItem p in parameters)
                {
                    if (!Parameters.Any(x => x.Name == p.Name))
                    {
                        AddParameter(p);
                    }
                }
            }

            if (Parameters != null && Parameters.Count > 0)
            {
                foreach (SqlParameterItem p in Parameters)
                {
                    CurrentConnector.AddParameter(p.Name, p.Value, p.SqlDbType);
                }
            }

            //if (cnx.Configuration.PrintSqlQueries) System.Diagnostics.Debug.WriteLine(GetSql(cnx.Configuration.Connector, command));

            CurrentConnector.ExecuteStoredProcedure(cnx, command);

            ClearParameters();

            if (AutoDispose)
                Dispose();
        }

        public async Task ExecuteStoredProcedureAsync(OpenOrmDbConnection cnx, string command, List<SqlParameterItem> parameters = null)
        {
            if (CurrentConnector == null) CurrentConnector = cnx.Configuration.ConnectorProvider; //GetConnector(cnx.Configuration);

            if (parameters != null && parameters.Count > 0)
            {
                foreach (SqlParameterItem p in parameters)
                {
                    if (!Parameters.Any(x => x.Name == p.Name))
                    {
                        AddParameter(p);
                    }
                }
            }

            if (Parameters != null && Parameters.Count > 0)
            {
                foreach (SqlParameterItem p in Parameters)
                {
                    CurrentConnector.AddParameter(p.Name, p.Value, p.SqlDbType);
                }
            }

            //if (cnx.Configuration.PrintSqlQueries) System.Diagnostics.Debug.WriteLine(GetSql(cnx.Configuration.Connector, command));

            await CurrentConnector.ExecuteStoredProcedureAsync(cnx, command);

            ClearParameters();

            if (AutoDispose)
                Dispose();
        }


        public SqlResult Read(OpenOrmDbConnection cnx, string command, SqlQueryType sqlQueryType = SqlQueryType.Sql)
        {
            if (CurrentConnector == null) CurrentConnector = cnx.Configuration.ConnectorProvider; //GetConnector(cnx.Configuration);

            if (Parameters != null && Parameters.Count > 0)
            {
                foreach (SqlParameterItem p in Parameters)
                {
                    CurrentConnector.AddParameter(p.Name, p.Value, p.SqlDbType);
                }
            }

            //if (cnx.Configuration.PrintSqlQueries) System.Diagnostics.Debug.WriteLine(GetSql(cnx.Configuration.Connector, command));

            if (sqlQueryType == SqlQueryType.Sql)
            {
                SR = DataReaderToSqlResult(CurrentConnector.GetDataReader(cnx, command, CommandType.Text));
            }
            else if (sqlQueryType == SqlQueryType.StoredProcedure)
            {
                SR = DataReaderToSqlResult(CurrentConnector.GetDataReader(cnx, command, CommandType.StoredProcedure));
            }

            ClearParameters();

            return SR;
        }

        public async Task<SqlResult> ReadAsync(OpenOrmDbConnection cnx, string command, SqlQueryType sqlQueryType = SqlQueryType.Sql)
        {
            if (CurrentConnector == null) CurrentConnector = cnx.Configuration.ConnectorProvider; //GetConnector(cnx.Configuration);

            if (Parameters != null && Parameters.Count > 0)
            {
                foreach (SqlParameterItem p in Parameters)
                {
                    CurrentConnector.AddParameter(p.Name, p.Value, p.SqlDbType);
                }
            }

            //if (cnx.Configuration.PrintSqlQueries) System.Diagnostics.Debug.WriteLine(GetSql(cnx.Configuration.Connector, command));

            if (sqlQueryType == SqlQueryType.Sql)
            {
                SR = DataReaderToSqlResult(await CurrentConnector.GetDataReaderAsync(cnx, command, CommandType.Text));
            }
            else if (sqlQueryType == SqlQueryType.StoredProcedure)
            {
                SR = DataReaderToSqlResult(await CurrentConnector.GetDataReaderAsync(cnx, command, CommandType.StoredProcedure));
            }

            ClearParameters();

            return SR;
        }



        public SqlResult Read(OpenOrmDbConnection cnx, string command, List<SqlParameterItem> parameters, SqlQueryType sqlQueryType = SqlQueryType.Sql)
        {
            if (CurrentConnector == null) CurrentConnector = cnx.Configuration.ConnectorProvider; //GetConnector(cnx.Configuration);

            if (parameters != null && parameters.Count > 0)
            {
                foreach (SqlParameterItem p in parameters)
                {
                    if (!Parameters.Any(x => x.Name == p.Name))
                    {
                        AddParameter(p);
                    }
                }
            }

            if (Parameters != null && Parameters.Count > 0)
            {
                foreach (SqlParameterItem p in Parameters)
                {
                    CurrentConnector.AddParameter(p.Name, p.Value, p.SqlDbType);
                }
            }

            //if (cnx.Configuration.PrintSqlQueries) System.Diagnostics.Debug.WriteLine(GetSql(cnx.Configuration.Connector, command));

            if (sqlQueryType == SqlQueryType.Sql)
            {
                SR = DataReaderToSqlResult(CurrentConnector.GetDataReader(cnx, command, CommandType.Text));
            }
            else if (sqlQueryType == SqlQueryType.StoredProcedure)
            {
                SR = DataReaderToSqlResult(CurrentConnector.GetDataReader(cnx, command, CommandType.StoredProcedure));
            }

            ClearParameters();

            return SR;
        }

        public async Task<SqlResult> ReadAsync(OpenOrmDbConnection cnx, string command, List<SqlParameterItem> parameters, SqlQueryType sqlQueryType = SqlQueryType.Sql)
        {
            if (CurrentConnector == null) CurrentConnector = cnx.Configuration.ConnectorProvider; //GetConnector(cnx.Configuration);

            if (parameters != null && parameters.Count > 0)
            {
                foreach (SqlParameterItem p in parameters)
                {
                    if (!Parameters.Any(x => x.Name == p.Name))
                    {
                        AddParameter(p);
                    }
                }
            }

            if (Parameters != null && Parameters.Count > 0)
            {
                foreach (SqlParameterItem p in Parameters)
                {
                    CurrentConnector.AddParameter(p.Name, p.Value, p.SqlDbType);
                }
            }

            //if (cnx.Configuration.PrintSqlQueries) System.Diagnostics.Debug.WriteLine(GetSql(cnx.Configuration.Connector, command));

            if (sqlQueryType == SqlQueryType.Sql)
            {
                SR = DataReaderToSqlResult(await CurrentConnector.GetDataReaderAsync(cnx, command, CommandType.Text));
            }
            else if (sqlQueryType == SqlQueryType.StoredProcedure)
            {
                SR = DataReaderToSqlResult(await CurrentConnector.GetDataReaderAsync(cnx, command, CommandType.StoredProcedure));
            }

            ClearParameters();

            return SR;
        }








        public static void Execute(OpenOrmDbConnection cnx, string command, SqlQueryType sqlQueryType = SqlQueryType.Sql)
        {
            SqlQuery sq = new SqlQuery(cnx);
            if (sqlQueryType == SqlQueryType.StoredProcedure)
            {
                sq.ExecuteStoredProcedure(cnx, command);
            }
            else
            {
                sq.ExecuteSql(cnx, command);
            }
            sq.Dispose();
        }

        public async static Task ExecuteAsync(OpenOrmDbConnection cnx, string command, SqlQueryType sqlQueryType = SqlQueryType.Sql)
        {
            SqlQuery sq = new SqlQuery(cnx);
            if (sqlQueryType == SqlQueryType.StoredProcedure)
            {
                await sq.ExecuteStoredProcedureAsync(cnx, command);
            }
            else
            {
                await sq.ExecuteSqlAsync(cnx, command);
            }
            sq.Dispose();
        }






        public static void Execute(OpenOrmDbConnection cnx, string command, List<SqlParameterItem> parameters, SqlQueryType sqlQueryType = SqlQueryType.Sql)
        {
            SqlQuery sq = new SqlQuery(cnx);
            if (sqlQueryType == SqlQueryType.StoredProcedure)
            {
                sq.ExecuteStoredProcedure(cnx, command, parameters);
            }
            else
            {
                sq.ExecuteSql(cnx, command, parameters);
            }
            sq.Dispose();
        }

        public async static Task ExecuteAsync(OpenOrmDbConnection cnx, string command, List<SqlParameterItem> parameters, SqlQueryType sqlQueryType = SqlQueryType.Sql)
        {
            SqlQuery sq = new SqlQuery(cnx);
            if (sqlQueryType == SqlQueryType.StoredProcedure)
            {
                await sq.ExecuteStoredProcedureAsync(cnx, command, parameters);
            }
            else
            {
                await sq.ExecuteSqlAsync(cnx, command, parameters);
            }
            sq.Dispose();
        }






        public static SqlResult ExecuteRead(OpenOrmDbConnection cnx, string command, SqlQueryType sqlQueryType = SqlQueryType.Sql)
        {
            SqlQuery sq = new SqlQuery(cnx);
            return sq.Read(cnx, command, sqlQueryType);
        }

        public async static Task<SqlResult> ExecuteReadAsync(OpenOrmDbConnection cnx, string command, SqlQueryType sqlQueryType = SqlQueryType.Sql)
        {
            SqlQuery sq = new SqlQuery(cnx);
            return await sq.ReadAsync(cnx, command, sqlQueryType);
        }

        public static SqlResult ExecuteRead(OpenOrmDbConnection cnx, string command, List<SqlParameterItem> parameters, SqlQueryType sqlQueryType = SqlQueryType.Sql)
        {
            SqlQuery sq = new SqlQuery(cnx);
            SqlResult sr = sq.Read(cnx, command, parameters, sqlQueryType);
            sq.Dispose();
            return sr;
        }

        public async static Task<SqlResult> ExecuteReadAsync(OpenOrmDbConnection cnx, string command, List<SqlParameterItem> parameters, SqlQueryType sqlQueryType = SqlQueryType.Sql)
        {
            SqlQuery sq = new SqlQuery(cnx);
            SqlResult sr = await sq.ReadAsync(cnx, command, parameters, sqlQueryType);
            sq.Dispose();
            return sr;
        }
        #endregion

        #region Helpers
        //private ISqlConnector GetConnector(OpenOrmConfiguration config)
        //{
        //    ISqlConnector connector = null;

        //    switch (config.Connector)
        //    {
        //        case Connector.SqlServer:
        //            connector = new SqlServerConnector();
        //            break;
        //        case Connector.SqLite:
        //            connector = new SQLiteConnector();
        //            break;
        //        case Connector.MySql:
        //            connector = new MySqlConnector();
        //            break;
        //    }

        //    return connector;
        //}

        /// <summary>
        /// Détecte le type de variable donné à AddParameter, afin d'entrer le bon type dans l'objet sqlcommand
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlDbType DetectParameterType(object value)
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
                return SqlDbType.Variant;
        }

        //public string GetSql(Connector sqlDialect = Connector.SqlServer, string command = null)
        //{
        //    string sql = "";
        //    if (Parameters?.Count > 0)
        //    {
        //        foreach (SqlParameterItem p in Parameters)
        //        {
        //            switch (sqlDialect)
        //            {
        //                default:
        //                case Connector.SqlServer:
        //                    sql += $"DECLARE {p.Name} {SqlServerTools.ToStringType(p.SqlDbType)} = {SqlServerTools.FormatValueToString(p.Value)};{Environment.NewLine}";
        //                    break;
        //                case Connector.SqLite:
        //                    sql += $"DECLARE {p.Name} {SQLiteTools.ToStringType(p.SqlDbType)} = {SQLiteTools.FormatValueToString(p.Value)};{Environment.NewLine}";
        //                    break;
        //            }

        //        }
        //    }

        //    sql += command ?? Sql + Environment.NewLine + Environment.NewLine;

        //    return sql;
        //}
        #endregion

        #region Conversion
        internal SqlResult DataReaderToSqlResult(DbDataReader dr)
        {
            SqlResult result = new SqlResult(_cnx);//int.MaxValue

            if (dr.HasRows)
            {
                result = new SqlResult(_cnx);
                while (dr.Read())
                {
                    SqlResultRow srr = new SqlResultRow(dr.FieldCount);
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        if (dr[i] is DBNull)
                        {
                            srr.Add(dr.GetName(i), null);
                        }
                        else
                        {
                            srr.Add(dr.GetName(i), dr[i]);
                        }
                        //srr.Add(dr.GetName(i), dr[dr.GetName(i)]);
                    }
                    result.AddRow(srr);
                }
            }

            dr.Close();
            dr.Dispose();

            SR = result;

            return result;
        }

        internal async Task<SqlResult> DataReaderToSqlResultAsync(DbDataReader dr)
        {
            SqlResult result = new SqlResult(_cnx);//int.MaxValue

            if (dr.HasRows)
            {
                result = new SqlResult(_cnx);
                while (await dr.ReadAsync())
                {
                    SqlResultRow srr = new SqlResultRow(dr.FieldCount);
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        if (dr[i] is DBNull)
                        {
                            srr.Add(dr.GetName(i), null);
                        }
                        else
                        {
                            srr.Add(dr.GetName(i), dr[i]);
                        }
                        //srr.Add(dr.GetName(i), dr[dr.GetName(i)]);
                    }
                    result.AddRow(srr);
                }
            }

            dr.Close();
            dr.Dispose();

            SR = result;

            return result;
        }





        public T ReadToObject<T>()
        {
            Read(_cnx, Sql, QueryType);
            return ToObject<T>();
        }

        public async Task<T> ReadToObjectAsync<T>()
        {
            await ReadAsync(_cnx, Sql, QueryType);
            return ToObject<T>();
        }



        public T ReadToObjectLast<T>()
        {
            Read(_cnx, Sql, QueryType);
            return ToObjectLast<T>();
        }

        public async Task<T> ReadToObjectLastAsync<T>()
        {
            await ReadAsync(_cnx, Sql, QueryType);
            return ToObjectLast<T>();
        }



        public List<T> ReadToObjectList<T>()
        {
            Read(_cnx, Sql, QueryType);
            return ToObjectList<T>();
        }

        public List<object> ReadToObjectList(Type t)
        {
            Read(_cnx, Sql, QueryType);
            return ToObjectList(t);
        }

        public async Task<List<T>> ReadToObjectListAsync<T>()
        {
            await ReadAsync(_cnx, Sql, QueryType);
            return ToObjectList<T>();
        }



        public Dictionary<string, object> ReadToDictionary()
        {
            Read(_cnx, Sql, QueryType);
            return ToDictionary();
        }

        public async Task<Dictionary<string, object>> ReadToDictionaryAsync()
        {
            await ReadAsync(_cnx, Sql, QueryType);
            return ToDictionary();
        }



        public List<Dictionary<string, object>> ReadToDictionaryList()
        {
            Read(_cnx, Sql, QueryType);
            return ToDictionaryList();
        }

        public async Task<List<Dictionary<string, object>>> ReadToDictionaryListAsync()
        {
            await ReadAsync(_cnx, Sql, QueryType);
            return ToDictionaryList();
        }



        public T ToObject<T>(int index = -1)
        {
            if (SR == null)
                return default(T);
            else
            {
                return SR.ToObject<T>(index);
            }
        }

        public T ToObjectLast<T>(int index = -1)
        {
            if (SR == null)
                return default(T);
            else
            {
                return SR.ToObjectLast<T>(index);
            }
        }

        public List<T> ToObjectList<T>()
        {
            if (SR == null)
                return new List<T>();
            else
            {
                return SR.ToObjectList<T>();
            }
        }

        public List<object> ToObjectList(Type t)
        {
            if (SR == null)
                return new List<object>();
            else
            {
                return SR.ToObjectList(t);
            }
        }

        public Dictionary<string, object> ToDictionary(int index = -1)
        {
            return SR.ToDictionary(index);
        }

        public List<Dictionary<string, object>> ToDictionaryList()
        {
            return SR.ToDictionaryList();
        }
        #endregion

        #region IDisposable
        private bool disposedValue = false; // Pour détecter les appels redondants

        public void Dispose()
        {
            if (!disposedValue)
            {
                if (SR != null && !SR.disposedValue)
                    SR.Dispose();
                disposedValue = true;
            }
        }

        // Ce code est ajouté pour implémenter correctement le modèle supprimable.
        //public void Dispose()
        //{
        //    Dispose(true);
        //}
        #endregion
    }
}
