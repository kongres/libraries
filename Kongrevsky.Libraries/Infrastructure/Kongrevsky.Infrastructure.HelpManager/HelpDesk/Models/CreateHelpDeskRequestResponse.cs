namespace Kongrevsky.Infrastructure.HelpManager.HelpDesk.Models
{
    #region << Using >>

    using Newtonsoft.Json;

    #endregion

    public class CreateHelpDeskRequestResponse
    {
        public enum Status
        {
            Success = 1,

            Failure = 2
        }

        [JsonProperty("response")] public ResponseItem Response { get; set; }

        public class ResponseItem
        {
            [JsonProperty("result")] public ResultItem Result { get; set; }

            [JsonProperty("uri")] public string Uri { get; set; }
        }

        public class ResultItem
        {
            [JsonProperty("status")] public Status Status { get; set; }

            [JsonProperty("statusmessage")] public string Message { get; set; }

            [JsonProperty("statuscode")] public int Code { get; set; }
        }
    }
}