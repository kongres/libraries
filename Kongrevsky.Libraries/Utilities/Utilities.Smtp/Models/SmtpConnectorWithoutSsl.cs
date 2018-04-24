namespace Kongrevsky.Utilities.Smtp.Models
{
    #region << Using >>

    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    #endregion

    internal class SmtpConnectorWithoutSsl : SmtpConnectorBase
    {
        #region Properties

        private Socket _socket = null;

        #endregion

        #region Constructors

        public SmtpConnectorWithoutSsl(string smtpServerAddress, int port) : base(smtpServerAddress, port)
        {
            var hostEntry = Dns.GetHostEntry(smtpServerAddress);
            var endPoint = new IPEndPoint(hostEntry.AddressList[0], port);
            this._socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //try to connect and test the rsponse for code 220 = success
            this._socket.Connect(endPoint);
        }

        #endregion

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
                System.Threading.Thread.Sleep(100);

            var responseArray = new byte[1024];
            this._socket.Receive(responseArray, 0, this._socket.Available, SocketFlags.None);
            var responseData = Encoding.UTF8.GetString(responseArray);
            var responseCode = Convert.ToInt32(responseData.Substring(0, 3));
            if (responseCode == expectedCode)
                return true;

            return false;
        }

        public override void SendData(string data)
        {
            var dataArray = Encoding.UTF8.GetBytes(data);
            this._socket.Send(dataArray, 0, dataArray.Length, SocketFlags.None);
        }
    }
}