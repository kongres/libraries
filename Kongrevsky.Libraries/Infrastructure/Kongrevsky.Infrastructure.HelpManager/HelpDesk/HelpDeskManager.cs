namespace Kongrevsky.Infrastructure.HelpManager.HelpDesk
{
    #region << Using >>

    using System;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web;
    using Kongrevsky.Infrastructure.HelpManager.HelpDesk.Models;
    using Kongrevsky.Infrastructure.HelpManager.Models;
    using Kongrevsky.Infrastructure.Models;
    using Kongrevsky.Utilities.Common;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using RestSharp;

    #endregion

    class HelpDeskManager : IHelpDeskManager
    {
        private const string _logFileName = nameof(HelpDeskManager);

        private const string _invalidRequestMessage = "Invalid request";

        private const string _addRequestApiMethod = "api/json/addRequest";

        public HelpDeskManager(IOptions<HelpDeskOptions> options)
        {
            _options = options.Value ?? new HelpDeskOptions();

            if (_options.Url?.EndsWith("/") ?? false)
                _options.Url += "/";
        }

        private HelpDeskOptions _options { get; }

        public Task<ResultInfo> AddRequestAsync(CreateHelpRequest model)
        {
            return RetryUtils.DoAsync(async () =>
                                      {
                                          var baseUrl = new Uri(new Uri(_options.Url), _addRequestApiMethod);
                                          var client = new RestClient(baseUrl);

                                          var request = new RestRequest(Method.POST);
                                          var outgoingQueryString = HttpUtility.ParseQueryString(string.Empty);
                                          outgoingQueryString.Add("apikey", _options.ApiKey);
                                          outgoingQueryString.Add("group", _options.Group);
                                          outgoingQueryString.Add("businessUnit", _options.BusinessUnit);
                                          outgoingQueryString.Add("email", model.Email);
                                          outgoingQueryString.Add("subject", model.Subject);
                                          outgoingQueryString.Add("description", model.Description);
                                          var postdata = outgoingQueryString.ToString();

                                          request.AddParameter("application/x-www-form-urlencoded", postdata, ParameterType.RequestBody);

                                          var response = await client.ExecuteTaskAsync(request);

                                          if (!response.IsSuccessful)
                                          {
                                              LoggerUtils.Log($"HelpDesk API request was failed with code: {response.StatusCode}" + Environment.NewLine
                                                                                                                                  + $"Response: {response.Content}",
                                                              _logFileName);
                                              return new ResultInfo(response.StatusCode, _invalidRequestMessage);
                                          }

                                          var responseModel = JsonConvert.DeserializeObject<CreateHelpDeskRequestResponse>(response.Content);

                                          if (responseModel.Response.Result.Status == CreateHelpDeskRequestResponse.Status.Failure)
                                          {
                                              LoggerUtils.Log($"HelpDesk API AddRequest was failed with message: {responseModel.Response.Result.Message}", _logFileName);
                                              return new ResultInfo(HttpStatusCode.BadRequest, _invalidRequestMessage);
                                          }

                                          return new ResultInfo(HttpStatusCode.OK);
                                      },
                                      TimeSpan.FromMilliseconds(500),
                                      3,
                                      false,
                                      exception => { LoggerUtils.Log("HelpDeskManager exception", exception, _logFileName); });
        }
    }
}