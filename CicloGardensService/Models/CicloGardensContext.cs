using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Tables;
using CicloGardensService.DataObjects;

namespace CicloGardensService.Models
{
    public class CicloGardensContext : DbContext
    {
        private const string connectionStringName = "Name=MS_TableConnectionString";

        public CicloGardensContext() : base(connectionStringName)
        {
        } 

        public DbSet<Garden> Garden{ get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add(
                new AttributeToColumnAnnotationConvention<TableColumnAttribute, string>(
                    "ServiceTableColumn", (property, attributes) => attributes.Single().ColumnType.ToString()));
        }
    }

}
