namespace eUp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.User",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Email = c.String(),
                        Phone = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.UserTables",
                c => new
                    {
                        UserTableId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        TableName = c.String(),
                    })
                .PrimaryKey(t => t.UserTableId)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Fields",
                c => new
                    {
                        FieldId = c.Int(nullable: false, identity: true),
                        UserTableId = c.Int(nullable: false),
                        FieldName = c.String(),
                        FieldType = c.String(),
                    })
                .PrimaryKey(t => t.FieldId)
                .ForeignKey("dbo.UserTables", t => t.UserTableId, cascadeDelete: true)
                .Index(t => t.UserTableId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Fields", new[] { "UserTableId" });
            DropIndex("dbo.UserTables", new[] { "UserId" });
            DropForeignKey("dbo.Fields", "UserTableId", "dbo.UserTables");
            DropForeignKey("dbo.UserTables", "UserId", "dbo.User");
            DropTable("dbo.Fields");
            DropTable("dbo.UserTables");
            DropTable("dbo.User");
        }
    }
}
