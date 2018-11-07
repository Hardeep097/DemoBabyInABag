namespace BabyInABag.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class name1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AspNetUsers", new[] { "LastName" });
            CreateIndex("dbo.AspNetUsers", "LastName");
            CreateIndex("dbo.AspNetUsers", "UserName");
        }
        
        public override void Down()
        {
            DropIndex("dbo.AspNetUsers", new[] { "UserName" });
            DropIndex("dbo.AspNetUsers", new[] { "LastName" });
            CreateIndex("dbo.AspNetUsers", "LastName", unique: true);
        }
    }
}
