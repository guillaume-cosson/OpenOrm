using OpenOrm.Migration;
using OpenOrm.Schema;
using OpenOrm.Extensions;
using OpenOrm.SqlProvider.Shared;
using OpenOrm.SqlProvider.SqlServer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenOrm.SqlServer.Schema;

namespace OpenOrm.SqlServer
{
    public static class OpenOrmDbConnectionExtensions
    {
        //private static ISqlQueryBuilder currentBuilder;
        private static ISqlQueryBuilder GetQueryBuilder(OpenOrmDbConnection cnx)
        {
            //if(currentBuilder == null)
            //{
            //    currentBuilder = new SqlServerQueryBuilder(config);
            //}

            //return currentBuilder;

            //if(!SqlServerDbDefinition.IsInited(cnx.Database))
            //{
            //    SqlServerDbDefinition.InitDbDefinition(cnx);
            //}

            return new SqlServerQueryBuilder(cnx.Configuration);
        }

        public static OpenOrmDbConnection CreateConnection(OpenOrmConfiguration config)
        {
            OpenOrmDbConnection cnx = new OpenOrmDbConnection(config)
            {
                Connection = new SqlConnection(config.ConnectionString)
            };
            ((SqlConnection)cnx.Connection).Open();
            return cnx;
        }

        //public static void SetConfiguration(OpenOrmConfiguration config)
        //{
        //    GetQueryBuilder().Configuration = config;
        //}

        #region Table
        public static bool TableExists<T>(this OpenOrmDbConnection cnx)
        {
            return GetQueryBuilder(cnx).TableExists<T>(cnx);
        }

        public static bool TableExists(this OpenOrmDbConnection cnx, string tableName)
        {
            return GetQueryBuilder(cnx).TableExists(cnx, tableName);
        }

        public static bool TemporaryTableExists<T>(this OpenOrmDbConnection cnx)
        {
            return GetQueryBuilder(cnx).TemporaryTableExists<T>(cnx);
        }

        public static bool TemporaryTableExists(this OpenOrmDbConnection cnx, string tableName)
        {
            return GetQueryBuilder(cnx).TemporaryTableExists(cnx, tableName);
        }

        public static void CreateTable<T>(this OpenOrmDbConnection cnx)
        {
            GetQueryBuilder(cnx).CreateTable<T>(cnx);
        }

        public static void CreateTable(this OpenOrmDbConnection cnx, Type modelType, bool dropAndRecreateIfExists = false)
        {
            GetQueryBuilder(cnx).CreateTable(cnx, modelType);
        }

        public static void DropTable<T>(this OpenOrmDbConnection cnx)
        {
            GetQueryBuilder(cnx).DropTable<T>(cnx);
        }

        public static void DropTable(this OpenOrmDbConnection cnx, Type modelType)
        {
            GetQueryBuilder(cnx).DropTable(cnx, modelType);
        }

        public static void DropTable(this OpenOrmDbConnection cnx, string tableName)
        {
            GetQueryBuilder(cnx).DropTable(cnx, tableName);
        }

        public static void TruncateTable<T>(this OpenOrmDbConnection cnx)
        {
            GetQueryBuilder(cnx).TruncateTable<T>(cnx);
        }

        public static void TruncateTable(this OpenOrmDbConnection cnx, Type modelType)
        {
            GetQueryBuilder(cnx).TruncateTable(cnx, modelType);
        }

        //public static List<string> ListTables(this OpenOrmDbConnection cnx)
        //{
        //    return GetQueryBuilder(cnx).ListTables(cnx);
        //}

        //public static List<TableDefinition> GetTablesDefinitions(this OpenOrmDbConnection cnx)
        //{
        //    return GetQueryBuilder(cnx).GetTablesDefinitions(cnx);
        //}

        #region Async
        public static async Task<bool> TableExistsAsync<T>(this OpenOrmDbConnection cnx)
        {
            return await Task.Run(() => {
                return TableExists<T>(cnx);
            });
        }

        public static async Task<bool> TableExistsAsync(this OpenOrmDbConnection cnx, string tableName)
        {
            return await Task.Run(() => {
                return TableExists(cnx, tableName);
            });
        }

