﻿namespace Kongrevsky.Resources.Location.Models
{
    #region << Using >>

    using Kongrevsky.Infrastructure.Models;

    #endregion

    public class StatePaging : PagingModel<State>
    {
        #region Properties

        public string CountryId { get; set; }

        #endregion
    }
}