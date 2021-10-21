using OpenOrm.Extensions;
using OpenOrm.Schema;
using OpenOrm.SqlProvider.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenOrm.SqlServer.Schema
{
    public static class SqlServerDbDefinition
    {
        public static void InitDbDefinition(OpenOrmDbConnection db, bool reinit = false)
        {
            if(!db.Configuration.UseDatabaseSchema)
            {
                return;
            }
            if(reinit)
            {
                DbDefinition.Clear(db.Connection.Database);
            }
            if(!DbDefinition.Definitions.ContainsKey(db.Connection.Database))
            {
                DbDefinition.SetDbDefinition(db.Connection.Database, "#", GetTablesDefinitionsFromDatabase(db));
            }
        }

        private static List<TableDefinition> GetTablesDefinitionsFromDatabase(OpenOrmDbConnection db)
        {
            DbDefinition.Definitions.Add(db.Connection.Database, new List<TableDefinition>());
            List<TableDefinition> Tables = new List<TableDefinition>();

            //Récupération des tables
            SqlResult r = db.Sql("SELECT name FROM sys.sysobjects WHERE xtype = 'U'");
            if (r.HasRows)
            {
                foreach (var row in r.Rows)
                {
                    Tables.Add(new TableDefinition { TableName = row.Get("name") });
                }

                //Récupération des colonnes
                foreach (TableDefinition td in Tables)
                {
                    r = db.Sql($@"  SELECT tab.name AS table_name, col.name AS [col_name], col.column_id AS col_order, t.name AS [type_name], col.max_length, col.precision, col.scale, 
                                col.is_identity, col.is_nullable, ISNULL(is_primary_key, 0) AS is_primary_key, ISNULL(is_unique, 0) AS is_unique
                                FROM sys.tables AS tab
                                INNER JOIN sys.columns AS col ON tab.object_id = col.object_id
                                LEFT JOIN sys.index_columns ic ON ic.object_id = col.object_id and ic.column_id = col.column_id
                                LEFT JOIN sys.indexes i on ic.object_id = i.object_id and ic.index_id = i.index_id
                                LEFT JOIN sys.types AS t ON col.user_type_id = t.user_type_id
                                LEFT JOIN sys.default_constraints def ON def.object_id = col.default_object_id
                                WHERE tab.name = '{td.TableName}'
                                ORDER BY col.column_id");

                    foreach (var row in r.Rows)
                    {
                        ColumnDefinition cd = new ColumnDefinition
                        {
                            Name = row.Get("col_name"),
                            IsPrimaryKey = row.Get<bool>("is_primary_key"),
                            IsAutoIncrement = row.Get<bool>("is_identity"),
                            IsNotNullColumn = !row.Get<bool>("is_nullable"),
                            IsUnique = row.Get<bool>("is_unique"),
                            Precision = row.Get<int>("precision"),
                            Scale = row.Get<int>("scale"),
                            SqlDbType = OpenOrm.OpenOrmTools.SqlDbTypeFromString(row.Get("type_name")),
                            Size = row.Get<int>("max_length")
                        };

                        if (cd.SqlDbType == System.Data.SqlDbType.NVarChar) cd.Size /= 2;
                        cd.DbType = cd.SqlDbType.ToDbType();
                        cd.HasSize = cd.DbType == System.Data.DbType.String && cd.Size > -1;
                        cd.HasDecimalSize = cd.DbType != System.Data.DbType.Int32 && cd.DbType != System.Data.DbType.Int64 && cd.Precision > 0;

                        td.Columns.Add(cd);
                    }
                }
            }


            return Tables;
        }

        public static void Clear(string db_name = "")
        {
            DbDefinition.Clear(db_name);
        }

        public static bool IsInited(string db_name)
        {
            return DbDefinition.IsInited(db_name);
        }
    }
}
