﻿namespace Kongrevsky.Utilities.Enumerable.Models
{
    #region << Using >>

    using System.Collections.Generic;

    #endregion

    public class PageResult<T> : PageResult
    {
        public IEnumerable<T> Items { get; set; }
    }

    public class PageResult
    {
        public int TotalItemCount { get; set; }

        public int PageCount { get; set; }
    }
}