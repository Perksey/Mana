using System;
using System.Diagnostics;
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
        
        private string _label;
        public string Label
        {
            get => _label;
            set
            {
                if (GraphicsDevice.IsVersionAtLeast(4, 3) || GraphicsDevice.Extensions.KHR_Debug)
                {
                    GL.ObjectLabel(ObjectLabelIdentifier.Texture, Handle, value.Length, value);    
                }

                _label = value;
            }
        }

        private TextureFilterMode _filterMode = TextureFilterMode.Nearest;
        private TextureWrapMode _wrapMode = TextureWrapMode.Repeat;

        private Texture2D(GraphicsDevice graphicsDevice, int width, int height)
        {
            GraphicsDevice = graphicsDevice;
            GraphicsDevice.Resources.Add(this);

            if (graphicsDevice.DirectStateAccessSupported)
            {
                GL.CreateTextures(TextureTarget.Texture2D, 1, out int textureInt);
                Handle = (GLHandle)textureInt;
            }
            else
            {
                Handle = (GLHandle)GL.GenTexture();
            }

            Width = width;
            Height = height;

            if (graphicsDevice.DirectStateAccessSupported)
            {
                GL.TextureStorage2D(Handle, 1, SizedInternalFormat.Rgba8, Width, Height);
            }
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
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)value);
                
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
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)value);

                _wrapMode = value;
            }
        }

        public static unsafe Texture2D CreateFromStream(GraphicsDevice graphicsDevice, Stream stream)
        {
            Texture2D texture;

            var sw1 = Stopwatch.StartNew();
            
            using (Image<Rgba32> image = Image.Load(stream))
            {
                image.Mutate(x => x.Flip(FlipMode.Vertical));
                
                sw1.Stop();
                
                _log.Debug("Texture Load CPU: " + sw1.Elapsed.TotalMilliseconds + "ms");

                sw1.Restart();
                
                texture = new Texture2D(graphicsDevice, image.Width, image.Height);
                
                fixed (void* data = &MemoryMarshal.GetReference(image.GetPixelSpan()))
                {
                    if (graphicsDevice.DirectStateAccessSupported)
                    {
                        GL.TextureSubImage2D(texture.Handle,
                                             0,
                                             0,
                                             0,
                                             texture.Width,
                                             texture.Height,
                                             PixelFormat.Rgba,
                                             PixelType.UnsignedByte,
                                             new IntPtr(data));
                    }
                    else
                    {
                        graphicsDevice.BindTexture(0, texture);
                        
                        GL.TexImage2D(TextureTarget.Texture2D,
                                      0,
                                      PixelInternalFormat.Rgba,
                                      texture.Width,
                                      texture.Height,
                                      0,
                                      PixelFormat.Rgba,
                                      PixelType.UnsignedByte,
                                      new IntPtr(data));
                    }
                }
                
                sw1.Stop();
                
                _log.Debug("Texture Load GPU: " + sw1.Elapsed.TotalMilliseconds + "ms");
            }
            
            texture.FilterMode = TextureFilterMode.Nearest;
            texture.WrapMode = TextureWrapMode.Repeat;

            return texture;
        }
        
        public static unsafe Texture2D CreateFromRGBAPointer(GraphicsDevice graphicsDevice, int width, int height, byte* data)
        {
            Texture2D texture = new Texture2D(graphicsDevice, width, height);

            if (graphicsDevice.DirectStateAccessSupported)
            {
                GL.TextureSubImage2D(texture.Handle, 
                                     0,
                                     0,
                                     0,
                                     texture.Width,
                                     texture.Height,
                                     PixelFormat.Rgba,
                                     PixelType.UnsignedByte,
                                     new IntPtr(data));
            }
            else
            {
                graphicsDevice.BindTexture(0, texture);
                
                GL.TexImage2D(TextureTarget.Texture2D,
                              0,
                              PixelInternalFormat.Rgba,
                              width,
                              height,
                              0,
                              PixelFormat.Rgba,
                              PixelType.UnsignedByte,
                              new IntPtr(data));
            }
            
            texture.FilterMode = TextureFilterMode.Nearest;
            texture.WrapMode = TextureWrapMode.Repeat;

            return texture;
        }
        
        //
        // public Color GetPixel(int x, int y)
        // {
        //     throw new NotImplementedException();
        //     
        //     var data = new Color[1];
        //     
        //     GetPixels(0, new Rectangle(x, y, 1, 1), data, 0);
        //     
        //     return data[0];
        // }
        //
        // public unsafe void GetPixels(int mipLevel, Rectangle rect, Color[] data, int startIndex)
        // {
        //     throw new NotImplementedException();
        //     
        //     GraphicsDevice.BindTexture(0, this);
        //     
        //     var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        //
        //     var intPtr = handle.AddrOfPinnedObject();
        //     
        //     GL.GetTextureSubImage(Handle,
        //                           0,
        //                           rect.X,
        //                           rect.Y,
        //                           0,
        //                           rect.Width,
        //                           rect.Height,
        //                           1,
        //                           PixelFormat.Rgba,
        //                           PixelType.UnsignedByte,
        //                           data.Length * sizeof(Color),
        //                           ref intPtr);
        //     GLHelper.CheckLastError();
        //     
        //     handle.Free();
        // }

        public override void Dispose()
        {
            Debug.Assert(!Disposed);
            
            GraphicsDevice.Resources.Remove(this);

            GL.DeleteTexture(Handle);
            
            GC.SuppressFinalize(this);

            Disposed = true;
        }

        public override bool Reload(AssetManager assetManager)
        {
            if (Disposed)
                return false;

            GLHandle handle = (GLHandle)GL.GenTexture();

            int newWidth = 0;
            int newHeight = 0;

            FileHelper.WaitForFile(SourcePath);
            
            using (Stream stream = File.OpenRead(SourcePath))
            using (Image<Rgba32> image = Image.Load(stream))
            {
                GraphicsDevice.SetActiveTexture(0);

                GL.BindTexture(TextureTarget.Texture2D, handle);
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
                    }
                }
            }
            
            GL.DeleteTexture(Handle);

            Handle = handle;
            Width = newWidth;
            Height = newHeight;

            // Reapply filter mode and wrap mode to new texture.
            FilterMode = FilterMode;
            WrapMode = WrapMode;

            return true;
        }
    }
}