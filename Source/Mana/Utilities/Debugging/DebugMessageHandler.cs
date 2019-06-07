using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Utilities
{
    public static class DebugMessageHandler
    {
        private static Logger _log = new Logger("OpenGL");

        public static event DebugMessageFunc DebugMessage;
        
        internal static void Initialize()
        {
            GL.DebugMessageCallback(_debugProcCallback, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);

            GC.Collect();
        }
        
        private static DebugProc _debugProcCallback = DebugCallback;

        [DebuggerStepThrough]
        private static void DebugCallback(DebugSource source,
                                          DebugType type,
                                          int id,
                                          DebugSeverity severity,
                                          int length,
                                          IntPtr message,
                                          IntPtr userParam)
        {
            string messageString = Marshal.PtrToStringAnsi(message, length);
            DebugMessage?.Invoke(messageString, source, type, severity, id);
            
            _log.Debug($"{severity.GetName()} {type.GetName()} | {messageString}");

            if (type == DebugType.DebugTypeError && severity == DebugSeverity.DebugSeverityHigh)
            {
                throw new Exception(messageString);
            }
        }
        
        public delegate void DebugMessageFunc(string message,
                                              DebugSource source, 
                                              DebugType type,
                                              DebugSeverity severity, 
                                              int id);
    }
}