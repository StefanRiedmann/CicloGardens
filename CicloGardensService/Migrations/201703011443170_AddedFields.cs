namespace CicloGardensService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Gardens", "Latitude", c => c.Double(nullable: false));
            AddColumn("dbo.Gardens", "Longitude", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Gardens", "Longitude");
            DropColumn("dbo.Gardens", "Latitude");
        }
    }
}
