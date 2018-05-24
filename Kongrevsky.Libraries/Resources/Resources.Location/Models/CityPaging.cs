﻿namespace Kongrevsky.Resources.Location.Models
{
    #region << Using >>

    using Kongrevsky.Infrastructure.Models;

    #endregion

    public class CityPaging : PagingModel<City>
    {
        #region Properties

        public string CountryId { get; set; }

        public string StateId { get; set; }

        #endregion
    }
}