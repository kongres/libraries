namespace Resources.Location.Services
{
    using System.Collections.Generic;
    using Resources.Location.Models;

    public interface ILocationService
    {
        List<City> GetAllCities();
        List<City> GetCitiesByCountryAndState(string countryId, string stateId = null, string search = null);

        List<State> GetAllStates();
        List<State> GetStatesByCountry(string countryId, string search = null);

        List<Country> GetAllContries();
        List<Country> GetContries(string search);
    }
}