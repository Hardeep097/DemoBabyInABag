namespace BabyInABag.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderupdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Order_Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Orders", "Full_Name", c => c.String());
            DropColumn("dbo.Orders", "Order_Details");
            DropColumn("dbo.Orders", "Order_Date_Paid");
            DropColumn("dbo.Orders", "Invoice_Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "Invoice_Status", c => c.String());
            AddColumn("dbo.Orders", "Order_Date_Paid", c => c.DateTime(nullable: false));
            AddColumn("dbo.Orders", "Order_Details", c => c.String());
            DropColumn("dbo.Orders", "Full_Name");
            DropColumn("dbo.Orders", "Order_Total");
        }
    }
}
