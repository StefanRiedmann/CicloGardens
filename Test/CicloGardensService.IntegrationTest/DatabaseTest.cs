using System;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CicloGardensService.IntegrationTest
{
    [TestClass]
    public class DatabaseTest
    {
        [TestMethod]
        public void MigrationsAreUpToDate()
        {
            var migrationsConfiguration = new Migrations.Configuration();
            var migrator = new DbMigrator(migrationsConfiguration);

            var migs = migrator.GetDatabaseMigrations().ToList();
            var localMigs = migrator.GetLocalMigrations().ToList();
            var pendingMigs = migrator.GetPendingMigrations().ToList();

            CollectionAssert.AreEqual(migs, localMigs);
            Assert.AreEqual(0, pendingMigs.Count);
        }
    }
}
