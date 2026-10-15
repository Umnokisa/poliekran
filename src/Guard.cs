using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Poliekran
{
    class Guard : NativeWindow
    {
        readonly IntPtr root;
        readonly string dev;
        Rectangle mon, tgt;
        bool locked, busy;
        int fights;
        public IntPtr Pane = IntPtr.Zero;
        public DateTime ExitKey = DateTime.MinValue, RClick = DateTime.MinValue;

        public Guard(IntPtr hwnd, Screen s)
        {
            root = W.GetAncestor(hwnd, W.GA_ROOT);
            if (root == IntPtr.Zero) root = hwnd;
            dev = s.DeviceName;
            mon = s.Bounds;
            AssignHandle(root);
        }

        public IntPtr Root => root;

        static string R(W.RECT r) => r.L + "," + r.T + " " + (r.R - r.L) + "x" + (r.B - r.T);

        public void Fit()
        {
            var old = W.DpiOf(root);
            try
            {
                locked = false;
                mon = W.Mon(dev) ?? mon;
                W.GetWindowRect(root, out var r0);
                bool z = W.IsZoomed(root);
                if (z) W.ShowWindow(root, W.SW_SHOWNOACTIVATE);
                long st = W.GetLong(root, W.GWL_STYLE).ToInt64();
                st &= ~(W.WS_CAPTION | W.WS_THICKFRAME | W.WS_SYSMENU | W.WS_MINIMIZEBOX | W.WS_MAXIMIZEBOX | W.WS_MAXIMIZE);
                W.SetLong(root, W.GWL_STYLE, new IntPtr(st));
                tgt = mon;
                Place(true);
                for (int i = 0; i < 4; i++)
                {
                    var c = Inner();
                    if (c == null) break;
                    W.GetWindowRect(root, out var r);
                    var p = c.Value;
                    int dl = p.L - r.L, dt = p.T - r.T, dr = r.R - p.R, db = r.B - p.B;
                    var n = new Rectangle(mon.X - dl, mon.Y - dt, mon.Width + dl + dr, mon.Height + dt + db);
                    if (n == tgt) break;
                    tgt = n;
                    Place(false);
                }
                W.GetWindowRect(root, out var f);
                var rgn = W.CreateRectRgn(mon.X - f.L, mon.Y - f.T, mon.Right - f.L, mon.Bottom - f.T);
                if (W.SetWindowRgn(root, rgn, true) == 0) W.DeleteObject(rgn);
                locked = true;
                var pi = Inner();
                L.W("Fit: was " + R(r0) + (z ? " maximized" : "") + " -> window " + R(f) + ", pane " + (Pane != IntPtr.Zero ? W.Cls(Pane) : "?") + " " + (pi.HasValue ? R(pi.Value) : "?") + ", monitor " + dev + " " + mon);
            }
            finally { W.DpiBack(old); }
        }

        void Place(bool frame)
        {
            W.SetWindowPos(root, IntPtr.Zero, tgt.X, tgt.Y, tgt.Width, tgt.Height,
                W.SWP_NOZORDER | W.SWP_NOACTIVATE | W.SWP_SHOWWINDOW | (frame ? W.SWP_FRAMECHANGED : 0));
        }

        W.RECT? Inner()
        {
            IntPtr bh = IntPtr.Zero, dc = IntPtr.Zero;
            long ba = 0;
            W.EnumChildWindows(root, (h, _) =>
            {
                if (!W.IsWindowVisible(h)) return true;
                if (dc == IntPtr.Zero && W.Cls(h) == "paneClassDC") dc = h;
                W.GetWindowRect(h, out var r);
                long a = (long)(r.R - r.L) * (r.B - r.T);
                if (a > ba) { ba = a; bh = h; }
                return true;
            }, IntPtr.Zero);
            var x = dc != IntPtr.Zero ? dc : bh;
            if (x == IntPtr.Zero) return null;
            Pane = x;
            W.GetWindowRect(x, out var rr);
            return rr;
        }

        public void Enforce()
        {
            if (Handle == IntPtr.Zero) return;
            if (W.IsZoomed(root)) { Fit(); return; }
            var old = W.DpiOf(root);
            try
            {
                var m = W.Mon(dev);
                if (m.HasValue && m.Value != mon) { L.W("Monitor " + dev + " changed to " + m.Value); W.DpiBack(old); old = IntPtr.Zero; Fit(); return; }
                if (W.IsIconic(root) || !W.IsWindowVisible(root)) W.ShowWindow(root, W.SW_SHOWNOACTIVATE);
                W.GetWindowRect(root, out var r);
                if (r.L != tgt.X || r.T != tgt.Y || r.R != tgt.Right || r.B != tgt.Bottom)
                {
                    if (fights++ < 5) L.W("Enforce: window moved to " + R(r) + ", putting back");
                    Place(false);
                }
            }
            finally { W.DpiBack(old); }
        }

        public void Release()
        {
            locked = false;
            if (Handle != IntPtr.Zero) ReleaseHandle();
        }

        protected override void WndProc(ref Message m)
        {
            bool done = false;
            try { done = Filter(ref m); }
            catch (Exception ex) { L.E("Guard.Filter", ex); }
            if (!done) base.WndProc(ref m);
        }

        bool Filter(ref Message m)
        {
            switch (m.Msg)
            {
                case W.WM_KEYDOWN:
                case W.WM_SYSKEYDOWN:
                    int vk = (int)(m.WParam.ToInt64() & 0xFF);
                    if (vk == 0x1B || vk == 0x03 || vk == 0xBD || vk == 0x6D) ExitKey = DateTime.Now;
                    break;
                case W.WM_RBUTTONUP:
                case W.WM_CONTEXTMENU:
                    RClick = DateTime.Now;
                    break;
                case W.WM_ACTIVATE:
                    if ((m.WParam.ToInt64() & 0xFFFF) == 0) { m.Result = IntPtr.Zero; return true; }
                    break;
                case W.WM_ACTIVATEAPP:
                    if (m.WParam == IntPtr.Zero) { m.Result = IntPtr.Zero; return true; }
                    break;
                case W.WM_NCACTIVATE:
                    if (m.WParam == IntPtr.Zero) { m.Result = new IntPtr(1); return true; }
                    break;
                case W.WM_SYSCOMMAND:
                    long sc = m.WParam.ToInt64() & 0xFFF0;
                    if (sc == W.SC_MINIMIZE || sc == W.SC_MAXIMIZE || sc == W.SC_CLOSE) { m.Result = IntPtr.Zero; return true; }
                    break;
                case W.WM_GETMINMAXINFO:
                    if (m.LParam != IntPtr.Zero && tgt.Width > 0)
                    {
                        var mm = Marshal.PtrToStructure<W.MINMAXINFO>(m.LParam);
                        mm.maxSize.X = Math.Max(mm.maxSize.X, tgt.Width); mm.maxSize.Y = Math.Max(mm.maxSize.Y, tgt.Height);
                        mm.maxTrack.X = Math.Max(mm.maxTrack.X, tgt.Width); mm.maxTrack.Y = Math.Max(mm.maxTrack.Y, tgt.Height);
                        Marshal.StructureToPtr(mm, m.LParam, false);
                        m.Result = IntPtr.Zero;
                        return true;
                    }
                    break;
                case W.WM_WINDOWPOSCHANGING:
                    if (locked && !busy && m.LParam != IntPtr.Zero && !W.IsIconic(root))
                    {
                        busy = true;
                        try
                        {
                            var p = Marshal.PtrToStructure<W.WINDOWPOS>(m.LParam);
                            if ((p.flags & W.SWP_NOMOVE) == 0) { p.x = tgt.X; p.y = tgt.Y; }
                            if ((p.flags & W.SWP_NOSIZE) == 0) { p.cx = tgt.Width; p.cy = tgt.Height; }
                            Marshal.StructureToPtr(p, m.LParam, false);
                        }
                        finally { busy = false; }
                    }
                    break;
            }
            return false;
        }
    }
}
