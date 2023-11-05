using System;
using System.ComponentModel.DataAnnotations;

namespace CommunityCare.Core.Models
{
    // Add User roles relevant to your application
    public enum Role { admin, institution, volunteer }
    public enum RoleType { institution, volunteer }
    
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Surname { get; set; }
        public int PhoneNumber { get; set; }
        public string Address { get; set; }
      
        public string County { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        public string Password { get; set; }
       // [Range (10,10)]
        public int AccessNI { get; set; }
        public int CompanyNumber { get; set; }
        public bool Validator { get; set; } = false;

        public IList<Schedule> Schedules { get; set; } = new List<Schedule>();
        public IList<User> Users { get; set; } = new List<User>();

        // User role within application
        public Role Role { get; set; }

        public User ()
        {

        }

    }
}
