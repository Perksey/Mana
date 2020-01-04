using System;
using System.IO;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mana.Asset;
using Mana.Asset.Reloading;
using Mana.Graphics.Buffers;
using Mana.Utilities;
using osuTK.Graphics;
using osuTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Mana.Graphics.Textures
{
    /// <summary>
    /// Represents an OpenGL Texture2D object.
    /// </summary>
    public class Texture2D : Texture, IReloadableAsset
    {
        private static Logger _log = Logger.Create();

        private TextureMinFilter _minFilter;
        private TextureMagFilter _magFilter;
        private TextureWrapMode _wrapModeS;
        private TextureWrapMode _wrapModeT;

        private Texture2D(RenderContext renderContext)
            : base(renderContext)
        {
        }

        internal override TextureTarget? TextureTargetType => TextureTarget.Texture2D;

        protected override ObjectLabelIdentifier? LabelType => ObjectLabelIdentifier.Texture;

        public int Width { get; private set; } = -1;

        public int Height { get; private set; } = -1;

        public override void Bind(int slot, RenderContext renderContext) => renderContext.BindTexture(slot, this);
        public override void Unbind(RenderContext renderContext) => renderContext.UnbindTexture(this);

        public static Texture2D CreateEmpty(RenderContext renderContext, int width, int height)
        {
            var texture = new Texture2D(renderContext)
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
                var previousTexture = renderContext.TextureUnits[0].Texture2D;
                renderContext.BindTexture(0, texture);

                GL.TexImage2D(TextureTarget.Texture2D,
                              0,
                              PixelInternalFormat.Rgba,
                              texture.Width,
                              texture.Height,
                              0,
                              PixelFormat.Rgba,
                              PixelType.UnsignedByte,
                              IntPtr.Zero);

                renderContext.BindTexture(0, previousTexture);
            }

            texture.SetDefaultTextureParameters(renderContext);

            return texture;
        }

        public static unsafe Texture2D CreateFromStream(RenderContext renderContext, Stream stream)
        {
            using var image = Image.Load<Rgba32>(stream);
            image.Mutate(x => x.Flip(FlipMode.Vertical));

            var texture = new Texture2D(renderContext)
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
                    var previousTexture = renderContext.TextureUnits[0].Texture2D;
                    renderContext.BindTexture(0, texture);

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

                    renderContext.BindTexture(0, previousTexture);
                }
            }

            texture.SetDefaultTextureParameters(renderContext);

            return texture;
        }

        public static unsafe Texture2D CreateFromStreamUnsynchronized(RenderContext renderContext, Stream stream)
        {
            using var image = Image.Load<Rgba32>(stream);
            image.Mutate(x => x.Flip(FlipMode.Vertical));

            var texture = new Texture2D(renderContext)
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

                if (pixelPointer == IntPtr.Zero)
                {
                    throw new GraphicsContextException("Could not map PixelUnbackBuffer range.");
                }

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
                    var previousTexture = renderContext.TextureUnits[0].Texture2D;
                    renderContext.BindTexture(0, texture);

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

                    renderContext.BindTexture(0, previousTexture);
                }

                GL.BindBuffer(BufferTarget.PixelUnpackBuffer, prevPixelBuffer);
            }

            texture.SetDefaultTextureParameters(renderContext);

            // TODO: Look into if this is necessary (?)
            GL.Finish();

            return texture;
        }

        public static unsafe Texture2D CreateFromRGBAPointer(RenderContext renderContext, int width, int height, byte* pixelData)
        {
            var texture = new Texture2D(renderContext)
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
                var previousTexture = renderContext.TextureUnits[0].Texture2D;
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

                renderContext.BindTexture(0, previousTexture);
            }

            texture.SetDefaultTextureParameters(renderContext);

            return texture;
        }

        public string SourcePath { get; set; }

        public AssetManager AssetManager { get; set; }

        public void OnAssetLoaded()
        {
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
                var previousTexture = renderContext.TextureUnits[0].Texture2D;
                renderContext.BindTexture(0, this);

                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                renderContext.BindTexture(0, previousTexture);
            }
        }

        // internal void SetDataFromBitmap(BitmapData data)
        // {
        //     GL.TextureSubImage2D(Handle,
        //                          0,
        //                          0,
        //                          0,
        //                          Width,
        //                          Height,
        //                          PixelFormat.Bgra,
        //                          PixelType.UnsignedByte,
        //                          data.Scan0);
        // }

        public void Reload(AssetManager assetManager)
        {
            Texture2D texture = null;

            if (BoundContext != null)
            {
                Unbind(BoundContext);
            }

            try
            {
                using var stream = assetManager.GetStreamFromPath(SourcePath);

                var createdTexture = AssetManager.Texture2DLoader.Load(assetManager, assetManager.RenderContext, stream, SourcePath);
                texture = createdTexture;
            }
            catch (Exception e)
            {
                _log.Error("Error reloading Texture2D: " + e.Message);
            }

            if (texture != null)
            {
                var initialHandle = this.Handle;

                GL.DeleteTexture(Handle);

                this.Handle = texture.Handle;
                this.Height = texture.Height;
                this.Width = texture.Width;

                GLHelper.TextureParameter(assetManager.RenderContext,
                                          TextureTarget.Texture2D,
                                          this,
                                          TextureParameterName.TextureMinFilter,
                                          (int)_minFilter);

                GLHelper.TextureParameter(assetManager.RenderContext,
                                          TextureTarget.Texture2D,
                                          this,
                                          TextureParameterName.TextureMagFilter,
                                          (int)_magFilter);

                GLHelper.TextureParameter(assetManager.RenderContext,
                                          TextureTarget.Texture2D,
                                          this,
                                          TextureParameterName.TextureWrapS,
                                          (int)_wrapModeS);

                GLHelper.TextureParameter(assetManager.RenderContext,
                                          TextureTarget.Texture2D,
                                          this,
                                          TextureParameterName.TextureWrapT,
                                          (int)_wrapModeT);

                GL.GenerateTextureMipmap(Handle);

                _log.Info($"Texture reloaded successfully! {initialHandle} -> {Handle}");
            }
        }

        public string[] GetLiveReloadAssetPaths()
        {
            return new[] { SourcePath };
        }
    }
}
