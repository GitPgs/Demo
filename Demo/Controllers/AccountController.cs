using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
// Additional using statements
using Demo.Models;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using Microsoft.Owin.Security;
using PagedList;
using System.Net.Mail;
using Stripe;
using Stripe.Checkout;

namespace Demo.Controllers
{
    public class AccountController : Controller
    {
        DBEntities db = new DBEntities();
        // TODO: Initialize password hasher
        PasswordHasher ph = new PasswordHasher();

        // --------------------------------------------------------------------
        // Security helper functions
        // --------------------------------------------------------------------

        private string HashPassword(string password)
        {
            // TODO: Return hashed password
            return ph.HashPassword(password);
        }

        private bool VerifyPassword(string hash, string password)
        {
            // TODO: Verify hashed password (true or false)
            return ph.VerifyHashedPassword(hash, password)
                   == PasswordVerificationResult.Success;
        }

        private void SignIn(string username, string role, bool rememberMe)
        {
            // TODO(1): Identity and claims
            var iden = new ClaimsIdentity("AUTH");
            iden.AddClaim(new Claim(ClaimTypes.Name, username));
            iden.AddClaim(new Claim(ClaimTypes.Role, role));

            // TODO(2): Remember me
            var prop = new AuthenticationProperties
            {
                IsPersistent = rememberMe
            };

            // TODO(3): Sign in
            Request.GetOwinContext()
                   .Authentication
                   .SignIn(prop, iden);
        }

        private void SignOut()
        {
            // TODO: Sign out
            Request.GetOwinContext()
                   .Authentication
                   .SignOut();
        }

     
        // --------------------------------------------------------------------
        // Photo helper functions
        // --------------------------------------------------------------------

        private string ValidatePhoto(HttpPostedFileBase f)
        {
            var reType = new Regex(@"^image\/(jpeg|png)$", RegexOptions.IgnoreCase);
            var reName = new Regex(@"^.+\.(jpg|jpeg|png)$", RegexOptions.IgnoreCase);

            if (f == null)
            {
                return "No photo.";
            }
            else if (!reType.IsMatch(f.ContentType) || !reName.IsMatch(f.FileName))
            {
                return "Only JPG or PNG photo is allowed.";
            }
            else if (f.ContentLength > 1 * 1024 * 1024)
            {
                return "Photo size cannot more than 1MB.";
            }

            return null;
        }

        private string SavePhoto(HttpPostedFileBase f)
        {
            string name = Guid.NewGuid().ToString("n") + ".jpg";
            string path = Server.MapPath($"~/Photo/{name}");

            var img = new WebImage(f.InputStream);

            if (img.Width > img.Height)
            {
                int px = (img.Width - img.Height) / 2;
                img.Crop(0, px, 0, px);
            }
            else
            {
                int px = (img.Height - img.Width) / 2;
                img.Crop(px, 0, px, 0);
            }

            img.Resize(201, 201)
               .Crop(1, 1)
               .Save(path, "jpeg");

            return name;
        }

        private void DeletePhoto(string name)
        {
            name = System.IO.Path.GetFileName(name);
            string path = Server.MapPath($"~/Photo/{name}");
            System.IO.File.Delete(path);
        }

        // --------------------------------------------------------------------
        // Controller action methods
        // --------------------------------------------------------------------

        // GET: Account/LoginStudent
        public ActionResult LoginStudent()
        {
            return View();
        }

        // POST: Account/LoginStudent
        [HttpPost]
        public ActionResult LoginStudent(LoginStudentVM model, string returnURL = "")
        {
            if (ModelState.IsValid)
            {
                // TODO: Get user record based on studentID
                var user = db.Users.Find(model.Username);

                // TODO: AND if password matches
                if (user != null && VerifyPassword(user.Hash, model.Password))
                {
                    // TODO: Sign in user + session
                    SignIn(user.Username, user.Role, model.RememberMe);
                    Session["PhotoURL"] = user.PhotoURL;

                    // TODO: Handle return URL
                    if (returnURL == "")
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("Password", "StudentID and Password not matched.");
                }
            }

            return View(model);
        }

        // GET: Account/Login  (owner)
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(LoginVM model, string returnURL = "")
        {
            if (ModelState.IsValid)
            {
                // TODO: Get user record based on username
                var user = db.Users.Find(model.Username);

                // TODO: AND if password matches
                if (user != null && VerifyPassword(user.Hash, model.Password))
                {
                    // TODO: Sign in user + session
                    SignIn(user.Username, user.Role, model.RememberMe);
                    Session["PhotoURL"] = user.PhotoURL;

                    // TODO: Handle return URL
                    if (returnURL == "")
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("Password", "Username and Password not matched.");
                }
            }

            return View(model);
        }

        // GET: Account/Register
        public ActionResult RegisterAdmin()
        {
            return View();
        }

        // POST: Account/Register (student)
        [HttpPost]
        public ActionResult RegisterAdmin(RegisterAdminVM model)
        {
            //var a = db.Students.Find(model.StudentID);
            // TODO: AND if username duplicated
            if (ModelState.IsValidField("Username") && db.Users.Find(model.Username) != null)
            {
                ModelState.AddModelError("Username", "Duplicated AdminID.");
            }


          

            if (ModelState.IsValid)
            {
                var m = new Admin
                {
                    Username = model.Username,
                    Hash = HashPassword(model.Password), // TODO: Generate password hash
                    Name = model.Name,
                    Email = model.Email,
                    
                };

                db.Admins.Add(m);
                db.SaveChanges();

                TempData["Info"] = "Account registered. Please login.";
                //return RedirectToAction("LoginStudent", "Account");
            }

            return View(model);
        }


        // GET: Account/Login  (admin)
        [Authorize(Roles = "Admin")]
        public ActionResult LoginAdmin()
        {
            return View();
        }

        // POST: Account/LoginAdmin
        [HttpPost]
        public ActionResult LoginAdmin(LoginAdminVM model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Get user record based on username
                var user = db.Users.Find(model.Username);

                // TODO: AND if password matches
                if (user != null && VerifyPassword(user.Hash, model.Password))
                {
                    // TODO: Sign in user + session
                    SignIn(user.Username,user.Role, model.RememberMe);
                    //Session["PhotoURL"] = user.PhotoURL;

                    // TODO: Handle return URL
                    //if (returnURL == "")
                    //{
                    //    return RedirectToAction("Index", "Home");
                    //}
                }
                else
                {
                    ModelState.AddModelError("Password", "Username and Password not matched.");
                }
            }

