using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SamuraiStore.Models;
using Samurai;

namespace SamuraiStore.Controllers
{ 
    public class OrdersController : Controller
    {
        private StoreContext db = new StoreContext();

        //
        // GET: /Orders/

        public ViewResult Index()
        {
            var orders = db.Orders.OrderByDescending(x => x.CreatedAt).Include(o => o.Thing);
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
        // GET: /Orders/Void/5

        public ActionResult Void(int id)
        {
            Order order = db.Orders.Find(id);
            return View(order);
        }

        //
        // POST: /Orders/Void/5

        [HttpPost, ActionName("Void")]
        public ActionResult VoidConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            var transaction = Transaction.Fetch(order.TransactionRef);
            var voidedTr = transaction.Void();

            if (voidedTr.ProcessorResponse.Success)
            {
                order.VoidRef = voidedTr.ReferenceId;
                order.IsVoided = true;

                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Errors = voidedTr.ProcessorResponse.Messages;
            return View("Delete", order);
        }

        //
        // GET: /Orders/Credit/5

        public ActionResult Credit(int id)
        {
            Order order = db.Orders.Find(id);
            return View(order);
        }

        //
        // POST: /Orders/Credit/5

        [HttpPost, ActionName("Credit")]
        public ActionResult CreditConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            var transaction = Transaction.Fetch(order.TransactionRef);
            var creditedTr = transaction.Credit();

            if (creditedTr.ProcessorResponse.Success)
            {
                order.CreditRef = creditedTr.ReferenceId;
                order.IsCredited = true;

                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Errors = creditedTr.ProcessorResponse.Messages;
            return View("Delete", order);
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