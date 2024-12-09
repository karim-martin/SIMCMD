using System;
using System.ComponentModel.DataAnnotations;

namespace SIMCMD.Models
{
    public class SpecificArea
    {
        [Key]
        public int AreaId { get; set; }

        [Required, MaxLength(35)]
        public string Name { get; set; }
    }

    public class StatusOfFile
    {
        [Key]
        public int SId { get; set; }

        [Required, MaxLength(30)]
        public string SName { get; set; }
    }

    public class Step
    {
        [Key]
        public int StepsId { get; set; }

        [Required]
        public string StepText { get; set; }

        [Required]
        public string NextStep { get; set; }

        [Required, MaxLength(17)]
        public string Date { get; set; }

        [Required]
        public int ComplaintId { get; set; }

        [Required]
        public int UId { get; set; }
    }




}
