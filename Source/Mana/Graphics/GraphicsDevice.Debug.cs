using System;
using System.Runtime.InteropServices;
using Mana.Utilities.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics
{
    public partial class GraphicsDevice
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private static DebugProc _debugProcCallback;
        
        private void DebugCallback(DebugSource source, 
                                   DebugType type,
                                   int id,
                                   DebugSeverity severity,
                                   int length,
                                   IntPtr message,
                                   IntPtr userParam)
        {
            string msg = Marshal.PtrToStringAnsi(message, length);
            var color = type == DebugType.DebugTypeError ? ConsoleColor.Red : ConsoleColor.Gray;
            
            _glDebugLogger.WriteLine($"{severity.GetName()} {type.GetName()} {msg}", color);
            
            if (type == DebugType.DebugTypeError && severity == DebugSeverity.DebugSeverityHigh)
            {
                throw new GLException(msg);
            }
            
            // For breakpoints:
            
            // if (type == DebugType.DebugTypePerformance)
            // {
            //     { }
            // }
        }
    }
}