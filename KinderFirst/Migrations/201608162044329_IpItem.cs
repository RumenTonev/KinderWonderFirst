namespace KinderFirst.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IpItem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IpItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IP = c.String(),
                        IteamId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.IpItems");
        }
    }
}
