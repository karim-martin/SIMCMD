using System;
using System.ComponentModel.DataAnnotations;

namespace SIMCMD.Models
{
    public class Provider
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Institution name is required.")]
        [Display(Name = "Institution Name")]
        [StringLength(255)]
        public string InstitutionName { get; set; }
        
        [Required(ErrorMessage = "Institution type is required.")]
        [Display(Name = "Institution Type")]
        [StringLength(100)]
        public string InstitutionType { get; set; }
        
        [Required(ErrorMessage = "Provider email is required.")]
        [Display(Name = "Provider Email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string ProviderEmail { get; set; }
        
        [Required(ErrorMessage = "Provider code is required.")]
        [Display(Name = "Provider Code")]
        [StringLength(50)]
        public string ProviderCode { get; set; }

        [Required(ErrorMessage = "Folder Name is required.")]
        [Display(Name = "Provider Folder Name")]
        [StringLength(50)]
        public string FolderName { get; set; }

        [Display(Name = "File Unwrap")]
        public bool Unwrap { get; set; }

        [Display(Name = "Send Email To Provider")]
        public bool SendEmailToProvider { get; set; }

        public string UserCreated { get; set; }

        public string UserModified { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> DateCreated { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> DateModified { get; set; } = DateTime.Now;
    }
}
