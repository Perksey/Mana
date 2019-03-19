using System;
using Mana.Asset;
using Mana.Logging;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics
{
    public class Texture2D : ManaAsset, IGraphicsResource
    {
        private static Logger _log = Logger.Create();
        
        public readonly GLHandle Handle;
        
        internal bool Disposed = false;
        
        public Texture2D(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            GraphicsDevice.Resources.Add(this);
            
            Handle = (GLHandle)GL.GenTexture();
            GLHelper.CheckLastError();
        }

        ~Texture2D()
        {
            _log.Error("Texture2D Leaked");
#if DEBUG
            throw new InvalidOperationException("Texture2D Leaked");
#endif
        }

        public GraphicsDevice GraphicsDevice { get; }

        public override void Dispose()
        {
            GraphicsDevice.Resources.Remove(this);
            
            GL.DeleteTexture(Handle);
            GLHelper.CheckLastError();
            
            GC.SuppressFinalize(this);
        }
    }
}