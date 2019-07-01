namespace Kongrevsky.Resources.Location.Models
{
    #region << Using >>

    using Kongrevsky.Utilities.Enumerable.Models;

    #endregion

    public class StatePaging : PagingModel<State>
    {
        public string CountryId { get; set; }

        /// <summary>
        ///     If specified then used instead of <see cref="CountryId" />
        /// </summary>
        public string CountryName { get; set; }
        public bool? HasCities { get; set; }
        public string StateAbbr { get; set; }
    }
}