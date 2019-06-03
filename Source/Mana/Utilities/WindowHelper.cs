using System;
using System.Runtime.InteropServices;

namespace Mana.Utilities
{
    public static class WindowHelper
    {
        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int LWA_ALPHA = 0x2;
        private const int LWA_COLORKEY = 0x1;

        public static void SetAlpha(ManaWindow window, float alpha)
        {
            if (alpha < 0.0f || alpha > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(alpha));

            var handle = window.WindowInfo.Handle;

            SetWindowLongPtr(new HandleRef(window, handle), GWL_EXSTYLE, new IntPtr(GetWindowLongPtr(handle, GWL_EXSTYLE).ToInt32() ^ WS_EX_LAYERED));
            SetLayeredWindowAttributes(handle, 0, (byte)(alpha * byte.MaxValue), LWA_ALPHA);
        }
        
        public static IntPtr SetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 8)
                return GetWindowLongPtr64(hWnd, nIndex);
            else
                return GetWindowLongPtr32(hWnd, nIndex);
        }

        [DllImport("user32.dll", EntryPoint="SetWindowLong")]
        private static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint="SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);
        
        [DllImport("user32.dll", EntryPoint="GetWindowLong")]
        private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);
        
        [DllImport("user32.dll", EntryPoint="GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    }
}