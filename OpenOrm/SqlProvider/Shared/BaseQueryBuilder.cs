using OpenOrm.Configuration;
using OpenOrm.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenOrm.SqlProvider.Shared
{
	public class BaseQueryBuilder
	{
		//static Dictionary<string, TableDefinition> TableStructureCache;
		//static Dictionary<string, List<string>> TableFieldsCache;
		public OpenOrmConfigurationBase Configuration { get; set; }
		public string Schema
		{
			get
			{
				if (!string.IsNullOrEmpty(Configuration.Schema))
				{
					return Configuration.Schema + ".";
				}
				else
				{
					if (Configuration.Connector == Connector.SqlServer)
					{
						Configuration.Schema = "dbo";
						return Configuration.Schema + ".";
					}
				}
				return string.Empty;
			}
		}

	}
}
