namespace Kongrevsky.Infrastructure.Web.Middlewares
{
    #region << Using >>

    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    #endregion

    public class RequestBodyStreamReusingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestBodyStreamReusingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Keep the original stream in a separate
            // variable to restore it later if necessary.
            var stream = context.Request.Body;

            // Optimization: don't buffer the request if
            // there was no stream or if it is rewindable.
            if (stream == Stream.Null || stream.CanSeek)
            {
                await _next(context);

                return;
            }

            try
            {
                using (var buffer = new MemoryStream())
                {
                    // Copy the request stream to the memory stream.
                    await stream.CopyToAsync(buffer);

                    // Rewind the memory stream.
                    buffer.Position = 0L;

                    // Replace the request stream by the memory stream.
                    context.Request.Body = buffer;

                    // Invoke the rest of the pipeline.
                    await _next(context);
                }
            }

            finally
            {
                // Restore the original stream.
                context.Request.Body = stream;
            }
        }
    }
}