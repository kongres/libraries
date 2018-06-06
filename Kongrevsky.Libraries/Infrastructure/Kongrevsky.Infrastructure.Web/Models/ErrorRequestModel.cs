namespace Kongrevsky.Infrastructure.Web.Models
{
    #region << Using >>

    using System.Collections.Generic;

    #endregion

    public class ErrorRequestModel
    {
        public ErrorRequestModel(string message)
        {
            Errors = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("", message) };
        }

        public ErrorRequestModel(List<KeyValuePair<string, string>> errors)
        {
            Errors = errors;
        }

        public List<KeyValuePair<string, string>> Errors { get; set; }
    }
}