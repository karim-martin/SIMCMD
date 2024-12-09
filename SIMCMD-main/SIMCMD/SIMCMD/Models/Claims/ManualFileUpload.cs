using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMCMD.Models
{
    public class ManualFileUpload
    {
        public int Id { get; set; }
        
        [Display(Name = "File Name")]
        [StringLength(255)]
        public string FileName { get; set; }
        
        [Display(Name = "Provider")]
        [StringLength(255)]
        public string Provider { get; set; }

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
        
        [NotMapped, Required(ErrorMessage = "Please select a file to upload.")]
        [Display(Name = "Upload File")]
        public IFormFile UploadedFile { get; set; }
    }
}