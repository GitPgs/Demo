using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
// Additional using statements
using Demo.Models;

namespace Demo.Controllers
{
    public class HomeController : Controller
    {
        DBEntities db = new DBEntities();

        // GET: Home/Index
        public ActionResult Index()
        {
            return View();
        }

        // TODO: Authenticated users only
        // GET: Home/Both
        [Authorize]
        public ActionResult Both()
        {
            return View();
        }

        // TODO: Authenticated users only
        // POST: Home/Both
        [Authorize]
        [HttpPost]
        public ActionResult Both(string action)
        {
            // TODO(1): Handle 2 submit buttons
            // TODO(2): [Admin] and[Member] only buttons
            
            if (action == "btnAdmin" && User.IsInRole("Admin"))
            {
                TempData["Info"] = "[Admin] button is clicked.";
            }
            else if (action == "btnMember" && User.IsInRole("Member"))
            {
                TempData["Info"] = "[Member] button is clicked.";
            }

            return RedirectToAction(null);
        }

        // TODO: [Admin] only
        // GET: Home/Admin
        [Authorize(Roles = "Admin")]
        public ActionResult Admin()
        {
            return View();
        }

        // TODO: [Member] only
        // GET: Home/Member
        [Authorize(Roles = "Member")]
        public ActionResult Member()
        {
            return View();
        }

    }
}