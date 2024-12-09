using System;
using System.ComponentModel.DataAnnotations;

namespace SIMCMD.Models
{
    public class FileManagement
    {
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Display(Name = "File Path")]
        public string FilePath { get; set; }

        [Display(Name = "File Size")]
        public long FileSize { get; set; }

        [Display(Name = "Directory")]
        public bool IsDirectory { get; set; }

        [Display(Name = "Date & Time")]
        public DateTime LastModified { get; set; }
    }
}
