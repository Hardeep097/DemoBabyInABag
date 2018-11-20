using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BabyInABag.Models
{
    public class ProductCategory
    {

        [Key]
        public int Product_Category_Id { get; set; }

        [Display(Name = "Category Name")]
        public string Product_Category { get; set; }

        [Display(Name = "Image")]
        public string Default_Image { get; set; }

        [Display(Name = "Price")]
        public decimal Default_Price { get; set; }

        
        public bool Active { get; set; }

        [NotMapped]
        public HttpPostedFileBase ImageFile { get; set; }

        [NotMapped]
        public List<ProductCategory> CategoryCollection { get; set; }
    }
}