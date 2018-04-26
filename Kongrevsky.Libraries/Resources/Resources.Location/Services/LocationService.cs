namespace Kongrevsky.Resources.Location.Services
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Kongrevsky.Resources.Location.Models;
    using Newtonsoft.Json;

    #endregion

    public class LocationService : ILocationService
    {
        #region Constants

        private const string _citiesResourcePath = "Resource.Location.Resources.cities.json";

        private const string _statesResourcePath = "Resource.Location.Resources.states.json";

        private const string _countriesResourcePath = "Resource.Location.Resources.countries.json";

        #endregion

        #region Properties

        private List<City> _cities
        {
            get
            {
                if (this.citiesValue == null || !this.citiesValue.Any())
                    this.citiesValue = _tryRetrieveEmbeddedList<City>(_citiesResourcePath);

                return this.citiesValue;
            }
        }

        private List<State> _states
        {
            get
            {
                if (this.statesValue == null || !this.statesValue.Any())
                    this.statesValue = _tryRetrieveEmbeddedList<State>(_statesResourcePath);

                return this.statesValue;
            }
        }

        private List<Country> _countries
        {
            get
            {
                if (this.countriesValue == null || !this.countriesValue.Any())
                    this.countriesValue = _tryRetrieveEmbeddedList<Country>(_countriesResourcePath);

                return this.countriesValue;
            }
        }

        private List<City> citiesValue;

        private List<Country> countriesValue;

        private List<State> statesValue;

        #endregion

        #region Interface Implementations

        public List<City> GetAllCities()
        {
            return _cities;
        }

        public List<City> GetCitiesByCountryAndState(string countryId, string stateId = null, string search = null)
        {
            return _cities.Where(x => (string.IsNullOrWhiteSpace(countryId) || x.CountryId == countryId) && (string.IsNullOrWhiteSpace(stateId) || x.StateId == stateId))
                          .Where(x => string.IsNullOrWhiteSpace(search) || x.Name.Contains(search)).ToList();
        }

        public List<State> GetAllStates()
        {
            return _states;
        }

        public List<State> GetStatesByCountry(string countryId, string search = null)
        {
            return _states.Where(x => string.IsNullOrWhiteSpace(countryId) || x.CountryId == countryId)
                          .Where(x => string.IsNullOrWhiteSpace(search) || x.Name.Contains(search)).ToList();
        }

        public List<Country> GetAllCountries()
        {
            return _countries;
        }

        public List<Country> GetCountries(string search)
        {
            return _countries.Where(x => string.IsNullOrWhiteSpace(search) || x.Name.Contains(search) || x.Code.Contains(search)).ToList();
        }

        #endregion

        List<T> _tryRetrieveEmbeddedList<T>(string resourcePath)
        {
            try
            {
                List<T> result;
                var assembly = Assembly.GetExecutingAssembly();
                using (var ctFile = new StreamReader(assembly.GetManifestResourceStream(resourcePath) ?? throw new InvalidOperationException("Resource not found")))
                {
                    var ctContent = ctFile.ReadToEnd();
                    result = JsonConvert.DeserializeObject<List<T>>(ctContent);
                }

                return result;
            }
            catch (Exception e)
            {
                return new List<T>();
            }
        }
    }
}