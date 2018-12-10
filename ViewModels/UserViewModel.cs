using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.ViewModels
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "User Id is required")]
        [RegularExpression(@"^[0-9a-zA-Z]{1,40}$",ErrorMessage = "Special characters are not allowed.")]
        public string UserId { get; set; }
        public int Type { get; set; }
        [Required(ErrorMessage = "User Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?", ErrorMessage = "Invalid Email ID.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone is required")]
         [RegularExpression(@"(\+0?1\s)?\(?\d{3}\)?[\s.-]\d{3}[\s.-]\d{4}", ErrorMessage = "Invalid Phone Number")]
        
        public string Phone { get; set; }
        [Required(ErrorMessage = "Provider Name is required")]
        public string ProviderId { get; set; }
        public string Therapist { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Npi is required")]
        public string Npi { get; set; }
        public string Vseeid { get; set; }
       
    }
}
