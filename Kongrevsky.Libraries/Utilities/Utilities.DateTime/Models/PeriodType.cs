namespace Utilities.DateTime.Models
{
    using System.ComponentModel.DataAnnotations;

    public enum PeriodType
    {
        [Display(Name = "Year")]
        Year = 0,
        [Display(Name = "Month")]
        Month = 1,
        [Display(Name = "Week")]
        Week = 2,
        [Display(Name = "Day")]
        Day = 3
    }
}