using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// Additional using statements
using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class LoginStudentVM
    {
        [Required]
        [StringLength(4)]
        public string Username { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class LoginVM
    {
        [Required]
        [StringLength(4)]
        public string Username { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class LoginAdminVM
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class RegisterAdminVM
    {
        [Required]
        [StringLength(4)]
        [System.Web.Mvc.Remote("CheckUsername", "Account", ErrorMessage = "Duplicated {0}.")]
        public string Username { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

    }

        public class RegisterVM
    {
        [Required]
        [StringLength(4)]
        [System.Web.Mvc.Remote("CheckUsername", "Account", ErrorMessage = "Duplicated {0}.")]
        public string Username { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string Password { get; set; }

        //[Display(Name = "Confirm Password")]
        //[Required]
        //[StringLength(20, MinimumLength = 5)]
        // TODO: Compare
        //[Compare("Password")]
        //public string Confirm { get; set; }
        [Required]
        [StringLength(20)]
        public string Gender { get; set; }

        [Required]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{7})$",
                   ErrorMessage = "Entered phone format is not valid.")]
        //[System.Web.Mvc.Remote("CheckPhoneNumber", "Account", ErrorMessage = "Duplicated {0}.")]
        public string PhoneNumber { get; set; }


        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public System.DateTime Dob { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        //[System.Web.Mvc.Remote("CheckEmail", "Account", ErrorMessage = "Duplicated {0}.")]
        public string Email { get; set; }

        [Required]
        public HttpPostedFileBase Photo { get; set; }
    }

    public class RegisterOwnerVM
    {
        [Required]
        [StringLength(4)]
        [System.Web.Mvc.Remote("CheckUsername", "Account", ErrorMessage = "Duplicated {0}.")]
        public string Username { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string Password { get; set; }

        //[Display(Name = "Confirm Password")]
        //[Required]
        //[StringLength(20, MinimumLength = 5)]
        // TODO: Compare
        //[Compare("Password")]
        //public string Confirm { get; set; }
        [Required]
        [StringLength(20)]
        public string Gender { get; set; }

        
        [Required]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{7})$",
                   ErrorMessage = "Entered phone format is not valid.")]
        public string PhoneNumber { get; set; }


        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public System.DateTime Dob { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public HttpPostedFileBase Photo { get; set; }
    }

    public class ProfileVM
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }


        [Required(ErrorMessage = "Phone Number Required!")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{7})$",
                   ErrorMessage = "Entered phone format is not valid.")]
        public string PhoneNumber { get; set; }


        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        public string PhotoURL { get; set; }
        public HttpPostedFileBase Photo { get; set; }
    }

    public class PasswordVM
    {
        [Display(Name = "Current Password")]
        [Required]
        [StringLength(20, MinimumLength = 5)]
        public string Current { get; set; }

        [Display(Name = "New Password")]
        [Required]
        [StringLength(20, MinimumLength = 5)]
        public string New { get; set; }

        [Display(Name = "Confirm Password")]
        [Required]
        [StringLength(20, MinimumLength = 5)]
        // TODO: Compare
        [Compare("New")]
        public string Confirm { get; set; }
    }

    public class PropertyVM
    {



        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string AdTitle { get; set; }

        public string PropImage { get; set; }
        public HttpPostedFileBase Photo2 { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        [StringLength(100)]
        public string PropDescription { get; set; }

        [Required]
        //[StringLength(100)]
        public string Location { get; set; }

        [Required]
        [StringLength(100)]
        public string FloorRange { get; set; }

        [Required]
        public int Bedroom { get; set; }

        [Required]
        public int Bathroom { get; set; }

        [Required]
        public int PropSize { get; set; }

        [Required]
        [StringLength(50)]
        public string Furnishings { get; set; }

        [Required]
        [StringLength(100)]
        //public string  { get; set; }
        public string Facilities { get; set; }

        [Required]
        [StringLength(100)]
        public string Conveniences { get; set; }

        [Required]
        // [StringLength(100)]
        public string Category { get; set; }

        //[Required]
        //[StringLength(20)]
        //public string Role { get; set; }
    }



    // ------------------------------------------------------------------------
    // More to come...
    // ------------------------------------------------------------------------
    public class ResetStudentVM
    {
        [Required]
        [StringLength(4)]
        public string StudentID { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class ResetVM
    {
        [Required]
        [StringLength(4)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class BookingVM
    {


        [Display(Name = "PropertyName: *")]
        // [Required(ErrorMessage = "Please fill in your name !")]

        public string propName { get; set; }

        [Display(Name = "PropertyId: *")]
        // [Required(ErrorMessage = "Please fill in your name !")]

        public int propId { get; set; }


        [Display(Name = "Name *")]
        // [Required(ErrorMessage = "Please fill in your name !")]

        public string Name { get; set; }

        [Display(Name = "Contact No *")]
        //[Required(ErrorMessage = "Please fill in mobile number !")]
        //[RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Please Enter Valid Mobile Number.")]
        public string ContactNo { get; set; }

        [Display(Name = "Date of Visit *")]
        [Required(ErrorMessage = "Please select a date !")]
        public string Date { get; set; }

        [Display(Name = "Time of Visit  *")]
        [Required(ErrorMessage = "Please select a time !")]
        public string Time { get; set; }

        [Display(Name = "Remark (if any)")]

        public string Remark { get; set; }
    }

    public class PaymentVM
    {

        [Required]
        public string Id { get; set; }

        //[Display(Name = "Total *")]
        [Required]
        public decimal Total { get; set; }

        // [Display(Name = "Contact No *")]
        [Required]
        [RegularExpression(@"^([0-9]{3})$", ErrorMessage = "Please Enter Valid Cvc(3digit) !.")]
        public string Cvc { get; set; }

        // [Display(Name = "Date of Visit *")]
        [Required]
        //[RegularExpression(@"^([0-9]{3})$", ErrorMessage = "Please Enter Valid Cvc(3digit) !.")]
        //[RegularExpression(@"^(0[1-9]|1[0-2])\/?(202[2 - 9]{1})$", ErrorMessage = "Please Enter Valid Cvc(3digit) !.")]
        public string ExpDate { get; set; }


        //[Display(Name = "Time of Visit  *")]
        [Required]
        [RegularExpression(@"^([0-9]{5})$", ErrorMessage = "Please Enter Valid Zip(5digit) !.")]
        public string Zip { get; set; }


        //  [Display(Name = "Remark (if any)")]
        [Required]
        public string HolderName { get; set; }

        [Required]
        public string PropertyName { get; set; }

        [Required(ErrorMessage = "Please input  card Number(master/visa) 💳 !")]
        [RegularExpression(@"^(?:4[0-9]{12}(?:[0-9]{3})?|(?:5[1-5][0-9]{2}|222[1-9]|22[3-9][0-9]|2[3-6][0-9]{2}|27[01][0-9]|2720)[0-9]{12})$",
                           ErrorMessage = "Please Enter Valid Card number(Master/Visa) !.")]
        //[RegularExpression(@"^([0-9]{5})$", ErrorMessage = "Please Enter Valid Zip(5digit) !.")]
        public string CardNo { get; set; }

        [Required]
        public bool Term { get; set; }

        [Required]
        public bool RememberMe { get; set; }

        [Required]
        public string FromAcc { get; set; }


        [Required(ErrorMessage = "Please input account Number !")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Please Enter Valid Account number(10digit) !.")]
        public string ToAcc { get; set; }


        [Required]
        public string bankType { get; set; }
    }
}