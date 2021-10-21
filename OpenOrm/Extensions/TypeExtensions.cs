using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenOrm.Extensions
{
    public static partial class Extensions
    {
        ///// <summary>
        ///// usage : string name = typeof(MyClass).GetAttributeValue((DomainNameAttribute dna) => dna.Name);
        ///// </summary>
        ///// <typeparam name="TAttribute"></typeparam>
        ///// <typeparam name="TValue"></typeparam>
        ///// <param name="type"></param>
        ///// <param name="valueSelector"></param>
        ///// <returns></returns>
        //public static TValue GetClassAttributeValue<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
        //{
        //    var att = type.GetCustomAttributes(
        //        typeof(TAttribute), true
        //    ).FirstOrDefault() as TAttribute;
        //    if (att != null)
        //    {
        //        return valueSelector(att);
        //    }
        //    return default(TValue);
        //}

        //public static TValue GetPropertyAttributeValue<TAttribute, TValue>(this PropertyInfo pi, Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
        //{
        //    var att = pi.GetCustomAttributes(
        //        typeof(TAttribute), true
        //    ).FirstOrDefault() as TAttribute;
        //    if (att != null)
        //    {
        //        return valueSelector(att);
        //    }
        //    return default(TValue);
        //}

        public static Type GetAnyElementType(Type type)
        {
            // Type is Array
            // short-circuit if you expect lots of arrays 
            if (type.IsArray)
                return type.GetElementType();

            // type is IEnumerable<T>;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return type.GetGenericArguments()[0];

            // type implements/extends IEnumerable<T>;
            var enumType = type.GetInterfaces()
                                    .Where(t => t.IsGenericType &&
                                           t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                                    .Select(t => t.GenericTypeArguments[0]).FirstOrDefault();
            return enumType ?? type;
        }

        public static object GetDefaultValue(this Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }

        //public static Type GetBaseType(this PropertyInfo pi)
        //{
        //    if (pi.PropertyType.Name.Contains("Nullable"))
        //    {
        //        return GetBaseType(((FieldInfo[])((TypeInfo)pi.PropertyType).DeclaredFields)[1].FieldType);
        //    }
        //    if (pi.PropertyType.IsGenericType && pi.PropertyType.FullName.Contains("Generic.List"))
        //    {
        //        return pi.PropertyType.GetGenericArguments()[0];
        //    }
        //    else return pi.PropertyType;
        //}

        //public static Type GetBaseType(this FieldInfo fi)
        //{
        //    if (fi.FieldType.Name.Contains("Nullable"))
        //    {
        //        return GetBaseType(((FieldInfo[])((TypeInfo)fi.FieldType).DeclaredFields)[1].FieldType);
        //    }
        //    if (fi.FieldType.IsGenericType && fi.FieldType.FullName.Contains("Generic.List"))
        //    {
        //        return fi.FieldType.GetGenericArguments()[0];
        //    }
        //    else return fi.FieldType;
        //}

        //public static Type GetBaseType<T>()
        //{
        //    return GetBaseType(typeof(T));
        //}

        //public static Type GetBaseType(this Type type)
        //{
        //    if (type.Name.Contains("Nullable"))
        //    {
        //        return GetBaseType(((FieldInfo[])((TypeInfo)type).DeclaredFields)[1].FieldType);
        //    }
        //    if (type.IsGenericType && type.FullName.Contains("Generic.List"))
        //    {
        //        return type.GetGenericArguments()[0];
        //    }
        //    else return type;
        //}

        public static DbType ToDbType(this SqlDbType sqldbtype)
        {
            switch (sqldbtype)
            {
                default:
                case SqlDbType.NVarChar:
                case SqlDbType.VarChar:
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.Text:
                case SqlDbType.NText:
                    return DbType.String;
                case SqlDbType.Date:
                    return DbType.Date;
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                    return DbType.DateTime;
                case SqlDbType.Int:
                case SqlDbType.SmallInt:
                case SqlDbType.TinyInt:
                    return DbType.Int32;
                case SqlDbType.BigInt:
                    return DbType.Int64;
                case SqlDbType.Bit:
                    return DbType.Boolean;
                case SqlDbType.Float:
                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.Real:
                case SqlDbType.SmallMoney:
                    return DbType.Decimal;
                case SqlDbType.Binary:
                case SqlDbType.VarBinary:
                    return DbType.Binary;
                case SqlDbType.UniqueIdentifier:
                    return DbType.Guid;
            }
        }

        public static bool IsListOrArray(this PropertyInfo pi)
        {
            return pi != null && pi.PropertyType.IsListOrArray();
        }

        public static bool IsList(this object instance)
        {
            return instance != null && instance.GetType().IsListOrArray();
        }

        public static bool IsListOrArray(this Type type)
        {
            if (type == null || type == typeof(string))
                return false;
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

    }
}
