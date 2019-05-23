using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mana.Asset;
using Mana.Graphics.Buffers;
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

        private Texture2D(GraphicsDevice graphicsDevice)
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
        
        public static Texture2D CreateEmpty(GraphicsDevice graphicsDevice, int width, int height)
        {
            Texture2D texture = new Texture2D(graphicsDevice, width, height)
            {
                FilterMode = TextureFilterMode.Linear, 
                WrapMode = TextureWrapMode.ClampToEdge,
            };
            
            if (!graphicsDevice.DirectStateAccessSupported)
            {
                GL.TexImage2D(TextureTarget.Texture2D,
                              0,
                              PixelInternalFormat.Rgba,
                              texture.Width,
                              texture.Height,
                              0,
                              PixelFormat.Rgba,
                              PixelType.UnsignedByte,
                              IntPtr.Zero);
            }

            return texture;
        }
        
        public static Texture2D CreateFromStream(GraphicsDevice graphicsDevice, Stream stream)
        {
            return ThreadHelper.IsMainThread 
                       ? CreateFromStreamSynchronized(graphicsDevice, stream)
                       : CreateFromStreamUnsynchronized(graphicsDevice, stream);
        }

        private static unsafe Texture2D CreateFromStreamSynchronized(GraphicsDevice graphicsDevice, Stream stream)
        {
            Texture2D texture;
            
            using (var image = Image.Load(stream))
            {
                image.Mutate(x => x.Flip(FlipMode.Vertical));
                texture = new Texture2D(graphicsDevice, image.Width, image.Height);
                
                var span = image.GetPixelSpan();
                
                fixed (void* data = &MemoryMarshal.GetReference(span))
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
                        GLHandle prevTexture = (GLHandle)GL.GetInteger(GetPName.TextureBinding2D);
                        GL.BindTexture(TextureTarget.Texture2D, texture.Handle);
                        
                        GL.TexImage2D(TextureTarget.Texture2D,
                                      0,
                                      PixelInternalFormat.Rgba,
                                      texture.Width,
                                      texture.Height,
                                      0,
                                      PixelFormat.Rgba,
                                      PixelType.UnsignedByte,
                                      new IntPtr(data));

                        GL.BindTexture(TextureTarget.Texture2D, prevTexture);
                    }
                }
            }
            
            texture.FilterMode = TextureFilterMode.Nearest;
            texture.WrapMode = TextureWrapMode.Repeat;

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);            
            
            return texture;
        }
        
        private static unsafe Texture2D CreateFromStreamUnsynchronized(GraphicsDevice graphicsDevice, Stream stream)
        {
            Texture2D texture;
            
            using (var image = Image.Load(stream))
            {
                image.Mutate(x => x.Flip(FlipMode.Vertical));
                
                texture = new Texture2D(graphicsDevice, image.Width, image.Height);

                var span = image.GetPixelSpan();
                int size = span.Length * sizeof(Rgba32);
                
                fixed (void* data = &MemoryMarshal.GetReference(span))
                {
                    var start = new IntPtr(data);
                    var pixelBuffer = PixelBuffer.Create<Rgba32>(graphicsDevice, size, immutable: true, mapWrite: true);

                    GLHandle previousPBO = (GLHandle)GL.GetInteger(GetPName.PixelUnpackBufferBinding);
                    GL.BindBuffer(BufferTarget.PixelUnpackBuffer, pixelBuffer.Handle);
                    
                    // When we load assets asynchronously, using SubData to set the data will cause the main thread
                    // to wait for the GL call to complete, so we use a PBO and fill it with a mapped memory range to
                    // prevent OpenGL synchronization from causing frame drops on the main thread.

                    IntPtr pixelPointer = GL.MapBufferRange(BufferTarget.PixelUnpackBuffer, 
                                                      IntPtr.Zero, 
                                                      size, 
                                                      BufferAccessMask.MapWriteBit
                                                      | BufferAccessMask.MapUnsynchronizedBit
                                                      | BufferAccessMask.MapInvalidateRangeBit);

                    Assert.That(pixelPointer != IntPtr.Zero);

                    // We send the data to mapped memory in multiple batches instead of one large one for the same
                    // reason.
                    
                    int remaining = size;
                    const int step = 2048;
                    while (remaining > 0)
                    {
                        int currentStep = Math.Min(remaining, step);
                        int point = size - remaining;
                        void* dest = (void*)IntPtr.Add(pixelPointer, point);
                        void* src = (void*)IntPtr.Add(start, point);
                        Unsafe.CopyBlock(dest, src, (uint)currentStep);
                    
                        remaining -= step;
                    }
                    
                    GL.UnmapBuffer(BufferTarget.PixelUnpackBuffer);
                    
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
                                             IntPtr.Zero);
                    }
                    else
                    {
                        GLHandle prevTexture = (GLHandle)GL.GetInteger(GetPName.TextureBinding2D);
                        GL.BindTexture(TextureTarget.Texture2D, texture.Handle);
                        
                        GL.TexImage2D(TextureTarget.Texture2D,
                                      0,
                                      PixelInternalFormat.Rgba,
                                      texture.Width,
                                      texture.Height,
                                      0,
                                      PixelFormat.Rgba,
                                      PixelType.UnsignedByte,
                                      IntPtr.Zero);

                        GL.BindTexture(TextureTarget.Texture2D, prevTexture);
                    }

                    GL.BindBuffer(BufferTarget.PixelUnpackBuffer, previousPBO);
                    pixelBuffer.Dispose();
                }
            }
            
            texture.FilterMode = TextureFilterMode.Nearest;
            texture.WrapMode = TextureWrapMode.Repeat;
            
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);         

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

        public override void Dispose()
        {
            Assert.That(!Disposed);

            if (!IsUnloading && AssetManager != null)
            {
                AssetManager.Unload(this);
                return;
            }
            
            GraphicsDevice.Resources.Remove(this);
            GraphicsDevice.UnbindTexture(this);

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
                int prevActiveTexture = GL.GetInteger(GetPName.ActiveTexture);
                GL.ActiveTexture(0);

                int prevTexture2D = GL.GetInteger(GetPName.TextureBinding2D);
                GL.BindTexture(TextureTarget.Texture2D, handle);
                
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

                GL.BindTexture(TextureTarget.Texture2D, prevTexture2D);
                GL.ActiveTexture((TextureUnit)prevActiveTexture);
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