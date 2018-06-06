namespace Kongrevsky.Infrastructure.Web.Controllers
{
    #region << Using >>

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Kongrevsky.Infrastructure.Models;
    using Kongrevsky.Infrastructure.Web.ActionFilters;
    using Kongrevsky.Infrastructure.Web.Models;
    using Kongrevsky.Utilities.File.Mime;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    #endregion

    [ValidateModel]
    [ProducesResponseType(typeof(ErrorRequestModel), 400)]
    [ProducesResponseType(typeof(ErrorRequestModel), 500)]
    [CheckModelForNull]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class KongrevskyController : Controller
    {
        protected IActionResult BadRequest(string message)
        {
            return BadRequest(new List<string>() { message });
        }

        protected new IActionResult BadRequest(ModelStateDictionary modelStateDictionary)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, new ErrorRequestModel(modelStateDictionary.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.Errors.First().ErrorMessage)).ToList()));
        }

        protected IActionResult BadRequest(List<string> messages)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, new ErrorRequestModel(messages.Select(x => new KeyValuePair<string, string>("", x)).Distinct().ToList()));
        }

        protected IActionResult InternalServerError(Exception exception)
        {
            return InternalServerError(exception.Message);
        }

        protected IActionResult InternalServerError(string errorMessage)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorRequestModel(errorMessage));
        }

        protected IActionResult HttpStatusCodeResult(HttpStatusCode code, string message = null)
        {
            return StatusCode((int)code, message == null ? null : new ErrorRequestModel(message));
        }

        protected IActionResult HttpStatusCodeResult<T>(T resultInfo) where T : ResultInfo
        {
            return StatusCode((int)resultInfo.StatusCode, resultInfo.Message == null ? null : new ErrorRequestModel(resultInfo.Message));
        }

        protected IActionResult FileActionResult(Stream stream, string fileName, bool isOctetStream = false)
        {
            var fileType = stream.GetFileType();
            var contentType = fileType == null || isOctetStream ? "application/octet-stream" : fileType.Mime;
            return File(stream, contentType, fileName);
        }

        protected IActionResult OkId(string id)
        {
            return Ok(new IdVm { Id = id });
        }
    }
}