using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using CommunityCare.Core.Models;

namespace CommunityCare.Web.ViewModels
{
    public class UserProfileViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Surname { get; set; }

        public int PhoneNumber { get; set; }

        public string Address { get; set; }

        public string County { get; set; }
 
        [Required]
        [EmailAddress]
        [Remote(action: "VerifyEmailAvailable", controller: "User", AdditionalFields = nameof(Id))]
        public string Email { get; set; }
        
        public int AccessNI { get; set; }

        public int CompanyNumber { get; set; }
    }
}