namespace Kongrevsky.Utilities.Smtp.Models
{
    #region << Using >>

    using System;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    #endregion

    internal class SmtpConnectorWithSsl : SmtpConnectorBase
    {
        #region Properties

        private TcpClient _client = null;

        private SslStream _sslStream = null;

        #endregion

        #region Constructors

        public SmtpConnectorWithSsl(string smtpServerAddress, int port) : base(smtpServerAddress, port)
        {
            var client = new TcpClient(smtpServerAddress, port);

            this._sslStream = new SslStream(
                                            client.GetStream(),
                                            false,
                                            new RemoteCertificateValidationCallback(ValidateServerCertificate),
                                            null
                                           );

            // The server name must match the name on the server certificate.
            try
            {
                this._sslStream.AuthenticateAsClient(smtpServerAddress);
            }
            catch (AuthenticationException e)
            {
                this._sslStream = null;
                Console.WriteLine("Exception: {0}", e.Message);
                if (e.InnerException != null)
                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);

                Console.WriteLine("Authentication failed - closing the connection.");
                client.Close();
            }
        }

        #endregion

        ~SmtpConnectorWithSsl()
        {
            try
            {
                if (this._sslStream != null)
                {
                    this._sslStream.Close();
                    this._sslStream.Dispose();
                    this._sslStream = null;
                }
            }
            catch (Exception)
            {
                ;
            }

            try
            {
                if (this._client != null)
                {
                    this._client.Close();
                    this._client = null;
                }
            }
            catch (Exception)
            {
                ;
            }
        }

        // The following method is invoked by the RemoteCertificateValidationDelegate.
        private static bool ValidateServerCertificate(
                object sender,
                X509Certificate certificate,
                X509Chain chain,
                SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        public override bool CheckResponse(int expectedCode)
        {
            if (this._sslStream == null)
                return false;

            var message = ReadMessageFromStream(this._sslStream);
            var responseCode = Convert.ToInt32(message.Substring(0, 3));
            if (responseCode == expectedCode)
                return true;

            return false;
        }

        public override void SendData(string data)
        {
            var messsage = Encoding.UTF8.GetBytes(data);
            // Send hello message to the server. 
            this._sslStream.Write(messsage);
            this._sslStream.Flush();
        }

        private string ReadMessageFromStream(SslStream stream)
        {
            var buffer = new byte[2048];
            var messageData = new StringBuilder();
            var bytes = -1;
            do
            {
                bytes = stream.Read(buffer, 0, buffer.Length);

                // Use Decoder class to convert from bytes to UTF8
                // in case a character spans two buffers.
                var decoder = Encoding.UTF8.GetDecoder();
                var chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
                // Check for EOF.
                if (messageData.ToString().IndexOf(EOF) != -1)
                    break;
            }
            while (bytes != 0);

            return messageData.ToString();
        }
    }
}