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
            return View(reserves.ToList());
        }

        //
        // GET: /Reserves/Details/5

        public ViewResult Details(int id)
        {
            Reserve reserve = db.Reserves.Find(id);
            return View(reserve);
        }

        //
        // GET: /Reserves/Create

        public ActionResult Create()
        {
            ViewBag.ThingId = new SelectList(db.Things, "ThingId", "Name");
            return View();
        } 

        //
        // POST: /Reserves/Create

        [HttpPost]
        public ActionResult Create(Reserve reserve)
        {
            if (ModelState.IsValid)
            {
                db.Reserves.Add(reserve);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.ThingId = new SelectList(db.Things, "ThingId", "Name", reserve.ThingId);
            return View(reserve);
        }
        
        //
        // GET: /Reserves/Pay/5
 
        public ActionResult Pay(int id)
        {
            Reserve reserve = db.Reserves.Find(id);
            //ViewBag.ThingId = new SelectList(db.Things, "ThingId", "Name", reserve.ThingId);
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
        // GET: /Reserves/Delete/5
 
        public ActionResult Delete(int id)
        {
            Reserve reserve = db.Reserves.Find(id);
            return View(reserve);
        }

        //
        // POST: /Reserves/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
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
            return View("Delete", reserve);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}