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
        // GET: /Reserves/Edit/5
 
        public ActionResult Edit(int id)
        {
            Reserve reserve = db.Reserves.Find(id);
            ViewBag.ThingId = new SelectList(db.Things, "ThingId", "Name", reserve.ThingId);
            return View(reserve);
        }

        //
        // POST: /Reserves/Edit/5

        [HttpPost]
        public ActionResult Edit(Reserve reserve)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reserve).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ThingId = new SelectList(db.Things, "ThingId", "Name", reserve.ThingId);
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
            db.Reserves.Remove(reserve);
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