            return View(model);
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            // TODO: Sign out user + session
            SignOut();
            Session.Remove("PhotoURL");

            return RedirectToAction("Index", "Home");
        }

        // GET: Account/CheckUsername
        public ActionResult CheckUsername(string Username)
        {
            // TODO: Check if username not duplicated.
            bool valid = db.Users.Find(Username) == null;
            return Json(valid, JsonRequestBehavior.AllowGet);
        }

        // GET: Account/CheckEmail
        public ActionResult CheckEmail(string Email)
        {
            // TODO: Check if email not duplicated.
            bool valid = db.Users.Where(s=>s.Email==Email) == null;
            return Json(valid, JsonRequestBehavior.AllowGet);
        }

        // GET: Account/CheckPhoneNumber
        public ActionResult CheckPhoneNumber(string PhoneNumber)
        {
            // TODO: Check if PhoneNumber not duplicated.
            bool valid = db.Users.Where(s => s.PhoneNumber == PhoneNumber) == null;
            return Json(valid, JsonRequestBehavior.AllowGet);
        }

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register (student)
        [HttpPost]
        public ActionResult Register(RegisterVM model)
        {
            //var a = db.Students.Find(model.StudentID);
            // TODO: AND if username duplicated
            if (ModelState.IsValidField("Username") && db.Users.Find(model.Username) != null)
            {
                ModelState.AddModelError("Username", "Duplicated StudentID.");
            }
       

            //if (ModelState.IsValidField("Email") && db.Students.Where(s=>s.Email==model.Email) != null)
            //{
            //    ModelState.AddModelError("Email", "Duplicated Email.");     
            //}
            //if (ModelState.IsValidField("PhoneNumber") && db.Students.Where(s => s.PhoneNumber == model.PhoneNumber) != null)
            //{
            //    ModelState.AddModelError("PhoneNumber", "Duplicated Phone Number.");
            //}

            string err = ValidatePhoto(model.Photo);
            if (err != null)
            {
                ModelState.AddModelError("Photo", err);
            }

            if (ModelState.IsValid)
            {
                var m1 = db.Students;
                foreach (var item in m1)
                {
                    if (item.Email == model.Email)
                    {
                        ModelState.AddModelError("Email", "Duplicated Email.");
                        return View(model);
                    }
                    else
                    {
                        TempData["info"] = item.Email;
                    }
                }
                var m2 = db.Students;
                foreach (var item in m2)
                {

                    if (item.PhoneNumber == model.PhoneNumber)
                    {
                        ModelState.AddModelError("PhoneNumber", "Duplicated Phone Number.");
                        return View(model);

                    }
                    else
                    {
                        TempData["info"] = item.PhoneNumber;
                    }
                }
                var m = new Student
                {
                    Username = model.Username,
                    Hash = HashPassword(model.Password), // TODO: Generate password hash
                    Name = model.Name,
                    Email = model.Email,
                    Dob = model.Dob,
                    PhoneNumber = model.PhoneNumber,
                    Gender = model.Gender,
                    PhotoURL = SavePhoto(model.Photo)
                };

                db.Students.Add(m);
                db.SaveChanges();

                TempData["Info"] = "Account registered. Please login.";
                return RedirectToAction("LoginStudent", "Account");
            }

            return View(model);
        }

        // GET: Account/RegisterOwner
        public ActionResult RegisterOwner()
        {
            return View();
        }

