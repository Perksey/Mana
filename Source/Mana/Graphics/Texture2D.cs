using System;
using Mana.Asset;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics
{
    public class Texture2D : GraphicsAsset
    {
        public readonly GLHandle Handle;
        
        private bool _disposed = false;
        
        public Texture2D(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            Handle = (GLHandle)GL.GenTexture();
            GLHelper.CheckLastError();
        }

        ~Texture2D()
        {
            Console.WriteLine("WARNING: Texture2D Leaked");
            Dispose(false);
#if DEBUG
            throw new InvalidOperationException("Texture2D Leaked");
#endif
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                GL.DeleteTexture((int)Handle);
                GLHelper.CheckLastError();
            }

            _disposed = true;
            
            base.Dispose(disposing);
        }
    }
}