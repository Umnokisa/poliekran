using System;
using System.Runtime.InteropServices;

namespace Poliekran
{
    static class Hook
    {
        const int WH_GETMESSAGE = 3, PM_REMOVE = 1;

        [StructLayout(LayoutKind.Sequential)]
        struct MSG { public IntPtr hwnd; public uint message; public IntPtr wParam, lParam; public uint time; public int x, y; }

        delegate IntPtr Proc(int code, IntPtr w, IntPtr l);

        [DllImport("user32.dll", SetLastError = true)] static extern IntPtr SetWindowsHookEx(int id, Proc f, IntPtr mod, uint tid);
        [DllImport("user32.dll")] static extern bool UnhookWindowsHookEx(IntPtr h);
        [DllImport("user32.dll")] static extern IntPtr CallNextHookEx(IntPtr h, int code, IntPtr w, IntPtr l);
        [DllImport("kernel32.dll")] static extern uint GetCurrentThreadId();

        static IntPtr hh;
        static Proc keep;
        public static event Action<IntPtr, bool> Input;

        public static void On()
        {
            if (hh != IntPtr.Zero) return;
            keep = Cb;
            hh = SetWindowsHookEx(WH_GETMESSAGE, keep, IntPtr.Zero, GetCurrentThreadId());
            L.W(hh == IntPtr.Zero ? "Hook: failed " + Marshal.GetLastWin32Error() : "Hook: on");
        }

        public static void Off()
        {
            if (hh == IntPtr.Zero) return;
            UnhookWindowsHookEx(hh);
            hh = IntPtr.Zero;
        }

        static IntPtr Cb(int code, IntPtr w, IntPtr l)
        {
            try
            {
                int msg = code >= 0 && w.ToInt64() == PM_REMOVE && l != IntPtr.Zero ? Marshal.ReadInt32(l, IntPtr.Size) : 0;
                if (msg == W.WM_KEYDOWN || msg == W.WM_SYSKEYDOWN || msg == W.WM_RBUTTONUP || msg == W.WM_CONTEXTMENU)
                {
                    var m = Marshal.PtrToStructure<MSG>(l);
                    bool key = m.message == W.WM_KEYDOWN || m.message == W.WM_SYSKEYDOWN;
                    bool exitKey = false, menuKey = false;
                    if (key)
                    {
                        int vk = (int)(m.wParam.ToInt64() & 0xFF);
                        exitKey = vk == 0x1B || vk == 0x03 || vk == 0xBD || vk == 0x6D;
                        menuKey = vk == 0x5D || vk == 0x79;
                    }
                    bool rc = menuKey || m.message == W.WM_RBUTTONUP || m.message == W.WM_CONTEXTMENU;
                    if ((exitKey || rc) && m.hwnd != IntPtr.Zero)
                        Input?.Invoke(W.GetAncestor(m.hwnd, W.GA_ROOT), exitKey);
                }
            }
            catch (Exception ex) { L.E("Hook", ex); }
            return CallNextHookEx(hh, code, w, l);
        }
    }
}
