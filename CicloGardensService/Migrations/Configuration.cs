using Microsoft.Azure.Mobile.Server.Tables;

namespace CicloGardensService.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<Models.CicloGardensContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            //this special generator is necessary if running update-database from the console. 
            //without it, the clustered index for the 'CreatedAt' field would lead to an exception from sql server
            //for some reason, this behaves different when running automatic migrations on startup from the webapp
            SetSqlGenerator("System.Data.SqlClient", new EntityTableSqlGenerator());
        }

        protected override void Seed(CicloGardensService.Models.CicloGardensContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
