namespace Kongrevsky.Resources.Location.Models
{
    #region << Using >>

    using Kongrevsky.Utilities.Enumerable.Models;

    #endregion

    public class CityPaging : PagingModel<City>
    {
        #region Properties

        public string CountryId { get; set; }

        /// <summary>
        ///     If specified then used instead of <see cref="CountryId" />
        /// </summary>
        public string CountryName { get; set; }

        public string StateId { get; set; }

        /// <summary>
        ///     If specified then used instead of <see cref="StateId" />
        /// </summary>
        public string StateName { get; set; }        
        
        /// <summary>
        ///     If specified then used instead of <see cref="StateName" />
        /// </summary>
        public string StateAbbr { get; set; }

        #endregion
    }
}