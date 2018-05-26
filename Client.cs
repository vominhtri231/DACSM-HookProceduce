using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HookHandlerDLL
{
    class Client
    {


        #region client ver2
        /*
        Socket socket;
        private EndPoint epServer;

        public DataSender(string address, int port)
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress iPAddress = IPAddress.Parse(address);
            epServer = new IPEndPoint(iPAddress, port);
            
        }

        public void Send(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            socket.BeginSendTo(buffer,0,buffer.Length,SocketFlags.None,this.epServer,new AsyncCallback(CallBack), null);
        }

        private void CallBack(IAsyncResult ar)
        {        
             socket.EndSend(ar);
        }

        */

        #endregion

        Socket socket;
        IPEndPoint iPEndPoint;
        public Client(string address, int port)
        {
            iPEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(iPEndPoint);

            ClientListener listener=new ClientListener(socket);
            new Thread(listener.Run).Start();
        }

        public void Send(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            socket.Send(new byte[] { 0 });
            SendVarData(buffer);
        }

        public void Send(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] bytes = ms.ToArray();
            socket.Send(new byte[] { 1 });
            SendVarData(bytes);
        }

        private void SendVarData(byte[] data)
        {
            int total = 0;
            int size = data.Length;
            int dataleft = size;
            

            byte[] datasize = new byte[4];
            datasize = BitConverter.GetBytes(size);
            socket.Send(datasize);

            while (total < size)
            {
                int sent = socket.Send(data, total, dataleft, SocketFlags.None);
                total += sent;
                dataleft -= sent;
            }
            
        }
    }
}
