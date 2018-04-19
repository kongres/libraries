namespace Resources.Location.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Newtonsoft.Json;
    using Resources.Location.Models;

    public class LocationService : ILocationService
    {
        private List<City> cities { get; set; } = new List<City>();

        private List<City> _cities
        {
            get
            {
                if (cities?.Any() ?? false)
                    return cities;

                var assembly = Assembly.GetExecutingAssembly();
                var citiesFilePath = "Resource.Location.Resources.cities.json";
                using (var ctFile = new StreamReader(assembly.GetManifestResourceStream(citiesFilePath)))
                {
                    string ctContent = ctFile.ReadToEnd();
                    cities = JsonConvert.DeserializeObject<List<City>>(ctContent);
                }

                return cities;
            }
        }

        private List<State> states { get; set; } = new List<State>();

        private List<State> _states
        {
            get
            {
                if (states?.Any() ?? false)
                    return states;

                var assembly = Assembly.GetExecutingAssembly();
                var countriesFilePath = "Resource.Location.Resources.countries.json";
                using (var ctFile = new StreamReader(assembly.GetManifestResourceStream(countriesFilePath)))
                {
                    string ctContent = ctFile.ReadToEnd();
                    states = JsonConvert.DeserializeObject<List<State>>(ctContent);
                }

                return states;
            }
        }

        private List<Country> countries { get; set; } = new List<Country>();

        private List<Country> _countries
        {
            get
            {
                if (countries?.Any() ?? false)
                    return countries;

                var assembly = Assembly.GetExecutingAssembly();
                var countriesFilePath = "Resource.Location.Resources.countries.json";
                using (var ctFile = new StreamReader(assembly.GetManifestResourceStream(countriesFilePath)))
                {
                    string ctContent = ctFile.ReadToEnd();
                    countries = JsonConvert.DeserializeObject<List<Country>>(ctContent);
                }

                return countries;
            }
        }

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
            return _states.Where(x => string.IsNullOrWhiteSpace(countryId) || x.CountryId == countryId).Where(x => string.IsNullOrWhiteSpace(search) || x.Name.Contains(search)).ToList();
        }


        public List<Country> GetAllContries()
        {
            return _countries;
        }

        public List<Country> GetContries(string search)
        {
            return _countries.Where(x => string.IsNullOrWhiteSpace(search) || x.Name.Contains(search) || x.Code.Contains(search)).ToList();
        }
    }
}