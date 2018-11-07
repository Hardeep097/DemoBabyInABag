using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BabyInABag.Models
{
    public class Product
    {

        [Key]
        public int Product_Id { get; set; }
        [Display(Name = "Name")]
        public string Product_Name { get; set; }
        [Display(Name = "Price")]
        public decimal Product_Price { get; set; }
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Product_Description { get; set; }
        [Display(Name = "Image")]
        public string Product_Image { get; set; }
        public bool Active { get; set; }

        public int Quantity { get; set; }
        public string Size { get; set; }
        [Display(Name = "Category")]
        public int Product_Category_Id { get; set; }
        public string Knit_Type { get; set; }
        public string Color { get; set; }

        public string AdditionalRequirements { get; set; }
        [NotMapped]
        public HttpPostedFileBase ImageFile { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        [ForeignKey("Product_Category_Id")]
        public virtual ProductCategory PCT { get; set; }
    }
}