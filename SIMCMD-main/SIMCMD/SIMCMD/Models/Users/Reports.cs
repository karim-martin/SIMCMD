using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class ReportRequest
{
    public int Id { get; set; }

    [Required, Display(Name = "Start Date*"), DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required, Display(Name = "End Date*"), DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    public string ReportData { get; set; }
}
