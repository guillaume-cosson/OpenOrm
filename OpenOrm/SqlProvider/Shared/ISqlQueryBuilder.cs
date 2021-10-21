using OpenOrm.Configuration;
using OpenOrm.Schema;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenOrm.SqlProvider.Shared
{
    public interface ISqlQueryBuilder
    {
        OpenOrmConfigurationBase Configuration { get; set; }

        #region Table
        bool TableExists<T>(OpenOrmDbConnection cnx);
        //public abstract Task<bool> TableExistsAsync<T>(OpenOrmDbConnection cnx);

        bool TableExists(OpenOrmDbConnection cnx, Type modelType);

        bool TableExists(OpenOrmDbConnection cnx, string tableName);

        bool TemporaryTableExists<T>(OpenOrmDbConnection cnx);

        bool TemporaryTableExists(OpenOrmDbConnection cnx, string tableName);

        //void CreateTemporaryTable(OpenOrmDbConnection cnx, string tableName);

        //void CreateTemporaryTable(OpenOrmDbConnection cnx, string tableName, SqlResult fromResult);

        void CreateTable<T>(OpenOrmDbConnection cnx);

        void CreateTable(OpenOrmDbConnection cnx, Type modelType);

        void DropTable<T>(OpenOrmDbConnection cnx);

        void DropTable(OpenOrmDbConnection cnx, Type modelType);

        void DropTable(OpenOrmDbConnection cnx, string tableName);

        void TruncateTable<T>(OpenOrmDbConnection cnx);

        void TruncateTable(OpenOrmDbConnection cnx, Type modelType);

        //#region Async
        //Task<bool> TableExistsAsync<T>(OpenOrmDbConnection cnx);
        ////public abstract Task<bool> TableExistsAsync<T>(OpenOrmDbConnection cnx);

        //Task<bool> TableExistsAsync(OpenOrmDbConnection cnx, Type modelType);

        //Task<bool> TableExistsAsync(OpenOrmDbConnection cnx, string tableName);

        //Task<bool> TemporaryTableExistsAsync<T>(OpenOrmDbConnection cnx);

        //Task<bool> TemporaryTableExistsAsync(OpenOrmDbConnection cnx, string tableName);

        ////void CreateTemporaryTable(OpenOrmDbConnection cnx, string tableName);

        ////void CreateTemporaryTable(OpenOrmDbConnection cnx, string tableName, SqlResult fromResult);

        //Task CreateTableAsync<T>(OpenOrmDbConnection cnx);

        //Task CreateTableAsync(OpenOrmDbConnection cnx, Type modelType);

        //Task DropTableAsync<T>(OpenOrmDbConnection cnx);

        //Task DropTableAsync(OpenOrmDbConnection cnx, Type modelType);

        //Task DropTableAsync(OpenOrmDbConnection cnx, string tableName);

        //Task TruncateTableAsync<T>(OpenOrmDbConnection cnx);

        //Task TruncateTableAsync(OpenOrmDbConnection cnx, Type modelType);
        //#endregion
        #endregion

        #region Column
        bool ColumnExists<T>(OpenOrmDbConnection cnx, string columnName);

        bool ColumnExists(OpenOrmDbConnection cnx, Type objType, string columnName);

        void AddColumn(OpenOrmDbConnection cnx, Type objType, ColumnDefinition colDef);

        void DropColumn(OpenOrmDbConnection cnx, Type objType, string colName);
        #endregion

        #region Field
        bool Exists<T>(OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate);
        #endregion

        #region Insert
        long Insert<T>(OpenOrmDbConnection cnx, T model);

        void Insert<T>(OpenOrmDbConnection cnx, List<T> models);
        #endregion

        #region Select
        List<T> Select<T>(OpenOrmDbConnection cnx, bool forceLoadNestedObjects = false);

        List<T> Select<T>(OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate, bool forceLoadNestedObjects = false);

        T SelectFirst<T>(OpenOrmDbConnection cnx, bool forceLoadNestedObjects = false);

        T SelectFirst<T>(OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate, bool forceLoadNestedObjects = false);

        T SelectLast<T>(OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate, bool forceLoadNestedObjects = false);

        T SelectById<T>(OpenOrmDbConnection cnx, object id, bool forceLoadNestedObjects = false);

        long Count<T>(OpenOrmDbConnection cnx);

        long Count<T>(OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate);
        #endregion

        #region Update
        void Update<T>(OpenOrmDbConnection cnx, T model, TableDefinition td = null);
        #endregion

        #region Delete
        void Delete<T>(OpenOrmDbConnection cnx, T model, TableDefinition td = null);
        //public abstract Task DeleteAsync<T>(OpenOrmDbConnection cnx, T model, TableDefinition td = null);

        void Delete<T>(OpenOrmDbConnection cnx, Expression<Func<T, bool>> predicate);

        void DeleteAll<T>(OpenOrmDbConnection cnx);
        #endregion

        #region Transaction
        void BeginTransaction(OpenOrmDbConnection cnx);

        void CommitTransaction(OpenOrmDbConnection cnx);

        void RollbackTransaction(OpenOrmDbConnection cnx);
        #endregion

        #region Migration

        #endregion

        #region SQL
        SqlResult Sql(OpenOrmDbConnection cnx, string free_sql_request, List<SqlParameterItem> parameters = null);
        SqlResult Sql(OpenOrmDbConnection cnx, string free_sql_request, SqlParameterItem parameter);
        void SqlNonQuery(OpenOrmDbConnection cnx, string free_sql_request, List<SqlParameterItem> parameters = null);
        void SqlNonQuery(OpenOrmDbConnection cnx, string free_sql_request, SqlParameterItem parameter);
        #endregion
    }
}
