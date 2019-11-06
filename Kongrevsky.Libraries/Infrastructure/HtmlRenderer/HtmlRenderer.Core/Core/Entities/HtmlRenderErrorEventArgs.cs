// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

namespace HtmlRenderer.Core.Core.Entities
{
    using System;

    /// <summary>
    /// Raised when an error occurred during html rendering.
    /// </summary>
    public sealed class HtmlRenderErrorEventArgs : EventArgs
    {
        /// <summary>
        /// error type that is reported
        /// </summary>
        private readonly HtmlRenderErrorType _type;

        /// <summary>
        /// the error message
        /// </summary>
        private readonly string _message;

        /// <summary>
        /// the exception that occurred (can be null)
        /// </summary>
        private readonly Exception _exception;

        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="type">the type of error to report</param>
        /// <param name="message">the error message</param>
        /// <param name="exception">optional: the exception that occurred</param>
        public HtmlRenderErrorEventArgs(HtmlRenderErrorType type, string message, Exception exception = null)
        {
            this._type = type;
            this._message = message;
            this._exception = exception;
        }

        /// <summary>
        /// error type that is reported
        /// </summary>
        public HtmlRenderErrorType Type
        {
            get { return this._type; }
        }

        /// <summary>
        /// the error message
        /// </summary>
        public string Message
        {
            get { return this._message; }
        }

        /// <summary>
        /// the exception that occurred (can be null)
        /// </summary>
        public Exception Exception
        {
            get { return this._exception; }
        }

        public override string ToString()
        {
            return string.Format("Type: {0}", this._type);
        }
    }
}