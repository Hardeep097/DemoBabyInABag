using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BabyInABag.Models;

namespace BabyInABag.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        List<CartItem> cart = new List<CartItem>();

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.Product_Category_Id = new SelectList(db.ProductCategories, "Product_Category_Id", "Product_Category");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Product_Id,Product_Name,Product_Price,Product_Description,Product_Image")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Product_Category_Id = new SelectList(db.ProductCategories, "Product_Category_Id", "Product_Category", product.Product_Category_Id);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Product product = db.Products.Find(id);

            Product product = db.Products.Find(id);
            TempData["imagePath"] = product.Product_Image;

            if (product == null)
            {
                return HttpNotFound();
            }
            var categories = db.ProductCategories.ToList();

            if (categories != null)
            {
                ViewBag.data = categories;
            }

            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Product_Id,Product_Name,Product_Price,Product_Description,Product_Image,Active,Size,Product_Category_Id,ImageFile")] Product product)
        {

            if (product.ImageFile == null)
            {
                product.Product_Image = TempData["imagePath"].ToString();
                using (db)
                {
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("ManageProducts", "Admin", new { area = "" });
                }
            }
            else
            {
                string fileName = Path.GetFileNameWithoutExtension(product.ImageFile.FileName);
                string extension = Path.GetExtension(product.ImageFile.FileName);

                if (extension.Equals(".jpg") || extension.Equals(".png") || extension.Equals(".jpeg"))
                {
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    product.Product_Image = "/images/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/images/"), fileName);
                    product.ImageFile.SaveAs(fileName);

                    using (db)
                    {
                        db.Entry(product).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("ManageProducts", "Admin", new { area = "" });
                    }
                }
                else
                {
                    String error = "You may only submit JPG or PNG";
                    ViewBag.error = error;
                }
            }
            //ModelState.Clear();

            return View(product);
        }

        public ActionResult Products()
        {

            var categories = db.ProductCategories.ToList();
            if (categories != null)
            {
                ViewBag.data = categories;
            }


            return View(db.Products.ToList());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCart(FormCollection frm, int? product_id, int? product_quan)
        {

            int id;
            int quan;

            if (product_id == null)
            {
                //Grab Product_ID and Quantity from FormCollection
                id = Int32.Parse(frm["pid"]);
                quan = Int32.Parse(frm["quantity"]);
            }
            else
            {
                id = (int)product_id;
                quan = (int)product_quan;
            }


            //Check if Session Username is Empty, Redirect to Login Page if Empty
            if (Session["username"] == null)
            {
               
                return RedirectToAction("Login", "Account", null); }

            //Check if the Temporary Cart is Empty, If YES do Below
            if (TempData["cart"] == null)
            {
                //Create CartItem containing the Product_Id and Quantity
                CartItem item = new CartItem((int)id, (int)quan);
                //Add the Cart Item to the Cart
                cart.Add(item);
                //Add the Cart to the Temporary Data
                TempData["cart"] = cart;
                //Add the Cart to the Session Data
                Session["cart"] = cart;
                //Redirect  to Cart Page after Product has been added
                return RedirectToAction("Cart", "Cart", null);
            }
            //If Temporary Cart is not empty
            else
            {
                //Create a Temp Cart to hold products currently in session
                List<CartItem> tempCart = TempData["cart"] as List<CartItem>;
                //Loop through the Temp cart that was just created
                for (int i = 0; i < tempCart.Count; i++)
                {
                    //If the Temp cart at [i]'s Product ID is equal to the Product_ID passed from the form
                    if (tempCart[i].ProductID == id)
                    {
                        tempCart[i].Quantity += quan;
                        cart = tempCart;
                        Session["cart"] = cart;
                        return RedirectToAction("Cart", "Cart", null);
                    }
                }
                CartItem item = new CartItem((int)id, (int)quan);
                cart = TempData["cart"] as List<CartItem>;
                cart.Add(item);
                Session["cart"] = cart;
                return RedirectToAction("Cart", "Cart", null);
            }
        }

        //Customize section
        public ActionResult CustomizeProduct(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProductCategory pc = db.ProductCategories.Find(id);

            return View(pc);
        }

        [HttpPost]
        public ActionResult CustomizeProduct(ProductCategory pcat, FormCollection frm)
        {
            //Product Creataed out of Customize product page
            Product product = new Product
            {
                Product_Name = "Custom: " + pcat.Product_Category,
                Product_Description = "This is a custom made product made by customer " + Session["username"],
                Product_Price = pcat.Default_Price,
                Product_Category_Id = pcat.Product_Category_Id,
                Active = false,
                Knit_Type = frm["knit"],
                Color = frm["color"],
                Product_Image = pcat.Default_Image,
                Quantity = Int32.Parse(frm["quantity"])
            };
            if (frm["additionalRequirements"] != "")
            {
                product.AdditionalRequirements = frm["additionalRequirements"];
            }

            using (db)
            {
                db.Products.Add(product);
                db.SaveChanges();
            }

            AddToCart(null, product.Product_Id, product.Quantity);
            return RedirectToAction("Cart", "Cart", null);
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
