namespace Kongrevsky.Resources.Location.Services
{
    #region << Using >>

    using System.Collections.Generic;
    using Kongrevsky.Resources.Location.Models;

    #endregion

    public interface ILocationService
    {
        /// <summary>
        /// Returns list of all cities
        /// </summary>
        /// <returns></returns>
        List<City> GetAllCities();

        /// <summary>
        /// Returns list of cities by specified filters
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        CityPaging GetCities(CityPaging filter);

        /// <summary>
        /// Returns list of all states
        /// </summary>
        /// <returns></returns>
        List<State> GetAllStates();

        /// <summary>
        /// Returns list of states by specified filters
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        StatePaging GetStates(StatePaging filter);

        /// <summary>
        /// Returns list of all countries
        /// </summary>
        /// <returns></returns>
        List<Country> GetAllCountries();

        /// <summary>
        /// Returns list of countries by specified filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        CountryPaging GetCountries(CountryPaging filter);
    }
}