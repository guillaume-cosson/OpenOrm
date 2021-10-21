using OpenOrm;
using OpenOrm.Configuration;
using OpenOrm.SqlServer;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Extensions.Ordering;
using XUnitTests.Models;

namespace XUnitTests
{
    public class PerfSqlServer
    {
        #region init
        public static OpenOrmDbConnection GetConnection()
        {
            return new OpenOrmConfiguration
            {
                ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=OpenOrmXUnit;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False",
                Connector = Connector.SqlServer,
                PrintSqlQueries = false,
                PutIdFieldAtFirstPosition = true,
                Schema = "dbo",
                MapPrivateProperties = true,
                ListInsertAllowBulkInsert = true,
                EnableRamCache = true
            }.GetConnection();
        }
        #endregion


        //#region Perf Insert
        //[Fact, Order(20)]
        //public void _01_PerfCreateDropTable_500_Model20Props()
        //{
        //    using var db = GetConnection();

        //    if (db.TableExists<Model20Props>()) db.DropTable<Model20Props>();

        //    for (int i = 0; i < 500; i++)
        //    {
        //        db.CreateTable<Model20Props>();
        //        db.DropTable<Model20Props>();
        //    }

        //    db.CreateTable<Model20Props>();

        //    Assert.True(db.TableExists<Model20Props>());
        //}

        //[Fact, Order(21)]
        //public void _02_PerfInsert_10k_Model20Props()
        //{
        //    using var db = GetConnection();
        //    Random rnd = new Random();

        //    for (int i = 0; i < 10000; i++)
        //    {
        //        db.Insert(new Model20Props
        //        {
        //            dec1 = rnd.Next(0, 100000),
        //            dec2 = rnd.Next(0, 100000),
        //            dec3 = rnd.Next(0, 100000),
        //            dec4 = rnd.Next(0, 100000),
        //            str1 = Guid.NewGuid().ToString(),
        //            str2 = Guid.NewGuid().ToString(),
        //            str3 = Guid.NewGuid().ToString(),
        //            str4 = Guid.NewGuid().ToString(),
        //            dbl1 = rnd.Next(0, 100000),
        //            dbl2 = rnd.Next(0, 100000),
        //            dbl3 = rnd.Next(0, 100000),
        //            dbl4 = rnd.Next(0, 100000),
        //            dt1 = DateTime.Now,
        //            dt2 = DateTime.Now,
        //            dt3 = DateTime.Now,
        //            dt4 = DateTime.Now
        //        });
        //    }

        //    Assert.Equal(10000, db.Count<Model20Props>());
        //}

        //[Fact, Order(22)]
        //public void _03_PerfInsertList_10k_Model20Props()
        //{
        //    using var db = GetConnection();
        //    Random rnd = new Random();

        //    db.TruncateTable<Model20Props>();

        //    List<Model20Props> lst = new List<Model20Props>(10000);

        //    for (int i = 0; i < 10000; i++)
        //    {
        //        lst.Add(new Model20Props
        //        {
        //            dec1 = rnd.Next(0, 100000),
        //            dec2 = rnd.Next(0, 100000),
        //            dec3 = rnd.Next(0, 100000),
        //            dec4 = rnd.Next(0, 100000),
        //            str1 = Guid.NewGuid().ToString(),
        //            str2 = Guid.NewGuid().ToString(),
        //            str3 = Guid.NewGuid().ToString(),
        //            str4 = Guid.NewGuid().ToString(),
        //            dbl1 = rnd.Next(0, 100000),
        //            dbl2 = rnd.Next(0, 100000),
        //            dbl3 = rnd.Next(0, 100000),
        //            dbl4 = rnd.Next(0, 100000),
        //            dt1 = DateTime.Now,
        //            dt2 = DateTime.Now,
        //            dt3 = DateTime.Now,
        //            dt4 = DateTime.Now
        //        });
        //    }

        //    db.Insert(lst);

        //    Assert.Equal(10000, db.Count<Model20Props>());
        //}
        //#endregion

        //[Fact, Order(24)]
        //public void _04_PerfSelectAll_10k_Model20Props()
        //{
        //    using var db = GetConnection();


        //    var data = db.Select<Model20Props>();
        //    data = null;


        //    Assert.Equal(10000, db.Count<Model20Props>());
        //}

        //[Fact, Order(24)]
        //public void _05_PerfSelectAll_20x10k_Model20Props()
        //{
        //    using var db = GetConnection();

        //    for (int i = 0; i < 20; i++)
        //    {
        //        var data = db.Select<Model20Props>();
        //    }

        //    Assert.Equal(10000, db.Count<Model20Props>());
        //}

