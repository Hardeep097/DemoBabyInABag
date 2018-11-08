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
    public class ProductCategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProductCategories
        public ActionResult Index()
        {
            return View(db.ProductCategories.ToList());
        }

        // GET: ProductCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategory productCategory = db.ProductCategories.Find(id);
            if (productCategory == null)
            {
                return HttpNotFound();
            }
            return View(productCategory);
        }

        // GET: ProductCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Product_Category_Id,Product_Category,Default_Image,Default_Price, ImageFile")] ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                string fileName = Path.GetFileNameWithoutExtension(productCategory.ImageFile.FileName);
                string extension = Path.GetExtension(productCategory.ImageFile.FileName);

                if (extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) || extension.Equals(".png", StringComparison.OrdinalIgnoreCase) || extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    productCategory.Default_Image = "/images/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/images/"), fileName);
                    productCategory.ImageFile.SaveAs(fileName);

                    using (db)
                    {
                        db.ProductCategories.Add(productCategory);
                        db.SaveChanges();
                        ViewBag.success = "Created!";
                    }
                }
                else
                {
                    String error = "You may only submit JPG or PNG";
                    ViewBag.errorcreateimage = error;
                    ViewBag.success = "Failed!";
                }
                ModelState.Clear();

                //return RedirectToAction("Index");

            }

           
            return View();
        }

        // GET: ProductCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategory productCategory = db.ProductCategories.Find(id);
            TempData["Categoryimage"] = productCategory.Default_Image;
            if (productCategory == null)
            {
                return HttpNotFound();
            }
            return View(productCategory);
        }

        // POST: ProductCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Product_Category_Id,Product_Category,Default_Image,Default_Price,ImageFile")] ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                if (productCategory.ImageFile == null)
                {
                    productCategory.Default_Image = TempData["Categoryimage"].ToString();
                    db.Entry(productCategory).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    string fileName = Path.GetFileNameWithoutExtension(productCategory.ImageFile.FileName);
                    string extension = Path.GetExtension(productCategory.ImageFile.FileName);

                    if (extension.Equals(".jpg") || extension.Equals(".png") || extension.Equals(".jpeg"))
                    {
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        productCategory.Default_Image = "/images/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/images/"), fileName);
                        productCategory.ImageFile.SaveAs(fileName);

                        using (db)
                        {
                            db.Entry(productCategory).State = EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        String error = "You may only submit JPG or PNG";
                        ViewBag.errorimage = error;
                    }

                }
                
            }
            //model valid ends

            return View(productCategory);
        }

        // GET: ProductCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategory productCategory = db.ProductCategories.Find(id);
            if (productCategory == null)
            {
                return HttpNotFound();
            }
            return View(productCategory);
        }

        // POST: ProductCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductCategory productCategory = db.ProductCategories.Find(id);
            db.ProductCategories.Remove(productCategory);
            db.SaveChanges();
            return RedirectToAction("Index");
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
