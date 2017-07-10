namespace MVCBackendIliyan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Galleries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Time = c.DateTime(nullable: false),
                        UserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Galleries", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Galleries", new[] { "UserId" });
            DropTable("dbo.Galleries");
        }
    }
}
