using System;
using System.ComponentModel.DataAnnotations;

namespace SIMCMD.Models
{
    public class FileConversion
    {
        public int Id { get; set; }
        
        [Display(Name = "File Name")]
        [StringLength(255)]
        public string FileName { get; set; }
        
        [Display(Name = "Provider")]
        [StringLength(255)]
        public string Provider { get; set; }
        
        [Display(Name = "Total Claims")]
        public int TotalClaims { get; set; }

        [Display(Name = "File Output")]
        public string InstitutionName { get; set; }
        
        [Display(Name = "Is File Uploaded")]
        public bool IsFileUploaded { get; set; }

        [Display(Name = "Is File Unwrap")]
        public bool IsFileUnwraped { get; set; }
        
        [Display(Name = "Is File Converted")]
        public bool IsFileConverted { get; set; }

        [Display(Name = "Is File Imported")]
        public bool IsFileImported { get; set; }

        [Display(Name = "File Log")]
        public string FileLog { get; set; }

        [Display(Name = "Database Log")]
        public string DatabaseLog { get; set; }

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
