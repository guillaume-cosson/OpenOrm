using OpenOrm.CoreTools;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace OpenOrm.SqlServer
{
    public class OpenOrmConfiguration : Configuration.OpenOrmConfigurationBase
    {
        public static OpenOrmDbConnection Connection { get; set; }

        public OpenOrmConfiguration() : base()
        {
            Connector = Connector.SqlServer;
        }

        public OpenOrmDbConnection GetConnection()
        {
            RamCache.Init();
            //ColumnDefinition.UseBrackets = true;
            OpenOrmDbConnection cnx = new OpenOrmDbConnection(this);
            cnx.Connection = new SqlConnection(ConnectionString);
            ((SqlConnection)cnx.Connection).Open();
            Connection = cnx;
            cnx.ServerVersion = ((SqlConnection)cnx.Connection).ServerVersion;
            return cnx;
        }
    }
}
