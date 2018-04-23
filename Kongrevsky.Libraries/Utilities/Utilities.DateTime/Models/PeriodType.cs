namespace Kongrevsky.Utilities.DateTime.Models
{
    #region << Using >>

    using System.ComponentModel.DataAnnotations;

    #endregion

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