using OpenOrm;
using OpenOrm.Extensions;
using OpenOrm.SqlServer;
using OpenOrm.Schema;
using OpenOrm.SqlProvider.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace OpenOrmTools.Classes
{
    public class ModelGenerator
    {
        #region Properties
        public string Language { get; set; }
        public Connector Database { get; set; }
        public string ConnectionString { get; set; }
        #endregion

        public ModelGenerator()
        {

        }

        public string Generate()
        {
            //Connexion
            OpenOrmConfiguration config = new OpenOrmConfiguration
            {
                ConnectionString = ConnectionString,
                Connector = Database,
                PrintSqlQueries = false,
                PutIdFieldAtFirstPosition = true,
                Schema = Database == Connector.SqlServer ? "dbo" : "",
                MapPrivateProperties = true
            };

            OpenOrmDbConnection db = config.GetConnection();

            List<TableDefinition> Tables = GetTablesDefinitionsFromDatabase(db);
            return GenerateModelsFromTablesDefinitions(db, Tables);
        }

        public List<TableDefinition> GetTablesDefinitionsFromDatabase(OpenOrmDbConnection db)
        {
            List<TableDefinition> Tables = new List<TableDefinition>();

            //Récupération des tables
            SqlResult r = db.Sql("SELECT name FROM sys.sysobjects WHERE xtype = 'U'");
            if(r.HasRows)
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

        public string GenerateModelsFromTablesDefinitions(OpenOrmDbConnection db, List<TableDefinition> Tables)
        {
            SqlResult r = db.Sql("SELECT db_name() AS db");
            string db_name = r.Get("db");
            string Folder = Path.Combine(Environment.CurrentDirectory, "GeneratorResults");
            string ResultFolder = Path.Combine(Folder, $"{db_name}_{DateTime.Now:yyyyMMdd_hhmmss}");

            if (Directory.Exists(ResultFolder)) Directory.Delete(ResultFolder, true);
            Directory.CreateDirectory(ResultFolder);

            foreach(TableDefinition td in Tables)
            {
                string filePath = Path.Combine(ResultFolder, $"{td.TableName}.cs");
                StringBuilder fileContent = new StringBuilder();

                fileContent.AppendLine("using OpenOrm;");
                fileContent.AppendLine("using System;");
                fileContent.AppendLine("using System.Collections.Generic;");
                fileContent.AppendLine("using System.Text;");
                fileContent.AppendLine("");
                fileContent.AppendLine("namespace RENAME_ME.Models");
                fileContent.AppendLine("{");
                fileContent.AppendLine($"    public class {td.TableName} : DbModel");
                fileContent.AppendLine("    {");
                fileContent.AppendLine("        #region Properties");
                foreach(ColumnDefinition cd in td.Columns)
                {
                    fileContent.AppendLine(GenerateProperty(cd));
                }
                fileContent.AppendLine("        #endregion");
                fileContent.AppendLine("    }");
                fileContent.AppendLine("}");


                File.WriteAllText(filePath, fileContent.ToString());
            }

            return ResultFolder;
        }

        private string GenerateProperty(ColumnDefinition cd)
        {
            string content = "";

            if (cd.IsPrimaryKey && cd.IsAutoIncrement)
                content += "        [DbPrimaryKey, DbAutoIncrement]";
            else if(cd.IsPrimaryKey && !cd.IsAutoIncrement)
                content += "        [DbPrimaryKey]";
            else if (!cd.IsPrimaryKey && cd.IsAutoIncrement)
                content += "        [DbAutoIncrement]";
            else
            {
                
                if (cd.IsNotNullColumn)
                {
                    if(cd.HasSize)
                        content += $"        [DbNotNull(), DbSize({cd.Size})]";
                    else if(cd.HasDecimalSize)
                        content += $"        [DbNotNull(), DbDecimalSize({cd.Scale}, {cd.Precision})]";
                    else
                        content += $"        [DbNotNull()]";
                }
                else
                {
                    if(cd.HasSize)
                        content += $"        [DbSize({cd.Size})]";
                    else if(cd.HasDecimalSize)
                        content += $"        [DbDecimalSize({cd.Scale}, {cd.Precision})]";
                }

            }

            if (!string.IsNullOrEmpty(content)) content += Environment.NewLine;

            content += $"        public {DbTypeToString(cd.DbType)} {cd.Name} " + "{ get; set; }" + Environment.NewLine;

            return content;
        }

        private string DbTypeToString(DbType dt)
        {
            switch(dt)
            {
                case DbType.String: return "string";
                case DbType.Int32: return "int";
                case DbType.Int64: return "long";
                case DbType.Boolean: return "bool";
                case DbType.Binary: return "Array[]";
                case DbType.Guid: return "Guid";
                case DbType.Currency:
                case DbType.Double:
                case DbType.Decimal: return "decimal";
                case DbType.Byte: return "byte";
                case DbType.Date: return "Date";
                case DbType.DateTime:
                case DbType.DateTime2: return "DateTime";
                case DbType.Time: return "TimeSpan";
            }

            return "object";
        }
    }
}