        public static async Task<bool> TemporaryTableExistsAsync<T>(this OpenOrmDbConnection cnx)
        {
            return await Task.Run(() => {
                return TemporaryTableExists<T>(cnx);
            });
        }

        public static async Task<bool> TemporaryTableExistsAsync(this OpenOrmDbConnection cnx, string tableName)
        {
            return await Task.Run(() => {
                return TemporaryTableExists(cnx, tableName);
            });
        }

        public static async Task CreateTableAsync<T>(this OpenOrmDbConnection cnx)
        {
            await Task.Run(() => {
                CreateTable<T>(cnx);
            });
        }

        public static async Task CreateTableAsync(this OpenOrmDbConnection cnx, Type modelType, bool dropAndRecreateIfExists = false)
        {
            await Task.Run(() => {
                CreateTable(cnx, modelType);
            });
        }

        public static async Task DropTableAsync<T>(this OpenOrmDbConnection cnx)
        {
            await Task.Run(() => {
                DropTable<T>(cnx);
            });
        }

        public static async Task DropTableAsync(this OpenOrmDbConnection cnx, Type modelType)
        {
            await Task.Run(() => {
                DropTable(cnx, modelType);
            });
        }

        public static async Task DropTableAsync(this OpenOrmDbConnection cnx, string tableName)
        {
            await Task.Run(() => {
                DropTable(cnx, tableName);
            });
        }

        public static async Task TruncateTableAsync<T>(this OpenOrmDbConnection cnx)
        {
            await Task.Run(() => {
                TruncateTable<T>(cnx);
            });
        }

        public static async Task TruncateTableAsync(this OpenOrmDbConnection cnx, Type modelType)
        {
            await Task.Run(() => {
                TruncateTable(cnx, modelType);
            });
        }

        #endregion

        #endregion

        #region Column
        public static bool ColumnExists<T>(this OpenOrmDbConnection cnx, string columnName)
        {
            return GetQueryBuilder(cnx).ColumnExists<T>(cnx, columnName);
        }

        public static bool ColumnExists(this OpenOrmDbConnection cnx, Type objType, string columnName)
        {
            return GetQueryBuilder(cnx).ColumnExists(cnx, objType, columnName);
        }

        public static void AddColumn(this OpenOrmDbConnection cnx, ColumnDefinition colDef)
        {
            GetQueryBuilder(cnx).AddColumn(cnx, colDef.OwnerType, colDef);
        }

        public static void AddColumn(this OpenOrmDbConnection cnx, Type objType, ColumnDefinition colDef)
        {
            GetQueryBuilder(cnx).AddColumn(cnx, objType, colDef);
        }

        public static void AddColumn(this OpenOrmDbConnection cnx, Type objType, string colName)
        {
            GetQueryBuilder(cnx).AddColumn(cnx, objType, new ColumnDefinition(objType, colName));
        }

        public static void DropColumn(this OpenOrmDbConnection cnx, Type objType, string colName)
        {
            GetQueryBuilder(cnx).DropColumn(cnx, objType, colName);
        }

        //public static List<string> ListColumns<T>(this OpenOrmDbConnection cnx)
        //{
        //    return GetQueryBuilder(cnx).ListColumns<T>(cnx);
        //}

        #region Async
        public async static Task<bool> ColumnExistsAsync<T>(this OpenOrmDbConnection cnx, string columnName)
        {
            return await Task.Run(() => {
                return GetQueryBuilder(cnx).ColumnExists<T>(cnx, columnName);
            });
        }

        public async static Task<bool> ColumnExistsAsync(this OpenOrmDbConnection cnx, Type objType, string columnName)
        {
            return await Task.Run(() => {
                return GetQueryBuilder(cnx).ColumnExists(cnx, objType, columnName);
            });
        }

        public async static Task AddColumnAsync(this OpenOrmDbConnection cnx, Type objType, ColumnDefinition colDef)
        {
            await Task.Run(() => {
                GetQueryBuilder(cnx).AddColumn(cnx, objType, colDef);
            });
        }

