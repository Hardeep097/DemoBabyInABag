namespace BabyInABag.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class name : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.AspNetUsers", "FirstName");
            CreateIndex("dbo.AspNetUsers", "LastName", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.AspNetUsers", new[] { "LastName" });
            DropIndex("dbo.AspNetUsers", new[] { "FirstName" });
        }
    }
}
