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

    public class Archive
    {
        [Key]
        public int ArId { get; set; }

        [Required]
        public int ComId { get; set; }

        [MaxLength(100)]
        public string FileName { get; set; }
    }

    public class AssignedHistory
    {
        [Key]
        public int AssignedId { get; set; }

        [Required]
        public int AssignedTo { get; set; }

        [Required, MaxLength(17)]
        public string Date { get; set; }
    }

    public class ComplaintType
    {
        [Key]
        public int CompTypeId { get; set; }

        [Required, MaxLength(30)]
        public string CompName { get; set; }
    }

    public class Department
    {
        [Key]
        public int DId { get; set; }

        [Required, MaxLength(100)]
        public string DName { get; set; }
    }

    public class LogFile
    {
        [Key]
        public int LogId { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(10)]
        public string Action { get; set; }

        public DateTime When { get; set; } = DateTime.Now;
    }
}
