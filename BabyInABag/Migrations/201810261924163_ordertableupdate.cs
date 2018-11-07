namespace BabyInABag.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ordertableupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Shipping_Address", c => c.String());
            AddColumn("dbo.Orders", "Invoice_Status", c => c.String());
            AddColumn("dbo.Orders", "Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Orders", "Order_Status", c => c.Int(nullable: false));
            CreateIndex("dbo.Orders", "Id");
            AddForeignKey("dbo.Orders", "Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "Id", "dbo.AspNetUsers");
            DropIndex("dbo.Orders", new[] { "Id" });
            AlterColumn("dbo.Orders", "Order_Status", c => c.String());
            DropColumn("dbo.Orders", "Id");
            DropColumn("dbo.Orders", "Invoice_Status");
            DropColumn("dbo.Orders", "Shipping_Address");
        }
    }
}
