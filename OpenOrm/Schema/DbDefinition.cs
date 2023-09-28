using OpenOrm.Extensions;
using OpenOrm.Schema;
using OpenOrm.SqlProvider.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenOrm.Schema
{
    public static class DbDefinition
    {
        private static Dictionary<string, List<TableDefinition>> _definitions;
        public static Dictionary<string, List<TableDefinition>> Definitions 
        {
            get
            {
                if(_definitions == null)
                {
                    _definitions = new Dictionary<string, List<TableDefinition>>();
                }

                return _definitions;
            }
            set
            {
                _definitions = value;
            }
        }

        public static void SetDbDefinition(string connection_string, List<TableDefinition> tables)
        {
            if(!Definitions.ContainsKey(connection_string))
            {
                Definitions.Add(connection_string, tables);
            }
            else
            {
                Definitions[connection_string] = tables;
            }
        }

        public static void Clear(string connection_string = "")
        {
            if(!string.IsNullOrEmpty(connection_string) && Definitions.ContainsKey(connection_string))
            {
                Definitions.Remove(connection_string);
            }
            else
            {
                Definitions.Clear();
                Definitions = null;
                Definitions = new Dictionary<string, List<TableDefinition>>();
            }
        }

        public static bool IsInited(string db_name)
        {
            return Definitions.ContainsKey(db_name);
        }
    }
}
