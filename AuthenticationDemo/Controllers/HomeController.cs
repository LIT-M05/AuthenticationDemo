using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AuthenticationDemo.Data;
using AuthenticationDemo.Models;

namespace AuthenticationDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var vm = new HomePageViewModel();
            vm.IsLoggedIn = User.Identity.IsAuthenticated;
            return View(vm);
        }

        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(User user, string password)
        {
            var db = new AuthDb(Properties.Settings.Default.ConStr);
            db.AddUser(user, password);
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Secret()
        {
            string currentEmail = User.Identity.Name;
            var db = new AuthDb(Properties.Settings.Default.ConStr);
            var user = db.GetByEmail(currentEmail);
            SecretPageViewModel vm = new SecretPageViewModel();
            vm.User = user;
            return View(vm);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var db = new AuthDb(Properties.Settings.Default.ConStr);
            var user = db.Login(email, password);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            FormsAuthentication.SetAuthCookie(email, true);
            return RedirectToAction("Secret");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
    }
}