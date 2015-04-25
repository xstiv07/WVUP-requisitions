using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApplication9.Data.Helpers;

namespace IdentitySample.Models
{

    public class ForgotViewModel
    {
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Email")]
        [RegularExpression(@"([_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4}))", ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember")]
        public bool RememberMe { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class EmailConfirmationViewModel
    {
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"([_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4}))", ErrorMessage = "Invalid Email")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Required")]
        [System.Web.Mvc.Remote("doesEmailExists", "Account", HttpMethod = "POST", ErrorMessage = "Already taken.")]
        [RegularExpression(@"([_a-z0-9-]+(\.[_a-z0-9-]+)*@wvup.edu$)", ErrorMessage = "Invalid Email. Email must be a part of wvup domain.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [RegularExpression(@"(?=.*\d)(?=.*[a-z]).{6,15}", ErrorMessage = "Password must be at least 6 characters long, contain one lowercase letter and one digit.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "First Name")]
        public string First_Name { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Last Name")]
        public string Last_Name { get; set; }

    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"([_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4}))", ErrorMessage = "Invalid Email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"(?=.*\d)(?=.*[a-z]).{6,15}", ErrorMessage = "Password must be at least 6 characters long, contain one lowercase letter and one digit.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"([_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4}))", ErrorMessage = "Invalid Email")]
        [Display(Name = "Email")]
        public string EmailForgot { get; set; }
    }
}