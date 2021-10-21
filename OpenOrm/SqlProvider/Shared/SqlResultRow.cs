using OpenOrm.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenOrm.SqlProvider.Shared
{
    public class SqlResultRow : IDisposable
    {
        public Dictionary<string, object> Row;

        public SqlResultRow()
        {
            Row = new Dictionary<string, object>();
        }

        public SqlResultRow(int size)
        {
            Row = new Dictionary<string, object>(size);
        }

        public string Get(string sColumnName)
        {
            if (Row.ContainsKey(sColumnName))
                return Row[sColumnName].ToString();
            else
                return "";
        }

        public T Get<T>(string sColumnName)
        {
            if (Row.ContainsKey(sColumnName))
            {
                if (typeof(T) == typeof(bool))
                {
                    if (Row[sColumnName].ToString() == "1" || Row[sColumnName].ToString().ToLower() == "true")
                    {
                        return (T)Convert.ChangeType(true, typeof(T));
                    }
                    else
                    {
                        return (T)Convert.ChangeType(false, typeof(T));
                    }
                }
                else return (T)Convert.ChangeType(Row[sColumnName], typeof(T));
            }
            else
                return default(T);
        }

        public T Get<T>(int colIndex)
        {
            if (Row.Count > colIndex)
            {
                if (typeof(T) == typeof(bool))
                {
                    if (Row.Values.ElementAt(colIndex).ToString() == "1" || Row.Values.ElementAt(colIndex).ToString().ToLower() == "true")
                    {
                        return (T)Convert.ChangeType(true, typeof(T));
                    }
                    else
                    {
                        return (T)Convert.ChangeType(false, typeof(T));
                    }
                }
                else return (T)Convert.ChangeType(Row.Values.ElementAt(colIndex), typeof(T));
            }
            else
                return default(T);
        }


        public void Add(string sColumnName, object oValue)
        {
            if (Row.ContainsKey(sColumnName))
                Row[sColumnName] = oValue;
            else
                Row.Add(sColumnName, oValue);
        }

        #region Conversion
        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            foreach (string key in Row.Keys)
            {
                result.Add(key, Row[key]);
            }

            return result;
        }

        public T ToObject<T>(TableDefinition td = null)
        {
            T oResult = (T)Activator.CreateInstance(typeof(T), new object[] { });

            if (Row.Count > 0)
            {
                if (td == null) td = TableDefinition.Get<T>();
                foreach (ColumnDefinition cd in td.Columns)
                {
                    if (Row.ContainsKey(cd.Name) && Row[cd.Name] != null)
                    {
                        if(cd.PropertyType.IsEnum)
                        {
                            cd.PropertyInfo.SetValue(oResult, Convert.ChangeType(Enum.Parse(cd.PropertyType, Row[cd.Name].ToString()), cd.PropertyType), null);
                        }
                        else
                        {
                            cd.PropertyInfo.SetValue(oResult, Convert.ChangeType(Row[cd.Name], cd.PropertyType), null);
                        }
                    }
                }


                //foreach (PropertyInfo prop in typeof(T).GetProperties())
                //{
                //    var attributes = prop.GetCustomAttributes(false);
                //    DbColumnName attribute = (DbColumnName)attributes.Where(x => x.GetType() == typeof(DbColumnName)).FirstOrDefault();
                //    if (attribute != null)
                //    {
                //        string name = attribute.Name;
                //        if (Row.ContainsKey(name) && Row[name] != null)
                //        {
                //            prop.SetValue(oResult, Convert.ChangeType(Row[name], prop.PropertyType.GetBaseType()), null);
                //        }
                //    }
                //    else
                //    {
                //        string name = prop.Name;
                //        if (Row.ContainsKey(name) && Row[name] != null)
                //        {
                //            prop.SetValue(oResult, Convert.ChangeType(Row[name], prop.PropertyType.GetBaseType()), null);
                //        }
                //    }
                //}

                return oResult;
            }
            else
            {
                return default(T);
            }
        }

        public object ToObject(Type t, TableDefinition td = null)
        {
            object oResult = Activator.CreateInstance(t, new object[] { });

            if (Row.Count > 0)
            {
                if (td == null) td = TableDefinition.Get(t);
                foreach (ColumnDefinition cd in td.Columns)
                {
                    if (Row.ContainsKey(cd.Name) && Row[cd.Name] != null)
                    {
                        //cd.PropertyInfo.SetValue(oResult, Convert.ChangeType(Row[cd.Name], cd.PropertyType), null);
                        if (cd.PropertyType.IsEnum)
                        {
                            cd.PropertyInfo.SetValue(oResult, Convert.ChangeType(Enum.Parse(cd.PropertyType, Row[cd.Name].ToString()), cd.PropertyType), null);
                        }
                        else
                        {
                            cd.PropertyInfo.SetValue(oResult, Convert.ChangeType(Row[cd.Name], cd.PropertyType), null);
                        }
                    }
                }

                return oResult;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region IDisposable Support
        internal bool disposedValue = false; // Pour détecter les appels redondants

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Row = null;
                }
                disposedValue = true;
            }
        }

        // Ce code est ajouté pour implémenter correctement le modèle supprimable.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
