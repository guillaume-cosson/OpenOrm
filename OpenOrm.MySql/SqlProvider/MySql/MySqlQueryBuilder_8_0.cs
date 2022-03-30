using OpenOrm.Configuration;
using OpenOrm.Extensions;
using OpenOrm.MySql;
using OpenOrm.Schema;
using OpenOrm.SqlProvider.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenOrm.SqlProvider.MySql
{
    public class MySqlQueryBuilder_8_0 : MySqlQueryBuilderBase, ISqlQueryBuilder
    {
        #region Constructor
        public MySqlQueryBuilder_8_0(OpenOrmConfigurationBase config) : base(config) { }
        #endregion

        #region Table
        public new void CreateTable<T>(OpenOrmDbConnection cnx)
        {
            Type modelType = typeof(T);
            CreateTable(cnx, modelType);
        }
        
        public new void CreateTable(OpenOrmDbConnection cnx, Type modelType)
        {
            List<string> primaryKeys = new List<string>();
            List<string> columns = new List<string>();
            List<string> colNames = new List<string>();
            TableDefinition td = TableDefinition.Get(modelType, cnx.Configuration.UseSchemaCache, cnx.Configuration.MapPrivateProperties);

            if(cnx.Configuration.PutIdFieldAtFirstPosition)
            {
                var idColumn = td.Columns.FindIndex(c => c.Name.Equals("id", StringComparison.CurrentCultureIgnoreCase));
                if(idColumn != -1)
                {
                    var item = td.Columns[idColumn];
                    td.Columns.RemoveAt(idColumn);
                    td.Columns.Insert(0, item);
                }
            }

            string sql = $"CREATE TABLE `{GetTableName(modelType)}` (";

            foreach (ColumnDefinition cd in td.Columns)
            {
                string fieldsql = $" `{cd.Name}` {MySqlTools.ToStringType(cd.PropertyType.GetBaseType(), cd.Size)}";

                if (cd.IsPrimaryKey)
                {
                    if(td.Columns.Where(x => x.IsPrimaryKey).Count() == 1)
                    {
                        fieldsql += " PRIMARY KEY";
                    }
                    
                    primaryKeys.Add($"`{cd.Name}`");
                }

                if (cd.IsAutoIncrement)
                {

                    fieldsql += " AUTO_INCREMENT";
                }

                if (cd.IsNotNullColumn || (cd.IsPrimaryKey && !cd.IsAutoIncrement))
                {
                    fieldsql += " NOT NULL";
                }

                if (cd.IsUnique && !cd.IsAutoIncrement && !cd.IsPrimaryKey)
                {
                    fieldsql += " UNIQUE";
                }

                if (!string.IsNullOrEmpty(fieldsql))
                {
                    columns.Add(fieldsql);
                    colNames.Add(cd.Name);
                }
            }

            string fields = string.Join(",", columns);
            sql += fields;

            if(td.Columns.Where(x => x.IsPrimaryKey).Count() > 1)
            {
                sql += $" , PRIMARY KEY ({string.Join(",", primaryKeys)})";
            }

            var foreigns = td.Columns.Where(x => x.IsForeignKey);
            foreach(var foreign in foreigns)
            {
                sql += $" , FOREIGN KEY (`{foreign.Name}`) REFERENCES `{OpenOrmTools.GetTableName(foreign.ForeignType)}` (`{foreign.ParentForeignKeyProperty}`) ON UPDATE {OpenOrmTools.GetDescription(foreign.ForeignOnUpdate)} ON DELETE {OpenOrmTools.GetDescription(foreign.ForeignOnDelete)}";
            }

            sql += ");";

            SqlQuery.Execute(cnx, sql, SqlQueryType.Sql);

            //Create default index for the table
            //SqlQuery.Execute(cnx, $"CREATE INDEX {GetTableName(modelType)}_INDEX ON {GetTableName(modelType)}({string.Join(",", colNames)});", SqlQueryType.Sql);
        }


        #endregion
    }
}
