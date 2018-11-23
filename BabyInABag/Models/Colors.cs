using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BabyInABag.Models
{
    public class Colors
    {

        [Key]
        public int Color_Id { get; set; }

        [Display(Name = "Color Name")]
        public string Color_Name { get; set; }

        [Display(Name = "Color Hex")]
        public string Color_HEX { get; set; }
    }
}