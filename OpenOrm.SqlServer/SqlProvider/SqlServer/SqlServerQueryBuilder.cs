using OpenOrm.Configuration;
using OpenOrm.Schema;
using OpenOrm.SqlProvider.Shared;
using OpenOrm.SqlServer;
using OpenOrm.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenOrm.SqlServer.Schema;

namespace OpenOrm.SqlProvider.SqlServer
{
    public class SqlServerQueryBuilder : BaseQueryBuilder, ISqlQueryBuilder
    {
        #region Constructor
        public SqlServerQueryBuilder(OpenOrmConfigurationBase config)
        {
            if (config == null) config = new OpenOrmConfiguration();
            config.ConnectorProvider = new SqlServerConnector();
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
            TableDefinition td = new TableDefinition(modelType, cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);

            string sql = $"CREATE TABLE {GetTableName(modelType)} (";

            foreach(ColumnDefinition cd in td.Columns)
            {
                string fieldsql = $" [{cd.Name}]";
                if(cd.HasSize) fieldsql += $" {SqlServerTools.ToStringType(cd.PropertyType.GetBaseType(), size: cd.Size)}";
                else if(cd.IsSizeMax) fieldsql += $" {SqlServerTools.ToStringType(cd.PropertyType.GetBaseType(), size: -1)}";
                else if(cd.HasDecimalSize) fieldsql += $" {SqlServerTools.ToStringType(cd.PropertyType.GetBaseType(), scale: cd.Scale, precision: cd.Precision)}";
                else fieldsql += $" {SqlServerTools.ToStringType(cd.PropertyType.GetBaseType())}";

                if (cd.IsPrimaryKey && td.PrimaryKeysCount == 1)
                {
                    fieldsql += " PRIMARY KEY";
                    primaryKeys.Add(cd.Name);
                }
                else if(cd.IsPrimaryKey && td.PrimaryKeysCount > 1)
                {
                    fieldsql += " NOT NULL";
                    primaryKeys.Add(cd.Name);
                }

                if (cd.IsAutoIncrement)
                {
                    fieldsql += " IDENTITY (1, 1)";
                }

                if (cd.IsNotNullColumn || cd.IsUnique)
                {
                    fieldsql += " NOT NULL";
                }
                else if (!cd.IsPrimaryKey && !cd.IsAutoIncrement)
                {
                    fieldsql += " NULL";
                }

                if (cd.IsUnique)
                {
                    fieldsql += " UNIQUE";
                }

                if (!string.IsNullOrEmpty(fieldsql))
                    columns.Add(fieldsql);
            }

            string fields = string.Join(",", columns);
            sql += fields;
            
            if(td.PrimaryKeysCount > 1)
            {
                sql += $" CONSTRAINT PK_{td.TableName} PRIMARY KEY ({string.Join(",", primaryKeys)})";
            }

            sql += ");";

            SqlQuery sq = new SqlQuery();
            sq.ExecuteSql(cnx, sql);
            sq.Dispose();
            //Create default index for the table
            //sq.ExecuteSql(cnx, $"CREATE INDEX {GetTableName(modelType, false)}_INDEX ON {GetTableName(modelType)}({fields})");
        }

        public bool TableExists<T>(OpenOrmDbConnection cnx)
        {
            string tableName = GetTableName<T>(false, false);
            return TableExists(cnx, tableName);
        }

        //public async Task<bool> TableExistsAsync<T>(OpenOrmDbConnection cnx)
        //{
        //    string tableName = GetTableName<T>(false, false);
        //    return await TableExistsAsync(cnx, tableName);
        //}

        public bool TableExists(OpenOrmDbConnection cnx, Type modelType)
        {
            string tableName = GetTableName(modelType, false, false);
            return TableExists(cnx, tableName);
        }

        //public async Task<bool> TableExistsAsync(OpenOrmDbConnection cnx, Type modelType)
        //{
        //    string tableName = GetTableName(modelType, false, false);
        //    return await TableExistsAsync(cnx, tableName);
        //}

        public bool TableExists(OpenOrmDbConnection cnx, string tableName)
        {
            string sql = @"IF (EXISTS (SELECT TOP 1 1
                             FROM INFORMATION_SCHEMA.TABLES 
                             WHERE TABLE_SCHEMA = @TABLE_SCHEMA 
                             AND  TABLE_NAME = @TABLE_NAME))
                            BEGIN
                                SELECT 1 AS TABLE_EXISTS
                            END
                            ELSE BEGIN
                                SELECT 0 AS TABLE_EXISTS
                            END";

