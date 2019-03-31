using System;
using System.IO;
using System.Runtime.InteropServices;
using Mana.Asset;
using Mana.Logging;
using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Mana.Graphics
{
    public class Texture2D : ManaAsset, IGraphicsResource
    {
        private static Logger _log = Logger.Create();
        
        public readonly GLHandle Handle;

        internal bool Disposed = false;
        
        private TextureFilterMode _filterMode = TextureFilterMode.Nearest;
        private TextureWrapMode _wrapMode = TextureWrapMode.Repeat;
        
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
            //throw new InvalidOperationException("Texture2D Leaked");
#endif
        }

        public GraphicsDevice GraphicsDevice { get; }

        public int Width { get; private set; } = -1;

        public int Height { get; private set; } = -1;

        public TextureFilterMode FilterMode
        {
            get => _filterMode;
            set
            {
                GraphicsDevice.BindTexture(0, this);
                
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)value);
                GLHelper.CheckLastError();
                
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)value);
                GLHelper.CheckLastError();
                
                _filterMode = value;
            }
        }

        public TextureWrapMode WrapMode
        {
            get => _wrapMode;
            set
            {
                GraphicsDevice.BindTexture(0, this);
                
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)value);
                GLHelper.CheckLastError();
                
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)value);
                GLHelper.CheckLastError();

                _wrapMode = value;
            }
        }

        public unsafe void SetDataFromStream(Stream stream)
        {
            GraphicsDevice.BindTexture(0, this);

            using (Image<Rgba32> image = Image.Load(stream))
            {
                image.Mutate(x => x.Flip(FlipMode.Vertical));

                Width = image.Width;
                Height = image.Height;

                fixed (void* data = &MemoryMarshal.GetReference(image.GetPixelSpan()))
                {
                    GL.TexImage2D(TextureTarget.Texture2D,
                                  0,
                                  PixelInternalFormat.Rgba,
                                  Width,
                                  Height,
                                  0,
                                  PixelFormat.Rgba,
                                  PixelType.UnsignedByte,
                                  new IntPtr(data));
                    GLHelper.CheckLastError();
                }
            }
            
            FilterMode = TextureFilterMode.Nearest;
            WrapMode = TextureWrapMode.Repeat;
        }

        public unsafe void SetDataFromAlpha(byte* data, int width, int height)
        {
            GraphicsDevice.BindTexture(0, this);

            Width = width;
            Height = height;
            
            GL.TexImage2D(TextureTarget.Texture2D,
                          0,
                          PixelInternalFormat.Alpha,
                          Width,
                          Height,
                          0,
                          PixelFormat.Alpha,
                          PixelType.UnsignedByte,
                          new IntPtr(data));
            GLHelper.CheckLastError();

            FilterMode = TextureFilterMode.Nearest;
            WrapMode = TextureWrapMode.Repeat;
        }
        
        public override void Dispose()
        {
            GraphicsDevice.Resources.Remove(this);
            
            GL.DeleteTexture(Handle);
            GLHelper.CheckLastError();
            
            GC.SuppressFinalize(this);
        }
    }
}