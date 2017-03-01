namespace CicloGardensService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CoordsNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Gardens", "Latitude", c => c.Double());
            AlterColumn("dbo.Gardens", "Longitude", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Gardens", "Longitude", c => c.Double(nullable: false));
            AlterColumn("dbo.Gardens", "Latitude", c => c.Double(nullable: false));
        }
    }
}
