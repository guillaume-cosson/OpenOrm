using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
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
		public string DbServerName { get; set; }
		public string DbName { get; set; }
		public string TableName { get; set; }
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

		public TableDefinition(Type t, bool useCache = true, bool includePrivateProperties = false)
		{
			if (Cache == null)
			{
				Cache = new ConcurrentDictionary<string, TableDefinition>();
			}

			Columns = new List<ColumnDefinition>();
			NestedColumns = new List<ColumnDefinition>();
			ForeignKeys = new List<ColumnDefinition>();

			Load(t, useCache, includePrivateProperties);

			//TableDefinition td = Load(t);
			//TableName = td.TableName;
			//Columns = td.Columns;
		}

		public void Load(Type t, bool useCache = true, bool includePrivateProperties = false)
        {
			if(useCache && Cache.ContainsKey($"{DbServerName}.{DbName}.{t.FullName}"))
            {
				TableDefinition cached = Cache[$"{DbServerName}.{DbName}.{t.FullName}"];
				TableName = cached.TableName;
				Columns = cached.Columns;
				NestedColumns = cached.NestedColumns;
				ForeignKeys = cached.ForeignKeys;
				PrimaryKeysCount = cached.PrimaryKeysCount;
			}
			else
            {
				Columns = new List<ColumnDefinition>();
				List<PropertyInfo> properties = OpenOrmTools.GetValidProperties(t, includePrivateProperties);
				TableName = OpenOrmTools.GetTableName(t);
				//ExistsInDb = DbDefinition.Definitions.ContainsKey(TableName);

				foreach (PropertyInfo pi in properties)
				{
					ColumnDefinition coldef = new ColumnDefinition(pi, useCache);
					coldef.TableDefinition = this;
					Columns.Add(coldef);

					if (coldef.IsPrimaryKey) PrimaryKeysCount++;
				}

				NestedColumns = new List<ColumnDefinition>();
				List<PropertyInfo> nestedProperties = OpenOrmTools.GetNestedProperties(t, includePrivateProperties);
				foreach (PropertyInfo pi in nestedProperties)
				{
					NestedColumns.Add(new ColumnDefinition(pi, useCache));
				}

				ForeignKeys = new List<ColumnDefinition>();
				List<PropertyInfo> foreignKeyProperties = OpenOrmTools.GetForeignKeyProperties(t, includePrivateProperties);
				foreach (PropertyInfo pi in foreignKeyProperties)
				{
					ForeignKeys.Add(new ColumnDefinition(pi, useCache));
				}

				if (PrimaryKeysCount == 0)
				{
					throw new KeyNotFoundException($"Model '{t.Name}' must have a primary key. Use Attribute [DbPrimaryKey] or [DbPrimaryKey, DbAutoIncrement] on a property.");
				}

				if (useCache) Cache[$"{DbServerName}.{DbName}.{t.FullName}"] = this;
            }
		}

        public static TableDefinition Get<T>(bool useCache = true, bool includePrivateProperties = false)
        {
            return Get(typeof(T), useCache, includePrivateProperties);
        }

        public static TableDefinition Get(Type t, bool useCache = true, bool includePrivateProperties = false)
        {
            return new TableDefinition(t, useCache, includePrivateProperties);
        }

        public string GetColumnNameFor(PropertyInfo pi)
		{
			ColumnDefinition cd = Columns.Where(x => x.PropertyInfo.Name == pi.Name).FirstOrDefault();
			if (cd != null) return cd.Name;
			return pi.Name;
		}

		public string GetFieldsStr(bool withBrackets = true)
        {
			if(withBrackets)
            {
				return "[" + string.Join("],[", Columns.Select(x => x.Name)) + "]";
			}
			else
            {
				return string.Join(",", Columns.Select(x => x.Name));
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
