using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Poliekran
{
    static class W
    {
        public const int GWL_STYLE = -16, GA_ROOT = 2, SW_SHOWNOACTIVATE = 4;
        public const long WS_CAPTION = 0x00C00000L, WS_THICKFRAME = 0x00040000L, WS_SYSMENU = 0x00080000L,
            WS_MINIMIZEBOX = 0x00020000L, WS_MAXIMIZEBOX = 0x00010000L, WS_MAXIMIZE = 0x01000000L;
        public const uint SWP_NOSIZE = 0x1, SWP_NOMOVE = 0x2, SWP_NOZORDER = 0x4, SWP_NOACTIVATE = 0x10,
            SWP_FRAMECHANGED = 0x20, SWP_SHOWWINDOW = 0x40, SWP_HIDEWINDOW = 0x80;
        public const int WM_ACTIVATE = 0x0006, WM_ACTIVATEAPP = 0x001C, WM_WINDOWPOSCHANGING = 0x0046,
            WM_NCACTIVATE = 0x0086, WM_SYSCOMMAND = 0x0112, WM_GETMINMAXINFO = 0x0024, WM_CONTEXTMENU = 0x007B,
            WM_KEYDOWN = 0x0100, WM_KEYUP = 0x0101, WM_SYSKEYDOWN = 0x0104, WM_RBUTTONUP = 0x0205,
            VK_LEFT = 0x25, VK_RIGHT = 0x27;
        public const long SC_MINIMIZE = 0xF020, SC_MAXIMIZE = 0xF030, SC_CLOSE = 0xF060;

        [StructLayout(LayoutKind.Sequential)] public struct POINT { public int X, Y; }
        [StructLayout(LayoutKind.Sequential)] public struct MINMAXINFO { public POINT reserved, maxSize, maxPos, minTrack, maxTrack; }
        [StructLayout(LayoutKind.Sequential)] public struct RECT { public int L, T, R, B; }
        [StructLayout(LayoutKind.Sequential)] public struct WINDOWPOS { public IntPtr hwnd, after; public int x, y, cx, cy; public uint flags; }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct MONITORINFOEX
        {
            public int cbSize;
            public RECT rcMonitor, rcWork;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string szDevice;
        }

        public delegate bool EnumProc(IntPtr h, IntPtr p);
        delegate bool MonProc(IntPtr hm, IntPtr dc, ref RECT r, IntPtr p);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongW")] static extern int G32(IntPtr h, int i);
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")] static extern IntPtr G64(IntPtr h, int i);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongW")] static extern int S32(IntPtr h, int i, int v);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW")] static extern IntPtr S64(IntPtr h, int i, IntPtr v);

        public static IntPtr GetLong(IntPtr h, int i) => IntPtr.Size == 8 ? G64(h, i) : new IntPtr(G32(h, i));
        public static void SetLong(IntPtr h, int i, IntPtr v) { if (IntPtr.Size == 8) S64(h, i, v); else S32(h, i, v.ToInt32()); }

        [DllImport("user32.dll")] public static extern IntPtr GetAncestor(IntPtr h, int f);
        [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr h, out RECT r);
        [DllImport("user32.dll")] public static extern bool SetWindowPos(IntPtr h, IntPtr after, int x, int y, int w, int hh, uint f);
        [DllImport("user32.dll")] public static extern bool IsIconic(IntPtr h);
        [DllImport("user32.dll")] public static extern bool IsWindow(IntPtr h);
        [DllImport("user32.dll")] public static extern bool IsZoomed(IntPtr h);
        [DllImport("user32.dll")] public static extern bool IsWindowVisible(IntPtr h);
        [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr h, int c);
        [DllImport("user32.dll")] public static extern bool PostMessage(IntPtr h, int msg, IntPtr w, IntPtr l);
        [DllImport("user32.dll")] public static extern int ShowCursor(bool show);
        [DllImport("user32.dll")] public static extern bool EnumChildWindows(IntPtr h, EnumProc f, IntPtr p);
        [DllImport("user32.dll")] public static extern bool EnumWindows(EnumProc f, IntPtr p);
        [DllImport("user32.dll")] public static extern uint GetWindowThreadProcessId(IntPtr h, out uint pid);
        [DllImport("user32.dll")] public static extern int SetWindowRgn(IntPtr h, IntPtr rgn, bool redraw);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern int GetClassName(IntPtr h, StringBuilder sb, int n);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern int GetWindowText(IntPtr h, StringBuilder sb, int n);
        [DllImport("user32.dll")] static extern bool EnumDisplayMonitors(IntPtr dc, IntPtr clip, MonProc f, IntPtr p);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern bool GetMonitorInfo(IntPtr hm, ref MONITORINFOEX mi);
        [DllImport("user32.dll")] static extern IntPtr GetWindowDpiAwarenessContext(IntPtr h);
        [DllImport("user32.dll")] static extern IntPtr SetThreadDpiAwarenessContext(IntPtr c);
        [DllImport("gdi32.dll")] public static extern IntPtr CreateRectRgn(int l, int t, int r, int b);
        [DllImport("gdi32.dll")] public static extern bool DeleteObject(IntPtr h);

        static readonly uint pid = (uint)System.Diagnostics.Process.GetCurrentProcess().Id;

        public static void CursorOn()
        {
            int c = ShowCursor(true);
            if (c > 0) { ShowCursor(false); return; }
            for (int i = 0; c < 0 && i < 50; i++) c = ShowCursor(true);
        }

        public static string Cls(IntPtr h)
        {
            var sb = new StringBuilder(256);
            GetClassName(h, sb, sb.Capacity);
            return sb.ToString();
        }

        public static string Txt(IntPtr h)
        {
            var sb = new StringBuilder(512);
            GetWindowText(h, sb, sb.Capacity);
            return sb.ToString();
        }

        public static List<IntPtr> Tops()
        {
            var r = new List<IntPtr>();
            EnumWindows((h, _) =>
            {
                GetWindowThreadProcessId(h, out var p);
                if (p == pid) r.Add(h);
                return true;
            }, IntPtr.Zero);
            return r;
        }

        public static Rectangle? Mon(string dev)
        {
            Rectangle? res = null;
            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hm, IntPtr dc, ref RECT rc, IntPtr p) =>
            {
                var mi = new MONITORINFOEX { cbSize = Marshal.SizeOf(typeof(MONITORINFOEX)) };
                if (GetMonitorInfo(hm, ref mi) && string.Equals(mi.szDevice, dev, StringComparison.OrdinalIgnoreCase))
                {
                    var m = mi.rcMonitor;
                    res = new Rectangle(m.L, m.T, m.R - m.L, m.B - m.T);
                    return false;
                }
                return true;
            }, IntPtr.Zero);
            return res;
        }

        static bool noDpi;

        public static IntPtr DpiOf(IntPtr h)
        {
            if (noDpi) return IntPtr.Zero;
            try { return SetThreadDpiAwarenessContext(GetWindowDpiAwarenessContext(h)); }
            catch (EntryPointNotFoundException) { noDpi = true; return IntPtr.Zero; }
        }

        public static void DpiBack(IntPtr old)
        {
            if (old == IntPtr.Zero || noDpi) return;
            try { SetThreadDpiAwarenessContext(old); } catch (EntryPointNotFoundException) { noDpi = true; }
        }
    }
}
