using OpenOrm;
using OpenOrm.SQLite;
using OpenOrm.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Extensions.Ordering;
using XUnitTests.Models;
using OpenOrm.SqlProvider.Shared;
using Faker;

namespace XUnitTests
{
    [Order(2)]
    public class TestSQLite
    {
        #region init
        public static OpenOrmDbConnection GetConnection()
        {
            return new OpenOrmConfiguration
            {
                ConnectionString = "db.sqlite",
                PrintSqlQueries = true,
                PutIdFieldAtFirstPosition = true,
                MapPrivateProperties = true,
                EnableAutomaticMigration = true,
                UseDatabaseSchema = true
            }.GetConnection();
        }

        private void DropTables(OpenOrmDbConnection db)
        {
            if (db.TableExists<Model20Props>()) db.DropTable<Model20Props>();
            if (db.TableExists<ModelDouble>()) db.DropTable<ModelDouble>();
            if (db.TableExists<ModelInt>()) db.DropTable<ModelInt>();
            if (db.TableExists<ModelKeywordColName>()) db.DropTable<ModelKeywordColName>();
            if (db.TableExists<ModelMultiplePK>()) db.DropTable<ModelMultiplePK>();
            if (db.TableExists<ModelNestedChild>()) db.DropTable<ModelNestedChild>();
            if (db.TableExists<ModelNestedParent>()) db.DropTable<ModelNestedParent>();
            if (db.TableExists<ModelNullableDouble>()) db.DropTable<ModelNullableDouble>();
            if (db.TableExists<ModelPrivate>()) db.DropTable<ModelPrivate>();
            if (db.TableExists<ModelPropRenamed>()) db.DropTable<ModelPropRenamed>();
            if (db.TableExists<ModelSerializeParent>()) db.DropTable<ModelSerializeParent>();
            if (db.TableExists<ModelString>()) db.DropTable<ModelString>();
            if (db.TableExists<ModelStringDefault>()) db.DropTable<ModelStringDefault>();
            if (db.TableExists<ModelStringNotNull>()) db.DropTable<ModelStringNotNull>();
            if (db.TableExists<ModelStringUnique>()) db.DropTable<ModelStringUnique>();
            if (db.TableExists<ModelTableRenamed>()) db.DropTable<ModelTableRenamed>();
            if (db.TableExists<ModelWithoutId>()) db.DropTable<ModelWithoutId>();
            if (db.TableExists<ObjectSerialize>()) db.DropTable<ObjectSerialize>();
            if (db.TableExists<Read>()) db.DropTable<Read>();
        }
        #endregion



        #region ModelString
        [Fact, Order(1)]
        public void CheckTableExists_ModelString_ShouldBeFalse()
        {
            using var db = GetConnection();
            DropTables(db);
            Assert.False(db.TableExists<ModelString>());
        }

        [Fact, Order(2)]
        public void CreateTableCheckExists_ModelString_ShouldBeTrue()
        {
            using var db = GetConnection();
            db.CreateTable<ModelString>();
            Assert.True(db.TableExists<ModelString>());
        }

        [Fact, Order(3)]
        public void InsertRandomData_Count_ModelString()
        {
            using var db = GetConnection();

            //int count = RandomNumber.Next(10, 100);
            db.Insert(new ModelString { String_test = "_test" });
            for (int i = 0; i < 99; i++)
            {
                db.Insert(new ModelString { String_test = Faker.Lorem.Sentence(10) });
            }

            Assert.Equal(100, db.Count<ModelString>());
        }

        [Fact, Order(4)]
        public void InsertRandomDataList_Count_ModelString()
        {
            using var db = GetConnection();
            List<ModelString> list = new List<ModelString>();
            for (int i = 0; i < 99; i++)
            {
                list.Add(new ModelString { String_test = Faker.Lorem.Sentence(10) });
            }
            list.Add(new ModelString { String_test = "_test" });

            db.Insert(list);

            Assert.Equal(200, db.Count<ModelString>());
        }

        [Fact, Order(5)]
        public void SelectAll_ModelString()
        {
            using var db = GetConnection();
            List<ModelString> list = db.Select<ModelString>();
            Assert.NotNull(list);
            Assert.Equal(200, list.Count);
        }

