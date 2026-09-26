using System;
using System.Runtime.InteropServices;

namespace CMaurer.Common
{
    /// <summary>
    /// See http://www.pinvoke.net/
    /// </summary>
    public class Win32Interop
    {

        /// <summary>
        /// MessageHeader for WM_NOTIFY
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct NMHDR
        {
            public IntPtr hwndFrom;
            public Int32 idFrom;
            public Int32 code;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HDITEM
        {
            public Int32 mask;
            public Int32 cxy;
            [MarshalAs(UnmanagedType.LPTStr)]
            public String pszText;
            public IntPtr hbm;
            public Int32 cchTextMax;
            public Int32 fmt;
            public Int32 lParam;
            public Int32 iImage;
            public Int32 iOrder;
        };

        [StructLayout(LayoutKind.Sequential)]
        public class LVBKIMAGE
        {
            public int ulFlags;
            public IntPtr hbm;
            public string pszImage;
            public int cchImageMax;
            public int xOffsetPercent;
            public int yOffsetPercent;
        }

/*
        [StructLayout(LayoutKind.Sequential)]
        public struct TOOLINFO
        {
            public int cbSize;
            public int uFlags;
            public IntPtr hwnd;
            public IntPtr uId;
            public RECT rect;
            public IntPtr hinst;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszText;
            public IntPtr lParam;
        }
*/

        // Parameters for ListView-Headers
        public const Int32 HDI_FORMAT = 0x0004;
        public const Int32 HDF_LEFT = 0x0000;
        public const Int32 HDF_STRING = 0x4000;
        public const Int32 HDF_SORTUP = 0x0400;
        public const Int32 HDF_SORTDOWN = 0x0200;
        public const Int32 HDM_GETITEM = 0x1200 + 11;  // HDM_FIRST + 11
        public const Int32 HDM_SETITEM = 0x1200 + 12;  // HDM_FIRST + 12

        // ListView messages
        private const int LVM_FIRST = 0x1000;
        public const int LVM_GETHEADER = LVM_FIRST + 31;
        public const int LVM_SETBKIMAGE = LVM_FIRST + 138;
        public const int LVM_SETTEXTBKCOLOR = LVM_FIRST + 38;
        public const int LVM_SETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 54;
        public const int LVM_GETCOLUMNORDERARRAY = (LVM_FIRST + 59);
        public const int LVS_EX_DOUBLEBUFFER = 0x10000;
        public const int LVBKIF_TYPE_WATERMARK = 0x10000000;
        public const int LVBKIF_SOURCE_NONE = 0;
        public const uint CLR_NONE = 0xFFFFFFFF;


        // Windows Messages that will abort editing
        public const int WM_HSCROLL = 0x114;
        public const int WM_VSCROLL = 0x115;
        public const int WM_SIZE = 0x05;
        public const int WM_NOTIFY = 0x4E;

        private const int HDN_FIRST = -300;
        public const int HDN_BEGINDRAG = (HDN_FIRST - 10);
        public const int HDN_ITEMCHANGINGA = (HDN_FIRST - 0);
        public const int HDN_ITEMCHANGINGW = (HDN_FIRST - 20);
        public const int HDN_ITEMCHANGEDW = (HDN_FIRST - 21);

        public const int SB_HORZ = 0x0;
        public const int SB_VERT = 0x1;


        public const int TTF_SUBCLASS = 0x10;
        public const int TTM_ADDTOOLA = 1028;
        public const int TTM_ADDTOOLW = 1074;
        public const int TTM_UPDATETIPTEXTA = 1036;
        public const int TTM_UPDATETIPTEXTW = 1081;
        public const int TTM_DELTOOLA = 1029;
        public const int TTM_DELTOOLW = 1075;

        public static readonly int TTM_ADDTOOL;
        public static readonly int TTM_UPDATETIPTEXT;
        public static readonly int TTM_DELTOOL;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern IntPtr SendMessageITEM(IntPtr Handle, Int32 msg, IntPtr wParam, ref HDITEM lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wPar, IntPtr lPar);

        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int len, ref	int[] order);

        [DllImport("ole32.dll")]
        public extern static void CoUninitialize();

        [DllImport("ole32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern void CoInitialize(int pvReserved);

        [DllImport("user32.dll", EntryPoint = "SendMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int SendMessage(int hwnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int SendMessage(IntPtr hwnd, uint wMsg, uint wParam, uint lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, LVBKIMAGE lParam);

        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern int GetScrollPos(IntPtr hWnd, int nBar);


        /// <summary>
        /// Benutzt man statt ShowWindow()
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="dwTime"></param>
        /// <param name="dwFlags"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool AnimateWindow(uint hwnd, uint dwTime, uint dwFlags);

/*
        static Win32Interop()
        {
            if (Marshal.SystemDefaultCharSize == 1)
            {
                //Win9x machines

                TTM_ADDTOOL = TTM_ADDTOOLA;
                TTM_UPDATETIPTEXT = TTM_UPDATETIPTEXTA;
                TTM_DELTOOL = TTM_DELTOOLA;
            }
            else
            {
                //WinNT machines

                TTM_ADDTOOL = TTM_ADDTOOLW;
                TTM_UPDATETIPTEXT = TTM_UPDATETIPTEXTW;
                TTM_DELTOOL = TTM_DELTOOLW;
            }
        }
 */
 
    }
}
