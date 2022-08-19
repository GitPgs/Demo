using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
// Additional using statements
using Microsoft.Owin.Security.Cookies;
using Stripe;

namespace Demo
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            StripeConfiguration.ApiKey = "sk_test_51K563ZG5VO8aymg5jvhSjN2NucjmLHEGlKyWApApZF6flJk6Wf4dkQzPwbvTwSCM2GgHAHlkVFRrHb81LAvvHR1b00icdPUU3Z";
            
            var options = new CookieAuthenticationOptions
            {
                AuthenticationType = "AUTH", // Id of authentication
                LoginPath = new PathString("/Account/Login"),
                LogoutPath = new PathString("/Account/Logout")
            };
            app.UseCookieAuthentication(options);
            app.MapSignalR();
        }
    }
}
