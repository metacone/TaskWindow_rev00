using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace TaskWindow
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            System.Timers.Timer objTimer = new System.Timers.Timer();
            objTimer.Interval = 10;
            objTimer.Elapsed += new ElapsedEventHandler(OneHdred_ms);
            objTimer.Start();
        }

        private void BT_Start_Click(object sender, EventArgs e)
        {
            
            ShowWindow(List[ProcessList.SelectedIndex].hWnd, SW_HIDE);
        }

        private void BT_End_Click(object sender, EventArgs e)
        {

            ShowWindow(List[ProcessList.SelectedIndex].hWnd, SW_SHOW);
        }

        public delegate bool EnumWindowCallback(int hwnd, int lParam);

        [DllImport("user32.dll")]
        public static extern int EnumWindows(EnumWindowCallback callback, int y);

        [DllImport("user32.dll")]
        public static extern int GetParent(int hWnd);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(int hWnd, StringBuilder text, int count);
                  
        [DllImport("user32.dll")]
        public static extern long GetWindowLong(int hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern IntPtr GetClassLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;

        const int GCL_HICON = -14; //GetWindowLong을 호출할 때 쓸 인자
        const int GCL_HMODULE = -16;

        

        public void OneHdred_ms(object sender, ElapsedEventArgs e)
        {
            EnumWindowCallback callback = new EnumWindowCallback(EnumWindowsProc);

            int nReturn;
            nReturn = EnumWindows(callback, 0);
            Debug.WriteLine(nReturn);
        }

        delegate bool DsetEnumWindowsProc(int hWnd, int lParam);

        struct TypeList
        {
            public StringBuilder str;
            public int hWnd;
            public int state;
        }
        TypeList[] List;
        Byte[] BitMap;

        public bool EnumWindowsProc(int hWnd, int lParam)
        {

            //윈도우 핸들로 그 윈도우의 스타일을 얻어옴
            UInt32 style = (UInt32)GetWindowLong(hWnd, GCL_HMODULE);
            //해당 윈도우의 캡션이 존재하는지 확인
            if ((style & 0x10000000L) == 0x10000000L && (style & 0x00C00000L) == 0x00C00000L)
            {
                if (GetParent(hWnd) == 0)
                {
                    StringBuilder Buf = new StringBuilder(256);
                    if (GetWindowText(hWnd, Buf, 256) > 0)
                    {
                        if (ProcessList.InvokeRequired)
                        {
                            DsetEnumWindowsProc call = new DsetEnumWindowsProc(EnumWindowsProc);
                            this.Invoke(call, hWnd, lParam);
                        }
                        else
                        {
                            if (List == null)
                            {
                                List = new TypeList[1];


                            }
                            else
                            {
                                for (int index = 0; index < List.Length; index++)
                                {
                                    // Already, Regist
                                    if ((List[index].hWnd) == hWnd)
                                    {
                                        return true;
                                    }
                                }
                                Array.Resize(ref List, List.Length + 1);
                            }
                            List[List.Length - 1].hWnd = hWnd;
                            List[List.Length - 1].str = Buf;
                            ProcessList.Items.Add(Buf);

                            //Bit Map Make
                            if (BitMap == null)
                            {
                                BitMap = new Byte[0];
                            }
                            if (List.Length % 32 == 1)
                            {
                                Array.Resize(ref BitMap, BitMap.Length + 1);
                            }
                        }
                    }
                }
            }
            return true;
        }
        
    }
}
