using OpenOrm.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenOrm.Schema
{
    public class ColumnDefinition
    {
        #region Cache
        private static ConcurrentDictionary<string, ColumnDefinition> Cache { get; set; }
        public static void ClearCache()
        {
            Cache = new ConcurrentDictionary<string, ColumnDefinition>();
        }
        #endregion

        public static bool UseBrackets { get; set; }
        public string Name { get; set; }
        public Type PropertyType { get; set; }
        public Type OwnerType { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsAutoIncrement { get; set; }
        public bool IsNullableType { get; set; }
        public bool IsNotNullColumn { get; set; }
        public bool SetDefaultValueForExistingRows { get; set; }
        public bool IsUnique { get; set; }
        public bool HasSize { get; set; }
        public bool IsSizeMax { get; set; }
        public bool HasDecimalSize { get; set; }
        public bool HasDefaultValue { get; set; }
        public bool ExistsInDb { get; set; }
        private object _defValue;
        public object DefaultValue 
        { 
            get {return _defValue; } 
            set 
            { 
                if(value != null && value.GetType().Name.ToLower().Contains("boolean"))
                {
                    if((bool)value)
                    {
                        _defValue = 1;
                    }
                    else
                    {
                        _defValue = 0;
                    }
                }
                else
                {
                    _defValue = value;
                }
            } 
        }
        public SqlDbType SqlDbType { get; set; }
        public DbType DbType { get; set; }
        public int Size { get; set; }
        public int Scale { get; set; }
        public int Precision { get; set; }
        public bool IsNestedProperty { get; set; }
        public Type NestedChildType;
        public string NestedChildForeignKeyProperty;
        public Type NestedChildForeignKeyPropertyType;
        public string NestedParentPrimaryKeyProperty;
        public Type NestedParentPrimaryKeyPropertyType;
        public string NestedChildPropertyToGet;
        public bool NestedAutoLoad;
        public bool IsForeignKey { get; set; }
        public Type ForeignType { get; set; }
        public string ForeignChildTargetProperty { get; set; }
        public string ParentForeignKeyProperty { get; set; }
        public bool ForeignAutoLoad;
        public PropertyInfo PropertyInfo { get; set; }
        public TableDefinition TableDefinition { get; set; }

        public ColumnDefinition()
        {
            if (Cache == null) Cache = new ConcurrentDictionary<string, ColumnDefinition>();
        }

        public ColumnDefinition(PropertyInfo pi, bool useCache = true)
        {
            if (Cache == null) Cache = new ConcurrentDictionary<string, ColumnDefinition>();

            if(useCache && Cache.ContainsKey(pi.DeclaringType.FullName + "_" + pi.Name))
            {
                ColumnDefinition cached = Cache[pi.DeclaringType.FullName + "_" + pi.Name];
                OwnerType = cached.OwnerType;
                DbType = cached.DbType;
                DefaultValue = cached.DefaultValue;
                HasDecimalSize = cached.HasDecimalSize;
                HasDefaultValue = cached.HasDefaultValue;
                HasSize = cached.HasSize;
                IsSizeMax = cached.IsSizeMax;
                IsAutoIncrement = cached.IsAutoIncrement;
                IsNotNullColumn = cached.IsNotNullColumn;
                IsNullableType = cached.IsNullableType;
                IsPrimaryKey = cached.IsPrimaryKey;
                IsUnique = cached.IsUnique;
                Name = cached.Name;
                Precision = cached.Precision;
                PropertyInfo = cached.PropertyInfo;
                PropertyType = cached.PropertyType;
                Scale = cached.Scale;
                SetDefaultValueForExistingRows = cached.SetDefaultValueForExistingRows;
                Size = cached.Size;
                SqlDbType = cached.SqlDbType;
                IsNestedProperty = cached.IsNestedProperty;
                NestedChildForeignKeyProperty = cached.NestedChildForeignKeyProperty;
                NestedChildForeignKeyPropertyType = cached.NestedChildForeignKeyPropertyType;
                NestedParentPrimaryKeyProperty = cached.NestedParentPrimaryKeyProperty;
                NestedParentPrimaryKeyPropertyType = cached.NestedParentPrimaryKeyPropertyType;
                NestedChildPropertyToGet = cached.NestedChildPropertyToGet;
                NestedAutoLoad = cached.NestedAutoLoad;
                IsForeignKey = cached.IsForeignKey;
                ForeignType = cached.ForeignType;
                ForeignChildTargetProperty = cached.ForeignChildTargetProperty;
                ParentForeignKeyProperty = cached.ParentForeignKeyProperty;
                ForeignAutoLoad = cached.ForeignAutoLoad;
            }
            else
            {
                LoadFromPropertyInfo(pi);
                //ExistsInDb = DbDefinition.Definitions.ContainsKey(Name);
                if (useCache) Cache[pi.DeclaringType.FullName + "_" + pi.Name] = this;
            }
        }

        public ColumnDefinition(Type type, string property_name, bool useCache = true)
        {
            if (Cache == null) Cache = new ConcurrentDictionary<string, ColumnDefinition>();
            OwnerType = type;

            if (useCache && Cache.ContainsKey(type.FullName + "_" + property_name))
            {
                ColumnDefinition cached = Cache[type.FullName + "_" + property_name];
                OwnerType = cached.OwnerType;
                DbType = cached.DbType;
                DefaultValue = cached.DefaultValue;
                HasDecimalSize = cached.HasDecimalSize;
                HasDefaultValue = cached.HasDefaultValue;
                HasSize = cached.HasSize;
                IsSizeMax = cached.IsSizeMax;
                IsAutoIncrement = cached.IsAutoIncrement;
                IsNotNullColumn = cached.IsNotNullColumn;
                IsNullableType = cached.IsNullableType;
                IsPrimaryKey = cached.IsPrimaryKey;
                IsUnique = cached.IsUnique;
                Name = cached.Name;
                Precision = cached.Precision;
                PropertyInfo = cached.PropertyInfo;
                PropertyType = cached.PropertyType;
                Scale = cached.Scale;
                SetDefaultValueForExistingRows = cached.SetDefaultValueForExistingRows;
                Size = cached.Size;
                SqlDbType = cached.SqlDbType;
                IsNestedProperty = cached.IsNestedProperty;
                NestedChildForeignKeyProperty = cached.NestedChildForeignKeyProperty;
                NestedChildForeignKeyPropertyType = cached.NestedChildForeignKeyPropertyType;
                NestedParentPrimaryKeyProperty = cached.NestedParentPrimaryKeyProperty;
                NestedParentPrimaryKeyPropertyType = cached.NestedParentPrimaryKeyPropertyType;
                NestedChildPropertyToGet = cached.NestedChildPropertyToGet;
                NestedAutoLoad = cached.NestedAutoLoad;
                IsForeignKey = cached.IsForeignKey;
                ForeignType = cached.ForeignType;
                ForeignChildTargetProperty = cached.ForeignChildTargetProperty;
                ParentForeignKeyProperty = cached.ParentForeignKeyProperty;
                ForeignAutoLoad = cached.ForeignAutoLoad;
            }
            else
            {
                bool found = false;
                foreach (PropertyInfo pi in OpenOrmTools.GetValidProperties(type))
                {
                    if (pi.Name == property_name)
                    {
                        found = true;
                        LoadFromPropertyInfo(pi);
                        break;
                    }
                }

                //ExistsInDb = DbDefinition.Definitions.ContainsKey(Name);

                if (!found) throw new KeyNotFoundException($"Property {property_name} does not exists in Type {type.Name}");

                if (useCache) Cache[type.FullName + "_" + property_name] = this;
            }
        }

        public void LoadFromPropertyInfo(PropertyInfo pi)
        {
            if (pi != null)
            {
                //if(UseBrackets)
                //{
                //    Name = "[" + pi.Name + "]";
                //}
                //else
                //{
                    Name = pi.Name;
                //}
                
                PropertyType = pi.PropertyType;
                IsNullableType = pi.PropertyType.Name.Contains("Nullable");
                DefaultValue = DBNull.Value;
                IsNotNullColumn = pi.CustomAttributes.Any(x => x.AttributeType.Name == "DbNotNull");
                IsPrimaryKey = pi.CustomAttributes.Any(x => x.AttributeType.Name == "DbPrimaryKey");
                IsAutoIncrement = pi.CustomAttributes.Any(x => x.AttributeType.Name == "DbAutoIncrement");
                IsUnique = pi.CustomAttributes.Any(x => x.AttributeType.Name == "DbUnique");
                HasSize = pi.CustomAttributes.Any(x => x.AttributeType.Name == "DbSize");
                IsSizeMax = pi.CustomAttributes.Any(x => x.AttributeType.Name == "DbSizeMax");
                HasDecimalSize = pi.CustomAttributes.Any(x => x.AttributeType.Name == "DbDecimalSize");
                HasDefaultValue = pi.CustomAttributes.Any(x => x.AttributeType.Name == "DbDefault") || IsNotNullColumn;
                SqlDbType = OpenOrmTools.ToSqlDbType(PropertyType);
                DbType = SqlDbType.ToDbType();
                PropertyInfo = pi;

                if (pi.CustomAttributes.Any(x => x.AttributeType.Name == "DbColumnName"))
                {
                    Name = pi.GetPropertyAttributeValue((DbColumnName dbcn) => dbcn.Name);
                }

                if (pi.CustomAttributes.Any(x => x.AttributeType.Name == "DbLoadNestedObject"))
                {
                    IsNestedProperty = true;
                    NestedChildType = pi.GetPropertyAttributeValue((DbLoadNestedObject attr) => attr.ChildType);
                    NestedChildForeignKeyProperty = pi.GetPropertyAttributeValue((DbLoadNestedObject attr) => attr.ChildForeignKeyProperty);
                    NestedChildForeignKeyPropertyType = OpenOrmTools.GetProperty(NestedChildType, NestedChildForeignKeyProperty).PropertyType;
                    NestedParentPrimaryKeyProperty = pi.GetPropertyAttributeValue((DbLoadNestedObject attr) => attr.ParentPrimaryKeyProperty);
                    //NestedParentPrimaryKeyPropertyType = OpenOrmTools.GetProperty(TableDefinition., NestedParentPrimaryKeyProperty).PropertyType;
                    NestedChildPropertyToGet = pi.GetPropertyAttributeValue((DbLoadNestedObject attr) => attr.ChildPropertyToGet);
                    NestedAutoLoad = pi.GetPropertyAttributeValue((DbLoadNestedObject attr) => attr.AutoLoad);
                }

                if (pi.CustomAttributes.Any(x => x.AttributeType.Name == "DbForeignKey"))
                {
                    IsForeignKey = true;
                    ForeignType = pi.GetPropertyAttributeValue((DbForeignKey attr) => attr.ParentType);
                    ForeignChildTargetProperty = pi.GetPropertyAttributeValue((DbForeignKey attr) => attr.ChildTargetProperty);
                    ParentForeignKeyProperty = pi.GetPropertyAttributeValue((DbForeignKey attr) => attr.ParentPrimaryKeyProperty);
                    ForeignAutoLoad = pi.GetPropertyAttributeValue((DbForeignKey attr) => attr.AutoLoad);
                }

                if (IsNotNullColumn)
                {
                    DefaultValue = pi.GetPropertyAttributeValue((DbNotNull dnn) => dnn.Value);
                    //if (DefaultValue == null) DefaultValue = pi.PropertyType.GetDefaultValue();
                }

                if (HasDefaultValue && DefaultValue == DBNull.Value)
                {
                    DefaultValue = pi.GetPropertyAttributeValue((DbDefault dbd) => dbd.Value);
                    //if (DefaultValue == null) DefaultValue = pi.PropertyType.GetDefaultValue();
                }

                if (DefaultValue == null)
                {
                    DefaultValue = DBNull.Value;
                }

                if(HasSize)
                {
                    Size = pi.GetPropertyAttributeValue((DbSize dbs) => dbs.Size);
                    if (Size <= 0) HasSize = false;
                }

                if (HasDecimalSize)
                {
                    Scale = pi.GetPropertyAttributeValue((DbDecimalSize dbs) => dbs.Scale);
                    Precision = pi.GetPropertyAttributeValue((DbDecimalSize dbs) => dbs.Precision);
                    if (Scale <= 0 || Precision < 0) HasDecimalSize = false;
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
