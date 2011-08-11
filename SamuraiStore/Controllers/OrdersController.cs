using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SamuraiStore.Models;

namespace SamuraiStore.Controllers
{ 
    public class OrdersController : Controller
    {
        private StoreContext db = new StoreContext();

        //
        // GET: /Orders/

        public ViewResult Index()
        {
            var orders = db.Orders.Include(o => o.Thing);
            return View(orders.ToList());
        }

        //
        // GET: /Orders/Details/5

        public ViewResult Details(int id)
        {
            Order order = db.Orders.Find(id);
            return View(order);
        }

        //
        // GET: /Orders/Create

        public ActionResult Create()
        {
            ViewBag.ThingId = new SelectList(db.Things, "ThingId", "Name");
            return View();
        } 

        //
        // POST: /Orders/Create

        [HttpPost]
        public ActionResult Create(Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.ThingId = new SelectList(db.Things, "ThingId", "Name", order.ThingId);
            return View(order);
        }
        
        //
        // GET: /Orders/Edit/5
 
        public ActionResult Edit(int id)
        {
            Order order = db.Orders.Find(id);
            ViewBag.ThingId = new SelectList(db.Things, "ThingId", "Name", order.ThingId);
            return View(order);
        }

        //
        // POST: /Orders/Edit/5

        [HttpPost]
        public ActionResult Edit(Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ThingId = new SelectList(db.Things, "ThingId", "Name", order.ThingId);
            return View(order);
        }

        //
        // GET: /Orders/Delete/5
 
        public ActionResult Delete(int id)
        {
            Order order = db.Orders.Find(id);
            return View(order);
        }

        //
        // POST: /Orders/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}