        [Fact, Order(6)]
        public void SelectByExpressionContains_ModelString()
        {
            using var db = GetConnection();
            List<ModelString> list = db.Select<ModelString>(x => x.String_test.Contains("_test"));
            Assert.NotNull(list);
            Assert.Equal(2, list.Count);
        }

        [Fact, Order(7)]
        public void SelectById_1_ModelString()
        {
            using var db = GetConnection();
            ModelString ms = db.SelectById<ModelString>(1);
            Assert.NotNull(ms);
            Assert.Equal("_test", ms.String_test);
        }

        [Fact, Order(8)]
        public void SelectFirst_Contains_test_ModelString()
        {
            using var db = GetConnection();
            ModelString ms = db.SelectFirst<ModelString>(x => x.String_test.Contains("_test"));
            ms = db.SelectFirst<ModelString>(x => x.String_test == "_test");
            Assert.NotNull(ms);
            Assert.Equal(1, ms.Id);
        }

        [Fact, Order(9)]
        public void SelectLast_Contains_mid_ModelString()
        {
            using var db = GetConnection();
            ModelString ms = db.SelectLast<ModelString>(x => x.String_test.Contains("_test"));
            Assert.NotNull(ms);
            Assert.Equal(200, ms.Id);
        }

        [Fact, Order(10)]
        public void Update_ModelString()
        {
            using var db = GetConnection();
            ModelString ms = db.SelectById<ModelString>(1);
            Assert.NotNull(ms);
            ms.String_test = "test_mid_end_updated";
            db.Update(ms);
            ModelString msu = db.SelectById<ModelString>(1);
            Assert.Equal("test_mid_end_updated", msu.String_test);
        }

        [Fact, Order(11)]
        public void SqlDirect_ModelString()
        {
            using var db = GetConnection();
            var ms = db.SelectFirst<ModelString>(x => x.String_test == "_test");
            Assert.NotNull(ms);
            db.SqlNonQuery($"UPDATE ModelString SET String_test = 'bbbb' WHERE Id = @p0", new SqlParameterItem("@p0", ms.Id));
            var ms2 = db.SelectById<ModelString>(ms.Id);
            Assert.Equal("bbbb", ms2.String_test);
        }

        [Fact, Order(11)]
        public void DeleteById_1_ModelString()
        {
            using var db = GetConnection();
            db.Delete<ModelString>(x => x.Id == 1);
            ModelString ms = db.SelectById<ModelString>(1);
            Assert.Null(ms);
        }

        [Fact, Order(12)]
        public void TruncateTable_ModelString()
        {
            using var db = GetConnection();
            db.TruncateTable<ModelString>();
            Assert.Equal(0, db.Count<ModelString>());
            Assert.True(db.TableExists<ModelString>());
        }

        [Fact, Order(13)]
        public void DropTable_ModelString()
        {
            using var db = GetConnection();
            db.DropTable<ModelString>();
            Assert.False(db.TableExists<ModelString>());
        }

        [Fact, Order(50)]
        public void Select_NoParameter()
        {
            using var db = GetConnection();

            if (db.TableExists<Model20Props>()) db.DropTable<Model20Props>();
            db.CreateTable<Model20Props>();

            db.Insert(new Model20Props { testbool = true });
            db.Insert(new Model20Props { testbool = false });

            var test = db.Select<Model20Props>(x => x.testbool);

            Assert.Single(test);

            db.DropTable<Model20Props>();
        }

        #endregion

        #region Unique
        [Fact, Order(14)]
        public void CreateTableCheckExists_ModelStringUnique_ShouldBeTrue()
        {
            using var db = GetConnection();
            if (db.TableExists<ModelStringUnique>()) db.DropTable<ModelStringUnique>();
            db.CreateTable<ModelStringUnique>();
            Assert.True(db.TableExists<ModelStringUnique>());
        }

        [Fact, Order(15)]
        public void InsertRandomData_ModelStringUnique()
        {
            using var db = GetConnection();

            db.Insert(new ModelStringUnique { String_unique = "test_1" });
            db.Insert(new ModelStringUnique { String_unique = "test_2" });

            Assert.Equal(2, db.Count<ModelStringUnique>());
        }

