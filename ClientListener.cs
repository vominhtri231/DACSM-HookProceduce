using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HookHandlerDLL
{
    class ClientListener
    {
        Socket socket;

        public ClientListener(Socket socket)
        {
            this.socket = socket;
        }

        public void Run()
        {
            try
            {
                byte[] type = new byte[1];
                socket.Receive(type);
                byte[] data = ReceiveVarData(socket);

                switch (type[0])
                {
                    case 0:
                        string message = Encoding.UTF8.GetString(data);
                        break;
                    case 1:
                        MemoryStream ms = new MemoryStream(data);
                        Image bmp = Image.FromStream(ms);
                        break;
                    case 2:
                        break;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private byte[] ReceiveVarData(Socket client)
        {

            byte[] datasize = new byte[4];
            client.Receive(datasize, 0, 4, 0);
            int size = BitConverter.ToInt32(datasize, 0);

            int total = 0;
            int dataleft = size;
            byte[] data = new byte[size];


            while (total < size)
            {
                int recv = client.Receive(data, total, dataleft, 0);
                if (recv == 0)
                {
                    break;
                }
                total += recv;
                dataleft -= recv;
            }
            return data;
        }
    }
}
