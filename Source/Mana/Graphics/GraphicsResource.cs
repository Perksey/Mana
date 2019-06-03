using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics
{
    public abstract class GraphicsResource : IDisposable
    {
        internal RenderContext BoundContext = null;
        internal bool Disposed = false;
        
        private string _label;
        
        protected GraphicsResource(ResourceManager resourceManager)
        {
            ResourceManager = resourceManager;
        }

        public GLHandle Handle { get; protected set; }
        
        public ResourceManager ResourceManager { get; }
        
        public string Label
        {
            get => _label;
            set
            {
                if (GLInfo.HasDebug)
                {
                    if (LabelType.HasValue)
                        GL.ObjectLabel(LabelType.Value, Handle, value.Length, value);
                        
                    else
                        throw new InvalidOperationException("Cannot set label when ObjectLabelIdentifier is null.");
                }

                _label = value;
            }
        }

        protected virtual ObjectLabelIdentifier? LabelType => null;

        public void Dispose()
        {
            if (Disposed)
                return;

            Dispose(true);
            GC.SuppressFinalize(this);

            Disposed = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            ResourceManager.OnResourceDisposed(this);
        }
    }
}