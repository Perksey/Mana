using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mana.Asset;
using Mana.Graphics.Buffers;
using Mana.Utilities;
using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Mana.Graphics.Textures
{
    public class Texture2D : Texture, IAsset
    {
        private TextureMinFilter _minFilter;
        private TextureMagFilter _magFilter;
        private TextureWrapMode _wrapModeS;
        private TextureWrapMode _wrapModeT;
        
        private Texture2D(ResourceManager resourceManager) 
            : base(resourceManager)
        {
        }

        internal override TextureTarget? TextureTargetType => TextureTarget.Texture2D;

        protected override ObjectLabelIdentifier? LabelType => ObjectLabelIdentifier.Texture;

        public string SourcePath { get; set; }
        
        public AssetManager AssetManager { get; set; }
        
        public int Width { get; private set; } = -1;

        public int Height { get; private set; } = -1;

        public override void Bind(int slot, RenderContext renderContext) => renderContext.BindTexture(slot, this);
        public override void Unbind(RenderContext renderContext) => renderContext.UnbindTexture(this);

        public static Texture2D CreateEmpty(RenderContext renderContext, int width, int height)
        {
            var texture = new Texture2D(renderContext.ResourceManager)
            {
                Width = width,
                Height = height,
            };

            if (GLInfo.HasDirectStateAccess)
            {
                GL.TextureStorage2D(texture.Handle,
                                    1,
                                    SizedInternalFormat.Rgba8,
                                    texture.Width,
                                    texture.Height);
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

            texture.SetDefaultTextureParameters(renderContext);
            
            return texture;
        }

        public static unsafe Texture2D CreateFromStream(RenderContext renderContext, Stream stream)
        {
            using var image = Image.Load(stream);
            image.Mutate(x => x.Flip(FlipMode.Vertical));

            var texture = new Texture2D(renderContext.ResourceManager)
            {
                Width = image.Width, 
                Height = image.Height,
            };

            fixed (void* data = &MemoryMarshal.GetReference(image.GetPixelSpan()))
            {
                if (GLInfo.HasDirectStateAccess)
                {
                    GL.TextureStorage2D(texture.Handle, 
                                        1, 
                                        SizedInternalFormat.Rgba8, 
                                        texture.Width, 
                                        texture.Height);
                    
                    GL.TextureSubImage2D(texture.Handle,
                                         0,
                                         0,
                                         0,
                                         texture.Width,
                                         texture.Height,
                                         PixelFormat.Rgba,
                                         PixelType.UnsignedByte,
                                         new IntPtr(data));
                    
                    GL.GenerateTextureMipmap(texture.Handle);
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
                    
                    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                    GL.BindTexture(TextureTarget.Texture2D, prevTexture);
                }
            }
            
            texture.SetDefaultTextureParameters(renderContext);

            return texture;
        }

        public static unsafe Texture2D CreateFromStreamUnsynchronized(RenderContext renderContext, Stream stream)
        {
            using var image = Image.Load<Rgba32>(stream);
            image.Mutate(x => x.Flip(FlipMode.Vertical));
            
            var texture = new Texture2D(renderContext.ResourceManager)
            {
                Width = image.Width, 
                Height = image.Height,
            };

            var span = image.GetPixelSpan();
            int size = span.Length * sizeof(Rgba32);

            fixed (void* data = &MemoryMarshal.GetReference(span))
            {
                var start = new IntPtr(data);

                using var pixelBuffer = PixelBuffer.Create<Rgba32>(renderContext, size, true, true);

                GLHandle prevPixelBuffer = (GLHandle)GL.GetInteger(GetPName.PixelUnpackBufferBinding);
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
                
                if (GLInfo.HasDirectStateAccess)
                {
                    GL.TextureStorage2D(texture.Handle, 
                                        1, 
                                        SizedInternalFormat.Rgba8, 
                                        texture.Width, 
                                        texture.Height);
                    
                    GL.TextureSubImage2D(texture.Handle,
                                         0,
                                         0,
                                         0,
                                         texture.Width,
                                         texture.Height,
                                         PixelFormat.Rgba,
                                         PixelType.UnsignedByte,
                                         IntPtr.Zero);
                    
                    GL.GenerateTextureMipmap(texture.Handle);
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
                    
                    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                    GL.BindTexture(TextureTarget.Texture2D, prevTexture);
                }

                GL.BindBuffer(BufferTarget.PixelUnpackBuffer, prevPixelBuffer);
            }
            
            texture.SetDefaultTextureParameters(renderContext);

            return texture;
        }

        public static unsafe Texture2D CreateFromRGBAPointer(RenderContext renderContext, int width, int height, byte* pixelData)
        {
            var texture = new Texture2D(renderContext.ResourceManager)
            {
                Width = width, 
                Height = height
            };

            if (GLInfo.HasDirectStateAccess)
            {
                GL.TextureStorage2D(texture.Handle, 
                                    1, 
                                    SizedInternalFormat.Rgba8, 
                                    texture.Width, 
                                    texture.Height);
                    
                GL.TextureSubImage2D(texture.Handle,
                                     0,
                                     0,
                                     0,
                                     texture.Width,
                                     texture.Height,
                                     PixelFormat.Rgba,
                                     PixelType.UnsignedByte,
                                     new IntPtr(pixelData));
            }
            else
            {
                renderContext.BindTexture(0, texture);
                
                GL.TexImage2D(TextureTarget.Texture2D,
                              0,
                              PixelInternalFormat.Rgba,
                              width,
                              height,
                              0,
                              PixelFormat.Rgba,
                              PixelType.UnsignedByte,
                              new IntPtr(pixelData));
            }
            
            texture.SetDefaultTextureParameters(renderContext);

            return texture;
        }

        public void OnAssetLoaded()
        {
            // throw new NotImplementedException();
        }

        public void SetMinFilter(RenderContext renderContext, TextureMinFilter minFilter)
        {
            if (_minFilter != minFilter)
            {
                GLHelper.TextureParameter(renderContext, 
                                          TextureTarget.Texture2D, 
                                          this, 
                                          TextureParameterName.TextureMinFilter,
                                          (int)minFilter);

                _minFilter = minFilter;
            }
        }

        public TextureMinFilter GetMinFilter() => _minFilter;

        public void SetMagFilter(RenderContext renderContext, TextureMagFilter magFilter)
        {
            if (_magFilter != magFilter)
            {
                GLHelper.TextureParameter(renderContext, 
                                          TextureTarget.Texture2D, 
                                          this, 
                                          TextureParameterName.TextureMagFilter,
                                          (int)magFilter);

                _magFilter = magFilter;
            }
        }

        public TextureMagFilter GetMagFilter() => _magFilter;

        public void SetWrapModeS(RenderContext renderContext, TextureWrapMode wrapModeS)
        {
            if (_wrapModeS != wrapModeS)
            {
                GLHelper.TextureParameter(renderContext, 
                                          TextureTarget.Texture2D, 
                                          this, 
                                          TextureParameterName.TextureWrapS,
                                          (int)wrapModeS);

                _wrapModeS = wrapModeS;
            }
        }

        public TextureWrapMode GetWrapModeS() => _wrapModeS;
        
        public void SetWrapModeT(RenderContext renderContext, TextureWrapMode wrapModeT)
        {
            if (_wrapModeT != wrapModeT)
            {
                GLHelper.TextureParameter(renderContext, 
                                          TextureTarget.Texture2D, 
                                          this, 
                                          TextureParameterName.TextureWrapT,
                                          (int)wrapModeT);

                _wrapModeT = wrapModeT;
            }
        }

        public TextureWrapMode GetWrapModeT() => _wrapModeT;

        private void SetDefaultTextureParameters(RenderContext renderContext)
        {
            SetMinFilter(renderContext, TextureMinFilter.NearestMipmapLinear);
            SetMagFilter(renderContext, TextureMagFilter.Linear);
            
            SetWrapModeS(renderContext, TextureWrapMode.Repeat);
            SetWrapModeT(renderContext, TextureWrapMode.Repeat);

            if (GLInfo.HasDirectStateAccess)
            {
                GL.GenerateTextureMipmap(Handle);
            }
            else
            {
                renderContext.BindTexture(0, this);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
        }
    }
}