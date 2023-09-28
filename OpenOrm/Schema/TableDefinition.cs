using OpenOrm.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenOrm.Schema
{
	public class TableDefinition
	{
		#region Cache
		private static ConcurrentDictionary<string, TableDefinition> Cache { get; set; }
		public static void ClearCache()
		{
			Cache = new ConcurrentDictionary<string, TableDefinition>();
		}
		#endregion

		#region Properties
		public static bool UseBrackets { get; set; }
		//public string DbServerName { get; set; }
		//public string DbName { get; set; }
		//public string ConnectionString { get; set; }
		public string TableName { get; set; }
		public Type ModelType { get; set; }
		public List<ColumnDefinition> Columns { get; set; }
		public List<ColumnDefinition> NestedColumns { get; set; }
		public List<ColumnDefinition> PrimaryKeys { get { return Columns.Where(x => x.IsPrimaryKey).ToList(); } }
		public List<ColumnDefinition> ForeignKeys { get; set; }
		public int PrimaryKeysCount { get; set; }
		public bool ContainsNestedColumns { get { return NestedColumns.Any(); } }
		public bool ContainsNestedColumnsAutoLoad { get { return NestedColumns.Any(x => x.NestedAutoLoad); } }
		public bool ContainsForeignKeys { get { return ForeignKeys.Any(); } }
		public bool ContainsForeignKeysAutoLoad { get { return ForeignKeys.Any(x => x.NestedAutoLoad); } }
		public bool IsDbCacheInitialized { get; set; }
		public bool ExistsInDb { get; set; }
		#endregion

		public TableDefinition()
		{
			if(Cache == null)
            {
				Cache = new ConcurrentDictionary<string, TableDefinition>();
			}

			Columns = new List<ColumnDefinition>();
			NestedColumns = new List<ColumnDefinition>();
			ForeignKeys = new List<ColumnDefinition>();
		}

        //public TableDefinition(Type t, bool UseSchemaCache = true, bool MapPrivateProperties = false, bool UseDatabaseSchema = true)
        //{
        //    if (Cache == null)
        //    {
        //        Cache = new ConcurrentDictionary<string, TableDefinition>();
        //    }

        //    Columns = new List<ColumnDefinition>();
        //    NestedColumns = new List<ColumnDefinition>();
        //    ForeignKeys = new List<ColumnDefinition>();

        //    Load(t, UseSchemaCache, MapPrivateProperties, UseDatabaseSchema);

        //    //TableDefinition td = Load(t);
        //    //TableName = td.TableName;
        //    //Columns = td.Columns;
        //}

        public TableDefinition(Type t, OpenOrmDbConnection cnx)
        {
            if (Cache == null)
            {
                Cache = new ConcurrentDictionary<string, TableDefinition>();
            }

            Columns = new List<ColumnDefinition>();
            NestedColumns = new List<ColumnDefinition>();
            ForeignKeys = new List<ColumnDefinition>();

            Load(t, cnx);

            //TableDefinition td = Load(t);
            //TableName = td.TableName;
            //Columns = td.Columns;
        }

        //public void Load(Type t, bool useCache = true, bool includePrivateProperties = false, bool mapColumnsFromDb = false)
        public void Load(Type t, OpenOrmDbConnection cnx)
        {
			if(cnx.Configuration.UseSchemaCache && Cache.ContainsKey($"{cnx.ConnectionString}.{t.FullName}"))
            {
				TableDefinition cached = Cache[$"{cnx.ConnectionString}.{t.FullName}"];
				TableName = cached.TableName;
				Columns = cached.Columns;
				NestedColumns = cached.NestedColumns;
				ForeignKeys = cached.ForeignKeys;
				PrimaryKeysCount = cached.PrimaryKeysCount;
				ModelType = cached.ModelType;
				ExistsInDb = cached.ExistsInDb;
            }
			else
            {
				Columns = new List<ColumnDefinition>();
				List<PropertyInfo> properties = OpenOrmTools.GetValidProperties(t, cnx.Configuration.MapPrivateProperties);
				TableName = OpenOrmTools.GetTableName(t);
				ModelType = t;

                TableDefinition td_temp = 
					DbDefinition.Definitions.ContainsKey(cnx.ConnectionString) ? 
					DbDefinition.Definitions[cnx.ConnectionString].FirstOrDefault(x => x.TableName.ToLower() == TableName.ToLower()) 
					: null;

                ExistsInDb = td_temp != null;

                foreach (PropertyInfo pi in properties)
				{
					ColumnDefinition coldef = new ColumnDefinition(pi, cnx.Configuration.UseSchemaCache);
					coldef.TableDefinition = this;

					coldef.ExistsInDb = true;
                    if (cnx.Configuration.UseDatabaseSchema && DbDefinition.Definitions.ContainsKey(cnx.ConnectionString))
					{
						coldef.ExistsInDb = false;
						
						if(td_temp != null)
						{
                            coldef.ExistsInDb = td_temp.Columns.Any(x => x.Name.ToLower() == coldef.Name.ToLower());
                        }
                    }

					Columns.Add(coldef);

					if (coldef.IsPrimaryKey) PrimaryKeysCount++;
				}

				NestedColumns = new List<ColumnDefinition>();
				List<PropertyInfo> nestedProperties = OpenOrmTools.GetNestedProperties(t, cnx.Configuration.MapPrivateProperties);
				foreach (PropertyInfo pi in nestedProperties)
				{
					NestedColumns.Add(new ColumnDefinition(pi, cnx.Configuration.UseSchemaCache));
				}

				ForeignKeys = new List<ColumnDefinition>();
				List<PropertyInfo> foreignKeyProperties = OpenOrmTools.GetForeignKeyProperties(t, cnx.Configuration.MapPrivateProperties);
				foreach (PropertyInfo pi in foreignKeyProperties)
				{
					ForeignKeys.Add(new ColumnDefinition(pi, cnx.Configuration.UseSchemaCache));
				}

				if (PrimaryKeysCount == 0)
				{
					throw new KeyNotFoundException($"Model '{t.Name}' must have a primary key. Use Attribute [DbPrimaryKey] or [DbPrimaryKey, DbAutoIncrement] on a property.");
				}

				if (cnx.Configuration.UseSchemaCache) Cache[$"{cnx.ConnectionString}.{t.FullName}"] = this;
            }
		}

        //public static TableDefinition Get<T>(bool useCache = true, bool includePrivateProperties = false)
        //{
        //    return Get(typeof(T), useCache, includePrivateProperties);
        //}

        //public static TableDefinition Get(Type t, bool useCache = true, bool includePrivateProperties = false)
        //{
        //    return new TableDefinition(t, useCache, includePrivateProperties);
        //}

        public static TableDefinition Get<T>(OpenOrmDbConnection cnx)
        {
            return Get(typeof(T), cnx);
        }

        public static TableDefinition Get(Type t, OpenOrmDbConnection cnx)
        {
            return new TableDefinition(t, cnx);
        }

        public string GetColumnNameFor(PropertyInfo pi)
		{
			ColumnDefinition cd = Columns.Where(x => x.PropertyInfo.Name == pi.Name).FirstOrDefault();
			if (cd != null) return cd.Name;
			return pi.Name;
		}

		public string GetFieldsStr(bool withBrackets = true, char leftBracket = '[', char rightBracket = ']')
        {
			if(withBrackets)
            {
				return leftBracket + string.Join($"{rightBracket},{leftBracket}", Columns.Where(x => x.ExistsInDb).Select(x => x.Name)) + rightBracket;
			}
			else
            {
				return string.Join(",", Columns.Where(x => x.ExistsInDb).Select(x => x.Name));
			}
        }

		public static void AddToCache(string connectionString, List<TableDefinition> tds)
		{
			foreach(var td in tds)
			{
				Cache[$"{connectionString}.{td.ModelType.FullName}"] = td;
            }
        }

		public static void AddToCache(string connectionString, TableDefinition td)
		{
            Cache[$"{connectionString}.{td.ModelType.FullName}"] = td;
        }

		public static void DbDefinitionChanged(string connectionString)
		{
            if (Cache == null)
            {
                Cache = new ConcurrentDictionary<string, TableDefinition>();
            }

            foreach (var td in Cache.Where(x => x.Key.StartsWith($"{connectionString}.")).Select(x => x.Value))
			{
                TableDefinition dbtd = DbDefinition.Definitions[connectionString].FirstOrDefault(x => x.TableName.ToLower() == td.TableName.ToLower());
				if(dbtd != null)
				{
                    td.ExistsInDb = true;

					foreach(var col in td.Columns)
					{
                        col.ExistsInDb = dbtd.Columns.Any(x => x.Name.ToLower() == col.Name.ToLower());
                    }
                }
				else
				{
					td.ExistsInDb = false;
                    foreach (var col in td.Columns)
                    {
                        col.ExistsInDb = false;
                    }
                }
            }
		}
		//public SqlCommand GetInsertCommand(OpenOrmDbConnection cnx)
		//{
		//	SqlCommand cmd = new SqlCommand();

		//	return cmd;
		//}

		//public SqlCommand GetUpdateCommand(OpenOrmDbConnection cnx)
		//{
		//	SqlCommand cmd = new SqlCommand();

		//	return cmd;
		//}

		//public SqlCommand GetDeleteCommand(OpenOrmDbConnection cnx)
		//{
		//	SqlCommand cmd = new SqlCommand();

		//	return cmd;
		//}

		public override string ToString()
		{
			return TableName;
		}
	}
}
