using OpenOrm.SqlProvider.Shared;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("OpenOrm.SqlServer")]
[assembly: InternalsVisibleTo("OpenOrm.SQLite")]
[assembly: InternalsVisibleTo("OpenOrm.MySql")]

namespace OpenOrm.Configuration
{
    public abstract class OpenOrmConfigurationBase
    {
        public Connector Connector { get; set; }
        public ISqlConnector ConnectorProvider { get; set; }
        public string Schema { get; set; }
        //public bool StoreComplexObjectsInJson { get; set; }
        public string ConnectionString { get; set; }
        public bool PutIdFieldAtFirstPosition { get; set; }
        //public bool EnableAutomaticMigration { get; set; }
        //public bool AutoDetectNestedObjects { get; set; }
        public bool ForceAutoLoadNestedObjects { get; set; }
        //public bool CreateIndexForEachColumn { get; set; }
        public bool GenerateOrmDocInDatabase { get; set; }
        //public int ListInsertChunkSize { get; set; } //Values limited to 1000 per request -> chunks size calculated at insert
        public bool ListInsertAllowBulkInsert { get; set; }
        public bool ListInsertFallBackToChunkInsertIfBulkInsertError { get; set; }
        public int ListInsertMinimumCountForBulkInsert { get; set; }
        public bool UseOpenOrmConfigurationInDatabase { get; set; }
        public bool PrintSqlQueries { get; set; }
        public bool UseSchemaCache { get; set; }
        public bool MapPrivateProperties { get; set; }
        public bool EnableRamCache { get; set; }
        public long RamCacheRowsLimitPerModel { get; set; }
        public bool UseDatabaseSchema { get; set; }


        internal OpenOrmConfigurationBase()
        {
            Connector = Connector.SqLite;
            ConnectionString = ":memory:";
            Schema = "";
            //StoreComplexObjectsInJson = false;
            PutIdFieldAtFirstPosition = true;
            //EnableAutomaticMigration = false;
            //AutoDetectNestedObjects = false;
            ForceAutoLoadNestedObjects = false;
            //CreateIndexForEachColumn = false;
            GenerateOrmDocInDatabase = false;
            //ListInsertChunkSize = 500;
            ListInsertAllowBulkInsert = false;
            ListInsertFallBackToChunkInsertIfBulkInsertError = true;
            ListInsertMinimumCountForBulkInsert = 100;
            UseOpenOrmConfigurationInDatabase = false;
            PrintSqlQueries = false;
            UseSchemaCache = true;
            MapPrivateProperties = false;
            EnableRamCache = false;
            RamCacheRowsLimitPerModel = 50000;
            UseDatabaseSchema = true;
        }
    }
}
