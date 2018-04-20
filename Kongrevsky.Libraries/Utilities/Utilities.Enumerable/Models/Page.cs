namespace Utilities.Enumerable.Models
{
    public class Page
    {
        #region Properties

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int Skip => (PageNumber - 1) * PageSize;

        #endregion

        #region Constructors

        public Page()
        {
            PageNumber = 1;
            PageSize = 10;
        }

        public Page(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        #endregion
    }
}