namespace Kongrevsky.Infrastructure.Repository.Utils.Options
{
    #region << Using >>

    using System.Data.SqlClient;

    #endregion

    public class BulkUpdateOptions<T>
            where T : class
    {
        /// <summary>
        ///     Fire triggers before and after Bulk Update
        /// </summary>
        public bool FireTriggers { get; set; } = true;

        /// <summary>
        ///     Number of rows in each batch. At the end of each batch, the rows in the batch are sent to the server.
        /// </summary>
        public int BatchSize { get; set; } = 5000;

        /// <summary>
        ///     Number of seconds for the operation to complete before it times out.
        /// </summary>
        public int Timeout { get; set; } = 600;

        /// <summary>
        ///     You can use the SqlBulkCopyOptions enumeration when you construct a SqlBulkCopy instance to change how the
        ///     WriteToServer methods for that instance behave.
        /// </summary>
        public SqlBulkCopyOptions SqlBulkCopyOptions { get; set; } = SqlBulkCopyOptions.Default;

        /// <summary>
        ///     Name of the entity table (if null or empty then autodetect of the name enabled)
        /// </summary>
        public string TableName { get; set; }
    }
}