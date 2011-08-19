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
    public class ReservesController : Controller
    {
        private StoreContext db = new StoreContext();

        //
        // GET: /Reserves/

        public ViewResult Index()
        {
            var reserves = db.Reserves.Include(r => r.Thing);
            return View(reserves.OrderByDescending(x => x.CreatedAt).ToList());
        }

        //
        // GET: /Reserves/Details/5

        public ViewResult Details(int id)
        {
            Reserve reserve = db.Reserves.Find(id);
            return View(reserve);
        }
        
        //
        // GET: /Reserves/Pay/5
 
        public ActionResult Pay(int id)
        {
            Reserve reserve = db.Reserves.Find(id);
            return View(reserve);
        }

        //
        // POST: /Reserves/Pay/5

        [HttpPost]
        public ActionResult Pay(int reserveId, int thingId)
        {
            var reserve = db.Reserves.Find(reserveId);

            if (ModelState.IsValid)
            {                
                var transaction = Transaction.Fetch(reserve.TransactionRef);
                var capturedTr = transaction.Capture();

                if (capturedTr.ProcessorResponse.Success)
                {
                    reserve.IsCaptured = true;
                    reserve.CapturingRef = capturedTr.ReferenceId;
                    reserve.CapturedAt = DateTime.UtcNow;

                    db.Entry(reserve).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                ViewBag.ErrorMessage = "Some errors occured, try again.";
            }

            return View(reserve);
        }

        //
        // GET: /Reserves/Void/5
 
        public ActionResult Void(int id)
        {
            Reserve reserve = db.Reserves.Find(id);
            return View(reserve);
        }

        //
        // POST: /Reserves/Void/5

        [HttpPost, ActionName("Void")]
        public ActionResult VoidConfirmed(int id)
        {            
            Reserve reserve = db.Reserves.Find(id);
            var transaction = Transaction.Fetch(reserve.TransactionRef);
            var voidedTr = transaction.Void();

            if (voidedTr.ProcessorResponse.Success)
            {
                reserve.VoidingRef = voidedTr.ReferenceId;
                reserve.IsVoided = true;

                db.Entry(reserve).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Errors = voidedTr.ProcessorResponse.Messages;
            return View("Void", reserve);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}