            SqlQuery sq = new SqlQuery();
            sq.AddParameter("TABLE_SCHEMA", Schema.Replace(".", ""), SqlDbType.NVarChar);
            sq.AddParameter("TABLE_NAME", tableName, SqlDbType.NVarChar);
            SqlResult r = sq.Read(cnx, sql, SqlQueryType.Sql);
            bool result = r.HasRows && (r.Get<bool>(0, 0));
            sq.Dispose();
            r.Dispose();
            return result;
        }

        //public async Task<bool> TableExistsAsync(OpenOrmDbConnection cnx, string tableName)
        //{
        //    string sql = @"IF (EXISTS (SELECT TOP 1 1
        //                     FROM INFORMATION_SCHEMA.TABLES 
        //                     WHERE TABLE_SCHEMA = @TABLE_SCHEMA 
        //                     AND  TABLE_NAME = @TABLE_NAME))
        //                    BEGIN
        //                        SELECT 1 AS TABLE_EXISTS
        //                    END
        //                    ELSE BEGIN
        //                        SELECT 0 AS TABLE_EXISTS
        //                    END";

        //    using SqlQuery sq = new SqlQuery();
        //    sq.AddParameter("TABLE_SCHEMA", Schema.Replace(".", ""), SqlDbType.NVarChar);
        //    sq.AddParameter("TABLE_NAME", tableName, SqlDbType.NVarChar);
        //    using SqlResult r = await sq.ReadAsync(cnx, sql, SqlQueryType.Sql);
        //    return r.HasRows && (r.Get<bool>(0, 0));
        //}

        public bool TemporaryTableExists<T>(OpenOrmDbConnection cnx)
        {
            string tableName = GetTableName<T>(false);
            return TemporaryTableExists(cnx, tableName);
        }

        public bool TemporaryTableExists(OpenOrmDbConnection cnx, string tableName)
        {
            string sql = "SELECT TOP 1 1 FROM tempdb.INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @TABLE_SCHEMA AND TABLE_NAME LIKE '#' + @TABLE_NAME + '%'";

            SqlQuery sq = new SqlQuery();
            sq.AddParameter("TABLE_SCHEMA", Configuration.Schema, SqlDbType.NVarChar);
            sq.AddParameter("TABLE_NAME", tableName, SqlDbType.NVarChar);
            SqlResult r = sq.Read(cnx, sql, SqlQueryType.Sql);
            bool result = r.HasRows && (r.Get<bool>(0, 0));
            sq.Dispose();
            r.Dispose();
            return result;
        }

        public void DropTable<T>(OpenOrmDbConnection cnx)
        {
            DropTable(cnx, GetTableName<T>());
        }

        public void DropTable(OpenOrmDbConnection cnx, Type modelType)
        {
            DropTable(cnx, GetTableName(modelType));
        }

        public void DropTable(OpenOrmDbConnection cnx, string tableName)
        {
            SqlQuery.Execute(cnx, $"DROP TABLE {tableName}");
        }

        public void TruncateTable<T>(OpenOrmDbConnection cnx)
        {
            DropTable<T>(cnx);
            CreateTable<T>(cnx);
        }

        public void TruncateTable(OpenOrmDbConnection cnx, Type modelType)
        {
            DropTable(cnx, modelType);
            CreateTable(cnx, modelType);
        }

        #endregion

        #region Column
        public bool ColumnExists<T>(OpenOrmDbConnection cnx, string columnName)
        {
            return ColumnExists(cnx, typeof(T), columnName);
        }

        public bool ColumnExists(OpenOrmDbConnection cnx, Type modelType, string columnName)
        {
            string sql = @"IF COL_LENGTH(@TABLE, @COL) IS NOT NULL
                               SELECT 1 AS COL_EXISTS
                           ELSE
                               SELECT 0 AS COL_EXISTS";

            SqlQuery sq = new SqlQuery();
            sq.AddParameter("TABLE", GetTableName(modelType, false, false), SqlDbType.NVarChar);
            sq.AddParameter("COL", columnName, SqlDbType.NVarChar);
            SqlResult r = sq.Read(cnx, sql, SqlQueryType.Sql);
            bool result = r.HasRows && (r.Get<bool>(0, 0));
            sq.Dispose();
            r.Dispose();
            return result;
        }

        public void AddColumn(OpenOrmDbConnection cnx, Type modelType, ColumnDefinition colDef)
        {
            if (!ColumnExists(cnx, modelType, colDef.Name))
            {
                string defValue = "";
                if (colDef.DefaultValue is string)
                {
                    defValue = $"'{colDef.DefaultValue.ToString().Replace("'", "''")}'";
                }
                else if (colDef.DefaultValue is DateTime)
                {
                    //defValue = $"'{((DateTime)colDef.DefaultValue).ToString("dd/MM/yyyy HH:mm:ss")}'";
                }
                else
                {
                    defValue = $"{colDef.DefaultValue}";
                }

                string sql = $"ALTER TABLE {GetTableName(modelType)} ADD [{colDef.Name}] ";

                if (colDef.HasSize) sql += $" {SqlServerTools.ToStringType(colDef.PropertyType.GetBaseType(), size: colDef.Size)}";
                else if (colDef.IsSizeMax) sql += $" {SqlServerTools.ToStringType(colDef.PropertyType.GetBaseType(), size: -1)}";
                else if (colDef.HasDecimalSize) sql += $" {SqlServerTools.ToStringType(colDef.PropertyType.GetBaseType(), scale: colDef.Scale, precision: colDef.Precision)}";
                else sql += $" {SqlServerTools.ToStringType(colDef.PropertyType.GetBaseType())}";

                if (colDef.IsNotNullColumn || colDef.IsUnique)
                {
                    sql += " NOT NULL";
                }
                else if (!colDef.IsPrimaryKey && colDef.IsAutoIncrement)
                {
                    sql += " NULL";
                }

                if (colDef.IsUnique)
                {
                    sql += " UNIQUE";
                }

                if (!string.IsNullOrEmpty(defValue))
                {
                    sql += $" DEFAULT {defValue}";
                    if (colDef.SetDefaultValueForExistingRows)
                    {
                        sql += " WITH VALUES";
                    }
                }

                SqlQuery.Execute(cnx, sql);
            }
        }

        public void DropColumn(OpenOrmDbConnection cnx, Type modelType, string colName)
        {
            if (ColumnExists(cnx, modelType, colName))
            {
                string sql = $@"declare @schema_name nvarchar(256)
                        declare @table_name nvarchar(256)
                        declare @col_name nvarchar(256)
                        declare @Command  nvarchar(1000)

                        set @schema_name = N'{Schema.Replace(".", "")}'
                        set @table_name = N'{GetTableName(modelType, false, false)}'
                        set @col_name = N'{colName}'

                        select @Command = 'ALTER TABLE ' + @schema_name + '.[' + @table_name + '] DROP CONSTRAINT ' + d.name
                            from sys.tables t
                            join sys.default_constraints d on d.parent_object_id = t.object_id
                            join sys.columns c on c.object_id = t.object_id and c.column_id = d.parent_column_id
                            where t.name = @table_name
                            and t.schema_id = schema_id(@schema_name)
                            and c.name = @col_name

                        execute (@Command)";
                SqlQuery.Execute(cnx, sql, SqlQueryType.Sql);


                sql = $@"declare @schema_name nvarchar(256)
                        declare @table_name nvarchar(256)
                        declare @col_name nvarchar(256)
                        declare @Command  nvarchar(1000)

                        set @schema_name = N'{Schema.Replace(".", "")}'
                        set @table_name = N'{GetTableName(modelType, false, false)}'
                        set @col_name = N'{colName}'

                        select @Command = 'ALTER TABLE ' + @schema_name + '.[' + @table_name + '] DROP CONSTRAINT ' + d.name
                            from sys.tables t 
                            join sys.indexes d on d.object_id = t.object_id  and d.type=2 and d.is_unique=1
                            join sys.index_columns ic on d.index_id=ic.index_id and ic.object_id=t.object_id
                            join sys.columns c on ic.column_id = c.column_id  and c.object_id=t.object_id
                            where t.name = @table_name and c.name=@col_name

                        --print @Command

                        execute (@Command)";
                SqlQuery.Execute(cnx, sql, SqlQueryType.Sql);


                SqlQuery.Execute(cnx, $"ALTER TABLE {GetTableName(modelType)} DROP COLUMN [{colName}];");
            }
        }
        #endregion

        #region Field
        public bool Exists<T>(OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate)
        {
            return Count<T>(cnx, predicate) > 0;
        }
        #endregion

        #region Insert
        public long Insert<T>(OpenOrmDbConnection cnx, T model)
        {
            string sql = "";
            List<string> columns = new List<string>();
            List<object> values = new List<object>();
            TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
            SqlQuery sq = new SqlQuery();

            sql += $"INSERT INTO {GetTableName<T>()} ([";

            foreach(ColumnDefinition cd in td.Columns)
            {
                var value = cd.PropertyInfo.GetValue(model);
                if (value == null && cd.IsNotNullColumn && cd.HasDefaultValue)
                {
                    value = cd.DefaultValue;
                }
                if (value == null || (value is DateTime time && time.Year == 1))
                {
                    value = DBNull.Value;
                }
                if (cd.DbType == DbType.DateTime && value != DBNull.Value && ((DateTime)value).Year == 1 && !cd.IsNotNullColumn)
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

            sql += string.Join("],[", columns) + "]) VALUES (" + string.Join(",", values) + "); SELECT SCOPE_IDENTITY();";

            SqlResult r = sq.Read(cnx, sql, SqlQueryType.Sql);
            long result = r.Get<long>(0, 0);

            sq.Dispose();
            r.Dispose();

            if (cnx.Configuration.EnableRamCache) CoreTools.RamCache.Invalidate<T>();

            return result;
        }

        public void Insert<T>(OpenOrmDbConnection cnx, List<T> models)
        {
            if (models != null && models.Count > 0)
            {
                if (cnx.Configuration.ListInsertAllowBulkInsert)
                {
                    try
                    {
                        InsertBulk(cnx, models);
                    }
                    catch (Exception ex)
                    {
                        if (cnx.Configuration.ListInsertFallBackToChunkInsertIfBulkInsertError)
                        {
                            InsertChunk(cnx, models);
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                }
                else
                {
                    InsertChunk(cnx, models);
                }

                if (cnx.Configuration.EnableRamCache) CoreTools.RamCache.Invalidate<T>();
            }
        }

        private void InsertBulk<T>(OpenOrmDbConnection cnx, List<T> models)
        {
            SqlBulkCopy bulk = new SqlBulkCopy((SqlConnection)cnx.Connection);
            TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);

            bulk.DestinationTableName = GetTableName<T>();

            DataTable dt = new DataTable();

            foreach(ColumnDefinition cd in td.Columns)
            {
                if(!cd.IsAutoIncrement)
                {
                    dt.Columns.Add(cd.Name);
                    bulk.ColumnMappings.Add(cd.PropertyInfo.Name, cd.Name);
                }
            }

            foreach (T model in models)
            {
                List<object> values = new List<object>();
                foreach(ColumnDefinition cd in td.Columns)
                {
                    if(!cd.IsAutoIncrement)
                    {
                        values.Add(cd.PropertyInfo.GetValue(model));
                    }
                }

                DataRow row = dt.NewRow();
                row.ItemArray = values.ToArray();
                dt.Rows.Add(row);
            }

            bulk.WriteToServer(dt);

            bulk.Close();
        }

        private void InsertChunk<T>(OpenOrmDbConnection cnx, List<T> models)
        {
            //List<PropertyInfo> properties = OpenOrmTools.GetValidProperties<T>();
            List<string> columns = new List<string>();
            List<string> values;
            List<string> rows;
            TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);

            int chunkSize = (1000 / td.Columns.Count) - 1;

            foreach(ColumnDefinition cd in td.Columns)
            {
                if(!cd.IsAutoIncrement)
                    columns.Add(cd.Name);
            }
            //foreach (PropertyInfo pi in properties)
            //{
            //    if (pi.Name.ToLower() != "id")
            //        columns.Add(pi.Name);
            //}

            string sql = $"INSERT INTO {GetTableName<T>()} ([" + string.Join("],[", columns) + "]) VALUES ";

            foreach (List<T> submodels in models.Chunk(chunkSize))
            {
                rows = new List<string>();

                foreach (T model in submodels)
                {
                    values = new List<string>();
                    foreach(ColumnDefinition cd in td.Columns)
                    {
                        string formated_value = SqlServerTools.FormatValueToString(cd.PropertyInfo.GetValue(model));

                        if (!cd.IsAutoIncrement)
                        {
                            values.Add(formated_value);
                        }
                    }
                    //foreach (PropertyInfo pi in properties)
                    //{
                    //    string formated_value = SqlServerTools.FormatValueToString(pi.GetValue(model));

                    //    if (pi.Name.ToLower() != "id")
                    //    {
                    //        values.Add(formated_value);
                    //    }
                    //}

                    rows.Add("(" + string.Join(",", values) + ")");
                }

                SqlQuery.Execute(cnx, sql + string.Join(",", rows));
            }
        }
        #endregion

        #region Select
        public List<T> Select<T>(OpenOrmDbConnection cnx, bool forceLoadNestedObjects = false)
        {
            if (cnx.Configuration.EnableRamCache && CoreTools.RamCache.Exists(CoreTools.RamCache.GetKey<T>())) return (List<T>)CoreTools.RamCache.Get(CoreTools.RamCache.GetKey<T>());
            TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
            
            //if(cnx.Configuration.UseDatabaseSchema)
            //{
            //    if(!DbDefinition.Definitions.Any()) SqlServerDbDefinition.InitDbDefinition(cnx);
            //    //if(!td.IsDbCacheInitialized) 
            //}
            //string fields = string.Join(",", OpenOrmTools.GetFieldNames<T>());
            //string fields = $"[{string.Join("],[", td.Columns.Select(x => x.Name))}]";
            //string sql = $"SELECT {fields} FROM {GetTableName<T>()};";
            string sql = $"SELECT {td.GetFieldsStr()} FROM {GetTableName<T>()};";
            SqlQuery sq = new SqlQuery(cnx, sql, SqlQueryType.Sql);
            List<T> result = sq.ReadToObjectList<T>();

            //Load nested objects/values
            if ((((td.ContainsNestedColumns || td.ContainsForeignKeys) && forceLoadNestedObjects) || td.ContainsNestedColumnsAutoLoad || td.ContainsForeignKeysAutoLoad || cnx.Configuration.ForceAutoLoadNestedObjects) && result != null)
            {
                LoadNestedValues(cnx, forceLoadNestedObjects, ref result);
            }

            if (cnx.Configuration.EnableRamCache) CoreTools.RamCache.Set(CoreTools.RamCache.GetKey<T>(), result);
            return result;
        }

        public List<T> Select<T>(OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate, bool forceLoadNestedObjects = false)
        {
            if (cnx.Configuration.EnableRamCache && CoreTools.RamCache.Exists(CoreTools.RamCache.GetKey<T>(predicate))) return (List<T>)CoreTools.RamCache.Get(CoreTools.RamCache.GetKey<T>(predicate));
            TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
            //string fields = string.Join(",", OpenOrmTools.GetFieldNames<T>());
            string sql = $"SELECT {td.GetFieldsStr()} FROM {GetTableName<T>()} ";
            string whereClause = predicate.ToSqlWhere(td, out List<SqlParameterItem> Parameters);

            SqlQuery sq = new SqlQuery(cnx, sql, SqlQueryType.Sql);

            if (Parameters.Any() || !string.IsNullOrEmpty(whereClause))
            {
                sql += $"WHERE {whereClause}";
                sq.Sql = sql;
                Parameters.ForEach(x => sq.AddParameter(x.Name, x.Value, x.SqlDbType));
            }

            List<T> result = sq.ReadToObjectList<T>();

            //Load nested objects/values
            if ((((td.ContainsNestedColumns || td.ContainsForeignKeys) && forceLoadNestedObjects) || td.ContainsNestedColumnsAutoLoad || td.ContainsForeignKeysAutoLoad || cnx.Configuration.ForceAutoLoadNestedObjects) && result != null)
            {
                LoadNestedValues(cnx, forceLoadNestedObjects, ref result);
            }

            if (cnx.Configuration.EnableRamCache) CoreTools.RamCache.Set(CoreTools.RamCache.GetKey<T>(predicate), result);
            return result;
        }

        public T SelectFirst<T>(OpenOrmDbConnection cnx, bool forceLoadNestedObjects = false)
        {
            TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
            //string fields = string.Join(",", OpenOrmTools.GetFieldNames<T>());
            //string fields = $"[{string.Join("],[", td.Columns.Select(x => x.Name))}]";
            string sql = $"SELECT TOP 1 {td.GetFieldsStr()} FROM {GetTableName<T>()} ";

            SqlQuery sq = new SqlQuery(cnx, sql, SqlQueryType.Sql);
            T result = sq.ReadToObject<T>();

            //Load nested objects/values
            if ((((td.ContainsNestedColumns || td.ContainsForeignKeys) && forceLoadNestedObjects) || td.ContainsNestedColumnsAutoLoad || td.ContainsForeignKeysAutoLoad || cnx.Configuration.ForceAutoLoadNestedObjects) && result != null)
            {
                List<T> list = new List<T>();
                list.Add(result);
                LoadNestedValues(cnx, forceLoadNestedObjects, ref list);
                result = list[0];
            }

            return result;
        }

        public T SelectFirst<T>(OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate, bool forceLoadNestedObjects = false)
        {
            TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
            //string fields = string.Join(",", OpenOrmTools.GetFieldNames<T>());
            //string fields = $"[{string.Join("],[", td.Columns.Select(x => x.Name))}]";
            string sql = $"SELECT {td.GetFieldsStr()} FROM {GetTableName<T>()} ";
            string whereClause = predicate.ToSqlWhere(td, out List<SqlParameterItem> Parameters);

            SqlQuery sq = new SqlQuery(cnx, sql, SqlQueryType.Sql);

            if (Parameters.Any() || !string.IsNullOrEmpty(whereClause))
            {
                sql += $"WHERE {whereClause}";
                sq.Sql = sql;
                Parameters.ForEach(x => sq.AddParameter(x.Name, x.Value, x.SqlDbType));
            }

            T result = sq.ReadToObject<T>();

            //Load nested objects/values
            if ((((td.ContainsNestedColumns || td.ContainsForeignKeys) && forceLoadNestedObjects) || td.ContainsNestedColumnsAutoLoad || td.ContainsForeignKeysAutoLoad || cnx.Configuration.ForceAutoLoadNestedObjects) && result != null)
            {
                List<T> list = new List<T>();
                list.Add(result);
                LoadNestedValues(cnx, forceLoadNestedObjects, ref list);
                result = list[0];
            }

            return result;
        }

        public T SelectLast<T>(OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate, bool forceLoadNestedObjects = false)
        {
            TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
            //string fields = string.Join(",", OpenOrmTools.GetFieldNames<T>());
            //string fields = $"[{string.Join("],[", td.Columns.Select(x => x.Name))}]";
            string sql = $"SELECT {td.GetFieldsStr()} FROM {GetTableName<T>()} ";
            string whereClause = predicate.ToSqlWhere(td, out List<SqlParameterItem> Parameters);

            SqlQuery sq = new SqlQuery(cnx, sql, SqlQueryType.Sql);

            if (Parameters.Any() || !string.IsNullOrEmpty(whereClause))
            {
                sql += $"WHERE {whereClause}";
                sq.Sql = sql;
                Parameters.ForEach(x => sq.AddParameter(x.Name, x.Value, x.SqlDbType));
            }

            T result = sq.ReadToObjectLast<T>();

            //Load nested objects/values
            if ((((td.ContainsNestedColumns || td.ContainsForeignKeys) && forceLoadNestedObjects) || td.ContainsNestedColumnsAutoLoad || td.ContainsForeignKeysAutoLoad || cnx.Configuration.ForceAutoLoadNestedObjects) && result != null)
            {
                List<T> list = new List<T>();
                list.Add(result);
                LoadNestedValues(cnx, forceLoadNestedObjects, ref list);
                result = list[0];
            }

            return result;
        }

        public T SelectById<T>(OpenOrmDbConnection cnx, object id, bool forceLoadNestedObjects = false)
        {
            TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
            //string fields = string.Join(",", OpenOrmTools.GetFieldNames<T>());
            //string fields = $"[{string.Join("],[", td.Columns.Select(x => x.Name))}]";
            if(!td.PrimaryKeys.Any())
            {
                throw new KeyNotFoundException($"PrimaryKey not found for model {typeof(T).FullName}");
            }
            string sql = $"SELECT TOP 1 {td.GetFieldsStr()} FROM {GetTableName<T>()} WHERE [{td.PrimaryKeys.First().Name}] = @p0;";
            SqlQuery sq = new SqlQuery(cnx, sql, SqlQueryType.Sql);
            sq.AddParameter("@p0", id);
            T result = sq.ReadToObject<T>();

            //Load nested objects/values
            if ((((td.ContainsNestedColumns || td.ContainsForeignKeys) && forceLoadNestedObjects) || td.ContainsNestedColumnsAutoLoad || td.ContainsForeignKeysAutoLoad || cnx.Configuration.ForceAutoLoadNestedObjects) && result != null)
            {
                List<T> list = new List<T>();
                list.Add(result);
                LoadNestedValues(cnx, forceLoadNestedObjects, ref list);
                result = list[0];
            }

            sq.Dispose();
            return result;
        }

        public long Count<T>(OpenOrmDbConnection cnx)
        {
            //List<PropertyInfo> pis = OpenOrmTools.GetValidProperties<T>();
            string sql = $"SELECT COUNT({OpenOrmTools.GetPrimaryKeyFieldName<T>()}) FROM {GetTableName<T>()}";
            SqlQuery sq = new SqlQuery();
            SqlResult r = sq.Read(cnx, sql, SqlQueryType.Sql);

            long result = 0;
            if (r.HasRows) result = r.Get<long>(0, 0);

            r.Dispose();
            sq.Dispose();

            return result;
        }

        public long Count<T>(OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate)
        {
            TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
            string sql = $"SELECT COUNT({OpenOrmTools.GetPrimaryKeyFieldName<T>()}) FROM {GetTableName<T>()} ";
            string whereClause = predicate.ToSqlWhere(td, out List<SqlParameterItem> Parameters);

            SqlQuery sq = new SqlQuery(cnx, sql, SqlQueryType.Sql);

            if (Parameters.Any() || !string.IsNullOrEmpty(whereClause))
            {
                sql += $"WHERE {whereClause}";
                sq.Sql = sql;
                Parameters.ForEach(x => sq.AddParameter(x.Name, x.Value, x.SqlDbType));
            }

            SqlResult r = sq.Read(cnx, sql, SqlQueryType.Sql);

            long result = 0;
            if (r.HasRows) result = r.Get<long>(0, 0);

            r.Dispose();
            sq.Dispose();

            return result;
        }
        #endregion

        #region Update
        public void Update<T>(OpenOrmDbConnection cnx, T model, TableDefinition td = null)
        {
            string sql = "";
            List<string> updateFields = new List<string>();
            List<string> keyFields = new List<string>();
            List<SqlParameterItem> parameters = new List<SqlParameterItem>();
            if(td == null) td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
            //sql += $"UPDATE {DefaultSchema}.{typeof(T).Name} (";

            foreach(ColumnDefinition cd in td.Columns)
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
                if(cd.DbType == DbType.DateTime && value != DBNull.Value && ((DateTime)value).Year == 1 && !cd.IsNotNullColumn)
                {
                    value = DBNull.Value;
                }

                string paramName = $"@p{parameters.Count}";

                if (cd.IsPrimaryKey)
                {
                    keyFields.Add($"[{cd.Name}]={paramName}");
                }
                else
                {
                    updateFields.Add($"[{cd.Name}]={paramName}");
                }
                parameters.Add(new SqlParameterItem { Name = paramName, Value = value, SqlDbType = OpenOrmTools.ToSqlDbType(cd.PropertyType) });
            }

            sql = $"UPDATE {GetTableName<T>()} SET {string.Join(",", updateFields)}";

            if (keyFields.Count > 0)
            {
                sql += $" WHERE {string.Join(" AND ", keyFields)} ";
            }

            sql += ";";

            SqlQuery sq = new SqlQuery(cnx);
            parameters.ForEach(x => sq.AddParameter(x.Name, x.Value, x.SqlDbType));
            sq.ExecuteSql(cnx, sql);
            sq.Dispose();

            if (cnx.Configuration.EnableRamCache) CoreTools.RamCache.Invalidate<T>();
        }

        #endregion

        #region Delete
        public void Delete<T>(OpenOrmDbConnection cnx, T model, TableDefinition td = null)
        {
            List<string> keyFields = new List<string>();
            SqlQuery sq = new SqlQuery();
            if (td == null) td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);

            foreach (ColumnDefinition cd in td.Columns)
            {
                if (cd.IsPrimaryKey)
                {
                    var value = cd.PropertyInfo.GetValue(model);
                    string paramName = $"@p{sq.Parameters.Count}";
                    keyFields.Add($"[{cd.Name}]={paramName}");
                    sq.AddParameter(new SqlParameterItem { Name = paramName, Value = value, SqlDbType = OpenOrmTools.ToSqlDbType(cd.PropertyType) });
                    if (td.PrimaryKeysCount == 1) break;
                }
            }

            sq.ExecuteSql(cnx, $"DELETE FROM {GetTableName<T>()} WHERE {string.Join(" AND ", keyFields)};");
            sq.Dispose();

            if(cnx.Configuration.EnableRamCache) CoreTools.RamCache.Invalidate<T>();
        }

        //public async Task DeleteAsync<T>(OpenOrmDbConnection cnx, T model, TableDefinition td = null)
        //{
        //    List<string> keyFields = new List<string>();
        //    SqlQuery sq = new SqlQuery();
        //    if (td == null) td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);

        //    foreach (ColumnDefinition cd in td.Columns)
        //    {
        //        if (cd.IsPrimaryKey)
        //        {
        //            var value = cd.PropertyInfo.GetValue(model);
        //            string paramName = $"@p{sq.Parameters.Count}";
        //            keyFields.Add($"{cd.Name}={paramName}");
        //            sq.AddParameter(new SqlParameterItem { Name = paramName, Value = value, SqlDbType = OpenOrmTools.ToSqlDbType(cd.PropertyType) });
        //            if (td.PrimaryKeysCount == 1) break;
        //        }
        //    }

        //    await sq.ExecuteSqlAsync(cnx, $"DELETE FROM {GetTableName<T>()} WHERE {string.Join(" AND ", keyFields)};");

        //    sq.Dispose();
        //}

        public void Delete<T>(OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate)
        {
            TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
            string sql = $"DELETE FROM {GetTableName<T>()} WHERE ";

            sql += predicate.ToSqlWhere(td, out var Parameters);

            SqlQuery sq = new SqlQuery(cnx, sql, SqlQueryType.Sql);
            Parameters.ForEach(x => sq.AddParameter(x));
            sq.ExecuteSql(cnx, sql);
            sq.Dispose();

            if (cnx.Configuration.EnableRamCache) CoreTools.RamCache.Invalidate<T>();
        }

        public void DeleteAll<T>(OpenOrmDbConnection cnx)
        {
            string sql = $"DELETE FROM {GetTableName<T>()} ";
            SqlQuery sq = new SqlQuery(cnx, sql, SqlQueryType.Sql);
            sq.ExecuteSql(cnx, sql);
            sq.Dispose();

            if (cnx.Configuration.EnableRamCache) CoreTools.RamCache.Invalidate<T>();
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
        }

        public void SqlNonQuery(OpenOrmDbConnection cnx, string free_sql_request, SqlParameterItem parameter)
        {
            SqlQuery sq = new SqlQuery();
            sq.ExecuteSql(cnx, free_sql_request, new List<SqlParameterItem> { parameter });
            CoreTools.RamCache.InvalidateAll();
        }
        #endregion

        #region Helpers
        private void LoadNestedValues<T>(OpenOrmDbConnection cnx, bool forceLoad, ref List<T> Result)
        {
            if (Result == null || (Result != null && Result.Count == 0)) return;

            TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);

            if(td.ContainsNestedColumns)
            {
                foreach (ColumnDefinition cd in td.NestedColumns)
                {
                    if (!cd.NestedAutoLoad && !forceLoad) continue;

                    TableDefinition nested_td = TableDefinition.Get(cd.NestedChildType, cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
                    string fields = nested_td.GetFieldsStr();

                    if (cd.PropertyType.IsListOrArray())
                    {
                        List<object> keys = Result.Select(x => x.GetType().GetProperty(cd.NestedParentPrimaryKeyProperty).GetValue(x, null)).Distinct().ToList();

                        string sql = "";
                        if (cd.NestedChildForeignKeyPropertyType.Name.ToLower() == "string")
                        {
                            sql = $"SELECT {fields} FROM {GetTableName(cd.NestedChildType)} WHERE {cd.NestedChildForeignKeyProperty} IN ('{string.Join("','", keys)}')";
                        }
                        else
                        {
                            sql = $"SELECT {fields} FROM {GetTableName(cd.NestedChildType)} WHERE {cd.NestedChildForeignKeyProperty} IN ({string.Join(",", keys)})";
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
                            sql = $"SELECT {fields} FROM {GetTableName(cd.NestedChildType)}";// WHERE {cd.NestedChildForeignKeyProperty} = '{string.Join("','", keys)}'";
                        }
                        else
                        {
                            sql = $"SELECT {fields} FROM {GetTableName(cd.NestedChildType)}";// WHERE {cd.NestedChildForeignKeyProperty} = {string.Join(",", keys)}";
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
            
            if(td.ContainsForeignKeys)
            {
                foreach (ColumnDefinition cd in td.ForeignKeys)
                {
                    if (!cd.ForeignAutoLoad && !forceLoad) continue;

                    TableDefinition nested_td = TableDefinition.Get(cd.ForeignType, cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
                    string fields = nested_td.GetFieldsStr();

                    if (cd.ForeignType.IsListOrArray())
                    {
                        List<object> keys = Result.Select(x => x.GetType().GetProperty(cd.ParentForeignKeyProperty).GetValue(x, null)).Distinct().ToList();

                        string sql = "";
                        if (cd.NestedChildForeignKeyPropertyType.Name.ToLower() == "string")
                        {
                            sql = $"SELECT {fields} FROM {GetTableName(cd.ForeignType)} WHERE {cd.NestedChildForeignKeyProperty} IN ('{string.Join("','", keys)}')";
                        }
                        else
                        {
                            sql = $"SELECT {fields} FROM {GetTableName(cd.ForeignType)} WHERE {cd.NestedChildForeignKeyProperty} IN ({string.Join(",", keys)})";
                        }

                        SqlQuery sq = new SqlQuery(cnx, sql, SqlQueryType.Sql);
                        var nested = sq.ReadToObjectList(cd.ForeignType);

                        //Load nested objects/values
                        if (string.IsNullOrEmpty(cd.NestedChildPropertyToGet))
                        {
                            TableDefinition inner_td = TableDefinition.Get(cd.ForeignType, cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
                            if (((inner_td.ContainsForeignKeys && forceLoad) || inner_td.ContainsForeignKeysAutoLoad || cnx.Configuration.ForceAutoLoadNestedObjects) && nested != null)
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
                                if (string.IsNullOrEmpty(cd.ForeignChildTargetProperty))
                                    listType = typeof(List<>).MakeGenericType(new[] { cd.ForeignType });
                                else
                                    listType = typeof(List<>).MakeGenericType(new[] { nested[0].GetType().GetProperty(cd.ForeignChildTargetProperty).PropertyType });

                                IList list = (IList)Activator.CreateInstance(listType);
                                foreach (object o in nested)
                                {
                                    if (o.GetType().GetProperty(cd.NestedChildForeignKeyProperty).GetValue(o, null).Equals(item.GetType().GetProperty(cd.ParentForeignKeyProperty).GetValue(item, null)))
                                    {
                                        if (string.IsNullOrEmpty(cd.ForeignChildTargetProperty))
                                            list.Add(o);
                                        else
                                            list.Add(o.GetType().GetProperty(cd.ForeignChildTargetProperty).GetValue(o, null));
                                    }
                                }

                                item.GetType().GetProperty(cd.PropertyInfo.Name).SetValue(item, list);
                            }
                        }
                    }
                    else
                    {
                        List<object> keys = Result.Select(x => x.GetType().GetProperty(cd.Name).GetValue(x, null)).Distinct().ToList();

                        string sql = "";
                        if (cd.PropertyType.GetType().Name.ToLower() == "string")
                        {
                            sql = $"SELECT {fields} FROM {GetTableName(cd.ForeignType)} WHERE {cd.ParentForeignKeyProperty} IN ('{string.Join("','", keys)}') ";// WHERE {cd.NestedChildForeignKeyProperty} IN '{string.Join("','", keys)}'";
                        }
                        else
                        {
                            sql = $"SELECT {fields} FROM {GetTableName(cd.ForeignType)} WHERE {cd.ParentForeignKeyProperty} IN ({string.Join(",", keys)}) ";// WHERE {cd.NestedChildForeignKeyProperty} IN {string.Join(",", keys)}";
                        }

                        SqlQuery sq = new SqlQuery(cnx, sql, SqlQueryType.Sql);
                        var nested = sq.ReadToObjectList(cd.ForeignType);

                        //Load nested objects/values
                        if (string.IsNullOrEmpty(cd.ForeignChildTargetProperty))
                        {
                            TableDefinition inner_td = TableDefinition.Get(cd.NestedChildType, cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
                            if (((inner_td.ContainsNestedColumns && forceLoad) || inner_td.ContainsNestedColumnsAutoLoad || cnx.Configuration.ForceAutoLoadNestedObjects) && nested != null)
                            {
                                LoadNestedValues(cnx, forceLoad, ref nested);
                            }
                        }

                        foreach (T item in Result)
                        {
                            var o = nested.Where(x => x.GetType().GetProperty(cd.ParentForeignKeyProperty).GetValue(x, null)
                                .Equals(item.GetType().GetProperty(cd.Name).GetValue(item, null))).FirstOrDefault();
                            if (o != null)
                            {
                                if (string.IsNullOrEmpty(cd.NestedChildPropertyToGet))
                                {
                                    item.GetType().GetProperty(cd.ForeignChildTargetProperty).SetValue(item, o);
                                }
                                else
                                {
                                    var value = o.GetType().GetProperty(cd.NestedChildPropertyToGet).GetValue(o, null);
                                    item.GetType().GetProperty(cd.ForeignChildTargetProperty).SetValue(item, value);
                                }
                            }
                        }
                    }
                }
            }
        }

        public string GetTableName(Type type, bool withSchema = true, bool withBrackets = true)
        {
            string name = OpenOrmTools.GetTableName(type);

            if (withSchema && !string.IsNullOrEmpty(Schema))
                return withBrackets ? Schema + "[" + name + "]" : Schema + name;

            return withBrackets ? "[" + name + "]" : name;
        }

        public string GetTableName(object model, bool withSchema = true, bool withBrackets = true)
        {
            string name = OpenOrmTools.GetTableName(model);

            if (withSchema && !string.IsNullOrEmpty(Schema))
                return withBrackets ? Schema + "[" + name + "]" : Schema + name;

            return withBrackets ? "[" + name + "]" : name;
        }

        public string GetTableName<T>(bool withSchema = true, bool withBrackets = true)
        {
            string name = OpenOrmTools.GetTableName<T>();

            if (withSchema && !string.IsNullOrEmpty(Schema))
                return withBrackets ? Schema + "[" + name + "]" : Schema + name;

            return withBrackets ? "[" + name + "]" : name;
        }
        #endregion
    }
}