        public async static Task DropColumnAsync(this OpenOrmDbConnection cnx, Type objType, string colName)
        {
            await Task.Run(() => {
                GetQueryBuilder(cnx).DropColumn(cnx, objType, colName);
            });
        }
        #endregion
        #endregion

        #region Field
        public static bool Exists<T>(this OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate)
        {
            return GetQueryBuilder(cnx).Exists<T>(cnx, predicate);
        }

        #region Async
        public static async Task<bool> ExistsAsync<T>(this OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate)
        {
            return await Task.Run(() => {
                return Exists<T>(cnx, predicate);
            });
        }
        #endregion
        #endregion

        #region Insert/Async
        public static long Insert<T>(this OpenOrmDbConnection cnx, T model)
        {
            if (model != null)
                return GetQueryBuilder(cnx).Insert<T>(cnx, model);
            else return -1;
        }

        public static void Insert<T>(this OpenOrmDbConnection cnx, List<T> models)
        {
            if (models != null && models.Any())
                GetQueryBuilder(cnx).Insert<T>(cnx, models);
        }

        #region Async
        public static async Task<long> InsertAsync<T>(this OpenOrmDbConnection cnx, T model)
        {
            return await Task.Run(() => {
                return Insert<T>(cnx, model);
            });
        }

        public static async Task InsertAsync<T>(this OpenOrmDbConnection cnx, List<T> models)
        {
            await Task.Run(() => {
                Insert<T>(cnx, models);
            });
        }
        #endregion
        #endregion

        #region Select/Async
        public static List<T> Select<T>(this OpenOrmDbConnection cnx, bool forceLoadNestedObjects = false)
        {
            return GetQueryBuilder(cnx).Select<T>(cnx, forceLoadNestedObjects);
        }

        public static List<T2> Select<T, T2>(this OpenOrmDbConnection cnx, bool forceLoadNestedObjects = false)
        {
            return OpenOrmTools.ChangeType<T2>((IEnumerable<object>)GetQueryBuilder(cnx).Select<T>(cnx, forceLoadNestedObjects));
        }

        public static List<T> Select<T>(this OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate, bool forceLoadNestedObjects = false)
        {
            return GetQueryBuilder(cnx).Select<T>(cnx, predicate, forceLoadNestedObjects);
        }

        public static T SelectFirst<T>(this OpenOrmDbConnection cnx, bool forceLoadNestedObjects = false)
        {
            return GetQueryBuilder(cnx).SelectFirst<T>(cnx, forceLoadNestedObjects);
        }

        public static T SelectFirst<T>(this OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate, bool forceLoadNestedObjects = false)
        {
            return GetQueryBuilder(cnx).SelectFirst<T>(cnx, predicate, forceLoadNestedObjects);
        }

        public static T SelectLast<T>(this OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate, bool forceLoadNestedObjects = false)
        {
            return GetQueryBuilder(cnx).SelectLast<T>(cnx, predicate, forceLoadNestedObjects);
        }

        public static T SelectById<T>(this OpenOrmDbConnection cnx, long id, bool forceLoadNestedObjects = false)
        {
            return GetQueryBuilder(cnx).SelectById<T>(cnx, id, forceLoadNestedObjects);
        }

        public static long Count<T>(this OpenOrmDbConnection cnx)
        {
            return GetQueryBuilder(cnx).Count<T>(cnx);
        }

        public static long Count<T>(this OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate)
        {
            return GetQueryBuilder(cnx).Count<T>(cnx, predicate);
        }


        #region Async
        public static async Task<List<T>> SelectAsync<T>(this OpenOrmDbConnection cnx)
        {
            return await Task.Run(() => {
                return Select<T>(cnx);
            });
        }

        public static async Task<List<T>> SelectAsync<T>(this OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate)
        {
            return await Task.Run(() => {
                return Select<T>(cnx, predicate);
            });
        }

        public static async Task<T> SelectFirstAsync<T>(this OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate)
        {
            return await Task.Run(() => {
                return SelectFirst<T>(cnx, predicate);
            });
        }

