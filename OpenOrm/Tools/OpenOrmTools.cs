using OpenOrm.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace OpenOrm
{
	public static partial class OpenOrmTools
	{
		#region GetEnumerableOfType
		public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class//, IComparable<T>
		{
			List<T> objects = new List<T>();
			foreach (Type type in
				Assembly.GetAssembly(typeof(T)).GetTypes()
				.Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
			{
				objects.Add((T)Activator.CreateInstance(type, constructorArgs));
			}
			//objects.Sort();
			return objects;
		}

		public static IEnumerable<T> GetEnumerableOfTypeFromEntryAssembly<T>(params object[] constructorArgs) where T : class//, IComparable<T>
        {
			//	var callerAssemblies = new StackTrace().GetFrames().Select(x => x.GetMethod().ReflectedType.Assembly).Distinct().ToList();
			//	callerAssemblies = callerAssemblies.Where(x => x.GetReferencedAssemblies().Any(y => y.FullName == Assembly.GetCallingAssembly().FullName)).ToList();

			TypeInfo ti = typeof(T).GetTypeInfo();
			//List<string> references = GetReferences();
			List<Type> types = new List<Type>();
			List<T> objects = new List<T>();

			Type targetType = typeof(T);

			//types = Assembly.GetEntryAssembly().GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(targetType)).ToList();

			types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => 
				ti.IsClass ? type.IsSubclassOf(targetType) :
				ti.IsInterface ? (type.GetInterfaces().Contains(targetType) || type.IsAssignableFrom(targetType)) && type.IsClass : 
			false).ToList();


			//var platform = Environment.OSVersion.Platform.ToString();
			//var runtimeAssemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(platform);
			//types = runtimeAssemblyNames.Select(Assembly.Load).SelectMany(a => a.ExportedTypes).Where(t => typeof(T).IsAssignableFrom(t));

			//types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => type.IsSubclassOf(targetType)).ToList();

			//types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => type.GetInterfaces().Contains(targetType)).ToList();

			//types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetExportedTypes()).Where(type => type.IsSubclassOf(targetType)).ToList();


			foreach (Type type in types)
			{
				if (type.FullName == "System.Object") continue;
				objects.Add((T)Activator.CreateInstance(type, constructorArgs));
			}


			//objects.Sort();

			return objects;
		}

		//public static List<string> GetReferences(string dll = "")
		//{
		//	List<string> result = new List<string>();

		//	if (string.IsNullOrEmpty(dll))
		//	{
		//		result.AddRange(GetReferences(Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName));
		//		result.AddRange(GetReferences(Assembly.GetCallingAssembly().ManifestModule.FullyQualifiedName));
		//		result.AddRange(GetReferences(Assembly.GetEntryAssembly().ManifestModule.FullyQualifiedName));
		//	}
		//	else
		//	{
		//		var assm = Assembly.ReflectionOnlyLoadFrom(dll);
		//		var reff = assm.GetReferencedAssemblies(); //Ne fonctionne pas avec .net core
		//	}

		//	return result.Distinct().ToList();
		//}
		#endregion

		#region GetTableName
		public static string GetTableName<T>()
		{
			return GetTableName(typeof(T));
		}

		public static string GetTableName(object model)
		{
			return GetTableName(model.GetType());
		}

		public static string GetTableName(Type t)
		{
			string name = t.GetClassAttributeValue((DbModelAttribute dbm) => dbm.TableName);
			if (string.IsNullOrEmpty(name)) name = t.Name;
			return name;
		}
		#endregion

		#region GetFields
		public static List<string> GetFieldNames(Type t)
		{
			//if (TableFieldsCache == null) TableFieldsCache = new Dictionary<string, List<string>>();
			//if (TableFieldsCache.ContainsKey(t.Name)) return TableFieldsCache[t.Name];
			//else
			//{
			List<string> fields = GetValidProperties(t).Select(x => $"[{x.Name}]").ToList();
			//TableFieldsCache.Add(t.Name, fields);
			return fields;
			//}
		}

		public static List<string> GetFieldNames<T>()
		{
			return GetFieldNames(typeof(T));
		}

		public static List<string> GetStaticFieldNames<T>()
        {
			return GetStaticFieldNames(typeof(T));
        }

		public static List<string> GetStaticFieldNames(Type t)
		{
			return GetStaticFields(t).Select(x => x.Name).OrderBy(x => x).ToList();
		}

		public static List<string> GetStaticFieldValues(Type t)
        {
			var fields = GetStaticFields(t);
			return fields.Select(x => x.GetValue(null).ToString()).ToList();
		}

		public static List<FieldInfo> GetStaticFields(Type t)
        {
			List<FieldInfo> fields_infos = t.GetFields(
					BindingFlags.FlattenHierarchy |
					BindingFlags.Public |
					BindingFlags.NonPublic |
					BindingFlags.Static).ToList();

			return fields_infos;
		}

		public static List<PropertyInfo> GetValidProperties(Type t, bool includePrivateProperties = false)
		{
			//if (TableStructureCache == null) TableStructureCache = new Dictionary<string, TableDefinition>();

			List<PropertyInfo> result = new List<PropertyInfo>();
			int idColIndex = -1;
			List<PropertyInfo> property_infos = null;
			
			if(includePrivateProperties)
            {
				property_infos = t.GetProperties(
					BindingFlags.FlattenHierarchy |
					BindingFlags.Instance |
					BindingFlags.Public |
					BindingFlags.NonPublic).ToList();
			}
			else
            {
				property_infos = t.GetProperties(
					BindingFlags.FlattenHierarchy |
					BindingFlags.Instance |
					BindingFlags.Public).ToList();
			}
			


			for (int i = 0; i < property_infos.Count; i++)
			{
				PropertyInfo info = property_infos[i];
				string name = info.Name;
				string attributes = info.PropertyType.Attributes.ToString();
				if (info.CanRead) attributes += " get";
				if (info.CanWrite) attributes += " set";

				string attributesStr = string.Join(",", info.CustomAttributes);
				//if (info.CustomAttributes != null && info.CustomAttributes.GetEnumerator().Current.NamedArguments.Count > 0 && attributesStr.Contains("IgnoreAttribute")) continue;
				if (info.CustomAttributes != null && info.CustomAttributes.Count() > 0 && 
					(attributesStr.Contains("DbIgnore") || attributesStr.Contains("DbLoadNestedObject"))) continue;
				if (!info.PropertyType.IsPublic) continue;
				if (!info.CanRead || !info.CanWrite) continue;

				if (name.ToLower() == "id")
				{
					idColIndex = i;
				}

				result.Add(info);
			}

			//Placer la colonne Id en premier
			if (idColIndex > 0)
			{
				PropertyInfo pi_id = property_infos.Where(x => x.Name.ToLower() == "id").FirstOrDefault();
				if (pi_id != null)
				{
					property_infos.RemoveAll(x => x.Name.ToLower() == "id");
					property_infos.Insert(0, pi_id);
				}
			}

			return result;
		}

		public static List<PropertyInfo> GetValidProperties<T>()
		{
			return GetValidProperties(typeof(T));
		}

		public static PropertyInfo GetProperty(Type t, string name)
        {
			List<PropertyInfo> properties = GetValidProperties(t);
			foreach(var prop in properties)
            {
				if(prop.Name == name)
                {
					return prop;
                }
            }
			return null;
		}

		public static PropertyInfo GetProperty<T>(string name)
		{
			return GetProperty(typeof(T), name);
		}

		public static List<PropertyInfo> GetNestedProperties(Type t, bool includePrivateProperties = false)
		{
			//if (TableStructureCache == null) TableStructureCache = new Dictionary<string, TableDefinition>();

			List<PropertyInfo> result = new List<PropertyInfo>();
			PropertyInfo[] property_infos;

			if (includePrivateProperties)
			{
				property_infos = t.GetProperties(
					BindingFlags.FlattenHierarchy |
					BindingFlags.Instance |
					BindingFlags.Public |
					BindingFlags.NonPublic);
			}
			else
			{
				property_infos = t.GetProperties(
					BindingFlags.FlattenHierarchy |
					BindingFlags.Instance |
					BindingFlags.Public);
			}

			for (int i = 0; i < property_infos.Length; i++)
			{
				PropertyInfo info = property_infos[i];
				//string name = info.Name;
				//string attributes = info.PropertyType.Attributes.ToString();
				//if (info.CanRead) attributes += " get";
				//if (info.CanWrite) attributes += " set";

				string attributesStr = string.Join(",", info.CustomAttributes);
				//if (info.CustomAttributes != null && info.CustomAttributes.GetEnumerator().Current.NamedArguments.Count > 0 && attributesStr.Contains("IgnoreAttribute")) continue;
				if (!attributesStr.Contains("DbLoadNestedObject")) continue;
				if (!info.PropertyType.IsPublic) continue;
				if (!info.CanRead || !info.CanWrite) continue;

				result.Add(info);
			}

			return result;
		}

		public static List<PropertyInfo> GetForeignKeyProperties(Type t, bool includePrivateProperties = false)
		{
			//if (TableStructureCache == null) TableStructureCache = new Dictionary<string, TableDefinition>();

			List<PropertyInfo> result = new List<PropertyInfo>();
			PropertyInfo[] property_infos;

			if (includePrivateProperties)
			{
				property_infos = t.GetProperties(
					BindingFlags.FlattenHierarchy |
					BindingFlags.Instance |
					BindingFlags.Public |
					BindingFlags.NonPublic);
			}
			else
			{
				property_infos = t.GetProperties(
					BindingFlags.FlattenHierarchy |
					BindingFlags.Instance |
					BindingFlags.Public);
			}

			for (int i = 0; i < property_infos.Length; i++)
			{
				PropertyInfo info = property_infos[i];
				//string name = info.Name;
				//string attributes = info.PropertyType.Attributes.ToString();
				//if (info.CanRead) attributes += " get";
				//if (info.CanWrite) attributes += " set";

				string attributesStr = string.Join(",", info.CustomAttributes);
				//if (info.CustomAttributes != null && info.CustomAttributes.GetEnumerator().Current.NamedArguments.Count > 0 && attributesStr.Contains("IgnoreAttribute")) continue;
				if (!attributesStr.Contains("DbForeignKey")) continue;
				if (!info.PropertyType.IsPublic) continue;
				if (!info.CanRead || !info.CanWrite) continue;

				result.Add(info);
			}

			return result;
		}

		public static PropertyInfo GetPrimaryKeyField<T>()
		{
			List<PropertyInfo> pis = GetValidProperties(typeof(T));
			if (pis.Count > 0)
			{
				foreach (PropertyInfo pi in pis)
				{
					string attributesStr = string.Join(",", pi.CustomAttributes);
					if (attributesStr.Contains("PrimaryKey"))
					{
						return pi;
					}
				}

				return pis.First();
			}
			else
			{
				return null;
			}
		}

		public static string GetPrimaryKeyFieldName<T>()
		{
			return GetPrimaryKeyField<T>()?.Name;
		}

		#endregion

		#region ToSqlDbType
		public static SqlDbType ToSqlDbType(PropertyInfo pi)
		{
			if (pi.PropertyType.Name.Contains("Nullable"))
			{
				Type baseType = ((FieldInfo[])((TypeInfo)pi.PropertyType).DeclaredFields)[1].FieldType;
				return ToSqlDbType(baseType);
			}
			else return ToSqlDbType(pi.PropertyType);
		}

		public static SqlDbType ToSqlDbType(FieldInfo fi)
		{
			if (fi.FieldType.Name.Contains("Nullable"))
			{
				Type baseType = ((FieldInfo[])((TypeInfo)fi.FieldType).DeclaredFields)[1].FieldType;
				return ToSqlDbType(baseType);
			}
			else return ToSqlDbType(fi.FieldType);
		}

		public static SqlDbType ToSqlDbType<T>()
		{
			Type t = typeof(T);

			if (t.Name.Contains("Nullable"))
			{
				Type baseType = ((FieldInfo[])((TypeInfo)t).DeclaredFields)[1].FieldType;
				return ToSqlDbType(baseType);
			}
			else return ToSqlDbType(t);
		}

		public static SqlDbType ToSqlDbType(Type t)
		{
			if (t == typeof(string))
			{
				return SqlDbType.NVarChar;
			}
			else if (t == typeof(int))
			{
				return SqlDbType.Int;
			}
			else if (t == typeof(long))
			{
				return SqlDbType.BigInt;
			}
			else if (t == typeof(bool))
			{
				return SqlDbType.Bit;
			}
			else if (t == typeof(DateTime))
			{
				return SqlDbType.DateTime;
			}
			else if (t == typeof(double))
			{
				return SqlDbType.Float;
			}
			else if (t == typeof(decimal))
			{
				return SqlDbType.Decimal;
			}
			else if (t == typeof(char))
			{
				return SqlDbType.Char;
			}
			else if (t == typeof(short))
			{
				return SqlDbType.SmallInt;
			}
			else if (t == typeof(byte))
			{
				return SqlDbType.TinyInt;
			}
			else if (t == typeof(byte[]))
			{
				return SqlDbType.VarBinary;
			}
			else if (t == typeof(object))
			{
				return SqlDbType.NVarChar;
			}
			else if (t == typeof(TimeSpan))
			{
				return SqlDbType.Time;
			}
			else if (t == typeof(Guid))
			{
				return SqlDbType.UniqueIdentifier;
			}
			else if(t.IsEnum)
            {
				return SqlDbType.BigInt;
            }

			return SqlDbType.NVarChar;
		}

		public static SqlDbType SqlDbTypeFromString(string db_type_name)
		{
			foreach (SqlDbType value in Enum.GetValues(typeof(SqlDbType)))
			{
				if (value.ToString().ToLower() == db_type_name.ToLower())
				{
					return value;
				}
			}

			//default
			return SqlDbType.NVarChar;
		}
		#endregion

		#region Reflexion
		public static List<T2> ChangeType<T2>(IEnumerable<object> copyFromList)
		{
			if (copyFromList != null)
			{
				Type listType = typeof(List<>).MakeGenericType(new[] { typeof(T2) });
				IList list = (IList)Activator.CreateInstance(listType);

				if (copyFromList.GetType().IsListOrArray())
				{
					foreach (var o in copyFromList)
					{
						T2 o2 = (T2)Activator.CreateInstance(typeof(T2));
						o.CopyTo(o2);
						list.Add(o2);
					}

					return (List<T2>)Convert.ChangeType(list, typeof(List<T2>));
				}
				else
				{
					T2 o2 = (T2)Activator.CreateInstance(typeof(T2));
					copyFromList.CopyTo(o2);
					list.Add(o2);
				}

				return (List<T2>)list;
			}

			return null;
		}
		#endregion

		public static string GetDescription(Enum value)
		{
			Type type = value.GetType();
			string name = Enum.GetName(type, value);
			if (name != null)
			{
				FieldInfo field = type.GetField(name);
				if (field != null)
				{
					DescriptionAttribute attr =
						   Attribute.GetCustomAttribute(field,
							 typeof(DescriptionAttribute)) as DescriptionAttribute;
					if (attr != null)
					{
						return attr.Description;
					}
				}
			}
			return null;
		}
	}
}
