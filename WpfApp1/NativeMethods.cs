using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace WpfApp1
{
    public static class NativeMethods
    {
        public const int WM_NCCALCSIZE = 0x83;
        public const int WM_NCPAINT = 0x85;

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong32(HandleRef hWnd, WindowLongFlags nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, WindowLongFlags nIndex, IntPtr dwNewLong);

        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        public static IntPtr SetWindowLongPtr(HandleRef hWnd, WindowLongFlags nIndex, IntPtr dwNewLong)
        {
            return IntPtr.Size == 8 ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong) : new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        private delegate int DwmExtendFrameIntoClientAreaDelegate(IntPtr hwnd, ref MARGINS margins);

        public static int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins)
        {
            var hModule = LoadLibrary("dwmapi");

            if (hModule == IntPtr.Zero)
            {
                return 0;
            }

            var procAddress = GetProcAddress(hModule, "DwmExtendFrameIntoClientArea");

            if (procAddress == IntPtr.Zero)
            {
                return 0;
            }

            var delegateForFunctionPointer = (DwmExtendFrameIntoClientAreaDelegate)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(DwmExtendFrameIntoClientAreaDelegate));

            return delegateForFunctionPointer(hwnd, ref margins);
        }

        public static bool IsDwmAvailable()
        {
            if (LoadLibrary("dwmapi") == IntPtr.Zero)
            {
                return false;
            }
            return true;
        }

        public enum WindowLongFlags
        {
            GWL_EXSTYLE = -20,
            GWLP_HINSTANCE = -6,
            GWLP_HWNDPARENT = -8,
            GWL_ID = -12,
            GWL_STYLE = -16,
            GWL_USERDATA = -21,
            GWL_WNDPROC = -4,
            DWLP_USER = 0x8,
            DWLP_MSGRESULT = 0x0,
            DWLP_DLGPROC = 0x4
        }

        internal enum WVR
        {
            ALIGNTOP = 0x0010,
            ALIGNLEFT = 0x0020,
            ALIGNBOTTOM = 0x0040,
            ALIGNRIGHT = 0x0080,
            HREDRAW = 0x0100,
            VREDRAW = 0x0200,
            VALIDRECTS = 0x0400,
            REDRAW = HREDRAW | VREDRAW
        }
    }
}