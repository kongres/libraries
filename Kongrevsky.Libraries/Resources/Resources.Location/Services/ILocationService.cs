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
        /// <param name="countryId"></param>
        /// <param name="stateId"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        List<City> GetCitiesByCountryAndState(string countryId, string stateId = null, string search = null);

        /// <summary>
        /// Returns list of all states
        /// </summary>
        /// <returns></returns>
        List<State> GetAllStates();

        /// <summary>
        /// Returns state by specified filters
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        List<State> GetStatesByCountry(string countryId, string search = null);

        /// <summary>
        /// Returns list of all countries
        /// </summary>
        /// <returns></returns>
        List<Country> GetAllCountries();

        /// <summary>
        /// Returns list of countries by specified filter
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        List<Country> GetCountries(string search);
    }
}