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

        public static void SetDbDefinition(string db_name, string server_name, List<TableDefinition> tables)
        {
            if(!Definitions.ContainsKey($"{server_name}.{db_name}"))
            {
                Definitions.Add($"{server_name}.{db_name}", tables);
            }
            else
            {
                Definitions[$"{server_name}.{db_name}"] = tables;
            }
        }

        public static void Clear(string db_name = "", string server_name = "")
        {
            if(!string.IsNullOrEmpty($"{server_name}{db_name}") && Definitions.ContainsKey($"{server_name}.{db_name}"))
            {
                Definitions.Remove($"{server_name}.{db_name}");
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
