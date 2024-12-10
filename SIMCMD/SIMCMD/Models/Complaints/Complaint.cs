using System;
using System.ComponentModel.DataAnnotations;

namespace SIMCMD.Models
{
    public class Complaint
    {
        [Key]
        public int CompId { get; set; }

        [Required, MaxLength(35)]
        public string FName { get; set; }

        [MaxLength(35)]
        public string MName { get; set; }

        [Required, MaxLength(35)]
        public string LName { get; set; }

        [MaxLength(17)]
        public string Tele1 { get; set; }

        public string Tele2 { get; set; }

        public string Tele3 { get; set; }

        [MaxLength(30)]
        public string Email { get; set; }

        [MaxLength(200)]
        public string MailingAddress { get; set; }

        [MaxLength(1)]
        public string Gender { get; set; }

        public string DateOfLetter { get; set; }

        public string PolicyId { get; set; }

        public string AssignedTo { get; set; }

        public int AssignToUser { get; set; }

        public string NatureOfComp { get; set; }

        public string DateAcknowledged { get; set; }

        public int StepsI { get; set; }

        public int Status { get; set; }

        public string CompDetails { get; set; }

        public string ReferenceId { get; set; }

        public string ModifyDate { get; set; }

        public int SpecificReason { get; set; }
    }
}