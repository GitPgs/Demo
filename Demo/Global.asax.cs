using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
// Additional using statements
using Demo.Models;

namespace Demo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        // TODO: DBEntities
        DBEntities db = new DBEntities();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        // TODO: Session_Start()
        protected void Session_Start()
        {
            if (User.IsInRole("Student"))
            {
                Session["PhotoURL"] = db.Students.Find(User.Identity.Name).PhotoURL;
            }
        }

    }
}
