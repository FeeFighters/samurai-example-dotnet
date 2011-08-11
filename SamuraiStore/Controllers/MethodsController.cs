using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SamuraiStore.Models;
using Samurai;

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
            db.Methods.Add(new Method { Token = payment_method_token });
            db.SaveChanges();

            return RedirectToAction("Index");
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

            return RedirectToAction("Index");
        }
    }
}
