using BabyInABag.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BabyInABag.Models.VMs;
using System.Text.RegularExpressions;


namespace BabyInABag.Controllers
{
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [Authorize(Roles = "Admin")]
        public ActionResult Index(){ return View(db.Orders.ToList()); }

       


        public ActionResult Edit(int? ids)
        {
            if (ids == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(ids);
            string list = order.cartQuantity;
            order.productListing = list.Split('|');
            if (order == null)
            {
                return HttpNotFound();
            }
            
            return View(order);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Order_Id,Order_Date_Placed,Order_Status,Order_Details,Order_Date_Paid,Invoice_Status,Id,Shipping_Address,Order_Total,Order_Number,Full_Name,cartQuantity")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerId = new SelectList(db.Users, "Customer_Id", "First_Name", order.Id);
            return View(order);
        }

        [Authorize(Roles = "Customer")]
        public ActionResult Checkout()
        {
            if (Session["cart"] == null)
            {
                ViewBag.empty = "Your cart is empty";
            }
            else
            {
                List<CartItem> currentCart = (List<CartItem>)Session["cart"];
                List<Product> activeCart = new List<Product>();
                List<Product> products = new List<Product>();
                decimal subtotalPrice = 0;
                int subtotalAmount = 0;

                products = db.Products.ToList();

                for (int c = 0; c < products.Count; c++)
                {
                    for (int d = 0; d < currentCart.Count; d++)
                    {
                        if (products[c].Product_Id.Equals(currentCart[d].ProductID))
                        {
                            products[c].Quantity = currentCart[d].Quantity;
                            activeCart.Add(products[c]);

                            for (int i = 0; i < products[c].Quantity; i++)
                            {
                                subtotalPrice += products[c].Product_Price;
                                subtotalAmount++;
                            }

                        }
                    }
                }
                ViewBag.Subtotal = "Subtotal (" + subtotalAmount + " item): CDN$ " + subtotalPrice;
                return View(activeCart);
            }
            return View();
        }

        [Authorize(Roles = "Customer")]
        public ActionResult CustomerOrders()
        {
            //Pull Customer Id from the Session
            String customer_id = Session["currentid"].ToString();

            //Creates 2 lists to contain all orders, and orders filtered down to customer
            List<Order> orders = db.Orders.ToList();
            List<Order> customer_orders = new List<Order>();

            for(int i = 0; i < orders.Count; i++)
            {
                if(orders[i].Id == customer_id)
                {
                    //if order belongs to customer, add to list
                    customer_orders.Add(orders[i]);
                }
            }

            return View(customer_orders);
        }

        [Authorize(Roles ="Customer")]
        public ActionResult CustomerOrderDetails(int? ids)
        {

            if (ids == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            //Pull Customer Id from the Session
            String customer_id = Session["currentid"].ToString();

            Order order = db.Orders.Find(ids);

            if (order == null)
            {
                return HttpNotFound();
            }

            if(order.Id == customer_id)
            {
                return View(order);
            }
            else
            {
                ViewBag.Scare = "Hey! Thats not your order! Your IP address has been logged.";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void Create(Order order)
        {
            string username = (string)Session["username"];
            string customer_id = null;
            List<ApplicationUser> customers = db.Users.ToList();
            for (int i = 0; i < customers.Count; i++)
            {
                if (customers[i].UserName == username)
                {
                    customer_id = customers[i].Id;
                }
            }

            order.Id = customer_id;
            order.cartQuantity = "";
            order.Products = GetCartProducts(order);
            order.Order_Status = order_status.Submitted;
            var timeUtc = DateTime.UtcNow;
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, timeZoneInfo);
            order.Order_Date_Placed = currentTime;

            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
            }
        }

        public string GenerateOrderNumber()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 10).Select(s => s [random.Next(s.Length)]).ToArray());
        }

        public ActionResult GetPayPalData()
        {
            //POST Response to PayPal with TX token, pull Order Response for SUCCESS or FAIL payment
            var getData = new GetPayPalData();
            string response = getData.GetPayPalResponse(Request.QueryString["tx"]);

            Regex payment_success_rgx = new Regex(@"SUCCESS");
            if (payment_success_rgx.Match(response).ToString().Equals("SUCCESS"))
            {

                //Regex to Parse through decoded response
                Regex address_name_rgx = new Regex(@"(?<=address_name=).*?(?=\s)");
                Regex address_street_rgx = new Regex(@"(?<=address_street=).*?(?=\s)");
                Regex address_city_rgx = new Regex(@"(?<=address_city=).*?(?=\s)");
                Regex address_country_rgx = new Regex(@"(?<=address_country=).*?(?=\s)");
                Regex address_state_rgx = new Regex(@"(?<=address_state=).*?(?=\s)");
                Regex address_zip_rgx = new Regex(@"(?<=address_zip=).*?(?=\s)");
                Regex payment_gross_rgx = new Regex(@"(?<=payment_gross=).*?(?=\s)");

                //Parsed Fields being Decoded
                string address_street_decoded = HttpUtility.UrlDecode(address_street_rgx.Match(response).ToString());
                string address_name_decoded = HttpUtility.UrlDecode(address_name_rgx.Match(response).ToString());
                string address_city_decoded = HttpUtility.UrlDecode(address_city_rgx.Match(response).ToString());
                string address_country_decoded = HttpUtility.UrlDecode(address_country_rgx.Match(response).ToString());
                string address_state_decoded = HttpUtility.UrlDecode(address_state_rgx.Match(response).ToString());
                string address_zip_decoded = HttpUtility.UrlDecode(address_zip_rgx.Match(response).ToString());
                string payment_gross_decoded = HttpUtility.UrlDecode(payment_gross_rgx.Match(response).ToString());


                String shipping_address = address_street_decoded + "\n" +
                                          address_city_decoded + ", " +
                                          address_state_decoded + "\n" +
                                          address_country_decoded + ", " +
                                          address_zip_decoded;

                Order order = new Order
                {
                    Shipping_Address = shipping_address,
                    Full_Name = address_name_decoded,
                    Order_Total = Convert.ToDecimal(payment_gross_decoded),
                    Order_Number = GenerateOrderNumber()
                };

                Create(order);
                ViewBag.OrderNumber = order.Order_Number;

                ViewBag.status_message = "Your Payment was Successful!";

            }
            else
            {
                ViewBag.status_message = "Your Payment didnt go through!";
            }

            return View();
        }

        public List<Product> GetCartProducts(Order order)
        {
            List<CartItem> currentCart = (List<CartItem>)Session["cart"];
            List<Product> activeCart = new List<Product>();
            List<Product> products = new List<Product>();

            products = db.Products.ToList();

            for (int c = 0; c < products.Count; c++)
            {
                for (int d = 0; d < currentCart.Count; d++)
                {
                    if (products[c].Product_Id.Equals(currentCart[d].ProductID))
                    {
                        activeCart.Add(products[c]);
                        order.cartQuantity = order.cartQuantity + products[c].Product_Id + ":" + currentCart[d].Quantity + "|";
                    }
                }
            }

            return activeCart;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }


}