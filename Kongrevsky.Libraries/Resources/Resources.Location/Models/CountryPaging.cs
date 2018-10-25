namespace Kongrevsky.Resources.Location.Models
{
    #region << Using >>

    using Kongrevsky.Utilities.Enumerable.Models;

    #endregion

    public class CountryPaging : PagingModel<Country>
    {
        public bool? HasCities { get; set; }
        public bool? HasStates { get; set; }
    }
}