        public static async Task<T> SelectLastAsync<T>(this OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate)
        {
            return await Task.Run(() => {
                return SelectLast<T>(cnx, predicate);
            });
        }

        public static async Task<T> SelectByIdAsync<T>(this OpenOrmDbConnection cnx, long id)
        {
            return await Task.Run(() => {
                return SelectById<T>(cnx, id);
            });
        }

        public static async Task<long> CountAsync<T>(this OpenOrmDbConnection cnx)
        {
            return await Task.Run(() => {
                return Count<T>(cnx);
            });
        }

        public static async Task<long> CountAsync<T>(this OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate)
        {
            return await Task.Run(() => {
                return Count<T>(cnx, predicate);
            });
        }

        #endregion

        #endregion

        #region Update/Async
        public static void Update<T>(this OpenOrmDbConnection cnx, T model)
        {
            if (model != null)
                GetQueryBuilder(cnx).Update<T>(cnx, model);
        }

        public static void Update<T>(this OpenOrmDbConnection cnx, List<T> models)
        {
            if (models != null && models.Any())
            {
                ISqlQueryBuilder builder = GetQueryBuilder(cnx);
                TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
                bool useTransaction = cnx.Transaction == null;

                try
                {
                    if(useTransaction) cnx.BeginTransaction();
                    foreach (T model in models)
                    {
                        builder.Update<T>(cnx, model, td);
                    }
                    if (useTransaction) cnx.CommitTransaction();
                }
                catch (Exception ex)
                {
                    if (useTransaction) cnx.RollbackTransaction();
                    throw ex;
                }
            }
        }

        #region Async
        public static async Task UpdateAsync<T>(this OpenOrmDbConnection cnx, T model)
        {
            await Task.Run(() => {
                Update<T>(cnx, model);
            });
        }

        public static async Task UpdateAsync<T>(this OpenOrmDbConnection cnx, List<T> models)
        {
            await Task.Run(() => {
                Update<T>(cnx, models);
            });
        }
        #endregion
        #endregion

        #region Delete/Async
        public static void Delete<T>(this OpenOrmDbConnection cnx, T model)
        {
            if (model != null)
                GetQueryBuilder(cnx).Delete<T>(cnx, model);
        }

        public static void Delete<T>(this OpenOrmDbConnection cnx, List<T> models)
        {
            if (models != null && models.Any())
            {
                ISqlQueryBuilder builder = GetQueryBuilder(cnx);
                TableDefinition td = TableDefinition.Get<T>(cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);
                bool useTransaction = cnx.Transaction == null;

                try
                {
                    if (useTransaction) cnx.BeginTransaction();
                    foreach (T model in models)
                    {
                        builder.Delete<T>(cnx, model, td);
                    }
                    if (useTransaction) cnx.CommitTransaction();
                }
                catch (Exception)
                {
                    if (useTransaction) cnx.RollbackTransaction();
                    throw;
                }
            }
        }

        public static void Delete<T>(this OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate)
        {
            GetQueryBuilder(cnx).Delete<T>(cnx, predicate);
        }

        public static void DeleteAll<T>(this OpenOrmDbConnection cnx)
        {
            GetQueryBuilder(cnx).DeleteAll<T>(cnx);
        }

        #region Async
        public static async void DeleteAsync<T>(this OpenOrmDbConnection cnx, T model)
        {
            //if (model != null)
            //    GetQueryBuilder(cnx).DeleteAsync<T>(cnx, model);
            await Task.Run(() => {
                Delete<T>(cnx, model);
            });
        }

        public static async void DeleteAsync<T>(this OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate)
        {
            await Task.Run(() => {
                Delete<T>(cnx, predicate);
            });
        }

        public static async void DeleteAllAsync<T>(this OpenOrmDbConnection cnx)
        {
            await Task.Run(() => {
                DeleteAll<T>(cnx);
            });
        }
        #endregion
        #endregion

