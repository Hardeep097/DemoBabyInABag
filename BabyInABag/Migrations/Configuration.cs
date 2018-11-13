namespace BabyInABag.Migrations
{
    using BabyInABag.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BabyInABag.Models.ApplicationDbContext>
    {

        DateTime value = new DateTime(2017, 1, 18);
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BabyInABag.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            context.Orders.AddOrUpdate(
                o => o.Order_Id,
               new Order { Order_Id = 1, Order_Date_Placed = value, Order_Status = order_status.Ongoing, Id = "b97df479-4318-4e27-b06c-d3a050e9e414" , cartQuantity="1,2,3"},
               new Order { Order_Id = 2, Order_Date_Placed = value, Order_Status = order_status.Ongoing, Id = "b97df479-4318-4e27-b06c-d3a050e9e414", cartQuantity="1,3,2" }
               );

            context.Products.AddOrUpdate(
                p => p.Product_Id,
                new Product { Product_Id = 1, Product_Description = "Blanket Bag white strip", Product_Name = "Blanket Bag", Product_Price = 55, Size = "Small", Active = true, Product_Image = "root", Product_Category_Id = 1 }
                );

            context.ProductCategories.AddOrUpdate(
                pc => pc.Product_Category_Id,
                new ProductCategory { Product_Category_Id = 1, Product_Category = "Blanket", Default_Image = "/images/blueb180237469.jpg", Default_Price = 50 },
                new ProductCategory { Product_Category_Id = 2, Product_Category = "Boots", Default_Image = "/images/bubblesprite180347444.jpg", Default_Price = 40 },
                new ProductCategory { Product_Category_Id = 3, Product_Category = "Sleeping Bag", Default_Image = "/images/bluesky180530225.jpg", Default_Price = 60 }
                );
        }
    }
}
