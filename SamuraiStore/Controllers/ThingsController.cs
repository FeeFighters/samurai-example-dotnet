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
    public class ThingsController : Controller
    {
        private StoreContext db = new StoreContext();

        //
        // GET: /Things/

        public ViewResult Index()
        {
            return View(db.Things.ToList());
        }

        //
        // GET: /Things/Details/5

        public ViewResult Details(int id)
        {
            Thing thing = db.Things.Find(id);
            return View(thing);
        }

        //
        // GET: /Things/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Things/Create

        [HttpPost]
        public ActionResult Create(Thing thing)
        {
            if (ModelState.IsValid)
            {
                db.Things.Add(thing);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(thing);
        }
        
        //
        // GET: /Things/Edit/5
 
        public ActionResult Edit(int id)
        {
            Thing thing = db.Things.Find(id);
            return View(thing);
        }

        //
        // POST: /Things/Edit/5

        [HttpPost]
        public ActionResult Edit(Thing thing)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thing).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(thing);
        }

        //
        // GET: /Things/Delete/5
 
        public ActionResult Delete(int id)
        {
            Thing thing = db.Things.Find(id);
            return View(thing);
        }

        //
        // POST: /Things/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Thing thing = db.Things.Find(id);
            db.Things.Remove(thing);
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