namespace Infrastructure.Models
{
    using System.Net;

    public class ResultObjectInfo<T> : ResultInfo
    {
        public ResultObjectInfo(HttpStatusCode statusCode, string message = null) : base(statusCode, message) { }

        public ResultObjectInfo(HttpStatusCode statusCode, T resultObject, string message = null)
                : base(statusCode, message)
        {
            ResultObject = resultObject;
        }

        public T ResultObject { get; set; }

        public ResultObjectInfo<TCast> ToResultObjectInfo<TCast>()
        {
            return new ResultObjectInfo<TCast>(StatusCode, Message);
        }
    }
}