        [Fact, Order(16)]
        public void InsertUniqueData_ModelStringUnique()
        {
            using var db = GetConnection();

            Assert.Throws<System.Data.SQLite.SQLiteException>(() =>
            {
                db.Insert(new ModelStringUnique { String_unique = "test_2" });
            });
        }

        [Fact, Order(17)]
        public void DropTable_ModelStringUnique()
        {
            using var db = GetConnection();

            db.DropTable<ModelStringUnique>();
            Assert.False(db.TableExists<ModelStringUnique>());
        }

        [Fact, Order(18)]
        public void CreateTableCheckExists_ModelString_ShouldBeTrue_Unique()
        {
            using var db = GetConnection();
            if (db.TableExists<ModelString>()) db.DropTable<ModelString>();
            db.CreateTable<ModelString>();
            Assert.True(db.TableExists<ModelString>());
        }

        [Fact, Order(19)]
        public void AddColumnUnique_ModelString()
        {
            using var db = GetConnection();

            ColumnDefinition cd = new ColumnDefinition(typeof(ModelStringUnique), "String_unique");

            db.AddColumn(typeof(ModelString), cd);

            Assert.True(db.ColumnExists<ModelString>("String_unique"));
        }

        //[Fact, Order(20)]
        //public void InsertData_Unique_ModelString()
        //{
        //    using var db = GetConnection();

        //    db.Insert(new ModelStringUnique { String_unique = "test_1" });
        //    db.Insert(new ModelStringUnique { String_unique = "test_2" });

        //    Assert.Equal(2, db.Count<ModelStringUnique>());
        //}

        [Fact, Order(20)]
        public void DropColumnUnique_ModelString()
        {
            using var db = GetConnection();

            db.DropColumn(typeof(ModelString), "String_unique");

            Assert.False(db.ColumnExists<ModelString>("String_unique"));
        }

        #endregion

        #region ModelWithKeyworkName
        [Fact, Order(26)]
        public void ModelWithKeyworkName()
        {
            using var db = GetConnection();

            if (db.TableExists<Read>()) db.DropTable<Read>();

            db.CreateTable<Read>();

            db.Insert(new Read { int64 = 5 });

            var data = db.Select<Read>();

            var data2 = db.SelectById<Read>(1);

            data2.int64 = 9;

            db.Update(data2);

            db.DropColumn(typeof(Read), "Select");

            Assert.False(db.ColumnExists<Read>("Select"));

            db.AddColumn(typeof(Read), new ColumnDefinition(typeof(Read), "Select"));

            db.DeleteAll<Read>();

            Assert.Equal(0, db.Count<Read>());

            db.DropTable<Read>();

            Assert.False(db.TableExists<Read>());
        }
        #endregion

        #region ModelWithoutId
        [Fact, Order(27)]
        public void ModelWithoutId_CreateTable()
        {
            using var db = GetConnection();
            Assert.Throws<KeyNotFoundException>(() => { db.CreateTable<ModelWithoutId>(); });
        }
        #endregion

        #region ModelMultiplePK
        [Fact, Order(28)]
        public void ModelMultiplePK_CreateTable()
        {
            using var db = GetConnection();
            if (db.TableExists<ModelMultiplePK>()) db.DropTable<ModelMultiplePK>();
            db.CreateTable<ModelMultiplePK>();
            Assert.True(db.TableExists<ModelMultiplePK>());
        }

        [Fact, Order(29)]
        public void ModelMultiplePK_Insert()
        {
            using var db = GetConnection();

            db.Insert(new ModelMultiplePK { SecondKey = "1", value3 = "1" });

            Assert.Equal(1, db.Count<ModelMultiplePK>());
        }

        [Fact, Order(30)]
        public void ModelMultiplePK_Select()
        {
            using var db = GetConnection();

            ModelMultiplePK m = db.SelectFirst<ModelMultiplePK>(x => x.SecondKey == "1");

            Assert.NotNull(m);
        }

        [Fact, Order(31)]
        public void ModelMultiplePK_Update()
        {
            using var db = GetConnection();

            ModelMultiplePK m = db.SelectFirst<ModelMultiplePK>(x => x.SecondKey == "1");
            m.value3 = "2";
            db.Update(m);

            ModelMultiplePK m2 = db.SelectFirst<ModelMultiplePK>(x => x.value3 == "2");

            Assert.NotNull(m2);
        }

