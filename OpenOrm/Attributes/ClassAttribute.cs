using System;
using System.Runtime.CompilerServices;

namespace OpenOrm
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DbModelAttribute : Attribute
    {
        public string TableName { get; set; }
        public DbModelAttribute([CallerMemberName] string name = null)
        {
            if (!string.IsNullOrEmpty(name))
            {
                TableName = name;
            }
        }

        public DbModelAttribute(string tableName, [CallerMemberName] string name = null)
        {
            TableName = tableName;

            if(TableName.Contains("{class}") && !string.IsNullOrEmpty(name))
            {
                TableName = TableName.Replace("{class}", name);
            }
        }
    }

}
