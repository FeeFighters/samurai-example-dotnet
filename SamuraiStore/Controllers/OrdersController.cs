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

            ViewBag.Errors = voidedTr.ProcessorResponse.Messages.Select(x =>
                    string.Format("({0}) {1}: {2}", x.Subclass, x.Context, x.Key)).ToList();
            return View("Void", order);
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

            ViewBag.Errors = creditedTr.ProcessorResponse.Messages.Select(x =>
                    string.Format("({0}) {1}: {2}", x.Subclass, x.Context, x.Key)).ToList();
            return View("Credit", order);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}