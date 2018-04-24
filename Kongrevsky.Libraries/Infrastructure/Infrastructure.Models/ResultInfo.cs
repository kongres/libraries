namespace Kongrevsky.Infrastructure.Models
{
    using System.Net;

    public class ResultInfo
    {
        public ResultInfo(HttpStatusCode statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message;

            if (string.IsNullOrEmpty(message))
            {
                switch (statusCode)
                {
                    case HttpStatusCode.Forbidden:
                        Message = "You don't have permission. Please contact administrator.";
                        break;
                }
            }
        }

        public HttpStatusCode StatusCode { get; }
        public string Message { get; }

        public bool IsSuccess => StatusCode == HttpStatusCode.OK;

        public ResultInfo ToResultInfo()
        {
            return new ResultInfo(StatusCode, Message);
        }
    }
}