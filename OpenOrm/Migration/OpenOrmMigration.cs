using System;
using System.Collections.Generic;
using System.Text;

namespace OpenOrm.Migration
{
    public class OpenOrmMigration
    {
        [DbPrimaryKey, DbAutoIncrement]
        public long Id { get; set; }
        [DbSize(100)]
        public string Version { get; set; }
        [DbSize(400)]
        public string Comment { get; set; }

        public virtual void Up(OpenOrmDbConnection db) { }

        public virtual void Down(OpenOrmDbConnection db) { }
    }
}
