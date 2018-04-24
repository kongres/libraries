namespace Kongrevsky.Utilities.Smtp.Models
{
    internal abstract class SmtpConnectorBase
    {
        #region Constants

        public const string EOF = "\r\n";

        #endregion

        #region Properties

        protected string SmtpServerAddress { get; set; }

        protected int Port { get; set; }

        #endregion

        #region Constructors

        protected SmtpConnectorBase(string smtpServerAddress, int port)
        {
            SmtpServerAddress = smtpServerAddress;
            Port = port;
        }

        #endregion

        /// <summary>
        /// Detects if response is valid
        /// </summary>
        /// <param name="expectedCode"></param>
        /// <returns></returns>
        public abstract bool CheckResponse(int expectedCode);

        /// <summary>
        /// Sends data
        /// </summary>
        /// <param name="data"></param>
        public abstract void SendData(string data);
    }
}