        [Fact, Order(32)]
        public void ModelMultiplePK_Delete()
        {
            using var db = GetConnection();

            ModelMultiplePK m = db.SelectById<ModelMultiplePK>(1);
            db.Delete(m);

            Assert.Equal(0, db.Count<ModelMultiplePK>());
        }

        [Fact, Order(33)]
        public void ModelMultiplePK_DropTable()
        {
            using var db = GetConnection();

            if (db.TableExists<ModelMultiplePK>()) db.DropTable<ModelMultiplePK>();

            Assert.False(db.TableExists<ModelMultiplePK>());
        }

        #endregion

        #region ModelPrivate
        [Fact, Order(34)]
        public void ModelPrivate()
        {
            using var db = GetConnection();

            if (db.TableExists<ModelPrivate>()) db.DropTable<ModelPrivate>();

            db.CreateTable<ModelPrivate>();

            db.Insert(new ModelPrivate(5, 6));

            var data = db.Select<ModelPrivate>();

            var data2 = db.SelectById<ModelPrivate>(1);

            data2.Double_public = 9;

            db.Update(data2);

            var data3 = db.Select<ModelPrivate>(x => x.Id == 1);

            Assert.Equal(9, data3.First().Double_public);

            Assert.NotNull(data3);

            db.DeleteAll<ModelPrivate>();

            db.DropTable<ModelPrivate>();

            Assert.False(db.TableExists<ModelPrivate>());
        }
        #endregion

        #region ModelPropRenamed
        [Fact, Order(35)]
        public void ModelPropRenamed()
        {
            using var db = GetConnection();

            if (db.TableExists<ModelPropRenamed>()) db.DropTable<ModelPropRenamed>();

            db.CreateTable<ModelPropRenamed>();

            Assert.True(db.ColumnExists<ModelPropRenamed>("Renamed"));

            db.Insert(new ModelPropRenamed { Str = "test" });

            var data = db.Select<ModelPropRenamed>();

            var data2 = db.Select<ModelPropRenamed>(x => x.Str == "test").FirstOrDefault();

            data2.Str = "updated";

            db.Update(data2);

            var data3 = db.Select<ModelPropRenamed>(x => x.Id == 1);

            Assert.NotNull(data3);

            db.DeleteAll<ModelPropRenamed>();

            db.DropTable<ModelPropRenamed>();

            Assert.False(db.TableExists<ModelPropRenamed>());
        }
        #endregion

        #region ModelTableRenamed
        [Fact, Order(36)]
        public void ModelTableRenamed()
        {
            using var db = GetConnection();

            if (db.TableExists<ModelTableRenamed>()) db.DropTable<ModelTableRenamed>();

            db.CreateTable<ModelTableRenamed>();

            db.Insert(new ModelTableRenamed { Str = "test" });

            var data = db.Select<ModelTableRenamed>();

            var data2 = db.Select<ModelTableRenamed>(x => x.Str == "test").FirstOrDefault();

            data2.Str = "updated";

            db.Update(data2);

            var data3 = db.Select<ModelTableRenamed>(x => x.Id == 1);

            Assert.NotNull(data3);

            db.DeleteAll<ModelTableRenamed>();

            db.DropTable<ModelTableRenamed>();

            Assert.False(db.TableExists<ModelTableRenamed>());
        }
        #endregion

