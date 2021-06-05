using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcAuth.Models
{
    public class RegisterViewModel2
    {
        [Required]
        [MaxLength(25, ErrorMessage = "Max Length is 25 Char")]
        [MinLength(5, ErrorMessage = "Min Length is 5 Char")]
        public string Name { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is Required with upper and lower case letters and special character")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^\da-zA-Z])(.{8,15})$", ErrorMessage = "Password must contain at least 1 number, 1 uppercase letter, and 1 special character.")]

        [Display(Name = "Password")]
        public string Pass { get; set; }
        [Required(ErrorMessage = "Confirm Password is Required")]
        [Compare("Pass", ErrorMessage = "Not Match")]
        [DataType(DataType.Password)]
        public string ComparePass { get; set; }
        public byte[] Image { get; set; }
    }
}