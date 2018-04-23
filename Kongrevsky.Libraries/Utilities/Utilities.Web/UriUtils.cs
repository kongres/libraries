namespace Kongrevsky.Utilities.Web
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Web;

    #endregion

    public static class UriUtils
    {
        /// <summary>
        /// Adds the specified parameter to the Query String.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paramName">Name of the parameter to add.</param>
        /// <param name="paramValue">Value for the parameter to add.</param>
        /// <returns>Url with added parameter.</returns>
        public static Uri AddParameter(this Uri url, string paramName, string paramValue)
        {
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[paramName] = paramValue;
            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri;
        }

        /// <summary>
        /// Adds parameter to the URL
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paramName"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public static Uri AddParameter(this Uri url, string paramName, List<string> paramValues)
        {
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            foreach (var paramValue in paramValues)
                query.Add(paramName, paramValue);

            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri;
        }

        /// <summary>
        /// Detects if URL is valid
        /// </summary>
        /// <param name="uriName"></param>
        /// <returns></returns>
        public static bool CheckUrl(string uriName)
        {
            if (string.IsNullOrWhiteSpace(uriName))
                return false;

            var result = Uri.TryCreate(uriName, UriKind.Absolute, out var uriResult)
                      && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps || uriResult.Scheme == Uri.UriSchemeFtp);

            if (!result)
                return false;

            try
            {
                //Creating the HttpWebRequest
                var request = WebRequest.Create(uriResult) as HttpWebRequest;
                //Setting the Request method HEAD, you can also use GET too.
                request.Method = "HEAD";
                //Getting the Web Response.
                var response = request.GetResponse() as HttpWebResponse;
                //Returns TRUE if the Status code == 200
                response.Close();
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch (Exception)
            {
                try
                {
                    //Creating the HttpWebRequest
                    var request = WebRequest.Create(uriResult) as HttpWebRequest;
                    //Setting the Request method HEAD, you can also use GET too.
                    request.Method = "GET";
                    //Getting the Web Response.
                    var response = request.GetResponse() as HttpWebResponse;
                    //Returns TRUE if the Status code == 200
                    response.Close();
                    return (response.StatusCode == HttpStatusCode.OK);
                }
                catch { }

                //Any exception will returns false.
                return false;
            }
        }
    }
}