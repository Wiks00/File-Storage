using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebUI.Models
{
    public class AccountViewModel
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }

        [Required(ErrorMessage = "Enter your username")]
        [Display(Name = "Your username")]
        [StringLength(40, ErrorMessage = "The username must contain at least {2} characters", MinimumLength = 4)]
        public string Name { get; set; }

        [Required(ErrorMessage = "The field can not be empty!")]
        [Display(Name = "Your email")]        
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Uncurrect email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter your password")]
        [Display(Name = "Your password")]
        [StringLength(100, ErrorMessage = "The password must contain at least {2} characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm the password")]
        [Display(Name = "Please confirm your password")]
        [Compare("Password", ErrorMessage = "Passwords must match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Captcha doesn't match with image")]
        [Display(Name = "Numbers from the image")]
        public string Captcha { get; set; }

        [Display(Name = "Keep me logged in")]
        public bool RememberMe { get; set; }
    }
}