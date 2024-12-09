using System;
using System.ComponentModel.DataAnnotations;

namespace SIMCMD.Models
{
    public class MyBackgroundJob
    {
        public int Id { get; set; }

        [Required, Display(Name = "Title"), DataType(DataType.Text)]
        public string Title { get; set; }

        [Display(Name = "Description"), DataType(DataType.Text)]
        public string Description { get; set; }

        [Display(Name = "Last Run"), DataType(DataType.DateTime)]
        public DateTime LastRun { get; set; }

        [Display(Name = "Run Successfully")]
        public bool Successful { get; set; }
    }
}
