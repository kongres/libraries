namespace Kongrevsky.Utilities.Smtp
{
    #region << Using >>

    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using Kongrevsky.Utilities.Smtp.Models;

    #endregion

    public static class SmtpUtils
    {
        /// <summary>
        /// Tests the SMTP connection by sending a 'HELLO' command
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
                        return false;

                    // send HELO and test the response for code 250 = proper response
                    SendData(tcpSocket, $"HELO {Dns.GetHostName()}\r\n");
                    if (!CheckResponse(tcpSocket, 250))
                        return false;

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
            var dataArray = Encoding.ASCII.GetBytes(data);
            socket.Send(dataArray, 0, dataArray.Length, SocketFlags.None);
        }

        private static bool CheckResponse(Socket socket, int expectedCode)
        {
            while (socket.Available == 0)
                System.Threading.Thread.Sleep(100);

            var responseArray = new byte[1024];
            socket.Receive(responseArray, 0, socket.Available, SocketFlags.None);
            var responseData = Encoding.ASCII.GetString(responseArray);
            var responseCode = Convert.ToInt32(responseData.Substring(0, 3));
            if (responseCode == expectedCode)
                return true;

            return false;
        }

        /// <summary>
        /// Validates SMTP credentials
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <param name="enableSsl"></param>
        /// <returns></returns>
        public static bool ValidateCredentials(string login, string password, string server, int port, bool enableSsl)
        {
            SmtpConnectorBase connector;
            if (enableSsl)
                connector = new SmtpConnectorWithSsl(server, port);
            else
                connector = new SmtpConnectorWithoutSsl(server, port);

            if (!connector.CheckResponse(220))
                return false;

            connector.SendData($"HELO {Dns.GetHostName()}{SmtpConnectorBase.EOF}");
            if (!connector.CheckResponse(250))
                return false;

            connector.SendData($"AUTH LOGIN{SmtpConnectorBase.EOF}");
            if (!connector.CheckResponse(334))
                return false;

            connector.SendData(Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}")) + SmtpConnectorBase.EOF);
            if (!connector.CheckResponse(334))
                return false;

            connector.SendData(Convert.ToBase64String(Encoding.UTF8.GetBytes($"{password}")) + SmtpConnectorBase.EOF);
            if (!connector.CheckResponse(235))
                return false;

            return true;
        }
    }
}