        #region ExpressionParser
        [Fact, Order(37)]
        public void ExpressionParser()
        {
            using var db = GetConnection();
            if (db.TableExists<Model20Props>()) db.DropTable<Model20Props>();
            db.CreateTable<Model20Props>();

            Model20Props m = new Model20Props();
            m.str2 = "test!";
            m.dbl2 = 5;
            m.dt1 = DateTime.Now;
            m.dt2 = DateTime.Now;
            m.dt3 = DateTime.Now;
            //m.dt4 = DateTime.Now;
            db.Insert(m);

            //Test bool col
            var model = db.SelectFirst<Model20Props>(x => !x.testbool);
            Assert.NotNull(model);

            //Compare col with value
            model = db.SelectFirst<Model20Props>(x => x.dbl1 == 0);
            Assert.NotNull(model);

            //compare 2 cols
            model = db.SelectFirst<Model20Props>(x => x.dbl1 != x.dbl2);
            Assert.NotNull(model);

            //direct bool
            model = db.SelectFirst<Model20Props>(x => true);
            Assert.NotNull(model);

            //basic calc
            model = db.SelectFirst<Model20Props>(x => 1 == 1);
            Assert.NotNull(model);

            //precalc + compare with col
            model = db.SelectFirst<Model20Props>(x => x.dbl1 == (1 * 0));
            Assert.NotNull(model);

            //method call
            model = db.SelectFirst<Model20Props>(x => string.IsNullOrEmpty(x.str1));
            Assert.NotNull(model);

            //method call + compare with bool
            model = db.SelectFirst<Model20Props>(x => string.IsNullOrEmpty(x.str1) == true);
            Assert.NotNull(model);

            //method call + negation
            model = db.SelectFirst<Model20Props>(x => !string.IsNullOrEmpty(x.str2));
            Assert.NotNull(model);

            //method call
            model = db.SelectFirst<Model20Props>(x => x.str2.Contains("e"));
            Assert.NotNull(model);

            //method call
            model = db.SelectFirst<Model20Props>(x => x.str2.StartsWith("t"));
            Assert.NotNull(model);

            //method call
            model = db.SelectFirst<Model20Props>(x => x.str2.EndsWith("!"));
            Assert.NotNull(model);

            //double negation + method call
            model = db.SelectFirst<Model20Props>(x => !x.testbool && !string.IsNullOrEmpty(x.str2));
            Assert.NotNull(model);

            //double negation + method call inverse
            model = db.SelectFirst<Model20Props>(x => !string.IsNullOrEmpty(x.str2) && !x.testbool);
            Assert.NotNull(model);

            db.DropTable<Model20Props>();
        }
        #endregion

        #region NestedList
        [Fact, Order(40)]
        public void NestedList()
        {
            using var db = GetConnection();

            if (db.TableExists<ModelNestedParent>()) db.DropTable<ModelNestedParent>();
            if (db.TableExists<ModelNestedChild>()) db.DropTable<ModelNestedChild>();
            if (db.TableExists<Model20Props>()) db.DropTable<Model20Props>();

            db.CreateTable<ModelNestedParent>();
            db.CreateTable<ModelNestedChild>();
            db.CreateTable<Model20Props>();

            db.Insert(new ModelNestedParent { emptyProp = "", code_lib = "test_code_lib", code_list_str = "code3", id_child = 2 });

            //nested unique primitive
            db.Insert(new Model20Props { str1 = "test_code_lib", str2 = "libeuuuuuuh" });

            //nested list primitive
            db.Insert(new Model20Props { str3 = "code3", str4 = "code3_1" });
            db.Insert(new Model20Props { str3 = "code3", str4 = "code3_2" });

            //nested unique object
            //nested list object
            db.Insert(new ModelNestedChild { ParentId = 1, NestedValue = 1 });
            db.Insert(new ModelNestedChild { ParentId = 1, NestedValue = 2 });
            db.Insert(new ModelNestedChild { ParentId = 1, NestedValue = 3 });


            ModelNestedParent test_nested = db.Select<ModelNestedParent>().FirstOrDefault();

            Assert.True(test_nested.NestedTest != null && test_nested.NestedTest.Count == 3);
        }
        #endregion

        #region Migration
        [Fact, Order(50)]
        public void AutomaticMigration()
        {
            using var db = GetConnection();

            if (db.TableExists<Model20Props>()) db.DropTable<Model20Props>();
            db.CreateTable<Model20Props>();

            db.DropColumn(typeof(Model20Props), "str4");

            Assert.False(db.ColumnExists<Model20Props>("str4"));

            db.Migrate();

            Assert.True(db.ColumnExists<Model20Props>("str4"));
        }
        #endregion

        #region SchemaDifferences
        [Fact, Order(70)]
        public void Select_MissingColumn()
        {
            bool exception = false;
            using var db = GetConnection();
            db.CreateTable<Model20Props>();
            db.DropColumn(typeof(Model20Props), "testbool");

            try
            {
                _ = db.Select<Model20Props>();
            }
            catch (Exception)
            {
                exception = true;
            }

            db.DropTable<Model20Props>();

            Assert.False(exception);
        }

        #endregion





    }
}
