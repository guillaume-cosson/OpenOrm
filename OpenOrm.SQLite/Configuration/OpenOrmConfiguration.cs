using OpenOrm.CoreTools;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace OpenOrm.SQLite
{
    public sealed class OpenOrmConfiguration : Configuration.OpenOrmConfigurationBase
    {
        public static OpenOrmDbConnection Connection { get; set; }

        public OpenOrmConfiguration() : base()
        {
            Connector = Connector.SqLite;
        }

        public OpenOrmDbConnection GetConnection()
        {
            RamCache.Init();

            if (string.IsNullOrEmpty(ConnectionString) || ConnectionString.ToLower().Contains(":memory:"))
            {
                ConnectionString = "Data Source=:memory:";
            }
            else
            {
                if (!ConnectionString.ToLower().Replace(" ", "").Contains("datasource="))
                {
                    string dbPath = Path.Combine(Environment.CurrentDirectory, ConnectionString);
                    ConnectionString = $"Data Source={dbPath};";
                }

                if (!ConnectionString.ToLower().Replace(" ", "").Contains("mode="))
                {
                    ConnectionString += "Mode=ReadWriteCreate;";
                }
            }

            OpenOrmDbConnection cnx = new OpenOrmDbConnection(this);
            cnx.Connection = new SQLiteConnection(ConnectionString);
            ((SQLiteConnection)cnx.Connection).Open();
            Connection = cnx;
            cnx.ServerVersion = ((SQLiteConnection)cnx.Connection).ServerVersion;
            return cnx;
        }
    }
}
