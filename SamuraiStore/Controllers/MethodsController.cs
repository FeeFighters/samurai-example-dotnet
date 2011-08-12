using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SamuraiStore.Models;
using Samurai;
using System.Data;

namespace SamuraiStore.Controllers
{
    public class MethodsController : Controller
    {
        private StoreContext db = new StoreContext();

        //
        // GET: /Methods/

        public ActionResult Index()
        {
            return View(db.Methods.ToList());
        }

        //
        // GET: /Methods/Create

        public ActionResult Create()
        {
            ViewData["MerchantKey"] = Samurai.Samurai.MerchantKey;
            return View();
        }

        //
        // GET, POST: /Methods/Register

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Register(string payment_method_token)
        {
            var paymentMethod = Samurai.PaymentMethod.Fetch(payment_method_token);
            if (paymentMethod.IsSensitiveDataValid)
            {
                db.Methods.Add(new Method
                {
                    Token = payment_method_token,
                    MethodName = paymentMethod.Custom,
                    HolderName = string.Format("{0} {1}", paymentMethod.FirstName, paymentMethod.LastName)
                });
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewData["MerchantKey"] = Samurai.Samurai.MerchantKey;
            ViewBag.Errors = new List<string>() { "Sensitive data is invalid, please fill correct data." };
            return View("Create");
        }

        //
        // GET, POST: /Methods/Details/1

        public ActionResult Details(int id)
        {
            var method = db.Methods.Find(id);

            ViewData["id"] = id;
            var paymentMethod = PaymentMethod.Fetch(method.Token);

            return View(paymentMethod);
        }

        //
        // GET: /Methods/Edit/1

        public ActionResult Edit(int id)
        {
            var method = db.Methods.Find(id);
            var paymentMethod = PaymentMethod.Fetch(method.Token);

            return View(paymentMethod);
        }

        //
        // POST: /Methods/Details/1

        [HttpPost]
        public ActionResult Edit(PaymentMethod paymentMethod)
        {
            paymentMethod.Update();

            var method = db.Methods.First(x => x.Token == paymentMethod.PaymentMethodToken);
            method.MethodName = paymentMethod.Custom;
            method.HolderName = string.Format("{0} {1}", paymentMethod.FirstName, paymentMethod.LastName);

            db.Entry(method).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
