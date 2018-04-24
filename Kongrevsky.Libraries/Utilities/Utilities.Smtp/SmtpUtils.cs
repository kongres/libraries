namespace Kongrevsky.Utilities.Smtp
{
    using System;
    using System.Net;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    public static class SmtpUtils
    {
        /// <summary>
        /// test the smtp connection by sending a HELO command
        /// </summary>
        /// <param name="smtpServerAddress"></param>
        /// <param name="port"></param>
        public static bool TestConnection(string smtpServerAddress, int port)
        {
            try
            {
                var hostEntry = Dns.GetHostEntry(smtpServerAddress);
                var endPoint = new IPEndPoint(hostEntry.AddressList[0], port);
                using (var tcpSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    //try to connect and test the rsponse for code 220 = success
                    tcpSocket.Connect(endPoint);
                    if (!CheckResponse(tcpSocket, 220))
                    {
                        return false;
                    }

                    // send HELO and test the response for code 250 = proper response
                    SendData(tcpSocket, $"HELO {Dns.GetHostName()}\r\n");
                    if (!CheckResponse(tcpSocket, 250))
                    {
                        return false;
                    }

                    // if we got here it's that we can connect to the smtp server
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private static void SendData(Socket socket, string data)
        {
            byte[] dataArray = Encoding.ASCII.GetBytes(data);
            socket.Send(dataArray, 0, dataArray.Length, SocketFlags.None);
        }

        private static bool CheckResponse(Socket socket, int expectedCode)
        {
            while (socket.Available == 0)
            {
                System.Threading.Thread.Sleep(100);
            }
            byte[] responseArray = new byte[1024];
            socket.Receive(responseArray, 0, socket.Available, SocketFlags.None);
            string responseData = Encoding.ASCII.GetString(responseArray);
            int responseCode = Convert.ToInt32(responseData.Substring(0, 3));
            if (responseCode == expectedCode)
            {
                return true;
            }
            return false;
        }

        public static bool ValidateCredentials(string login, string password, string server, int port, bool enableSsl)
        {
            SmtpConnectorBase connector;
            if (enableSsl)
            {
                connector = new SmtpConnectorWithSsl(server, port);
            }
            else
            {
                connector = new SmtpConnectorWithoutSsl(server, port);
            }

            if (!connector.CheckResponse(220))
            {
                return false;
            }

            connector.SendData($"HELO {Dns.GetHostName()}{SmtpConnectorBase.EOF}");
            if (!connector.CheckResponse(250))
            {
                return false;
            }

            connector.SendData($"AUTH LOGIN{SmtpConnectorBase.EOF}");
            if (!connector.CheckResponse(334))
            {
                return false;
            }

            connector.SendData(Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}")) + SmtpConnectorBase.EOF);
            if (!connector.CheckResponse(334))
            {
                return false;
            }

            connector.SendData(Convert.ToBase64String(Encoding.UTF8.GetBytes($"{password}")) + SmtpConnectorBase.EOF);
            if (!connector.CheckResponse(235))
            {
                return false;
            }

            return true;
        }

    }

    internal abstract class SmtpConnectorBase
    {
        protected string SmtpServerAddress { get; set; }
        protected int Port { get; set; }
        public const string EOF = "\r\n";

        protected SmtpConnectorBase(string smtpServerAddress, int port)
        {
            SmtpServerAddress = smtpServerAddress;
            Port = port;
        }

        public abstract bool CheckResponse(int expectedCode);
        public abstract void SendData(string data);
    }

    internal class SmtpConnectorWithSsl : SmtpConnectorBase
    {
        private SslStream _sslStream = null;
        private TcpClient _client = null;

        public SmtpConnectorWithSsl(string smtpServerAddress, int port) : base(smtpServerAddress, port)
        {
            TcpClient client = new TcpClient(smtpServerAddress, port);

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
                {
                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                }
                Console.WriteLine("Authentication failed - closing the connection.");
                client.Close();
            }
        }

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
            {
                return false;
            }
            var message = ReadMessageFromStream(this._sslStream);
            int responseCode = Convert.ToInt32(message.Substring(0, 3));
            if (responseCode == expectedCode)
            {
                return true;
            }
            return false;
        }

        public override void SendData(string data)
        {
            byte[] messsage = Encoding.UTF8.GetBytes(data);
            // Send hello message to the server. 
            this._sslStream.Write(messsage);
            this._sslStream.Flush();
        }

        private string ReadMessageFromStream(SslStream stream)
        {
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do
            {
                bytes = stream.Read(buffer, 0, buffer.Length);

                // Use Decoder class to convert from bytes to UTF8
                // in case a character spans two buffers.
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
                // Check for EOF.
                if (messageData.ToString().IndexOf(EOF) != -1)
                {
                    break;
                }
            } while (bytes != 0);

            return messageData.ToString();
        }
    }

    internal class SmtpConnectorWithoutSsl : SmtpConnectorBase
    {
        private Socket _socket = null;

        public SmtpConnectorWithoutSsl(string smtpServerAddress, int port) : base(smtpServerAddress, port)
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(smtpServerAddress);
            IPEndPoint endPoint = new IPEndPoint(hostEntry.AddressList[0], port);
            this._socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //try to connect and test the rsponse for code 220 = success
            this._socket.Connect(endPoint);

        }

        ~SmtpConnectorWithoutSsl()
        {
            try
            {
                if (this._socket != null)
                {
                    this._socket.Close();
                    this._socket.Dispose();
                    this._socket = null;
                }
            }
            catch (Exception)
            {
                ;
            }

        }

        public override bool CheckResponse(int expectedCode)
        {
            while (this._socket.Available == 0)
            {
                System.Threading.Thread.Sleep(100);
            }
            byte[] responseArray = new byte[1024];
            this._socket.Receive(responseArray, 0, this._socket.Available, SocketFlags.None);
            string responseData = Encoding.UTF8.GetString(responseArray);
            int responseCode = Convert.ToInt32(responseData.Substring(0, 3));
            if (responseCode == expectedCode)
            {
                return true;
            }
            return false;
        }

        public override void SendData(string data)
        {
            byte[] dataArray = Encoding.UTF8.GetBytes(data);
            this._socket.Send(dataArray, 0, dataArray.Length, SocketFlags.None);
        }
    }
}