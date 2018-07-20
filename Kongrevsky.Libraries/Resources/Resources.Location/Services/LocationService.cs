namespace Kongrevsky.Resources.Location.Services
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Kongrevsky.Resources.Location.Models;
    using Kongrevsky.Utilities.Enumerable;
    using Kongrevsky.Utilities.Enumerable.Models;
    using Kongrevsky.Utilities.String;
    using Newtonsoft.Json;

    #endregion

    public class LocationService : ILocationService
    {
        private LocationServiceOptions _options { get; set; } = new LocationServiceOptions() { StringComparison = StringComparison.InvariantCultureIgnoreCase };

        #region Constants

        private const string _citiesResourcePath = "Kongrevsky.Resources.Location.Resources.cities.json";

        private const string _statesResourcePath = "Kongrevsky.Resources.Location.Resources.states.json";

        private const string _countriesResourcePath = "Kongrevsky.Resources.Location.Resources.countries.json";

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

        public void SetOptions(LocationServiceOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public List<City> GetAllCities()
        {
            return _cities;
        }

        public CityPaging GetCities(CityPaging filter)
        {
            var search = filter.Search.SplitBySpaces();
            if (!filter.CountryName.IsNullOrEmpty())
            {
                var country = _countries.FirstOrDefault(x => string.Equals(x.Name, filter.CountryName, _options.StringComparison));
                if (country != null)
                    filter.CountryId = country.Code;
            }

            if (!filter.StateName.IsNullOrEmpty())
            {
                var state = _states.FirstOrDefault(x => string.Equals(x.Name, filter.StateName, _options.StringComparison));
                if (state != null)
                    filter.StateId = state.Name;
            }

            var cities = _cities.Where(x => (filter.CountryId.IsNullOrWhiteSpace() || x.CountryId == filter.CountryId) &&
                                            (filter.StateId.IsNullOrWhiteSpace() || x.StateId == filter.StateId))
                                .Where(x => !search.Any() || search.Any(r => x.Name.Contains(r, _options.StringComparison)))
                                .OrderBy(filter.OrderProperty, filter.IsDesc)
                                .ToList();

            filter.SetResult(cities.GetPage(new Page(filter.PageNumber, filter.PageSize)),
                             cities.Count, cities.GetPageCount(filter.PageSize));

            return filter;
        }

        public List<State> GetAllStates()
        {
            return _states;
        }

        public StatePaging GetStates(StatePaging filter)
        {
            var search = filter.Search.SplitBySpaces();
            if (!filter.CountryName.IsNullOrEmpty())
            {
                var country = _countries.FirstOrDefault(x => string.Equals(x.Name, filter.CountryName, _options.StringComparison));
                if (country != null)
                    filter.CountryId = country.Code;
            }

            var states = _states.Where(x => filter.CountryId.IsNullOrWhiteSpace() || x.CountryId == filter.CountryId)
                                .Where(x => !search.Any() || search.Any(r => x.Name.Contains(r, _options.StringComparison)))
                                .OrderBy(filter.OrderProperty, filter.IsDesc)
                                .ToList();

            filter.SetResult(states.GetPage(new Page(filter.PageNumber, filter.PageSize)),
                             states.Count, states.GetPageCount(filter.PageSize));

            return filter;
        }

        public List<Country> GetAllCountries()
        {
            return _countries;
        }

        public CountryPaging GetCountries(CountryPaging filter)
        {
            var search = filter.Search.SplitBySpaces();

            var countries = _countries.Where(x => !search.Any() || search.Any(r => x.Name.Contains(r, _options.StringComparison)) || search.Any(r => x.Code.Contains(r, _options.StringComparison)))
                                      .OrderBy(filter.OrderProperty, filter.IsDesc)
                                      .ToList();

            filter.SetResult(countries.GetPage(new Page(filter.PageNumber, filter.PageSize)),
                             countries.Count, countries.GetPageCount(filter.PageSize));

            return filter;
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