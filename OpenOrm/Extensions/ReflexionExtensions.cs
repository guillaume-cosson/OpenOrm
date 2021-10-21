using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenOrm.Extensions
{
    public static partial class Extension
    {
        public static object GetDefaultValue(this Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }

        public static Type GetBaseType(this PropertyInfo pi)
        {
            if (pi.PropertyType.Name.Contains("Nullable"))
            {
                return GetBaseType(((FieldInfo[])((TypeInfo)pi.PropertyType).DeclaredFields)[1].FieldType);
            }
            if (pi.PropertyType.IsGenericType && pi.PropertyType.FullName.Contains("Generic.List"))
            {
                return pi.PropertyType.GetGenericArguments()[0];
            }
            else return pi.PropertyType;
        }

        public static Type GetBaseType(this FieldInfo fi)
        {
            if (fi.FieldType.Name.Contains("Nullable"))
            {
                return GetBaseType(((FieldInfo[])((TypeInfo)fi.FieldType).DeclaredFields)[1].FieldType);
            }
            if (fi.FieldType.IsGenericType && fi.FieldType.FullName.Contains("Generic.List"))
            {
                return fi.FieldType.GetGenericArguments()[0];
            }
            else return fi.FieldType;
        }

        public static Type GetBaseType<T>()
        {
            return GetBaseType(typeof(T));
        }

        public static Type GetBaseType(this Type type)
        {
            if (type.Name.Contains("Nullable"))
            {
                return GetBaseType(((FieldInfo[])((TypeInfo)type).DeclaredFields)[1].FieldType);
            }
            if (type.IsGenericType && type.FullName.Contains("Generic.List"))
            {
                return type.GetGenericArguments()[0];
            }
            else return type;
        }

        public static List<string> ListProperties(this Type t)
        {
            List<string> result = new List<string>();
            BindingFlags flags = BindingFlags.FlattenHierarchy |
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic
                    | BindingFlags.Static;

            FieldInfo[] fields = t.GetType().GetFields(flags);
            result = fields.Select(x => x.Name).OrderBy(x => x).ToList();
            return result;
        }

        public static void CopyFrom<T>(this T copyToObject, T copyFromObject)
        {
            BindingFlags flags = BindingFlags.FlattenHierarchy |
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic
                    | BindingFlags.Static;

            FieldInfo[] fields = copyFromObject.GetType().GetFields(flags);
            for (int i = 0; i < fields.Length; ++i)
            {
                BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                    | BindingFlags.Static;
                FieldInfo field = copyFromObject.GetType().GetField(fields[i].Name, bindFlags);
                FieldInfo toField = copyToObject.GetType().GetField(fields[i].Name, bindFlags);
                if (field != null)
                {
                    toField.SetValue(copyToObject, field.GetValue(copyFromObject));
                }
            }
        }

        public static void CopyValuesFrom<T>(this T copyToObject, T copyFromObject)
        {
            BindingFlags flags = BindingFlags.FlattenHierarchy |
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic
                    | BindingFlags.Static;

            FieldInfo[] fields = copyFromObject.GetType().GetFields(flags);
            for (int i = 0; i < fields.Length; ++i)
            {
                if (fields[i].Name.ToLower().StartsWith("<id>")) continue;
                BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                    | BindingFlags.Static;
                FieldInfo field = copyFromObject.GetType().GetField(fields[i].Name, bindFlags);
                FieldInfo toField = copyToObject.GetType().GetField(fields[i].Name, bindFlags);
                if (field != null)
                {
                    toField.SetValue(copyToObject, field.GetValue(copyFromObject));
                }
            }
        }

        //public static void CopyTo<T>(this T copyFromObject, T copyToObject)
        //{
        //    BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

        //    FieldInfo[] fields = copyFromObject.GetType().GetFields(flags);
        //    for (int i = 0; i < fields.Length; ++i)
        //    {
        //        BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        //            | BindingFlags.Static;
        //        FieldInfo field = copyFromObject.GetType().GetField(fields[i].Name, bindFlags);
        //        FieldInfo toField = copyToObject.GetType().GetField(fields[i].Name, bindFlags);
        //        if (field != null)
        //        {
        //            toField.SetValue(copyToObject, field.GetValue(copyFromObject));
        //        }
        //    }
        //}

        public static List<T> ToList<T>(this IEnumerable<object> copyFromList)
        {
            if (copyFromList != null)
            {
                Type listType = typeof(List<>).MakeGenericType(new[] { typeof(T) });
                IList list = (IList)Activator.CreateInstance(listType);

                if (copyFromList.GetType().IsListOrArray())
                {
                    foreach (var o in copyFromList)
                    {
                        T o2 = (T)Activator.CreateInstance(typeof(T));
                        o.CopyTo(o2);
                        list.Add(o2);
                    }

                    return (List<T>)Convert.ChangeType(list, typeof(List<T>));
                }
                else
                {
                    T o2 = (T)Activator.CreateInstance(typeof(T));
                    copyFromList.CopyTo(o2);
                    list.Add(o2);
                }

                return (List<T>)list;
            }

            return null;
        }

        public static void CopyTo<T, T2>(this T copyFromObject, T2 copyToObject)
        {
            //BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

            FieldInfo[] fieldsFromObject = copyFromObject.GetType().GetFields(bindFlags);
            FieldInfo[] fieldsToObject = copyToObject.GetType().GetFields(bindFlags);

            for (int i = 0; i < fieldsFromObject.Length; i++)
            {
                FieldInfo fieldFrom = copyFromObject.GetType().GetField(fieldsFromObject[i].Name, bindFlags);
                if (fieldFrom != null)
                {
                    for (int j = 0; j < fieldsToObject.Length; j++)
                    {
                        FieldInfo fieldTo = copyToObject.GetType().GetField(fieldsToObject[j].Name, bindFlags);
                        if (fieldTo != null)
                        {
                            if (fieldFrom.Name == fieldTo.Name)
                            {
                                if (fieldFrom.FieldType != fieldTo.FieldType)
                                {
                                    object fromObjectValue = fieldFrom.GetValue(copyFromObject);
                                    object toObjectValue = Activator.CreateInstance(fieldTo.FieldType);

                                    if (fromObjectValue.IsList())
                                    {
                                        Type innerListItemType = fieldTo.GetBaseType();
                                        Type listType = typeof(List<>).MakeGenericType(new[] { innerListItemType });
                                        IList list = (IList)Activator.CreateInstance(listType);

                                        foreach (object o in fromObjectValue.GetEnumerable())
                                        {
                                            object innerListItem = Activator.CreateInstance(innerListItemType);
                                            o.CopyTo(innerListItem);
                                            list.Add(innerListItem);
                                        }

                                        fieldTo.SetValue(copyToObject, list);
                                    }
                                    else
                                    {
                                        fromObjectValue.CopyTo(toObjectValue);
                                        fieldTo.SetValue(copyToObject, toObjectValue);
                                    }
                                }
                                else
                                {
                                    fieldTo.SetValue(copyToObject, fieldFrom.GetValue(copyFromObject));
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Permet d'itérer sur un objet dont on ne connais pas le type
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static IEnumerable<object> GetEnumerable(this object o)
        {
            foreach (FieldInfo fi in ((TypeInfo)o.GetType()).DeclaredFields)
            {
                if (fi.FieldType.GetInterfaces().Contains(typeof(IEnumerable)))
                {
                    foreach (var item in (IEnumerable)fi.GetValue(o))
                    {
                        yield return item;
                    }
                }
                break;
            }
        }

        #region DataTable/DataRow
        public static List<T> ToList<T>(this System.Data.DataTable dataTable)
        {
            List<T> result = new List<T>();

            foreach(System.Data.DataRow row in dataTable.Rows)
            {
                result.Add(row.ToObject<T>());
            }

            return result;
        }

        public static T ToObject<T>(this System.Data.DataRow dataRow)
        {
            T result = (T)Activator.CreateInstance(typeof(T));
            PropertyInfo[] props = result.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

            foreach(System.Data.DataColumn c in dataRow.Table.Columns)
            {
                PropertyInfo p = props.FirstOrDefault(x => x.Name == c.ColumnName);
                if(p != null)
                {
                    if(p.PropertyType == typeof(bool))
                    {
                        p.SetValue(result, Convert.ToBoolean(dataRow[c], null));
                    }
                    else
                    {
                        p.SetValue(result, dataRow[c], null);
                    }
                }
            }

            return result;
        }

        public static string Get(this System.Data.DataRow dataRow, string colName, string default_value = null)
        {
            try
            {
                if(dataRow.Table.Columns.Contains(colName))
                {
                    if(dataRow[colName] is DBNull)
                    {
                        return default_value;
                    }

                    return dataRow[colName].ToString();
                }
                else
                {
                    return default_value;
                }
            }
            catch (Exception)
            {
                return default_value;
            }
        }

        public static T Get<T>(this System.Data.DataRow dataRow, string colName, object default_value = null)
        {
            try
            {
                if (dataRow.Table.Columns.Contains(colName))
                {
                    if (dataRow[colName] is DBNull)
                    {
                        if (default_value != null) return (T)default_value;
                        return default;
                    }

                    if(typeof(T) == typeof(bool))
                    {
                        if(dataRow[colName].ToString().ToLower().EqualsOr("1", "true", "on", "vrai", "oui", "yes"))
                        {
                            return (T)Convert.ChangeType(true, typeof(T));
                        }
                        else
                        {
                            return (T)Convert.ChangeType(false, typeof(T));
                        }
                    }
                    else
                    {
                        return (T)Convert.ChangeType(dataRow[colName], typeof(T));
                    }
                }
                else
                {
                    if (default_value != null) return (T)default_value;
                    return default;
                }
            }
            catch (Exception)
            {
                if (default_value != null) return (T)default_value;
                return default;
            }
        }
        #endregion

    }
}
