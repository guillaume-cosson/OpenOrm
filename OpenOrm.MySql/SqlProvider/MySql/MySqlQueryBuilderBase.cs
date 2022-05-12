using OpenOrm.Configuration;
using OpenOrm.Extensions;
using OpenOrm.MySql;
using OpenOrm.Schema;
using OpenOrm.SqlProvider.Shared;
using OpenOrm.CoreTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenOrm.SqlProvider.MySql
{
    public class MySqlQueryBuilderBase : BaseQueryBuilder, ISqlQueryBuilder
    {
        #region Constructor
        public MySqlQueryBuilderBase(OpenOrmConfigurationBase config)
        {
            if (config == null) config = new OpenOrmConfiguration();
            config.ConnectorProvider = new MySqlConnector();
            Configuration = config;
        }
        #endregion

        #region Table
        public void CreateTable<T>(OpenOrmDbConnection cnx)
        {
            Type modelType = typeof(T);
            CreateTable(cnx, modelType);
        }

        public void CreateTable(OpenOrmDbConnection cnx, Type modelType)
        {
            List<string> primaryKeys = new List<string>();
            List<string> columns = new List<string>();
            List<string> colNames = new List<string>();
            TableDefinition td = TableDefinition.Get(modelType, cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);

            string sql = $"CREATE TABLE `{GetTableName(modelType)}` (";

            foreach (ColumnDefinition cd in td.Columns)
            {
                string fieldsql = $" `{cd.Name}` {MySqlTools.ToStringType(cd.PropertyType.GetBaseType(), cd.Size)}";

                if (cd.IsPrimaryKey)
                {
                    if (td.Columns.Where(x => x.IsPrimaryKey).Count() == 1)
                    {
                        fieldsql += " PRIMARY KEY";
                    }

                    primaryKeys.Add($"`{cd.Name}`");
                }

                if (cd.IsAutoIncrement)
                {

                    fieldsql += " AUTO_INCREMENT";
                }

                if (cd.IsNotNullColumn || (cd.IsPrimaryKey && !cd.IsAutoIncrement))
                {
                    fieldsql += " NOT NULL";
                }

                if (cd.IsUnique && !cd.IsAutoIncrement && !cd.IsPrimaryKey)
                {
                    fieldsql += " UNIQUE";
                }

                if (!string.IsNullOrEmpty(fieldsql))
                {
                    columns.Add(fieldsql);
                    colNames.Add(cd.Name);
                }
            }

            string fields = string.Join(",", columns);
            sql += fields;

            if (td.Columns.Count(x => x.IsPrimaryKey) > 1)
            {
                sql += $" , PRIMARY KEY ({string.Join(",", primaryKeys)})";
            }

            sql += ");";

            SqlQuery.Execute(cnx, sql, SqlQueryType.Sql);

            //Create default index for the table
            //SqlQuery.Execute(cnx, $"CREATE INDEX {GetTableName(modelType)}_INDEX ON {GetTableName(modelType)}({string.Join(",", colNames)});", SqlQueryType.Sql);
        }

        public bool TableExists<T>(OpenOrmDbConnection cnx)
        {
            string tableName = GetTableName<T>();
            return TableExists(cnx, tableName);
        }

        public bool TableExists(OpenOrmDbConnection cnx, Type modelType)
        {
            string tableName = GetTableName(modelType);
            return TableExists(cnx, tableName);
        }

        public bool TableExists(OpenOrmDbConnection cnx, string tableName)
        {
            SqlQuery sq = new SqlQuery();
            string dbname = cnx.ConnectionString.ToLower().GetStringBetween("database=", ";");
            sq.AddParameter("DB_NAME", dbname, SqlDbType.NVarChar);
            sq.AddParameter("TABLE_NAME", tableName, SqlDbType.NVarChar);
            //SqlResult r = sq.Read(cnx, "SELECT COUNT(name) FROM sqlite_master WHERE type='table' AND name=@TABLE_NAME", SqlQueryType.Sql);
            SqlResult r = sq.Read(cnx, "SELECT count(*) FROM information_schema.TABLES WHERE(TABLE_SCHEMA = @DB_NAME) AND(TABLE_NAME = @TABLE_NAME);", SqlQueryType.Sql);
            bool result = r.HasRows && (r.Get<bool>(0, 0));
            sq.Dispose();
            r.Dispose();
            return result;
        }

        public bool TemporaryTableExists<T>(OpenOrmDbConnection cnx)
        {
            string tableName = GetTableName<T>();
            return TemporaryTableExists(cnx, tableName);
        }
        //todo
        public bool TemporaryTableExists(OpenOrmDbConnection cnx, string tableName)
        {
            string sql = "SELECT COUNT(name) FROM sqlite_temp_master WHERE type='table' AND name=@TABLE_NAME";

            SqlQuery sq = new SqlQuery();
            sq.AddParameter("TABLE_NAME", tableName, SqlDbType.NVarChar);
            SqlResult r = sq.Read(cnx, sql, SqlQueryType.Sql);
            bool result = r.HasRows && (r.Get<bool>(0, 0));

            r.Dispose();
            sq.Dispose();

            return result;
        }

        public void DropTable<T>(OpenOrmDbConnection cnx)
        {
            DropTable(cnx, GetTableName<T>());
        }

        public void DropTable(OpenOrmDbConnection cnx, Type modelType)
        {
            DropTable(cnx, modelType.Name);
        }

        public void DropTable(OpenOrmDbConnection cnx, string tableName)
        {
            SqlQuery sq = new SqlQuery(cnx);
            //sq.ExecuteSql(cnx, $"DROP INDEX IF EXISTS {tableName}_INDEX ON {DefaultSchema}.{tableName}");
            sq.ExecuteSql(cnx, $"DROP TABLE `{tableName}`;");
            sq.Dispose();
        }

        public void TruncateTable<T>(OpenOrmDbConnection cnx)
        {
            TruncateTable(cnx, typeof(T));
        }

        public void TruncateTable(OpenOrmDbConnection cnx, Type modelType)
        {
            SqlQuery.Execute(cnx, $"TRUNCATE TABLE `{GetTableName(modelType)}`;");
        }


        #endregion

        #region Column
        public bool ColumnExists<T>(OpenOrmDbConnection cnx, string columnName)
        {
            return ColumnExists(cnx, typeof(T), columnName);
        }

        public bool ColumnExists(OpenOrmDbConnection cnx, Type modelType, string columnName)
        {
            SqlQuery sq = new SqlQuery();
            string dbname = cnx.ConnectionString.ToLower().GetStringBetween("database=", ";");
            sq.AddParameter("DB_NAME", dbname, SqlDbType.NVarChar);
            sq.AddParameter("TABLE_NAME", GetTableName(modelType), SqlDbType.NVarChar);
            sq.AddParameter("COL_NAME", columnName, SqlDbType.NVarChar);
            //SqlResult r = sq.Read(cnx, "SELECT COUNT(name) FROM sqlite_master WHERE type='table' AND name=@TABLE_NAME", SqlQueryType.Sql);
            SqlResult r = sq.Read(cnx, "SELECT count(*) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = @DB_NAME AND TABLE_NAME = @TABLE_NAME AND COLUMN_NAME=@COL_NAME;", SqlQueryType.Sql);
            bool result = r.HasRows && (r.Get<bool>(0, 0));
            sq.Dispose();
            r.Dispose();
            return result;
        }
        //todo
        public void AddColumn(OpenOrmDbConnection cnx, Type modelType, ColumnDefinition colDef)
        {
            if (!ColumnExists(cnx, modelType, colDef.Name))
            {
                SqlQuery.Execute(cnx, $"ALTER TABLE `{GetTableName(modelType)}` ADD COLUMN `{colDef.Name}` {MySqlTools.ToStringType(colDef.PropertyType.GetBaseType())}; ");
                SqlQuery.Execute(cnx, $"ALTER TABLE `{GetTableName(modelType)}` DROP PRIMARY KEY, ADD PRIMARY KEY( `{ string.Join(",", colDef.TableDefinition.PrimaryKeys.Select(x => x.Name).ToList()) }` ); ");
                //throw new Exception("Modifier la primary key si ka colonne est une clé");
            }
        }
        //todo
        public void DropColumn(OpenOrmDbConnection cnx, Type modelType, string colName)
        {
            if (ColumnExists(cnx, modelType, colName))
            {
                SqlQuery.Execute(cnx, $"ALTER TABLE {GetTableName(modelType)} DROP COLUMN {colName};");


                //string temp_table = GetTableName(modelType) + "_" + Guid.NewGuid().ToString().Replace("-", "");
                //List<string> columns = new List<string>();
                //List<string> colNames = new List<string>();

                //string sql = $"CREATE TABLE `{temp_table}` (";

                //SqlQuery sq = new SqlQuery(cnx, $"PRAGMA table_info({GetTableName(modelType)})", SqlQueryType.Sql);
                //var tableCols = sq.ReadToDictionaryList();
                //sq.Dispose();

                //foreach (var col in tableCols)
                //{
                //    string tableColName = col["name"].ToString();
                //    string tableColType = col["type"].ToString();

                //    if (tableColName == colName) continue;

                //    colNames.Add(tableColName);

                //    string fieldsql = $" {tableColName} {tableColType}";
                //    if (col["pk"].ToString() == "1") fieldsql += " PRIMARY KEY";
                //    if (col["notnull"].ToString() == "1") fieldsql += " NOT NULL";
                //    if (col["dflt_value"] != null) fieldsql += $" DEFAULT {MySqlTools.FormatValueToString(col["dflt_value"])}";

                //    columns.Add(fieldsql);
                //}

                //string fields = string.Join(",", columns);
                //string names = string.Join(",", colNames);
                //sql += fields + ");";

                //SqlQuery.Execute(cnx, sql, SqlQueryType.Sql);
                //SqlQuery.Execute(cnx, $"INSERT INTO {temp_table} SELECT {names} FROM {GetTableName(modelType)};", SqlQueryType.Sql);
                //DropTable(cnx, GetTableName(modelType));
                //SqlQuery.Execute(cnx, $"ALTER TABLE {temp_table} RENAME TO {GetTableName(modelType)};", SqlQueryType.Sql);
                //SqlQuery.Execute(cnx, $"CREATE INDEX {GetTableName(modelType)}_INDEX ON {GetTableName(modelType)}({names});", SqlQueryType.Sql);
            }
        }
        #endregion

        #region Field
        public bool Exists<T>(OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate)
        {
            return SelectFirst<T>(cnx, predicate) != null;
        }
        #endregion

        #region Insert
        //todo
        public long Insert<T>(OpenOrmDbConnection cnx, T model)
        {
            SqlQuery sq = new SqlQuery();
            List<string> columns = new List<string>();
            List<string> values = new List<string>();
            TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);

            foreach (ColumnDefinition cd in td.Columns)
            {
                var value = cd.PropertyInfo.GetValue(model);
                if (value == null && cd.IsNotNullColumn && cd.HasDefaultValue)
                {
                    value = cd.DefaultValue;
                }
                if (value == null)
                {
                    value = DBNull.Value;
                }

                if (!cd.IsAutoIncrement)
                {
                    columns.Add(cd.Name);

                    string paramName = $"@p{sq.Parameters.Count}";
                    sq.AddParameter(paramName, value, OpenOrmTools.ToSqlDbType(cd.PropertyType));
                    values.Add(paramName);
                }
            }

            string sql = $"INSERT INTO `{GetTableName<T>()}` (`{string.Join("`,`", columns)}`) VALUES ({string.Join(",", values)}); SELECT LAST_INSERT_ID();";
            SqlResult r = sq.Read(cnx, sql, SqlQueryType.Sql);
            long result = r.Get<long>(0, 0);
            sq.Dispose();
            r.Dispose();
            return result;
        }
        //todo
        public void Insert<T>(OpenOrmDbConnection cnx, List<T> models)
        {
            if (models != null && models.Count > 0)
            {
                List<string> columns = new List<string>();
                List<string> values;
                TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);

                int chunkSize = (1000 / td.Columns.Count) - 1;

                foreach (List<T> submodels in models.Chunk(chunkSize))
                {
                    string sql = "START TRANSACTION; ";

                    foreach (ColumnDefinition cd in td.Columns)
                    {
                        var value = cd.PropertyInfo.GetValue(models[0]);

                        if (value != null)
                        {
                            if (!cd.IsAutoIncrement)
                            {
                                columns.Add(cd.Name);
                            }
                        }
                    }

                    foreach (T model in submodels)
                    {
                        columns = new List<string>();
                        values = new List<string>();

                        foreach (ColumnDefinition cd in td.Columns)
                        {
                            var value = cd.PropertyInfo.GetValue(model);

                            string formated_value = MySqlTools.FormatValueToString(value);
                            if (value != null)
                            {
                                if (!cd.IsAutoIncrement)
                                {
                                    columns.Add(cd.Name);
                                    values.Add(formated_value);
                                }
                            }
                        }

                        sql += $"INSERT INTO `{GetTableName<T>()}` (`{string.Join("`,`", columns)}`) VALUES ({string.Join(",", values)}); {Environment.NewLine}";
                    }

                    sql += " COMMIT;";

                    SqlQuery sq = new SqlQuery(cnx);
                    sq.ExecuteSql(cnx, sql);
                    sq.Dispose();
                }
            }
        }
        #endregion

        #region Select
        //todo
        public List<T> Select<T>(OpenOrmDbConnection cnx, bool forceLoadNestedObjects = false)
        {
            if (cnx.Configuration.EnableRamCache && RamCache.Exists(RamCache.GetKey<T>())) return (List<T>)RamCache.Get(RamCache.GetKey<T>());

            TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
            string sql = $"SELECT {td.GetFieldsStr(false)} FROM `{GetTableName<T>()}`;";
            SqlQuery sq = new SqlQuery(cnx, sql, SqlQueryType.Sql);
            List<T> result = sq.ReadToObjectList<T>();

            //Load nested objects/values
            if (((td.ContainsNestedColumns && forceLoadNestedObjects) || td.ContainsNestedColumnsAutoLoad || cnx.Configuration.ForceAutoLoadNestedObjects) && result != null)
            {
                LoadNestedValues(cnx, forceLoadNestedObjects, ref result);
            }

            if (cnx.Configuration.EnableRamCache) RamCache.Set(RamCache.GetKey<T>(), result);

            sq.Dispose();
            return result;
        }
        //todo
        public List<T> Select<T>(OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate, bool forceLoadNestedObjects = false)
        {
            if (cnx.Configuration.EnableRamCache && RamCache.Exists(RamCache.GetKey<T>(predicate))) return (List<T>)RamCache.Get(RamCache.GetKey<T>(predicate));

            TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
            string sql = $"SELECT {td.GetFieldsStr(false)} FROM `{GetTableName<T>()}` WHERE ";

            sql += predicate.ToSqlWhere(td, out List<SqlParameterItem> Parameters);

            SqlQuery sq = new SqlQuery(cnx, sql, SqlQueryType.Sql);
            Parameters.ForEach(x => sq.AddParameter(x.Name, x.Value, x.SqlDbType));
            List<T> result = sq.ReadToObjectList<T>();

            //Load nested objects/values
            if (((td.ContainsNestedColumns && forceLoadNestedObjects) || td.ContainsNestedColumnsAutoLoad || cnx.Configuration.ForceAutoLoadNestedObjects) && result != null)
            {
                LoadNestedValues(cnx, forceLoadNestedObjects, ref result);
            }

            if (cnx.Configuration.EnableRamCache) RamCache.Set(RamCache.GetKey<T>(predicate), result);

            sq.Dispose();
            return result;
        }
        //todo
        public T SelectFirst<T>(OpenOrmDbConnection cnx, bool forceLoadNestedObjects = false)
        {
            TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
            string sql = $"SELECT {td.GetFieldsStr(false)} FROM `{GetTableName<T>()}` WHERE ";

            SqlQuery sq = new SqlQuery(cnx, sql, SqlQueryType.Sql);
            T result = sq.ReadToObject<T>();

            //Load nested objects/values
            if (((td.ContainsNestedColumns && forceLoadNestedObjects) || td.ContainsNestedColumnsAutoLoad || cnx.Configuration.ForceAutoLoadNestedObjects) && result != null)
            {
                List<T> list = new List<T>();
                list.Add(result);
                LoadNestedValues(cnx, forceLoadNestedObjects, ref list);
                result = list[0];
            }

            sq.Dispose();
            return result;
        }
        //todo
        public T SelectFirst<T>(OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate, bool forceLoadNestedObjects = false)
        {
            TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
            string sql = $"SELECT {td.GetFieldsStr(false)} FROM `{GetTableName<T>()}` WHERE ";

            sql += predicate.ToSqlWhere(td, out List<SqlParameterItem> Parameters);

            SqlQuery sq = new SqlQuery(cnx, sql, SqlQueryType.Sql);
            Parameters.ForEach(x => sq.AddParameter(x.Name, x.Value, x.SqlDbType));
            T result = sq.ReadToObject<T>();

            //Load nested objects/values
            if (((td.ContainsNestedColumns && forceLoadNestedObjects) || td.ContainsNestedColumnsAutoLoad || cnx.Configuration.ForceAutoLoadNestedObjects) && result != null)
            {
                List<T> list = new List<T> { result };
                LoadNestedValues(cnx, forceLoadNestedObjects, ref list);
                result = list[0];
            }

            sq.Dispose();
            return result;
        }
        //todo
        public T SelectLast<T>(OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate, bool forceLoadNestedObjects = false)
        {
            TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
            string sql = $"SELECT {td.GetFieldsStr(false)} FROM `{GetTableName<T>()}` WHERE ";

            sql += predicate.ToSqlWhere(td, out List<SqlParameterItem> Parameters);

            SqlQuery sq = new SqlQuery(cnx, sql, SqlQueryType.Sql);
            Parameters.ForEach(x => sq.AddParameter(x.Name, x.Value, x.SqlDbType));
            T result = sq.ReadToObjectLast<T>();

            //Load nested objects/values
            if (((td.ContainsNestedColumns && forceLoadNestedObjects) || td.ContainsNestedColumnsAutoLoad || cnx.Configuration.ForceAutoLoadNestedObjects) && result != null)
            {
                List<T> list = new List<T>();
                list.Add(result);
                LoadNestedValues(cnx, forceLoadNestedObjects, ref list);
                result = list[0];
            }

            sq.Dispose();
            return result;
        }
        //todo
        public T SelectById<T>(OpenOrmDbConnection cnx, object id, bool forceLoadNestedObjects = false)
        {
            TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
            string sql = $"SELECT {td.GetFieldsStr(false)} FROM `{GetTableName<T>()}` WHERE Id = {id} LIMIT 1;";
            SqlQuery sq = new SqlQuery(cnx, sql, SqlQueryType.Sql);
            T result = sq.ReadToObject<T>();

            //Load nested objects/values
            if (((td.ContainsNestedColumns && forceLoadNestedObjects) || td.ContainsNestedColumnsAutoLoad || cnx.Configuration.ForceAutoLoadNestedObjects) && result != null)
            {
                List<T> list = new List<T>();
                list.Add(result);
                LoadNestedValues(cnx, forceLoadNestedObjects, ref list);
                result = list[0];
            }

            sq.Dispose();
            return result;
        }
        //todo
        public long Count<T>(OpenOrmDbConnection cnx)
        {
            string sql = $"SELECT COUNT({OpenOrmTools.GetPrimaryKeyFieldName<T>()}) FROM `{GetTableName<T>()}`";
            SqlResult r = SqlQuery.ExecuteRead(cnx, sql, SqlQueryType.Sql);

            long result = 0;
            if (r.HasRows) result = r.Get<long>(0, 0);
            r.Dispose();
            return result;
        }
        //todo
        public long Count<T>(OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate)
        {
            TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
            string sql = $"SELECT COUNT({OpenOrmTools.GetPrimaryKeyFieldName<T>()}) FROM `{GetTableName<T>()}` WHERE ";

            sql += predicate.ToSqlWhere(td, out List<SqlParameterItem> Parameters);

            SqlResult r = SqlQuery.ExecuteRead(cnx, sql, Parameters, SqlQueryType.Sql);

            long result = 0;
            if (r.HasRows) result = r.Get<long>(0, 0);
            r.Dispose();
            return result;
        }
        #endregion

        #region Update
        //todo
        public void Update<T>(OpenOrmDbConnection cnx, T model, TableDefinition td = null)
        {
            string sql = "";
            List<string> updateFields = new List<string>();
            List<string> keyFields = new List<string>();
            List<SqlParameterItem> parameters = new List<SqlParameterItem>();
            if (td == null) td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);

            foreach (ColumnDefinition cd in td.Columns)
            {
                var value = cd.PropertyInfo.GetValue(model);
                if (value == null && cd.IsNotNullColumn && cd.HasDefaultValue)
                {
                    value = cd.DefaultValue;
                }
                if (value == null)
                {
                    value = DBNull.Value;
                }

                string paramName = $"@p{parameters.Count}";

                if (cd.IsPrimaryKey)
                {
                    keyFields.Add($"`{cd.Name}`={paramName}");
                }
                else
                {
                    updateFields.Add($"`{cd.Name}`={paramName}");
                }
                parameters.Add(new SqlParameterItem { Name = paramName, Value = value, SqlDbType = OpenOrmTools.ToSqlDbType(cd.PropertyType) });
            }

            sql = $"UPDATE `{GetTableName<T>()}` SET {string.Join(",", updateFields)}";

            if (keyFields.Count > 0)
            {
                sql += $" WHERE {string.Join(" AND ", keyFields)} ";
            }

            sql += ";";

            SqlQuery sq = new SqlQuery();
            parameters.ForEach(x => sq.AddParameter(x.Name, x.Value, x.SqlDbType));
            sq.ExecuteSql(cnx, sql);
            sq.Dispose();
        }
        #endregion

        #region Delete
        //todo
        public void Delete<T>(OpenOrmDbConnection cnx, T model, TableDefinition td = null)
        {
            List<string> keyFields = new List<string>();
            List<SqlParameterItem> parameters = new List<SqlParameterItem>();
            if (td == null) td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);

            foreach (ColumnDefinition cd in td.Columns)
            {
                if (cd.IsPrimaryKey)
                {
                    var value = cd.PropertyInfo.GetValue(model);
                    string paramName = $"@p{parameters.Count}";
                    keyFields.Add($"`{cd.Name}`={paramName}");
                    parameters.Add(new SqlParameterItem { Name = paramName, Value = value });
                    
                    if (td.PrimaryKeysCount == 1) break;
                }
            }

            string sql = $"DELETE FROM `{GetTableName<T>()}` WHERE {string.Join(" AND ", keyFields)};";

            SqlQuery sq = new SqlQuery();
            sq.ExecuteSql(cnx, sql, parameters);
            sq.Dispose();
        }

        //public async Task DeleteAsync<T>(OpenOrmDbConnection cnx, T model, TableDefinition td = null)
        //{
        //    List<string> keyFields = new List<string>();
        //    List<SqlParameterItem> parameters = new List<SqlParameterItem>();
        //    if (td == null) td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);

        //    foreach (ColumnDefinition cd in td.Columns)
        //    {
        //        if (cd.IsPrimaryKey)
        //        {
        //            var value = cd.PropertyInfo.GetValue(model);
        //            string paramName = $"@p{parameters.Count}";
        //            keyFields.Add($"{cd.Name}={paramName}");
        //            parameters.Add(new SqlParameterItem { Name = paramName, Value = value });
        //        }
        //        if (td.PrimaryKeysCount == 1) break;
        //    }

        //    string sql = $"DELETE FROM {GetTableName<T>()} WHERE {string.Join(" AND ", keyFields)};";

        //    SqlQuery sq = new SqlQuery();
        //    await sq.ExecuteSqlAsync(cnx, sql, parameters);
        //    sq.Dispose();
        //}
        //todo
        public void Delete<T>(OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate)
        {
            TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
            string sql = $"DELETE FROM `{GetTableName<T>()}` WHERE ";

            sql += predicate.ToSqlWhere(td, out var Parameters);

            SqlQuery sq = new SqlQuery();
            Parameters.ForEach(x => sq.AddParameter(x.Name, x.Value, x.SqlDbType));
            sq.ExecuteSql(cnx, sql);
            sq.Dispose();
        }
        //todo
        public void DeleteAll<T>(OpenOrmDbConnection cnx)
        {
            //TruncateTable<T>(cnx);
            string sql = $"DELETE FROM `{GetTableName<T>()}`";


            SqlQuery sq = new SqlQuery();
            sq.ExecuteSql(cnx, sql);
            sq.Dispose();
        }
        #endregion

        #region Transaction
        public void BeginTransaction(OpenOrmDbConnection cnx)
        {
            if (cnx.Transaction != null)
            {
                throw new Exception("A transaction already exists for this connection. Commit or rollback first.");
            }
            cnx.Transaction = cnx.Connection.BeginTransaction();
        }

        public void CommitTransaction(OpenOrmDbConnection cnx)
        {
            cnx.Transaction.Commit();
            cnx.Transaction = null;
        }

        public void RollbackTransaction(OpenOrmDbConnection cnx)
        {
            cnx.Transaction.Rollback();
            cnx.Transaction = null;
        }
        #endregion

        #region SQL
        public SqlResult Sql(OpenOrmDbConnection cnx, string free_sql_request, List<SqlParameterItem> parameters = null)
        {
            SqlQuery sq = new SqlQuery();
            SqlResult sr = sq.Read(cnx, free_sql_request, parameters, SqlQueryType.Sql);
            CoreTools.RamCache.InvalidateAll();
            return sr;
        }

        public SqlResult Sql(OpenOrmDbConnection cnx, string free_sql_request, SqlParameterItem parameter)
        {
            SqlQuery sq = new SqlQuery();
            SqlResult sr = sq.Read(cnx, free_sql_request, new List<SqlParameterItem> { parameter }, SqlQueryType.Sql);
            CoreTools.RamCache.InvalidateAll();
            return sr;
        }

        public void SqlNonQuery(OpenOrmDbConnection cnx, string free_sql_request, List<SqlParameterItem> parameters = null)
        {
            SqlQuery sq = new SqlQuery();
            sq.ExecuteSql(cnx, free_sql_request, parameters);
            CoreTools.RamCache.InvalidateAll();
            sq.Dispose();
        }

        public void SqlNonQuery(OpenOrmDbConnection cnx, string free_sql_request, SqlParameterItem parameter)
        {
            SqlQuery sq = new SqlQuery();
            sq.ExecuteSql(cnx, free_sql_request, new List<SqlParameterItem> { parameter });
            CoreTools.RamCache.InvalidateAll();
            sq.Dispose();
        }
        #endregion

        #region Helpers
        //todo
        public void LoadNestedValues<T>(OpenOrmDbConnection cnx, bool forceLoad, ref List<T> Result)
        {
            if (Result == null || (Result != null && Result.Count == 0)) return;

            TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);

            foreach (ColumnDefinition cd in td.NestedColumns)
            {
                if (!cd.NestedAutoLoad && !forceLoad) continue;

                TableDefinition nested_td = TableDefinition.Get(cd.NestedChildType, cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
                string fields = string.Join(",", nested_td.Columns.Select(x => x.Name));

                if (cd.PropertyType.IsListOrArray())
                {
                    List<object> keys = Result.Select(x => x.GetType().GetProperty(cd.NestedParentPrimaryKeyProperty).GetValue(x, null)).Distinct().ToList();

                    string sql = "";
                    if (cd.NestedChildForeignKeyPropertyType.Name.ToLower() == "string")
                    {
                        sql = $"SELECT {fields} FROM `{GetTableName(cd.NestedChildType)}` WHERE {cd.NestedChildForeignKeyProperty} IN ('{string.Join("','", keys)}')";
                    }
                    else
                    {
                        sql = $"SELECT {fields} FROM `{GetTableName(cd.NestedChildType)}` WHERE {cd.NestedChildForeignKeyProperty} IN ({string.Join(",", keys)})";
                    }

                    SqlQuery sq = new SqlQuery(cnx, sql, SqlQueryType.Sql);
                    var nested = sq.ReadToObjectList(cd.NestedChildType);

                    //Load nested objects/values
                    if (string.IsNullOrEmpty(cd.NestedChildPropertyToGet))
                    {
                        TableDefinition inner_td = TableDefinition.Get(cd.NestedChildType, cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
                        if (((inner_td.ContainsNestedColumns && forceLoad) || inner_td.ContainsNestedColumnsAutoLoad || cnx.Configuration.ForceAutoLoadNestedObjects) && nested != null)
                        {
                            LoadNestedValues(cnx, forceLoad, ref nested);
                        }
                    }

                    if (nested.Count > 0)
                    {
                        foreach (T item in Result)
                        {
                            //var values = nested.Where(x => x.GetType().GetProperty(cd.NestedChildForeignKeyProperty).GetValue(x, null).Equals(item.GetType().GetProperty(cd.NestedParentPrimaryKeyProperty).GetValue(item, null))).ToList();

                            Type listType = null;
                            if (string.IsNullOrEmpty(cd.NestedChildPropertyToGet))
                                listType = typeof(List<>).MakeGenericType(new[] { cd.NestedChildType });
                            else
                                listType = typeof(List<>).MakeGenericType(new[] { nested[0].GetType().GetProperty(cd.NestedChildPropertyToGet).PropertyType });

                            IList list = (IList)Activator.CreateInstance(listType);
                            foreach (object o in nested)
                            {
                                if (o.GetType().GetProperty(cd.NestedChildForeignKeyProperty).GetValue(o, null).Equals(item.GetType().GetProperty(cd.NestedParentPrimaryKeyProperty).GetValue(item, null)))
                                {
                                    if (string.IsNullOrEmpty(cd.NestedChildPropertyToGet))
                                        list.Add(o);
                                    else
                                        list.Add(o.GetType().GetProperty(cd.NestedChildPropertyToGet).GetValue(o, null));
                                }
                            }

                            item.GetType().GetProperty(cd.PropertyInfo.Name).SetValue(item, list);
                        }
                    }
                }
                else
                {
                    List<object> keys = Result.Select(x => x.GetType().GetProperty(cd.NestedParentPrimaryKeyProperty).GetValue(x, null)).Distinct().ToList();

                    string sql = "";
                    if (cd.NestedParentPrimaryKeyProperty.GetType().Name.ToLower() == "string")
                    {
                        sql = $"SELECT {fields} FROM `{GetTableName(cd.NestedChildType)}`";// WHERE {cd.NestedChildForeignKeyProperty} = '{string.Join("','", keys)}'";
                    }
                    else
                    {
                        sql = $"SELECT {fields} FROM `{GetTableName(cd.NestedChildType)}`";// WHERE {cd.NestedChildForeignKeyProperty} = {string.Join(",", keys)}";
                    }

                    SqlQuery sq = new SqlQuery(cnx, sql, SqlQueryType.Sql);
                    var nested = sq.ReadToObjectList(cd.NestedChildType);

                    //Load nested objects/values
                    if (string.IsNullOrEmpty(cd.NestedChildPropertyToGet))
                    {
                        TableDefinition inner_td = TableDefinition.Get(cd.NestedChildType, cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
                        if (((inner_td.ContainsNestedColumns && forceLoad) || inner_td.ContainsNestedColumnsAutoLoad || cnx.Configuration.ForceAutoLoadNestedObjects) && nested != null)
                        {
                            LoadNestedValues(cnx, forceLoad, ref nested);
                        }
                    }

                    foreach (T item in Result)
                    {
                        var o = nested.Where(x => x.GetType().GetProperty(cd.NestedChildForeignKeyProperty).GetValue(x, null)
                            .Equals(item.GetType().GetProperty(cd.NestedParentPrimaryKeyProperty).GetValue(item, null))).FirstOrDefault();
                        if (o != null)
                        {
                            if (string.IsNullOrEmpty(cd.NestedChildPropertyToGet))
                            {
                                item.GetType().GetProperty(cd.PropertyInfo.Name).SetValue(item, o);
                            }
                            else
                            {
                                var value = o.GetType().GetProperty(cd.NestedChildPropertyToGet).GetValue(o, null);
                                item.GetType().GetProperty(cd.PropertyInfo.Name).SetValue(item, value);
                            }
                        }
                    }
                }
            }
        }

        public string GetTableName(Type type, bool withSchema = true)
        {
            string name = OpenOrmTools.GetTableName(type);

            if (withSchema && !string.IsNullOrEmpty(Schema))
                return Schema + name;

            return name;
        }

        public string GetTableName(object model, bool withSchema = true)
        {
            string name = OpenOrmTools.GetTableName(model);

            if (withSchema && !string.IsNullOrEmpty(Schema))
                return Schema + name;

            return name;
        }

        public string GetTableName<T>(bool withSchema = true)
        {
            string name = OpenOrmTools.GetTableName<T>();

            if (withSchema && !string.IsNullOrEmpty(Schema))
                return Schema + name;

            return name;
        }
        #endregion
    }
}