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
            ViewData["MerchantKey"] = Samurai.Samurai.MerchantKey;
            ViewBag.RedirectUrl = string.Format("http://{0}:{1}/Store/BuyConfirmed/{2}", Request.Url.Host, Request.Url.Port, id);
            ViewData["methods"] = new SelectList(db.Methods.Where(x => !x.IsRedacted).ToList(), "Token", "MethodName");
            
            return View(thing);
        }

        //
        // GET, POST: /Store/BuyConfirmed/1

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult BuyConfirmed(int id, string payment_method_token)
        {
            var thing = db.Things.Find(id);

            // process pm
            var paymentMethod = Samurai.PaymentMethod.Fetch(payment_method_token);
            if (paymentMethod.IsSensitiveDataValid)
            {
                // process order
                var transaction = Processor.TheProcessor.Purchase(payment_method_token, (decimal)thing.Price,
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

                // show errors of transaction
                ViewBag.Errors = transaction.ProcessorResponse.Messages.Select(x =>
                    string.Format("({0}) {1}: {2}", x.Subclass, x.Context, x.Key)).ToList();
                ViewData["MerchantKey"] = Samurai.Samurai.MerchantKey;
                ViewBag.RedirectUrl = string.Format("http://{0}:{1}/Store/BuyConfirmed/{2}", Request.Url.Host, Request.Url.Port, id);

                return View("Buy", thing);
            }

            // show errors of payment method
            ViewBag.Errors = paymentMethod.Messages.Select(x => string.Format("({0}) {1}: {2}", x.Subclass, x.Context, x.Key)).ToList();
            ViewData["MerchantKey"] = Samurai.Samurai.MerchantKey;
            ViewBag.RedirectUrl = string.Format("http://{0}:{1}/Store/BuyConfirmed/{2}", Request.Url.Host, Request.Url.Port, id);
            return View("Buy", thing);
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

            ViewBag.Errors = transaction.ProcessorResponse.Messages.Select(x => 
                string.Format("({0}) {1}: {2}", x.Subclass, x.Context, x.Key)).ToList();

            ViewData["methods"] = new SelectList(db.Methods.Where(x => !x.IsRedacted).ToList(), "Token", "MethodName");
            return View(thing);
        }

        //
        // GET: /Store/Reserve/1

        public ActionResult Reserve(int id)
        {
            var thing = db.Things.Find(id);
            ViewData["methods"] = new SelectList(db.Methods.Where(x => !x.IsRedacted).ToList(), "Token", "MethodName");

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
                    VoidingRef = string.Empty,
                    CapturingRef = string.Empty
                };

                db.Reserves.Add(reserve);
                db.SaveChanges();

                return RedirectToAction("Index", "Reserves");
            }

            ViewBag.Errors = transaction.ProcessorResponse.Messages.Select(x =>
                string.Format("({0}) {1}: {2}", x.Subclass, x.Context, x.Key)).ToList();

            ViewData["methods"] = new SelectList(db.Methods.Where(x => !x.IsRedacted).ToList(), "Token", "MethodName");
            return View(thing);
        }

    }
}
