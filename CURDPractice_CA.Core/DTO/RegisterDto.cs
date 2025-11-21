using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CURDPractice_CA.Core.DTO
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Name can't be blank")]
        public string PersonName { get; set; }

        [Required(ErrorMessage = "Email can't be blank")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Email value should be a valid email")]
        [Remote(action: "IsEmailRegistered", controller:"Account", ErrorMessage = "Email already taken")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number can't be blank")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number should only contain numbers")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password can't be blank")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password can't be blank")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirm password must match")]
        public string ConfirmPassword { get; set; }
    }
}
