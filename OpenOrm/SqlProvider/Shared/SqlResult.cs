using OpenOrm.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenOrm.SqlProvider.Shared
{
    public class SqlResult : IDisposable
    {
        public List<SqlResultRow> Rows;
        private int _currentIndex = 0;

        public SqlResult()
        {
            Rows = new List<SqlResultRow>();
        }

        public SqlResult(int size)
        {
            Rows = new List<SqlResultRow>(size);
        }

        public void AddRow(SqlResultRow srr)
        {
            Rows.Add(srr);
        }

        public bool HasRows
        {
            get { return (Rows != null && Rows.Count > 0); }
        }

        public SqlResultRow GetRow()
        {
            return Rows[_currentIndex];
        }

        public SqlResultRow GetRow(int index)
        {
            if (Rows.Count > index)
                return Rows[index];
            else return null;
        }

        public List<SqlResultRow> GetRows()
        {
            return Rows;
        }

        public object GetRawValue(int rowIndex, int colIndex)
        {
            return Rows[rowIndex].Row.ElementAt(colIndex);
        }

        public T Get<T>(int rowIndex, int colIndex)
        {
            if (Rows.Count > rowIndex && Rows[rowIndex].Row.Count > colIndex)
            {
                if (typeof(T) == typeof(bool))
                {
                    if (Rows[rowIndex].Row.Values.ElementAt(colIndex).ToString() == "1")
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
                    if(Rows[rowIndex].Row.Values.ElementAt(colIndex) != null)
                    {
                        return (T)Convert.ChangeType(Rows[rowIndex].Row.Values.ElementAt(colIndex), typeof(T));
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
            else return default(T);
        }

        public T Get<T>(string sColumnName)
        {
            return Rows[_currentIndex].Get<T>(sColumnName);
        }

        public string Get(string sColumnName)
        {
            return (string)Rows[_currentIndex].Get(sColumnName);
        }

        public Dictionary<string, object> GetFields(int index = 0)
        {
            SqlResultRow rr = GetRow(index);
            Dictionary<string, object> result = new Dictionary<string, object>(rr.Row.Keys.Count);
            foreach (string key in rr.Row.Keys)
            {
                if (!result.ContainsKey(key))
                    result.Add(key, rr.Row[key]);
            }

            return result;
        }

        public SqlResultRow NextRow()
        {
            _currentIndex++;
            return Rows[_currentIndex];
        }

        public SqlResultRow PreviousRow()
        {
            _currentIndex--;
            return Rows[_currentIndex];
        }

        public SqlResultRow GoToFirstRow()
        {
            _currentIndex = 0;
            return Rows[_currentIndex];
        }

        public SqlResultRow GoToLastRow()
        {
            _currentIndex = Rows.Count - 1;
            return Rows[_currentIndex];
        }

        #region Conversion
        public Dictionary<string, object> ToDictionary(int index = -1)
        {
            if (Rows.Count == 1 || (Rows.Count >= 1 && index == -1))
            {
                return Rows[0].ToDictionary();
            }
            else if (index >= 0)
            {
                if (Rows.Count > index)
                    return Rows[index].ToDictionary();
                else
                    return new Dictionary<string, object>();
            }
            else
            {
                return new Dictionary<string, object>();
            }
        }

        public List<Dictionary<string, object>> ToDictionaryList()
        {
            List<Dictionary<string, object>> lst = new List<Dictionary<string, object>>(Rows.Count);

            for (int i = 0; i < Rows.Count; i++)
            {
                lst.Add(Rows[i].ToDictionary());
            }

            return lst;
        }

        public T ToObject<T>(int index = -1)
        {
            if (index >= 0)
            {
                if (Rows.Count > index)
                    return Rows[index].ToObject<T>();
                else return default(T);
            }
            else if (Rows.Count >= 1)
            {
                return Rows[0].ToObject<T>();
            }
            else
            {
                return default(T);
            }
        }

        public object ToObject(Type t)
        {
            if (Rows.Count >= 1)
            {
                return Rows[0].ToObject(t);
            }
            else
            {
                return default;
            }
        }

        public T ToObjectLast<T>(int index = -1)
        {
            if (index >= 0)
            {
                if (Rows.Count > index)
                    return Rows[index].ToObject<T>();
                else return default(T);
            }
            else if (Rows.Count >= 1)
            {
                return Rows[Rows.Count - 1].ToObject<T>();
            }
            else
            {
                return default(T);
            }
        }

        public List<T> ToObjectList<T>()
        {
            List<T> lstResult = new List<T>(Rows.Count);
            TableDefinition td = TableDefinition.Get<T>();

            for (int i = 0; i < Rows.Count; i++)
            {
                lstResult.Add(Rows[i].ToObject<T>(td));
            }

            return lstResult;
        }

        public List<object> ToObjectList(Type t)
        {
            List<object> lstResult = new List<object>(Rows.Count);
            TableDefinition td = TableDefinition.Get(t);

            for (int i = 0; i < Rows.Count; i++)
            {
                lstResult.Add(Rows[i].ToObject(t, td));
            }

            return lstResult;
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
                    Rows = null;
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
