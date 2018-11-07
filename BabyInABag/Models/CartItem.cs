using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BabyInABag.Models
{
    public class CartItem
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }

        public CartItem(int id, int quantity)
        {
            ProductID = id;
            Quantity = quantity;

        }

        public CartItem()
        {

        }
    }
}