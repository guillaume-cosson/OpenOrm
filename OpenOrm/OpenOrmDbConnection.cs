using OpenOrm.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OpenOrm
{
    public class OpenOrmDbConnection : IDbConnection, IDisposable
    {
        #region Configuration
        public OpenOrmConfigurationBase Configuration { get; set; }
        public IDbConnection Connection { get; set; }
        public string ServerVersion { get; set; }
        //public IDbTransaction Transaction { get; set; }
        #endregion

        #region Constructors
        //public OpenOrmDbConnection()
        //{
        //    Configuration = new OpenOrmConfigurationBase();
        //}

        //public OpenOrmDbConnection(string connectionString)
        //{
        //    Configuration = new OpenOrmConfigurationBase();
        //    Configuration.ConnectionString = connectionString;
        //}

        public OpenOrmDbConnection(OpenOrmConfigurationBase config)
        {
            Configuration = config;
        }
        #endregion

        public IDbTransaction Transaction { get; set; }

        #region IDbConnection implementation
        public string ConnectionString { get => Connection.ConnectionString; set => Connection.ConnectionString = value; }

        public int ConnectionTimeout => Connection.ConnectionTimeout;

        public string Database => Connection.Database;

        public ConnectionState State => Connection.State;

        public IDbTransaction BeginTransaction()
        {
            Transaction = Connection.BeginTransaction();
            return Transaction;
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            Transaction = Connection.BeginTransaction(il);
            return Transaction;
        }

        public void CommitTransaction()
        {
            if(Transaction != null)
            {
                Transaction.Commit();
                Transaction = null;
            }
        }

        public void RollbackTransaction()
        {
            if (Transaction != null)
            {
                Transaction.Rollback();
                Transaction = null;
            }
        }

        public void ChangeDatabase(string databaseName)
        {
            Connection.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            Connection.Close();
        }

        public IDbCommand CreateCommand()
        {
            return Connection.CreateCommand();
        }

        public void Open()
        {
            Connection.Open();
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (Connection.State == ConnectionState.Open) Connection.Close();
            Connection.Dispose();
            Configuration = null;
        }
        #endregion
    }
}
