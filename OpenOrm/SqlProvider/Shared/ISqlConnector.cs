using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace OpenOrm.SqlProvider.Shared
{
    public interface ISqlConnector
    {
        ArrayList Parameters { get; set; }
        //public void AddParameter(string paramName, object value);
        void AddParameter(string paramName, object value, SqlDbType type);
        void ClearParameters();

        long ExecuteSql(OpenOrmDbConnection cnx, string sql);
        Task<long> ExecuteSqlAsync(OpenOrmDbConnection cnx, string sql);
        long ExecuteStoredProcedure(OpenOrmDbConnection cnx, string storedprocedure);
        Task<long> ExecuteStoredProcedureAsync(OpenOrmDbConnection cnx, string storedprocedure);
        DbDataReader GetDataReader(OpenOrmDbConnection cnx, string command, CommandType type);
        Task<DbDataReader> GetDataReaderAsync(OpenOrmDbConnection cnx, string command, CommandType type);
    }
}