        //POST: Account/RegisterOwner
       [HttpPost]
        public ActionResult RegisterOwner(RegisterOwnerVM model)
        {
            // TODO: AND if username duplicated
            if (ModelState.IsValidField("Username") && db.Users.Find(model.Username) != null)
            {
                ModelState.AddModelError("Username", "Duplicated Username.");
            }

       
            string err = ValidatePhoto(model.Photo);
            if (err != null)
            {
                ModelState.AddModelError("Photo", err);
            }

            if (ModelState.IsValid)
            {
                var m = new Owner
                {
                    Username = model.Username,
                    Hash = HashPassword(model.Password), // TODO: Generate password hash
                    Name = model.Name,
                    Email = model.Email,
                    Dob = model.Dob,
                    Gender = model.Gender,
                    PhoneNumber = model.PhoneNumber,
                    PhotoURL = SavePhoto(model.Photo)
                };

                db.Owners.Add(m);
                db.SaveChanges();

                TempData["Info"] = "Account registered. Please login.";
                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }

        // TODO: Authorize (Member)
        // GET: Account/Detail
        [Authorize(Roles = "Student,Owner")]
        public ActionResult Profile()
        {
            // TODO: Get member record of the current member
            var m = db.Users.Find(User.Identity.Name);

            if (m == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new ProfileVM
            {
                Name = m.Name,
                Email = m.Email,
                PhoneNumber = m.PhoneNumber,
                PhotoURL = m.PhotoURL
            };

            return View(model);
        }

        // TODO: Authorize (Member)
        // POST: Account/Detail
        [Authorize(Roles = "Student,Owner")]
        [HttpPost]
        public ActionResult Profile(ProfileVM model)
        {
            // TODO: Get member record of the current member
            var m = db.Users.Find(User.Identity.Name);
            Student s = null;
            Owner o = null;

            if (m.Role == "Student")
            {
                s = db.Students.Find(m.Username);
            }
            else if(m.Role == "Owner")
            {
                o = db.Owners.Find(m.Username);

            }

            if (s == null && o == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (model.Photo != null)
            {
                string err = ValidatePhoto(model.Photo);
                if (err != null)
                {
                    ModelState.AddModelError("Photo", err);
                }
            }

            if (ModelState.IsValid)
            {
                if (model.Photo != null)
                {
                    if (s != null)
                    {
                        DeletePhoto(s.PhotoURL);
                        Session["PhotoURL"] = s.PhotoURL = SavePhoto(model.Photo);
                    }
                    else if (o != null)
                    {
                        DeletePhoto(o.PhotoURL);
                        Session["PhotoURL"] = o.PhotoURL = SavePhoto(model.Photo);
                    }
                    
                    // TODO: Update session
                    
                }

                if (s != null)
                {
                    s.Name = model.Name;
                    s.Email = model.Email;
                    s.PhoneNumber = model.PhoneNumber;
                }
                else if (o != null)
                {
                    o.Name = model.Name;
                    o.Email = model.Email;
                    o.PhoneNumber = model.PhoneNumber;
                }

                db.SaveChanges();

                TempData["Info"] = "Detail updated.";
                return RedirectToAction(null);
            }

            model.PhotoURL = m.PhotoURL;
            return View(model);
        }

        // TODO: Authorize
        // GET: Account/Password
        [Authorize]
        public ActionResult Password()
        {
            return View();
        }

        // TODO: Authorize
        // POST: Account/Password
        [Authorize]
        [HttpPost]
        public ActionResult Password(PasswordVM model)
        {
            // TODO: Get user record of the current user
            var user = db.Users.Find(User.Identity.Name);

            // TODO: OR if password not matches
            if (user == null || VerifyPassword(user.Hash, model.Current) == false)
            {
                ModelState.AddModelError("Current", "Current Password not matched.");
            }

            if (ModelState.IsValid)
            {
                // TOOD: Generate password hash
                string hash = HashPassword(model.New);

                // TODO: Update member or admin record
                if (user.Role == "Admin")
                {
                    db.Admins.Find(user.Username).Hash = hash;
                }
                else if (user.Role == "Student")
                {
                    db.Students.Find(user.Username).Hash = hash;
                }
                else if (user.Role == "Owner")
                {
                    db.Owners.Find(user.Username).Hash = hash;
                }
                db.SaveChanges();

                TempData["Info"] = "Password updated.";
                return RedirectToAction(null);
            }

            return View(model);
        }



        [Authorize(Roles = "Student,Owner,Admin")]
        public ActionResult Property(string name = "", string OrderBy = "", string ad = "")
        {
            name = name.Trim();

            if (name == "" && OrderBy == "")
            {


                var model1 = db.Properties.Where(s => s.Status != "unavailable" && s.Status!="pending");
                if (User.IsInRole("Admin"))
                {
                    model1 = db.Properties.Where(s => s.Status != "unavailable" );
                }
                if (Request.IsAjaxRequest()) return PartialView("_A", model1);
                return View(model1);
            }
            if (name != "" && OrderBy == "")
            {


                var model1 = db.Properties.Where(s => s.AdTitle.Contains(name) && s.Status != "unavailable" && s.Status != "pending");
                if (User.IsInRole("Admin"))
                {
                    model1 = db.Properties.Where(s => s.AdTitle.Contains(name) && s.Status != "unavailable");
                }

                if (Request.IsAjaxRequest()) return PartialView("_A", model1);
                return View(model1);
            }


            if (OrderBy == "Bed" && name != "" && ad == "asc")
            {
                var model1 = db.Properties.Where(s => s.AdTitle.Contains(name) && s.Status != "unavailable" && s.Status != "pending").OrderBy(s => s.Bedroom);
                //ViewBag.Bed = "dscBed";

                if (User.IsInRole("Admin"))
                {
                    model1 = db.Properties.Where(s => s.AdTitle.Contains(name) && s.Status != "unavailable").OrderBy(s => s.Bedroom);
                }
                if (Request.IsAjaxRequest()) return PartialView("_A", model1);
                return View(model1);
            }
            if (OrderBy == "Bed" && name == "" && ad == "asc")
            {
                var model1 = db.Properties.Where(s => s.Status != "unavailable" && s.Status != "pending").OrderBy(s => s.Bedroom);
                //ViewBag.Bed = "dscBed";
                if (User.IsInRole("Admin"))
                {
                     model1 = db.Properties.Where(s => s.Status != "unavailable").OrderBy(s => s.Bedroom);
                }

                if (Request.IsAjaxRequest()) return PartialView("_A", model1);
                return View(model1);
            }




            if (OrderBy == "Bath" && name != "" && ad == "asc")
            {
                var model1 = db.Properties.Where(s => s.AdTitle.Contains(name) && s.Status != "unavailable" && s.Status != "pending").OrderBy(s => s.Bathroom);
                //ViewBag.Bath = "dscBath";

                if (User.IsInRole("Admin"))
                {
                    model1 = db.Properties.Where(s => s.AdTitle.Contains(name) && s.Status != "unavailable" ).OrderBy(s => s.Bathroom);
                }
                if (Request.IsAjaxRequest()) return PartialView("_A", model1);
                return View(model1);
            }
            if (OrderBy == "Bath" && name == "" && ad == "asc")
            {
                var model1 = db.Properties.Where(s => s.Status != "unavailable" && s.Status != "pending").OrderBy(s => s.Bathroom);
                //ViewBag.Bath = "dscBath";
                if (User.IsInRole("Admin"))
                {
                    model1 = db.Properties.Where(s => s.Status != "unavailable" ).OrderBy(s => s.Bathroom);
                }

                if (Request.IsAjaxRequest()) return PartialView("_A", model1);
                return View(model1);
            }




            if (OrderBy == "Price" && name == "" && ad == "asc")
            {
                var model1 = db.Properties.Where(s => s.Status != "unavailable" && s.Status != "pending").OrderBy(s => s.Price);
                //ViewBag.Price = "dscPrice";
                // ViewBag.First = "2";
                if (User.IsInRole("Admin"))
                {
                    model1 = db.Properties.Where(s => s.Status != "unavailable" ).OrderBy(s => s.Price);
                }
                if (Request.IsAjaxRequest()) return PartialView("_A", model1);
                return View(model1);
            }
            if (OrderBy == "Price" && name != "" && ad == "asc")
            {
                var model1 = db.Properties.Where(s => s.AdTitle.Contains(name) && s.Status != "unavailable" && s.Status != "pending").OrderBy(s => s.Price);
                // ViewBag.Price = "dscPrice";
                //ViewBag.First = "2";

                if (User.IsInRole("Admin"))
                {
                    model1 = db.Properties.Where(s => s.AdTitle.Contains(name) && s.Status != "unavailable" ).OrderBy(s => s.Price);
                }
                if (Request.IsAjaxRequest()) return PartialView("_A", model1);
                return View(model1);
            }



            if (OrderBy == "Prop" && name == "" && ad == "asc")
            {
                var model1 = db.Properties.Where(s => s.Status != "unavailable" && s.Status != "pending").OrderBy(s => s.PropSize);
                // ViewBag.Prop = "dscProp";

                if (User.IsInRole("Admin"))
                {
                    model1 = db.Properties.Where(s => s.Status != "unavailable" ).OrderBy(s => s.PropSize);
                }
                if (Request.IsAjaxRequest()) return PartialView("_A", model1);
                return View(model1);
            }

            if (OrderBy == "Prop" && name != "" && ad == "asc")
            {
                var model1 = db.Properties.Where(s => s.AdTitle.Contains(name) && s.Status != "unavailable" && s.Status != "pending").OrderBy(s => s.PropSize);
                //ViewBag.Prop = "dscProp";

                if (User.IsInRole("Admin"))
                {
                    model1 = db.Properties.Where(s => s.AdTitle.Contains(name) && s.Status != "unavailable" ).OrderBy(s => s.PropSize);
                }
                if (Request.IsAjaxRequest()) return PartialView("_A", model1);
                return View(model1);
            }

            //dsc


            if (OrderBy == "Bed" && name == "" && ad == "dsc")
            {
                var model1 = db.Properties.Where(s => s.Status != "unavailable" && s.Status != "pending").OrderByDescending(s => s.Bedroom);
                //ViewBag.Bed = "ascBed";

                if (User.IsInRole("Admin"))
                {
                    model1 = db.Properties.Where(s => s.Status != "unavailable" ).OrderByDescending(s => s.Bedroom);
                }
                if (Request.IsAjaxRequest()) return PartialView("_A", model1);
                return View(model1);
            }
            if (OrderBy == "Bed" && name != "" && ad == "dsc")
            {
                var model1 = db.Properties.Where(s => s.AdTitle.Contains(name) && s.Status != "unavailable" && s.Status != "pending").OrderByDescending(s => s.Bedroom);
                ViewBag.Bed = "ascBed";

                if (User.IsInRole("Admin"))
                {
                    model1 = db.Properties.Where(s => s.AdTitle.Contains(name) && s.Status != "unavailable" ).OrderByDescending(s => s.Bedroom);
                }
                if (Request.IsAjaxRequest()) return PartialView("_A", model1);
                return View(model1);
            }



            if (OrderBy == "Bath" && name == "" && ad == "dsc")
            {
                var model1 = db.Properties.Where(s => s.Status != "unavailable" && s.Status != "pending").OrderByDescending(s => s.Bathroom);
                // ViewBag.Bath = "Bath";

                if (User.IsInRole("Admin"))
                {
                    model1 = db.Properties.Where(s => s.Status != "unavailable" ).OrderByDescending(s => s.Bathroom);
                }
                if (Request.IsAjaxRequest()) return PartialView("_A", model1);
                return View(model1);
            }
            if (OrderBy == "Bath" && name != "" && ad == "dsc")
            {
                var model1 = db.Properties.Where(s => s.AdTitle.Contains(name) && s.Status != "unavailable" && s.Status != "pending").OrderByDescending(s => s.Bathroom);
                //ViewBag.Bath = "ascBath";


                if (User.IsInRole("Admin"))
                {
                    model1 = db.Properties.Where(s => s.AdTitle.Contains(name) && s.Status != "unavailable" ).OrderByDescending(s => s.Bathroom);
                }
                if (Request.IsAjaxRequest()) return PartialView("_A", model1);
                return View(model1);
            }



            if (OrderBy == "Price" && name == "" && ad == "dsc")
            {
                var model1 = db.Properties.Where(s => s.Status != "unavailable" && s.Status != "pending").OrderByDescending(s => s.Price);
                // ViewBag.Price = "ascPrice";

                if (User.IsInRole("Admin"))
                {
                    model1 = db.Properties.Where(s => s.Status != "unavailable" ).OrderByDescending(s => s.Price);
                }
                if (Request.IsAjaxRequest()) return PartialView("_A", model1);
                return View(model1);
            }
            if (OrderBy == "Price" && name != "" && ad == "dsc")
            {
                var model1 = db.Properties.Where(s => s.AdTitle.Contains(name) && s.Status != "unavailable" && s.Status != "pending").OrderByDescending(s => s.Price);
                //ViewBag.Price = "ascPrice";


                if (User.IsInRole("Admin"))
                {
                    model1 = db.Properties.Where(s => s.AdTitle.Contains(name) && s.Status != "unavailable" ).OrderByDescending(s => s.Price);
                }
                if (Request.IsAjaxRequest()) return PartialView("_A", model1);
                return View(model1);
            }


            if (OrderBy == "Prop" && name == "" && ad == "dsc")
            {
                var model1 = db.Properties.Where(s => s.Status != "unavailable" && s.Status != "pending").OrderByDescending(s => s.PropSize);
                //   ViewBag.Prop = "ascProp";

                if (User.IsInRole("Admin"))
                {
                    model1 = db.Properties.Where(s => s.Status != "unavailable" ).OrderByDescending(s => s.PropSize);
                }
                if (Request.IsAjaxRequest()) return PartialView("_A", model1);
                return View(model1);
            }


            if (OrderBy == "Prop" && name != "" && ad == "dsc")
            {
                var model1 = db.Properties.Where(s => s.AdTitle.Contains(name) && s.Status != "unavailable" && s.Status != "pending").OrderByDescending(s => s.PropSize);
                //  ViewBag.Prop = "ascProp";

                if (User.IsInRole("Admin"))
                {
                    model1 = db.Properties.Where(s => s.AdTitle.Contains(name) && s.Status != "unavailable" ).OrderByDescending(s => s.PropSize);
                }
                if (Request.IsAjaxRequest()) return PartialView("_A", model1);
                return View(model1);
            }

            if (OrderBy == "" && name == "" && ad != "")
            {
                var model1 = db.Properties.Where(s => s.Status != "unavailable" && s.Status != "pending");
                if (User.IsInRole("Admin"))
                {
                    model1 = db.Properties.Where(s => s.Status != "unavailable" );
                }

                if (Request.IsAjaxRequest()) return PartialView("_A", model1);
                return View(model1);
            }
            if (OrderBy != "" && name == "" && ad == "")
            {
                var model1 = db.Properties.Where(s => s.Status != "unavailable" && s.Status != "pending");
                if (User.IsInRole("Admin"))
                {
                    model1 = db.Properties.Where(s => s.Status != "unavailable" );
                }

                if (Request.IsAjaxRequest()) return PartialView("_A", model1);
                return View(model1);
            }

            if (OrderBy != "" && name != "" && ad == "")
            {
                var model1 = db.Properties.Where(s => s.Status != "unavailable" && s.Status != "pending");
                if (User.IsInRole("Admin"))
                {
                    model1 = db.Properties.Where(s => s.Status != "unavailable" );
                }
                if (Request.IsAjaxRequest()) return PartialView("_A", model1);
                return View(model1);
            }





            return RedirectToAction("Login");


        }
        //public ActionResult Property2()
        //{
        //    return View();
        //}

        [Authorize(Roles = "Student,Owner")]
        public ActionResult Property2(int id)
        {

            var user = db.Users.Find(User.Identity.Name);

            //    List<(string, string)> list = new List<(string, string)>();
            //    if(list == null)
            //    {
            //var current = User.Identity.Name;
            //    list.Add((current, "yo"));
            //    }

            ViewBag.Role = user.Role;
            //ViewBag.fix = list;
            var model = db.Properties.Find(id);
            ViewBag.other = model.OwnerName;
            ViewBag.connect = User.Identity.Name;//current user sender

            if (User.Identity.Name != ViewBag.other)
            {
                //now is the user 

            }


            //if(User.Identity.Name == ViewBag.other)
            //{
            //    //seller login now 
            //    ViewBag.connect = ViewBag.other;
            //}

            ViewBag.propId = id;
            if (model == null)
            {
                return RedirectToAction("Index");
            }


            return View(model);
        }

        [Authorize(Roles = "Owner")]
        public ActionResult UpdateProperty(int id)
        {

            if (id == 0)
            {
                return RedirectToAction("Login", "Account");
            }
            var m = db.Properties.Find(id);

            var model = new PropertyVM
            {
                  Id = m.Id,
                AdTitle = m.AdTitle,
                Price = m.Price,
                PropDescription = m.PropDescription,
                Location = m.Location,
                FloorRange = m.FloorRange,
                Category = m.Category,
                Bedroom = (int)m.Bedroom,
                Bathroom = (int)m.Bathroom,
                PropSize = (int)m.PropSize,
                Furnishings = m.Furnishings,
                Conveniences = m.Conveniences,
                Facilities = m.Facilities,
                PropImage=m.PropImage
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        public ActionResult UpdateProperty(PropertyVM model)
        {
            var m = db.Properties.Find(model.Id);




          
                      if (model.Photo2 != null)
                {
                    string err = ValidatePhoto(model.Photo2);
                    if (err != null)
                    {
                        ModelState.AddModelError("Photo2", err);
                    }
                }

                if (ModelState.IsValid)
                {
                    if (model.Photo2 != null)
                    {

                        DeletePhoto(m.PropImage);
                        Session["PropImage"] = m.PropImage = SavePhoto(model.Photo2);

                    }
               
                    m.AdTitle = model.AdTitle;
                    m.Price = model.Price;
                    m.PropDescription = model.PropDescription;
                 m.Location = model.Location;
                    m.FloorRange = model.FloorRange;
                    m.Bedroom = model.Bedroom;
                    m.Bathroom = model.Bathroom;
                 m.PropSize = model.PropSize;
                    m.Furnishings = model.Furnishings;
                    m.Conveniences = model.Conveniences;
                    m.Facilities = model.Facilities;
                    db.SaveChanges();

                    TempData["Info"] = "Property updated.";
                    return RedirectToAction("Property","Account");
                }

                model.PropImage = m.PropImage;
                return View(model);

            }

        private string RandomPassword()
        {
            string s = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string password = "";

            // Generate random password
            Random r = new Random();

            for (int i = 1; i <= 10; i++)
            {
                password += s[r.Next(s.Length)];
            }

            return password;
        }

        private void SendEmail(User user, string password)
        {
            // Send email
            var m = new MailMessage();
            m.To.Add($"{user.Name} <{user.Email}>");
            m.Subject = "Reset Password";
            m.IsBodyHtml = true;

            string url = Url.Action("Login", "Account", null, "https");

            //Attachment
            //string path = Server.MapPath($"~/Images/admin.png");
            string path = Server.MapPath($"");
            if (user.PhotoURL != null)
            {
                path = Server.MapPath($"~/Photo/{user.PhotoURL}");
            }

            var att = new Attachment(path);
            att.ContentId = "photo";
            m.Attachments.Add(att);

            m.Body = $@"
                <img src='cid:photo' style='width: 100px; height: 100px;
                                            border: 1px solid #333'>
                <p>Dear {user.Name},<p>
                <p>Your password has been reset to:</p>
                <h1 style='color: red'>{password}</h1>
                <p>
                    Please <a href='{url}'>login</a>
                    with your new password.
                </p>
                <p>From, 👷 Super Admin</p>
            ";

            new SmtpClient().Send(m);
        }

        // GET: Account/Reset
        public ActionResult Reset()
        {
            return View();
        }

        // POST: Account/Reset
        [HttpPost]
        public ActionResult Reset(ResetVM model)
        {
            // Find user by username
            var user = db.Users.Find(model.Username);

            // If user null OR email not matched
            if (user == null || user.Email != model.Email)
            {
                ModelState.AddModelError("Email", "Username and email not matched.");
            }

            if (ModelState.IsValid)
            {
                // Generate random password and password hash
                string password = RandomPassword();
                string hash = HashPassword(password);

                // Update admin or member record
                if (user.Role == "Student")
                {
                    db.Students.Find(user.Username).Hash = hash;
                }
                else if (user.Role == "Owner")
                {
                    db.Owners.Find(user.Username).Hash = hash;
                }
                else if (user.Role == "Admin")
                {
                    db.Admins.Find(user.Username).Hash = hash;
                }
                db.SaveChanges();

                // Send email
                SendEmail(user, password);

                TempData["Info"] = "Password reset. Check your email.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        // GET: Account/Reset
        public ActionResult ResetStudent()
        {
            return View();
        }
        // GET: Account/PropertyOwner
        [Authorize(Roles = "Owner")]
        public ActionResult PropertyOwner()
        {

            return View();
        }

       

        [Authorize(Roles = "Owner")]
        public ActionResult PropertyOwner2()
        {

            return View();
        }


        [Authorize(Roles = "Owner")]
        [HttpPost]
        public ActionResult PropertyOwner2(PropertyVM model)
        {
            string err = ValidatePhoto(model.Photo2);
            if (err != null)
            {
                ModelState.AddModelError("Photo2", err);
            }




            if (ModelState.IsValid)
            {
                //add
                var m = new Property
                {
                    //Id = 1,
                    AdTitle = model.AdTitle,
                    Price = model.Price,
                    PropDescription = model.PropDescription,
                    Location = model.Location,
                    Category=model.Category,

                    //Role = model.Role,



                    FloorRange = model.FloorRange,
                    Bedroom = model.Bedroom,
                    Bathroom = model.Bathroom,
                    PropSize = model.PropSize,
                    Furnishings = model.Furnishings,
                    Conveniences = model.Conveniences,
                    Facilities = model.Facilities,
                    PropImage = SavePhoto(model.Photo2),
                    OwnerName = User.Identity.Name,
                    Status="pending"
                    
                };

                db.Properties.Add(m);
                db.SaveChanges();
                return View();
            }
            return RedirectToAction("Index", "Home");


        }




        //------------------
        // GET: Home/Booking
        public ActionResult Booking(int id)
        {

            ViewBag.page = "";
            ViewBag.logUser = id;
            var propName = db.Properties.Find(id);
            ViewBag.propName = propName.AdTitle;
            ViewBag.propImageURL = propName.PropImage;
            ViewBag.propId = id;


          

            var m = db.Users.Find(User.Identity.Name);
            var model = new BookingVM
            {
                Name = m.Name,
                ContactNo =m.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Booking(BookingVM model)
        {
            //if(model1 != null)
            //{
            //    model = null;
            //}
            if (ModelState.IsValid)
            {






                //if(cardyes !=null)
                //  {
                //   bookid =db.Payments.Where(s=>s.CardId==)   
                //  }
                //if (returnURL == "")
                //{
                //    return RedirectToAction("Index", "Home");
                //    //not return url then let user back to Index page which in home controller 
                //}


            }
            if (model != null)
            {
                var str = model.Date;



                //DateTime dt;
                var prop = db.Properties.Where(s => s.AdTitle.Contains(model.propName));
                var format = "dd/mm/yyyy hh:mm:ss tt";
                DateTime dt = DateTime.Parse(model.Date);
                //        var isValidDate = DateTime.TryParseExact(str, format,null,
                //DateTimeStyles.None, out dt);
                //var book = db.Bookings.OrderByDescending(s => s.t).FirstOrDefault();
                var book = db.Bookings.OrderByDescending(s => s.id).First();








                var idlastNo = book.id.Substring(3, 1);//1

                var idlastNo2 = book.id.Substring(2, 1);//11
                var idlastNo3 = book.id.Substring(1, 1);//111
                int a = int.Parse(idlastNo);
                int b = int.Parse(idlastNo2);
                int c = int.Parse(idlastNo3);
                var idtwo = book.id.Substring(2, 2);//11
                int g = int.Parse(idtwo);

                var idthree = book.id.Substring(1, 3);//11
                int f = int.Parse(idthree);
                string first = "B00";

                if (g >= 10 && g <= 99)
                {
                    first = "B0";


                    a = g;

                }


                if (a == 9)
                {
                    first = "B0";
                }
                if (g >= 100 && g <= 199)
                {
                    first = "B";

                    a = f;
                }

                var propname = model.propName;
                var propid = model.propId;
                var m = db.Users.Find(User.Identity.Name);
                var datetime = dt.ToString("dd/MM/yyyy hh:mm:00 tt");
                //var iu = db.Bookings.Where(s=>s.bookingDatetime.ToString("dd/MM/yyyy hh:mm:00 tt").CompareTo(datetime)==0);

                var all = db.Bookings.Where(s => s.propId == propid);
                foreach (var item in all)
                {
                    var datetime2 = item.bookingDatetime.ToString("dd/MM/yyyy hh:mm:00 tt");
                    if (datetime2.CompareTo(datetime) == 0)
                    {
                        var photo = db.Properties.Find(propid);

                        ViewBag.propName = photo.AdTitle;

                        ViewBag.propImageURL = photo.PropImage;
                        ViewBag.propId = model.propId;

                        var s = db.Users.Find(User.Identity.Name);
                        var model2 = new BookingVM
                        {
                            Name = m.Name,
                            ContactNo = m.PhoneNumber,

                        };


                        ViewBag.page = "";
                        ModelState.AddModelError("Date", "Aldy booked by other person ,Please select other date to book !.");

                        return View(model2);

                    }
                }
                //   var id4 = db.Bookings.Find("B007");
                //   ViewBag.date1 = dt.ToString("dd/MM/yyyy hh:mm:00 tt");
                //  var date1 = dt.ToString("dd/MM/yyyy hh:mm:00 tt");
                //   ViewBag.mod = model.Date;
                //var mod = model.Date;
                //       ViewBag.data = id4.bookingDatetime;
                //       var data = id4.bookingDatetime;
                //   ViewBag.data2 = id4.bookingDatetime.ToString("dd/MM/yyyy hh:mm:00 tt");
                // var data2 = id4.bookingDatetime.ToString("dd/MM/yyyy hh:mm:00 tt");

                //   if (iu != null)
                //   {

                //       if (date1.CompareTo(data2)==0)
                //       {

                //           return View("Login");

                //       }
                //       return View("Register");


                //       //got bug
                //   }
                //{
                //    //return View("Login");
                //}
                //14 / 7 / 2022 1:03:00 PM(Datetime



                //if (duplicated !=null)
                //{
                //    ModelState.AddModelError("Date", "Aldy booked by other person ,Please select other date to book !.");
                //    return RedirectToAction("Index", "Home");
                //}
                var booked = new Booking
                {
                    id = first + (a + 1),

                    bookingDatetime = dt,
                    bookingStatus = "pending",

                    propId = model.propId,
                    Username = m.Username




                };
                db.Bookings.Add(booked);
                db.SaveChanges();



                //ViewBag.bookingID = booked.id;
                //ViewBag.cardBorder = "border: 4px solid red";

                //ViewBag.bookingBorder = " ";

                //ViewBag.page = "card";



                ViewBag.cardBorder = "border: 3px solid red"; ViewBag.bookingBorder = " ";

                ViewBag.page = "card";
                var pr = db.Properties.Find(model.propId);
                //var d = db.Properties.Count(); this can count how many record in one table
                //// then we can use count +1 to auto increment varchar(A001,A002)


                //var a  = pr.AdTitle.Substring(2, 2);
                // int b = int.Parse(a);
                // ViewBag.price = (double)(b + 1);

                //this also work use for C001, C002 auto increment
                //ViewBag.propName = "S0"+1; this equal S01

                ViewBag.propName = pr.AdTitle;
                ViewBag.price = pr.Price;
                //ViewBag.price = (double)pr.Price + 1; is work 
                model = null;




                // GO TO ORDER PAGE  clear model 



            }

            return RedirectToAction("Order", new { id1 = "" });

            //if (model1 != null)
            //{
            //    var type = "";

            //    var method = "";

            //    if (model1.CardNo != null)
            //    {
            //        method = "card";
            //        if (model1.CardNo.StartsWith("4"))
            //        {
            //            type = "Visa";
            //        }

            //        if (model1.CardNo.StartsWith("5") || model1.CardNo.StartsWith("2"))
            //        {
            //            type = "Master";
            //        }
            //        var card = new Models.Card
            //        {
            //            cardNo = model1.CardNo,
            //            cardType = type
            //        };
            //        db.Cards.Add(card);
            //        db.SaveChanges();
            //        var p = db.Payments.OrderByDescending(s => s.Id).First();

            //        var idlastNo = p.Id.Substring(3, 1);//1

            //        var idlastNo2 = p.Id.Substring(2, 1);//11
            //        var idlastNo3 = p.Id.Substring(1, 1);//111
            //        int a = int.Parse(idlastNo);
            //        int b = int.Parse(idlastNo2);
            //        int c = int.Parse(idlastNo3);
            //        var idtwo = p.Id.Substring(2, 2);//11
            //        int g = int.Parse(idtwo);

            //        var idthree = p.Id.Substring(1, 3);//11
            //        int f = int.Parse(idthree);
            //        string first = "P00";

            //        if (g >= 10 && g <= 99)
            //        {
            //            first = "P0";


            //            a = g;

            //        }


            //        if (a == 9)
            //        {
            //            first = "P0";
            //        }
            //        if (g >= 100 && g <= 199)
            //        {
            //            first = "P";

            //            a = f;
            //        }

            //        var pay = new Models.Payment
            //        {
            //            Id = first + (a + 1),
            //            paymentDate = DateTime.Today,
            //            amount = model1.Total,
            //            paymentMethod = method,
            //            CardId = card.Id

            //        };
            //        db.Payments.Add(pay);
            //        db.SaveChanges();
            //        var i = pay.Id;
            //        //var bookingUpdate = db.Bookings.Find(model1.Id);
            //        var bookingUpdate = db.Bookings.OrderByDescending(s => s.id).First();
            //        if (bookingUpdate != null)
            //        {

            //            bookingUpdate.PaymentId = pay.Id;
            //            bookingUpdate.bookingStatus = "paid";
            //            db.SaveChanges();
            //        }

            //        //need update  booking 
            //        //need find booking id
            //        var paydetails = db.Payments.Where(s => s.CardId == card.Id);

            //        var order = db.Bookings.Where(s => s.PaymentId == i);
            //        var model3 = db.Bookings.Where(s => s.PaymentId == i);

            //        //return View("Order", i) ;
            //        return RedirectToAction("Order", new { id = i });

            //    }



            //    if (model1.ToAcc != null)
            //    {
            //        method = "bank";
            //        var bank = new Models.Bank
            //        {
            //            accNo = model1.FromAcc,
            //            accType = model1.bankType
            //        };
            //        db.Banks.Add(bank);

            //        db.SaveChanges();
            //        var p = db.Payments.OrderByDescending(s => s.Id).First();

            //        var idlastNo = p.Id.Substring(3, 1);//1

            //        var idlastNo2 = p.Id.Substring(2, 1);//11
            //        var idlastNo3 = p.Id.Substring(1, 1);//111
            //        int a = int.Parse(idlastNo);
            //        int b = int.Parse(idlastNo2);
            //        int c = int.Parse(idlastNo3);
            //        var idtwo = p.Id.Substring(2, 2);//11
            //        int g = int.Parse(idtwo);

            //        var idthree = p.Id.Substring(1, 3);//11
            //        int f = int.Parse(idthree);
            //        string first = "P00";

            //        if (g >= 10 && g <= 99)
            //        {
            //            first = "P0";


            //            a = g;

            //        }


            //        if (a == 9)
            //        {
            //            first = "P0";
            //        }
            //        if (g >= 100 && g <= 199)
            //        {
            //            first = "P";

            //            a = f;
            //        }

            //        var pay = new Models.Payment
            //        {
            //            Id = first + (a + 1),
            //            paymentDate = DateTime.Today,
            //            amount = (decimal)model1.Total,
            //            paymentMethod = method,
            //            BankId = bank.Id

            //        };
            //        db.Payments.Add(pay);
            //        db.SaveChanges();
            //        var i = pay.Id;
            //        var bookingUpdate = db.Bookings.OrderByDescending(s => s.id).First();
            //        if (bookingUpdate != null)
            //        {

            //            bookingUpdate.PaymentId = pay.Id;
            //            bookingUpdate.bookingStatus = "paid";
            //            db.SaveChanges();
            //        }
            //        //need update  booking 
            //        var paydetails = db.Payments.Find(i);

            //        var model3 = db.Bookings.Where(s => s.PaymentId == i);


            //        return View("Order", model3);

            //    }



            //    // GO TO ORDER PAGE 


            //}
            //return View();

        }
        [Authorize(Roles = "Admin,Owner")]
        public ActionResult AllBooking(int page = 1, string name = "")
        {
            //if (name != "")
            //{
            //    page = 1;
            //}
            var bookingUpdate1 = db.Bookings.OrderByDescending(s => s.id).ToPagedList(page, 10);
            //if (name != "")
            //{
            //  bookingUpdate1 = db.Bookings.OrderByDescending(s => s.id).Where(s => s.Student.Name == name).ToPagedList(page, 10);
            //}


            //ViewBag.User = "giap seng";
            //var bookingUpdate = db.Bookings.OrderByDescending(s => s.id).Where(s => s.Username == User.Identity.Name).ToPagedList(page, 10);
            if (page < 1)

            {

                return RedirectToAction(null, new { page = 1 });

                //    object routeValue means we need add a new{} it also call as anormemous object(object that without class), so thing that to pass in url ( controller/action/id) which is id /other passing variable can be write inside new{} 

                //return RedirectToAction("Demo4"); 

                //if redirect page is same as current page demo4=demo4 we just write null also can redirect user to this demo4 page  



            }







            if (page > bookingUpdate1.PageCount)

            {

                //inside model got also store pagelist property so pagecount means total paging been created if > pagecount means larger than last page  



                return RedirectToAction(null, new { page = bookingUpdate1.PageCount });

                //so means if user input a page number which is larger than page created by paging will be auto direct to last page of paging  

            }

            //all user payment,booking able to view 
            // want do the booking 

            return View(bookingUpdate1);
        }

       

        public ActionResult Order(string id1 = " ", int page = 1)


        {



            if (id1 != " ")
            {
                var p = db.Payments.OrderByDescending(s => s.Id).First();

                var idlastNo = p.Id.Substring(3, 1);//1

                var idlastNo2 = p.Id.Substring(2, 1);//11
                var idlastNo3 = p.Id.Substring(1, 1);//111
                int a = int.Parse(idlastNo);
                int b = int.Parse(idlastNo2);
                int c = int.Parse(idlastNo3);
                var idtwo = p.Id.Substring(2, 2);//11
                int g = int.Parse(idtwo);

                var idthree = p.Id.Substring(1, 3);//11
                int f = int.Parse(idthree);
                string first = "P00";

                if (g >= 10 && g <= 99)
                {
                    first = "P0";


                    a = g;

                }


                if (a == 9)
                {
                    first = "P0";
                }
                if (g >= 100 && g <= 199)
                {
                    first = "P";

                    a = f;
                }
                var booking = db.Bookings.Find(id1);

                var pay = new Payment
                {
                    Id = first + (a + 1),
                    paymentDate = DateTime.Today,
                    amount = booking.Property.Price,


                };
                db.Payments.Add(pay);
                db.SaveChanges();
                var i = pay.Id;




                booking.PaymentId = pay.Id;
                booking.bookingStatus = "paid";
                db.SaveChanges();
                var prop = db.Properties.Find(booking.propId);
                prop.Status = "unavailable";

                db.SaveChanges();

            }

            //ViewBag.User = "giap seng";
            var bookingUpdate = db.Bookings.OrderByDescending(s => s.id).Where(s => s.Username == User.Identity.Name).ToPagedList(page, 10);
            if (page < 1)

            {

                return RedirectToAction(null, new { page = 1 });

                //    object routeValue means we need add a new{} it also call as anormemous object(object that without class), so thing that to pass in url ( controller/action/id) which is id /other passing variable can be write inside new{} 

                //return RedirectToAction("Demo4"); 

                //if redirect page is same as current page demo4=demo4 we just write null also can redirect user to this demo4 page  



            }







            if (page > bookingUpdate.PageCount)

            {

                //inside model got also store pagelist property so pagecount means total paging been created if > pagecount means larger than last page  



                return RedirectToAction(null, new { page = bookingUpdate.PageCount });

                //so means if user input a page number which is larger than page created by paging will be auto direct to last page of paging  

            }

            return View(bookingUpdate);
            //paging






        }


        [HttpPost]
        public ActionResult Order(string id = "", decimal Total = 0)
        {
            var order = db.Bookings.Find(id);
            if (order != null)
            {
                db.Bookings.Remove(order);
                db.SaveChanges();
                //TempData["Info"] = "Product record deleted.";
            }
            ViewBag.User = "giap seng";
            var bookingUpdate = db.Bookings.Where(s => s.Username == User.Identity.Name);


            return View(bookingUpdate);

        }

        public ActionResult Receipt(string id = "")
        {
            var model = db.Bookings.Find(id);
            return View(model);
        }

        public ActionResult CheckOut(string id = "")
        {
            var model = db.Bookings.Find(id);




            StripeConfiguration.ApiKey = "sk_test_51K563ZG5VO8aymg5jvhSjN2NucjmLHEGlKyWApApZF6flJk6Wf4dkQzPwbvTwSCM2GgHAHlkVFRrHb81LAvvHR1b00icdPUU3Z";

            var options = new SessionCreateOptions
            {


                //CustomerEmail= model.Student.Email,

                Metadata = new Dictionary<string, string>
                {
                    { "Booking_Id", model.id


                    },
                },
                PaymentMethodTypes = new List<string>
                {
                    "card",
                    "fpx",
                },




                LineItems = new List<SessionLineItemOptions>
                  {
                    new SessionLineItemOptions
                    {
                      PriceData = new SessionLineItemPriceDataOptions
                      {
                        UnitAmount = (long?)(model.Property.Price)*100,
                        Currency = "myr",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                          Name = "Accomodation Rent",
                        },


                      },
                      Quantity = 1,

                    },
                  },
                Mode = "payment",



                SuccessUrl = $"https://localhost:44327/Account/Order?id1={id}",


                //SuccessUrl = $"https://localhost:44327/Product/Checkout?total={Total}&discount={discount}",
                CancelUrl = "https://localhost:44327/Account/Order",
            };

            var service = new SessionService();
            Session session = service.Create(options);
            //var a = session.PaymentIntentId;


            //need update  booking 
            //var paydetails = db.Payments.Find(i);

            //var model3 = db.Bookings.Where(s => s.PaymentId == i);

            Response.Headers.Add("Location", session.Url);
            return new HttpStatusCodeResult(303);


        }
        [Authorize(Roles = "Admin,Owner")]
        public ActionResult ViewHistoryChat()
        {

            var model = db.Owners;

            return View(model);
        }


        public ActionResult UserRecord(string name = "")
        {
            name = name.Trim();
            var model = db.Users.Where(s => s.Name.Contains(name));
            ViewBag.User = db.Users.Where(s => s.Name.Contains(name));
            // TODO: AJAX request
            if (Request.IsAjaxRequest()) return PartialView("_B", model);

            return View(model);
        }

        public ActionResult PropertyRecord(string adtitle = "")
        {
            adtitle = adtitle.Trim();
            var model = db.Properties.Where(s => s.AdTitle.Contains(adtitle));

            // TODO: AJAX request
            if (Request.IsAjaxRequest()) return PartialView("_C", model);

            return View(model);
        }
        [Authorize(Roles = "Admin,Owner")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var m = db.Properties.Find(id);
            if (m != null)
            {
               
                
                m.Status = "unavailable";

                db.SaveChanges();

                TempData["Info"] = "Property deleted.";
            }

            string url = Request.UrlReferrer?.AbsolutePath ?? "/";
            return Redirect(url);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Accept(int id)
        {
            var m = db.Properties.Find(id);
            if (m != null)
            {


                m.Status = "accepted";

                db.SaveChanges();

                TempData["Info"] = "Property post accepted.";
            }

            string url = Request.UrlReferrer?.AbsolutePath ?? "/";
            return Redirect(url);
        }
        public ActionResult AIChat()
        {
            return View();
        }


    }
}