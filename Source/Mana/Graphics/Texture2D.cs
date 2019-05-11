using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Mana.Asset;
using Mana.Logging;
using Mana.Utilities;
using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Mana.Graphics
{
    public class Texture2D : ReloadableAsset, IGraphicsResource
    {
        private static Logger _log = Logger.Create();
        
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
        
        public GLHandle Handle { get; private set; }

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

        public void SetDataFromBitmap(Bitmap bitmap)
        {
            GraphicsDevice.BindTexture(0, this);

            var data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                              ImageLockMode.ReadOnly,
                                              System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D,
                          0,
                          PixelInternalFormat.Rgba,
                          bitmap.Width,
                          bitmap.Height,
                          0,
                          PixelFormat.Bgra,
                          PixelType.UnsignedByte,
                          data.Scan0);
            
            bitmap.UnlockBits(data);
            
            FilterMode = TextureFilterMode.Nearest;
            WrapMode = TextureWrapMode.Repeat;
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
                                  image.Width,
                                  image.Height,
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

        public unsafe void SetDataFromRgba(byte* data, int width, int height)
        {
            GraphicsDevice.BindTexture(0, this);

            Width = width;
            Height = height;
            
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

            FilterMode = TextureFilterMode.Nearest;
            WrapMode = TextureWrapMode.Repeat;
        }

        public override void Dispose()
        {
            Debug.Assert(!Disposed);
            
            GraphicsDevice.Resources.Remove(this);
            
            GL.DeleteTexture(Handle);
            GLHelper.CheckLastError();
            
            GC.SuppressFinalize(this);

            Disposed = true;
        }

        public override bool Reload(AssetManager assetManager)
        {
            if (Disposed)
                return false;

            GLHandle handle = (GLHandle)GL.GenTexture();
            GLHelper.CheckLastError();

            int newWidth = 0;
            int newHeight = 0;

            FileHelper.WaitForFile(SourcePath);
            
            using (Stream stream = File.OpenRead(SourcePath))
            using (Image<Rgba32> image = Image.Load(stream))
            {
                GraphicsDevice.SetActiveTexture(0);

                GL.BindTexture(TextureTarget.Texture2D, handle);
                GLHelper.CheckLastError();
                GraphicsDevice.Bindings.Texture = handle;

                image.Mutate(x => x.Flip(FlipMode.Vertical));

                newWidth = image.Width;
                newHeight = image.Height;

                unsafe
                {
                    fixed (void* data = &MemoryMarshal.GetReference(image.GetPixelSpan()))
                    {
                        GL.TexImage2D(TextureTarget.Texture2D,
                                      0,
                                      PixelInternalFormat.Rgba,
                                      image.Width,
                                      image.Height,
                                      0,
                                      PixelFormat.Rgba,
                                      PixelType.UnsignedByte,
                                      new IntPtr(data));
                        GLHelper.CheckLastError();
                    }
                }
            }
            
            GL.DeleteTexture(Handle);
            GLHelper.CheckLastError();

            Handle = handle;
            Width = newWidth;
            Height = newHeight;

            // Reapply filter mode and wrap mode to new texture.
            FilterMode = this.FilterMode;
            WrapMode = this.WrapMode;

            return true;
        }
    }
}