        #region Migration
        ///// <summary>
        ///// Automatic add columns to tables if properties are added to class that inherit from DbModel
        ///// </summary>
        ///// <param name="db"></param>
        //public static void AutomaticMigration(OpenOrmDbConnection db)
        //{
        //    List<DbModel> dbmodels = OpenOrmTools.GetEnumerableOfType<DbModel>().ToList();

        //    foreach(DbModel model in dbmodels)
        //    {
        //        Type modelType = model.GetType();
        //        bool tableExists = db.TableExists(modelType.Name);
        //        if (!tableExists)
        //        {
        //            db.CreateTable(modelType);
        //        }
        //        else
        //        {
        //            List<PropertyInfo> property_infos = OpenOrmTools.GetFields(modelType);

        //            foreach (PropertyInfo pi in property_infos)
        //            {
        //                if(!db.ColumnExists(modelType, pi.Name))
        //                {
        //                    ColumnDefinition field = new ColumnDefinition(pi);
        //                    db.AddColumn(modelType, field);
        //                }
        //            }
        //        }
        //    }
        //}

        public static void Migrate(this OpenOrmDbConnection db, string specificVersion = "")
        {
            //if (db.Configuration.EnableAutomaticMigration)
            //    AutomaticMigration(db);

            if (db.Configuration.EnableRamCache) CoreTools.RamCache.InvalidateAll();

            //Init Migration
            if (!db.TableExists<OpenOrmMigration>())
            {
                db.CreateTable<OpenOrmMigration>();
            }

            //Logger.System("Lancement de la migration bdd");

            var ExistingMigrations = db.Select<OpenOrmMigration>();
            string currentVersion = ExistingMigrations.Count > 0 ? ExistingMigrations.Select(x => x.Version).OrderBy(x => x.PadNumbers()).Last() : "";
            List<OpenOrmMigration> migrations = OpenOrmTools.GetEnumerableOfTypeFromEntryAssembly<OpenOrmMigration>()
                .Where(x => !string.IsNullOrEmpty(x.Version)).OrderBy(x => x.Version.PadNumbers()).ToList();

            if(!string.IsNullOrEmpty(specificVersion) && !migrations.Any(x => x.Version == specificVersion))
            {
                throw new System.Data.VersionNotFoundException($"Version {specificVersion} not fount in migrations classes");
            }


            if (!string.IsNullOrEmpty(specificVersion))
            {
                System.Data.IDbTransaction transac = null;
                try
                {
                    //Logger.System("Downgrade vers la version " + downgradeToVersion);
                    //On passe les migrations à l'envers pour downgrade la bdd
                    for (int i = migrations.Count - 1; i > 0; i--)
                    {
                        OpenOrmMigration m = migrations[i];
                        if (string.IsNullOrEmpty(m.Version)) continue;

                        if(m.Version.CompareVersions(specificVersion) == 1 /*&& currentVersion.CompareVersion(m.Version) >= 0*/)
                        {
                            //Logger.System("Downgrade de la version " + m.Version);
                            transac = db.BeginTransaction();
                            m.Down(db);
                            db.Delete<OpenOrmMigration>(x => x.Version == m.Version);
                            ExistingMigrations = ExistingMigrations.Where(x => x.Version != m.Version).ToList();
                            currentVersion = ExistingMigrations.Count > 0 ? ExistingMigrations.Where(x => !string.IsNullOrEmpty(x.Version)).Select(x => x.Version).OrderBy(x => x.PadNumbers()).Last() : "";
                            db.CommitTransaction();
                            //transac.Commit();
                        }

                        if (m.Version.CompareVersions(specificVersion) <= 0) break;
                    }

                    for (int i = 0; i < migrations.Count; i++)
                    {
                        OpenOrmMigration m = migrations[i];
                        if (string.IsNullOrEmpty(m.Version)) continue;


                        if (m.Version.CompareVersions(specificVersion) <= 0 && m.Version.CompareVersions(currentVersion) == 1)
                        {
                            //Logger.System("Downgrade de la version " + m.Version);
                            transac = db.BeginTransaction();
                            m.Up(db);
                            db.Insert(m);
                            currentVersion = m.Version;
                            db.CommitTransaction();
                        }

                        if (m.Version.CompareVersions(specificVersion) >= 0) break;
                    }
                }
                catch (Exception)
                {
                    db.RollbackTransaction();
                    //Logger.Error(ex);
                }
            }
            else
            {
                System.Data.IDbTransaction transac = null;
                try
                {
                    foreach (OpenOrmMigration m in migrations)
                    {
                        if (m.Version.CompareVersions(currentVersion) == 1)
                        {
                            transac = db.BeginTransaction();
                            //Logger.System("Upgrade vers la version " + m.Version);
                            m.Up(db);
                            db.Insert(m);
                            currentVersion = m.Version;
                            db.CommitTransaction();
                        }
                    }
                }
                catch (Exception e)
                {
                    db.RollbackTransaction();
                    //Logger.Error(e);
                    throw e;
                }

                //Logger.System("Fin de la migration");
            }
        }