        //[Fact, Order(25)]
        //public void _06_PerfUpdateAll_10k_Model20Props()
        //{
        //    using var db = GetConnection();

        //    var data = db.Select<Model20Props>();

        //    foreach (Model20Props model in data)
        //    {
        //        db.Update(model);
        //    }

        //    Assert.Equal(10000, db.Count<Model20Props>());
        //}

        //[Fact, Order(25)]
        //public void _07_PerfUpdateAllList_10k_Model20Props()
        //{
        //    using var db = GetConnection();

        //    var data = db.Select<Model20Props>();

        //    db.Update(data);

        //    Assert.Equal(10000, db.Count<Model20Props>());
        //}

        //[Fact, Order(26)]
        //public void _08_PerfDeleteAll_10k_Model20Props()
        //{
        //    using var db = GetConnection();

        //    var data = db.Select<Model20Props>();

        //    foreach (Model20Props model in data)
        //    {
        //        db.Delete(model);
        //    }

        //    Assert.Equal(0, db.Count<Model20Props>());
        //}

        //[Fact, Order(27)]
        //public void _09_PerfInsertList_10k_Model20Props_bis()
        //{
        //    using var db = GetConnection();
        //    Random rnd = new Random();

        //    db.TruncateTable<Model20Props>();

        //    List<Model20Props> lst = new List<Model20Props>(10000);

        //    for (int i = 0; i < 10000; i++)
        //    {
        //        lst.Add(new Model20Props
        //        {
        //            dec1 = rnd.Next(0, 100000),
        //            dec2 = rnd.Next(0, 100000),
        //            dec3 = rnd.Next(0, 100000),
        //            dec4 = rnd.Next(0, 100000),
        //            str1 = Guid.NewGuid().ToString(),
        //            str2 = Guid.NewGuid().ToString(),
        //            str3 = Guid.NewGuid().ToString(),
        //            str4 = Guid.NewGuid().ToString(),
        //            dbl1 = rnd.Next(0, 100000),
        //            dbl2 = rnd.Next(0, 100000),
        //            dbl3 = rnd.Next(0, 100000),
        //            dbl4 = rnd.Next(0, 100000),
        //            dt1 = DateTime.Now,
        //            dt2 = DateTime.Now,
        //            dt3 = DateTime.Now,
        //            dt4 = DateTime.Now
        //        });
        //    }

        //    db.Insert(lst);

        //    var data = db.Select<Model20Props>();

        //    Assert.Equal(10000, db.Count<Model20Props>());
        //}

        //[Fact, Order(28)]
        //public void _10_PerfDeleteAllList_10k_Model20Props()
        //{
        //    using var db = GetConnection();

        //    var data = db.Select<Model20Props>();

        //    db.Delete(data);

        //    Assert.Equal(0, db.Count<Model20Props>());

        //    db.DropTable<Model20Props>();
        //}


        ////[Fact, Order(29)]
        ////public void _11_PerfInsertList_1M_Model20Props()
        ////{
        ////    using var db = GetConnection();
        ////    Random rnd = new Random();

        ////    db.CreateTable<Model20Props>();

        ////    List<Model20Props> lst = new List<Model20Props>(1000000);

        ////    for (int i = 0; i < 1000000; i++)
        ////    {
        ////        lst.Add(new Model20Props
        ////        {
        ////            dec1 = rnd.Next(0, 100000),
        ////            dec2 = rnd.Next(0, 100000),
        ////            dec3 = rnd.Next(0, 100000),
        ////            dec4 = rnd.Next(0, 100000),
        ////            str1 = Guid.NewGuid().ToString(),
        ////            str2 = Guid.NewGuid().ToString(),
        ////            str3 = Guid.NewGuid().ToString(),
        ////            str4 = Guid.NewGuid().ToString(),
        ////            dbl1 = rnd.Next(0, 100000),
        ////            dbl2 = rnd.Next(0, 100000),
        ////            dbl3 = rnd.Next(0, 100000),
        ////            dbl4 = rnd.Next(0, 100000),
        ////            dt1 = DateTime.Now,
        ////            dt2 = DateTime.Now,
        ////            dt3 = DateTime.Now,
        ////            dt4 = DateTime.Now
        ////        });
        ////    }

        ////    db.Insert(lst);

        ////    var data = db.Select<Model20Props>();

        ////    Assert.Equal(1000000, db.Count<Model20Props>());
        ////}

        ////[Fact, Order(30)]
        ////public void _12_PerfSelectAll_1M_Model20Props()
        ////{
        ////    using var db = GetConnection();

        ////    var data = db.Select<Model20Props>();

        ////    Assert.Equal(1000000, db.Count<Model20Props>());

        ////    db.DropTable<Model20Props>();
        ////}


    }
}
