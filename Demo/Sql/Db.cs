using OpenOrm;
using OpenOrm.SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Sql
{
    public class Db
    {
        public static string cnx { get; set; }
        public static Connector connector = Connector.SqLite;
        public static OpenOrmConfiguration dbconfig;


        public static OpenOrmDbConnection GetConnection()
        {
#if DEBUG
            cnx = "db.dev.sqlite";
#else
            cnx = "db.prod.sqlite";
#endif

            if(dbconfig == null)
            {
                dbconfig = new OpenOrmConfiguration
                {
                    ConnectionString = cnx,
                    Schema = "",
                    EnableRamCache = true,
                    MapPrivateProperties = true,
                    //PrintSqlQueries = true
                };
            }

            return dbconfig.GetConnection();
        }
    }
}