        public static void MigrateToPreviousVersion(this OpenOrmDbConnection db)
        {
            if(db.TableExists<OpenOrmMigration>())
            {
                var ExistingMigrations = db.Select<OpenOrmMigration>().OrderBy(x => x.Version.PadNumbers()).ToList();
                string currentVersion = ExistingMigrations.Count > 0 ? ExistingMigrations.Select(x => x.Version).OrderBy(x => x.PadNumbers()).Last() : "";
                List<OpenOrmMigration> migrations = OpenOrmTools.GetEnumerableOfTypeFromEntryAssembly<OpenOrmMigration>()
                    .Where(x => !string.IsNullOrEmpty(x.Version)).OrderBy(x => x.Version.PadNumbers()).ToList();


                string previousExistingMigration = "";
                if (ExistingMigrations.Count > 1)
                {
                    previousExistingMigration = ExistingMigrations[ExistingMigrations.Count - 2].Version;
                }

                string previousVersion = "";
                if (migrations.Count > 1)
                {
                    previousVersion = migrations[migrations.Count - 2].Version;
                }

                if (!string.IsNullOrEmpty(previousExistingMigration) && !string.IsNullOrEmpty(previousVersion) && previousExistingMigration.Equals(previousVersion) && previousVersion.CompareVersions(currentVersion) == -1)
                {
                    db.Migrate(previousVersion);
                }
            }
        }
        #endregion

        #region SQL
        public static SqlResult Sql(this OpenOrmDbConnection cnx, string free_sql_request, List<SqlParameterItem> parameters = null)
        {
            return GetQueryBuilder(cnx).Sql(cnx, free_sql_request, parameters);
        }

        public static SqlResult Sql(this OpenOrmDbConnection cnx, string free_sql_request, SqlParameterItem parameter)
        {
            return GetQueryBuilder(cnx).Sql(cnx, free_sql_request, new List<SqlParameterItem> { parameter });
        }

        public static void SqlNonQuery(this OpenOrmDbConnection cnx, string free_sql_request, List<SqlParameterItem> parameters = null)
        {
            GetQueryBuilder(cnx).SqlNonQuery(cnx, free_sql_request, parameters);
        }

        public static void SqlNonQuery(this OpenOrmDbConnection cnx, string free_sql_request, SqlParameterItem parameter)
        {
            GetQueryBuilder(cnx).SqlNonQuery(cnx, free_sql_request, new List<SqlParameterItem> { parameter });
        }
        #endregion

        #region Maping
        public static void InitDatabaseColumnMaping(this OpenOrmDbConnection cnx, bool force_reinit = false)
        {
            //GetQueryBuilder(cnx).Sql(cnx, free_sql_request, parameters);
        }
        #endregion


        //public static class Drop
        //{
        //    public static void Table(string tableName)
        //    {

        //    }

        //    public static void Table<T>()
        //    {

        //    }

        //    public static class Column
        //    {
        //        public static void FromTable(string tableName)
        //        {

        //        }
        //    }
        //}

        //public static class Alter
        //{
        //    public static class Table
        //    {
        //        public static class AddColumn
        //        {

        //        }
        //    }

        //    public static class Column
        //    {

        //    }
        //}



    }
}
