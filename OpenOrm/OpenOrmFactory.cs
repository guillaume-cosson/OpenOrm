//using MySql.Data.MySqlClient;
//using OpenOrm.Configuration;
//using OpenOrm.Tools;
//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Data.SQLite;
//using System.IO;
//using System.Text;

//namespace OpenOrm
//{
//    public static class OpenOrmFactory
//    {
//        public static OpenOrmDbConnection CreateConnection(string connectionString = "", Connector sqlConnector = Connector.SqLite)
//        {
//            OpenOrmConfiguration config = new OpenOrmConfiguration();
//            config.ConnectionString = connectionString;
//            config.Connector = sqlConnector;

//            RamCache.Init();

//            switch (config.Connector)
//            {
//                case Connector.SqlServer:
//                    return CreateSqlServerConnection(config);
//                default:
//                case Connector.SqLite:
//                    return CreateSQLiteConnection(config);
//            }
//        }

//        public static OpenOrmDbConnection CreateConnection(OpenOrmConfiguration config)
//        {
//            OpenOrmDbConnection cnx = null;

//            switch (config.Connector)
//            {
//                case Connector.SqlServer:
//                    cnx = CreateSqlServerConnection(config);
//                    break;
//                case Connector.MySql:
//                    cnx = CreateMySqlConnection(config);
//                    break;
//                default:
//                case Connector.SqLite:
//                    cnx = CreateSQLiteConnection(config);
//                    break;
//            }

//            //IDbConnectionExtensions.SetConfiguration(config);
//            cnx.Configuration = config;

//            RamCache.Init();

//            return cnx;
//        }

//        #region Create Connections
//        private static OpenOrmDbConnection CreateSqlServerConnection(OpenOrmConfiguration config)
//        {
//            OpenOrmDbConnection cnx = new OpenOrmDbConnection(config);
//            cnx.Connection = new SqlConnection(config.ConnectionString);
//            ((SqlConnection)cnx.Connection).Open();
//            return cnx;
//        }

//        private static OpenOrmDbConnection CreateMySqlConnection(OpenOrmConfiguration config)
//        {
//            OpenOrmDbConnection cnx = new OpenOrmDbConnection(config);
//            cnx.Connection = new MySqlConnection(config.ConnectionString);
//            ((MySqlConnection)cnx.Connection).Open();
//            return cnx;
//        }

//        private static OpenOrmDbConnection CreateSQLiteConnection(OpenOrmConfiguration config)
//        {
//            OpenOrmDbConnection cnx = new OpenOrmDbConnection(config);
//            if (string.IsNullOrEmpty(config.ConnectionString) || config.ConnectionString.Contains(":memory:"))
//            {
//                cnx.Connection = new SQLiteConnection("Data Source=:memory:");
//                ((SQLiteConnection)cnx.Connection).Open();
//                return cnx;
//            }
//            else
//            {
//                if (!config.ConnectionString.Replace(" ", "").Contains("DataSource="))
//                {
//                    string dbPath = Path.Combine(Environment.CurrentDirectory, config.ConnectionString);
//                    config.ConnectionString = $"Data Source={dbPath};";
//                }

//                if (!config.ConnectionString.Replace(" ", "").Contains("Mode="))
//                {
//                    config.ConnectionString += "Mode=ReadWriteCreate;";
//                }

//                cnx.Connection = new SQLiteConnection(config.ConnectionString);
//            }

//            ((SQLiteConnection)cnx.Connection).Open();
//            return cnx;
//        }
//        #endregion
//    }
//}
