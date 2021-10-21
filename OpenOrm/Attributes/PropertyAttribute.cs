using System;
using System.Collections.Generic;
using System.Text;

namespace OpenOrm
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DbPrimaryKey : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DbAutoIncrement : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DbSerialize : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DbColumnName : Attribute
    {
        public readonly string Name;
        public DbColumnName(string _name)
        {
            this.Name = _name;
        }
    }

    /// <summary>
    /// Auto map nested object by foreignkey
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DbForeignKey : Attribute
    {
        public readonly Type ParentType;
        public readonly string ParentPrimaryKeyProperty;
        public readonly string ChildTargetProperty;

        /// <summary>
        /// Auto map nested object by foreignkey
        /// </summary>
        /// <param name="remote_type">Type of model that is the nested object in current model</param>
        /// <param name="remote_property_primary_key_name">Primary key (property name) of nested object (Id, Code, ...)</param>
        /// <param name="target_property_name">Target property name of current model that receive the loaded nested object</param>
        public DbForeignKey(Type parent_type, string parent_property_primary_key_name = "Id", string target_property_name = "")
        {
            this.ParentType = parent_type;
            this.ParentPrimaryKeyProperty = parent_property_primary_key_name;
            this.ChildTargetProperty = target_property_name;
        }
    }

    /// <summary>
    /// Auto map nested object by foreignkey
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DbLoadNestedList : Attribute
    {
        public readonly Type ChildType;
        public readonly string ChildForeignKeyProperty;
        public readonly string ParentPrimaryKeyProperty;
        public readonly string ChildPropertyToGet; //If left empty, will return the entire object instead of the property
        public readonly bool AutoLoad;

        /// <summary>
        /// Auto map nested object by foreignkey
        /// </summary>
        /// <param name="remote_type">Type of model that is the nested object in current model</param>
        /// <param name="remote_property_primary_key_name">Primary key (property name) of nested object (Id, Code, ...)</param>
        /// <param name="target_property_name">Target property name of current model that receive the loaded nested object</param>
        public DbLoadNestedList(Type child_type, string child_foreign_key_property_name, string parent_primary_key_property_name = "Id", bool auto_load = true)
        {
            this.ChildType = child_type;
            this.ChildForeignKeyProperty = child_foreign_key_property_name;
            this.ParentPrimaryKeyProperty = parent_primary_key_property_name;
            this.AutoLoad = auto_load;
        }

        /// <summary>
        /// Auto map nested object by foreignkey
        /// </summary>
        /// <param name="remote_type">Type of model that is the nested object in current model</param>
        /// <param name="remote_property_primary_key_name">Primary key (property name) of nested object (Id, Code, ...)</param>
        /// <param name="target_property_name">Target property name of current model that receive the loaded nested object</param>
        public DbLoadNestedList(Type child_type, string child_foreign_key_property_name, string parent_primary_key_property_name, string child_property_to_get, bool auto_load = true)
        {
            this.ChildType = child_type;
            this.ChildForeignKeyProperty = child_foreign_key_property_name;
            this.ParentPrimaryKeyProperty = parent_primary_key_property_name;
            this.ChildPropertyToGet = child_property_to_get;
            this.AutoLoad = auto_load;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class DbIgnore : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DbUnique : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DbNotNull : Attribute
    {
        public object Value { get; }

        public DbNotNull()
        {
            Value = DBNull.Value;
        }

        public DbNotNull(object value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DbDefault : Attribute
    {
        public object Value { get; }
        public bool SetValueForExistingRows { get; }

        public DbDefault(object value, bool setValueForExistingRows = false)
        {
            Value = value;
            SetValueForExistingRows = setValueForExistingRows;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DbSize : Attribute
    {
        public int Size { get; }

        public DbSize(int size)
        {
            Size = size;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DbSizeMax : Attribute
    {
        public DbSizeMax()
        {
            
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DbDecimalSize : Attribute
    {
        public int Scale { get; }
        public int Precision { get; }

        public DbDecimalSize(int scale, int precision)
        {
            Scale = scale;
            Precision = precision;
        }
    }
}
