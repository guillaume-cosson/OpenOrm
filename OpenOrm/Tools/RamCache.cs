using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace OpenOrm.CoreTools
{
    public static class RamCache
    {
        static ConcurrentDictionary<string, object> Cache;

        public static void Init()
        {
            if(Cache == null)
                Cache = new ConcurrentDictionary<string, object>();
        }

        #region GetKey

        public static string GetKey<T>()
        {
            return typeof(T).AssemblyQualifiedName;
        }

        public static string GetKey<T>(Expression<Func<T, bool>> predicate)
        {
            return typeof(T).AssemblyQualifiedName + predicate.ToString();
        }

        #endregion


        #region Exists
        public static bool Exists(string key)
        {
            return Cache.ContainsKey(key) && Cache[key] != null;
        }

        public static bool Exists<T>()
        {
            string key = GetKey<T>();
            return Cache.ContainsKey(key) && Cache[key] != null;
        }

        public static bool Exists<T>(Expression<Func<T, bool>> predicate)
        {
            return Cache.ContainsKey(GetKey<T>(predicate));
        }

        #endregion

        #region Get/Set
        public static object Get(string key)
        {
            if (!Cache.ContainsKey(key) || (Cache.ContainsKey(key) && Cache[key] == null)) return default;

            return Cache[key];
        }

        public static T Get<T>(object value = null)
        {
            string key = GetKey<T>();

            if (!Cache.ContainsKey(key) && value != null)
            {
                Cache[key] = value;
                return (T)value;
            }

            if (!Cache.ContainsKey(key) || (Cache.ContainsKey(key) && Cache[key] == null)) return default;

            return (T)Cache[key];
        }

        public static T Get<T>(Expression<Func<T, bool>> predicate, object value = null)
        {
            string key = GetKey<T>(predicate);

            if (!Cache.ContainsKey(key) && value != null)
            {
                Cache[key] = value;
                return (T)value;
            }

            if (!Cache.ContainsKey(key) || (Cache.ContainsKey(key) && Cache[key] == null)) return default;

            return (T)Cache[key];
        }

        public static void Set(string key, object value)
        {
            Cache[key] = value;
        }

        public static void Set<T>(object value)
        {
            Cache[GetKey<T>()] = value;
        }

        public static void Set<T>(Expression<Func<T, bool>> predicate, object value)
        {
            Cache[GetKey<T>(predicate)] = value;
        }

        #endregion

        #region Invalidate
        public static void Invalidate<T>()
        {
            Cache[GetKey<T>()] = null;
        }

        public static void Invalidate<T>(Expression<Func<T, bool>> predicate)
        {
            Cache[GetKey<T>(predicate)] = null;
        }

        public static void InvalidateAll()
        {
            Cache = new ConcurrentDictionary<string, object>();
        }
        #endregion
    }
}
