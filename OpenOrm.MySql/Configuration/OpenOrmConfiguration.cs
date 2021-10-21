using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using OpenOrm.CoreTools;

namespace OpenOrm.MySql
{
    public class OpenOrmConfiguration : Configuration.OpenOrmConfigurationBase
    {
        public static OpenOrmDbConnection Connection { get; set; }

        public OpenOrmConfiguration() : base()
        {
            Connector = Connector.MySql;
        }

        public OpenOrmDbConnection GetConnection()
        {
            RamCache.Init();
            OpenOrmDbConnection cnx = new OpenOrmDbConnection(this)
            {
                Connection = new MySqlConnection(ConnectionString)
            };
            ((MySqlConnection)cnx.Connection).Open();
            Connection = cnx;
            cnx.ServerVersion = ((MySqlConnection)cnx.Connection).ServerVersion;
            return cnx;
        }
    }
}
