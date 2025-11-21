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
        public string PersonName;

        [Required(ErrorMessage = "Email can't be blank")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Email value should be a valid email")]
        public string Email;

        [Required(ErrorMessage = "Phone number can't be blank")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number should only contain numbers")]
        public string PhoneNumber;

        [Required(ErrorMessage = "Password can't be blank")]
        [DataType(DataType.Password)]
        public string Password;

        [Required(ErrorMessage = "Confirm Password can't be blank")]
        [DataType(DataType.Password)]
        public string ConfirmPassword;
    }
}
