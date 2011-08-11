using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SamuraiStore.Models;
using Samurai;

namespace SamuraiStore.Controllers
{
    public class StoreController : Controller
    {
        private StoreContext db = new StoreContext();

        //
        // GET: /Store/

        public ActionResult Index()
        {
            return View(db.Things.ToList());
        }

        //
        // GET: /Store/Buy/1

        public ActionResult Buy(int id)
        {
            var thing = db.Things.Find(id);
            ViewData["methods"] = new SelectList(db.Methods.ToList(), "Token", "Token");
            
            return View(thing);
        }

        //
        // POST: /Store/Buy

        [HttpPost]
        public ActionResult Buy(int thingId, string token)
        {
            var thing = db.Things.Find(thingId);
            var transaction = Processor.TheProcessor.Purchase(token, (decimal)thing.Price,
                string.Format("Buying {0} for {1} at Samurai Store", thing.Name, thing.Price));

            if (transaction.ProcessorResponse.Success)
            {
                var order = new Order()
                {
                    TransactionRef = transaction.ReferenceId,
                    Thing = thing,
                    CreatedAt = DateTime.UtcNow,
                    IsCredited = false,
                    CreditRef = string.Empty
                };

                db.Orders.Add(order);
                db.SaveChanges();

                return RedirectToAction("Index", "Orders");
            }

            ViewBag.ErrorMessage = "Some errors occured, try again.";
            return View(thing);
        }

        //
        // GET: /Store/Reserve/1

        public ActionResult Reserve(int id)
        {
            var thing = db.Things.Find(id);
            ViewData["methods"] = new SelectList(db.Methods.ToList(), "Token", "Token");

            return View(thing);
        }

        //
        // POST: /Store/Reserve

        [HttpPost]
        public ActionResult Reserve(int thingId, string token)
        {
            var thing = db.Things.Find(thingId);
            var transaction = Processor.TheProcessor.Authorize(token, (decimal)thing.Price,
                string.Format("Authorize ${0} for {1} at Samurai Store", thing.Price, thing.Name));

            if (transaction.ProcessorResponse.Success)
            {
                var reserve = new Models.Reserve()
                {
                    TransactionRef = transaction.ReferenceId,
                    Thing = thing,
                    CreatedAt = DateTime.UtcNow,
                    IsCaptured = false,
                    IsVoided = false,
                    VoidingRef = string.Empty
                };

                db.Reserves.Add(reserve);
                db.SaveChanges();

                return RedirectToAction("Index", "Reserves");
            }

            ViewBag.ErrorMessage = "Some errors occured, try again.";
            return View(thing);
        }

    }
}
