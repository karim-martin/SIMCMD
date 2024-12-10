using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SIMCMD.Models
{
    public class IdentityExtendUser : IdentityUser
    {
        [Required, MaxLength(40), Display(Name = "Full name*")]
        public string FullName { get; set; }

        [MaxLength(35)]
        public string FirstName { get; set; }

        [MaxLength(35)]
        public string LastName { get; set; }

        [MaxLength(16)]
        public string Date { get; set; }

        [Required, MaxLength(12)]
        public string DepartmentNo { get; set; }

        public string AccountStatus { get; set; }

    }

    public class UserRole : IdentityRole
    {
        public string Description { get; set; }
    }

    public class UserViewModel
    {
        public string Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string AccountStatus { get; set; }

        [Required]
        public bool EmailConfirmed { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public List<RoleViewModel> Roles { get; set; }

        public string Role { get; set; }
    }

    public class RoleViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Role Name")]
        public string Name { get; set; }
    }
}
