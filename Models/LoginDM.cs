using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace WebApplication3.Models
{
    public class LoginDM
    {
        [Required]
        public int ID { get; set; }
        [Required(ErrorMessage = "Invalid Name")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Invalid MailId")]
        [RegularExpression(@"^\w+([-+.']\w+)*@123g.com$", ErrorMessage = "Invalid Email Address")]
        [DataType(DataType.EmailAddress)]
        public string EmailID { get; set; }
        [Required(ErrorMessage = "Invalid PhoneNumber")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Invalid Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Invalid Password")]
        [Compare("Password")]
        public string ConformPassword { get; set; }
        [Required(ErrorMessage = "Incorrect Selected States")]
        public string State { get; set; }
        [Required(ErrorMessage = "Invalid City")]
        public string City { get; set; }
        [DataType(DataType.PostalCode)]
        [Required(ErrorMessage = "Incorrect Pincode")]
        public string Pincode { get; set; }
        [Required(ErrorMessage = "Invalid OTP")]
        public string Otp { get; set; }        
        [Required(ErrorMessage = "Invalid OTP")]
        public string LoginOtp { get; set; }
    }
}