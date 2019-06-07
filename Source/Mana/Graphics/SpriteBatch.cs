using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using Mana.Graphics.Buffers;
using Mana.Graphics.Shader;
using Mana.Graphics.Textures;
using Mana.Graphics.Vertex.Types;
using Mana.Utilities;
using OpenTK.Graphics.OpenGL4;
using Buffer = System.Buffer;

namespace Mana.Graphics
{
    /// <summary>
    /// Used for efficient sprite rendering via batches.
    /// </summary>
    public class SpriteBatch : GraphicsResource
    {
        private static Logger _log = Logger.Create();
        
        private const int MAX_BATCH_SIZE = 3000;

        private VertexBuffer _vertexBuffer;
        private int _vertexBufferSize;
        private VertexPosition2DTextureColor[] _vertexData;

        private IndexBuffer _indexBuffer;
        private int _indexBufferSize;
        private ushort[] _indexData;

        private Texture2D _lastTexture;
        private ShaderProgram _lastShader;

        private int _count = 0;
        private bool _active = false;
        
        public SpriteBatch(RenderContext renderContext) 
            : base(renderContext.ResourceManager)
        {
            RenderContext = renderContext;
            
            _vertexBufferSize = 64;
            _indexBufferSize = 96;

            CreateVertexBuffer();
            _vertexData = new VertexPosition2DTextureColor[_vertexBufferSize];

            CreateIndexBuffer();
            _indexData = new ushort[_indexBufferSize];
            
            renderContext.ResourceManager.OnResourceCreated(this);
        }

        public RenderContext RenderContext { get; }
        
        public ShaderProgram Shader { get; set; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
        }

        public void Begin()
        {
            if (_active)
                throw new InvalidOperationException("SpriteBatch already began.");

            _count = 0;
            _active = true;
        }

        public void End()
        {
            if (!_active)
                throw new InvalidOperationException("End() must be called before Begin() may be called.");

            if (_count != 0)
                Flush();

            _active = false;
        }
        
        public void DrawQuad(Texture2D texture,
                             VertexPosition2DTextureColor bl,
                             VertexPosition2DTextureColor br,
                             VertexPosition2DTextureColor tr,
                             VertexPosition2DTextureColor tl)
        {
            if (!_active)
                throw new InvalidOperationException("Begin() must be called before SpriteBatch may be used for drawing.");

            if (texture == null)
                throw new ArgumentNullException(nameof(texture));
            
            FlushIfNeeded(texture);
                
            _count++;
            EnsureBufferLargeEnough();

            int vertexOffset = (_count - 1) * 4;
            int indexOffset = (_count - 1) * 6;
                
            _vertexData[vertexOffset + 0] = bl;
            _vertexData[vertexOffset + 1] = br;
            _vertexData[vertexOffset + 2] = tr;
            _vertexData[vertexOffset + 3] = tl;
                
            _indexData[indexOffset + 0] = (ushort)(vertexOffset + 0);
            _indexData[indexOffset + 1] = (ushort)(vertexOffset + 1);
            _indexData[indexOffset + 2] = (ushort)(vertexOffset + 2);
            _indexData[indexOffset + 3] = (ushort)(vertexOffset + 0);
            _indexData[indexOffset + 4] = (ushort)(vertexOffset + 2);
            _indexData[indexOffset + 5] = (ushort)(vertexOffset + 3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(Texture2D texture, Rectangle destination)
        {
            Draw(texture, destination, new Rectangle(0, 0, texture.Width, texture.Height), Color.White);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(Texture2D texture, RectangleF destination)
        {
            Draw(texture, destination, new Rectangle(0, 0, texture.Width, texture.Height), Color.White);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]   
        public void Draw(Texture2D texture, Rectangle destination, Rectangle source)
        {
            Draw(texture, destination, source, Color.White);
        }
        
        public void Draw(Texture2D texture, RectangleF destination, Rectangle source, Color color)
        {
            if (!_active)
                throw new InvalidOperationException("Begin() must be called before SpriteBatch may be used for drawing.");
            
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));

            FlushIfNeeded(texture);
            TransformSourceRectangle(ref source, texture.Height);
            
            _count++;
            EnsureBufferLargeEnough();

            int vertexOffset = (_count - 1) * 4;
            int indexOffset = (_count - 1) * 6;

            float l = source.X / (float)texture.Width;
            float r = source.Right / (float)texture.Width;
            float t = source.Y / (float)texture.Height;
            float b = source.Bottom / (float)texture.Height;
                
            _vertexData[vertexOffset + 0].Position.X = destination.Left;     // Bottom Left
            _vertexData[vertexOffset + 0].Position.Y = destination.Bottom;
            _vertexData[vertexOffset + 0].TexCoord.X = l;
            _vertexData[vertexOffset + 0].TexCoord.Y = t;
            _vertexData[vertexOffset + 0].Color = color;
                
            _vertexData[vertexOffset + 1].Position.X = destination.Right;    // Bottom Right
            _vertexData[vertexOffset + 1].Position.Y = destination.Bottom;
            _vertexData[vertexOffset + 1].TexCoord.X = r;
            _vertexData[vertexOffset + 1].TexCoord.Y = t;
            _vertexData[vertexOffset + 1].Color = color;
                
            _vertexData[vertexOffset + 2].Position.X = destination.Right;    // Top Right
            _vertexData[vertexOffset + 2].Position.Y = destination.Top;
            _vertexData[vertexOffset + 2].TexCoord.X = r;
            _vertexData[vertexOffset + 2].TexCoord.Y = b;
            _vertexData[vertexOffset + 2].Color = color;
                
            _vertexData[vertexOffset + 3].Position.X = destination.Left;     // Top Left
            _vertexData[vertexOffset + 3].Position.Y = destination.Top;        
            _vertexData[vertexOffset + 3].TexCoord.X = l;
            _vertexData[vertexOffset + 3].TexCoord.Y = b;                    
            _vertexData[vertexOffset + 3].Color = color;
                
            _indexData[indexOffset + 0] = (ushort)(vertexOffset + 0);        // Bottom Left
            _indexData[indexOffset + 1] = (ushort)(vertexOffset + 1);        // Bottom Right    
            _indexData[indexOffset + 2] = (ushort)(vertexOffset + 2);        // Top Right
            _indexData[indexOffset + 3] = (ushort)(vertexOffset + 0);        // Bottom Left
            _indexData[indexOffset + 4] = (ushort)(vertexOffset + 2);        // Top Right
            _indexData[indexOffset + 5] = (ushort)(vertexOffset + 3);        // Top Left
        }
        
        public void Draw(Texture2D texture, Rectangle destination, Rectangle source, Color color)
        {
            if (!_active)
                throw new InvalidOperationException("Begin() must be called before SpriteBatch may be used for drawing.");
            
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));
            
            FlushIfNeeded(texture);
            TransformSourceRectangle(ref source, texture.Height);

            _count++;
            EnsureBufferLargeEnough();

            int vertexOffset = (_count - 1) * 4;
            int indexOffset = (_count - 1) * 6;

            float l = source.X / (float)texture.Width;
            float r = source.Right / (float)texture.Width;
            float t = source.Y / (float)texture.Height;
            float b = source.Bottom / (float)texture.Height;
                
            _vertexData[vertexOffset + 0].Position.X = destination.Left;     // Bottom Left
            _vertexData[vertexOffset + 0].Position.Y = destination.Bottom;
            _vertexData[vertexOffset + 0].TexCoord.X = l;
            _vertexData[vertexOffset + 0].TexCoord.Y = t;
            _vertexData[vertexOffset + 0].Color = color;
                
            _vertexData[vertexOffset + 1].Position.X = destination.Right;    // Bottom Right
            _vertexData[vertexOffset + 1].Position.Y = destination.Bottom;
            _vertexData[vertexOffset + 1].TexCoord.X = r;
            _vertexData[vertexOffset + 1].TexCoord.Y = t;
            _vertexData[vertexOffset + 1].Color = color;
                
            _vertexData[vertexOffset + 2].Position.X = destination.Right;    // Top Right
            _vertexData[vertexOffset + 2].Position.Y = destination.Top;
            _vertexData[vertexOffset + 2].TexCoord.X = r;
            _vertexData[vertexOffset + 2].TexCoord.Y = b;
            _vertexData[vertexOffset + 2].Color = color;
                
            _vertexData[vertexOffset + 3].Position.X = destination.Left;     // Top Left
            _vertexData[vertexOffset + 3].Position.Y = destination.Top;        
            _vertexData[vertexOffset + 3].TexCoord.X = l;
            _vertexData[vertexOffset + 3].TexCoord.Y = b;                    
            _vertexData[vertexOffset + 3].Color = color;
                
            _indexData[indexOffset + 0] = (ushort)(vertexOffset + 0);        // Bottom Left
            _indexData[indexOffset + 1] = (ushort)(vertexOffset + 1);        // Bottom Right    
            _indexData[indexOffset + 2] = (ushort)(vertexOffset + 2);        // Top Right
            _indexData[indexOffset + 3] = (ushort)(vertexOffset + 0);        // Bottom Left
            _indexData[indexOffset + 4] = (ushort)(vertexOffset + 2);        // Top Right
            _indexData[indexOffset + 5] = (ushort)(vertexOffset + 3);        // Top Left
        }
        
        private void FlushIfNeeded(Texture2D texture)
        {
            bool flushed = false;
                
            if (_lastTexture == null)
            {
                _lastTexture = texture;
            }
            else if (_lastTexture != texture)
            {
                Flush();
                flushed = true;
                _lastTexture = texture;                
            }
                
            if (Shader != _lastShader)
            {
                if (!flushed)
                    Flush();
                    
                _lastShader = Shader;
            }
            
            if (_count * 4 + 3 > ushort.MaxValue || _count >= MAX_BATCH_SIZE)
            {
                Flush();
            }
        }

        private void Flush()
        {
            Assert.That(Shader != null);
            Assert.That(_lastTexture != null);

            unsafe
            {
                fixed (VertexPosition2DTextureColor* vertexPtr = &_vertexData[0])
                fixed (ushort* indexPtr = &_indexData[0])
                {
                    _vertexBuffer.SubData<VertexPosition2DTextureColor>(RenderContext, (IntPtr)vertexPtr, 0, _count * 4);
                    _indexBuffer.SubData<ushort>(RenderContext, (IntPtr)indexPtr, 0, _count * 6);
                }
            }

            RenderContext.BindVertexBuffer(_vertexBuffer);
            RenderContext.BindIndexBuffer(_indexBuffer);
            RenderContext.BindShaderProgram(Shader);
            RenderContext.BindTexture(0, _lastTexture);

            _vertexBuffer.VertexTypeInfo.Apply(Shader);

            bool prevDepthTest = RenderContext.DepthTest;
            RenderContext.DepthTest = false;
            
            
            GL.DrawRangeElements(PrimitiveType.Triangles,
                                 0,
                                 _count * 4,
                                 _count * 6,
                                 DrawElementsType.UnsignedShort,
                                 IntPtr.Zero);
            
            RenderContext.DepthTest = prevDepthTest;

            unchecked
            {
                // Metrics._drawCalls++;
                // Metrics._primitiveCount += _count * 6;
            }
            
            _count = 0;
        }
        
        private unsafe void EnsureBufferLargeEnough()
        {
            if (_count * 4 > _vertexBufferSize)
            {
                //_log.Debug("SpriteBatch capacity increased from " + _vertexBufferSize + " to " + (int)(_vertexBufferSize * 1.5f));
                _vertexBufferSize = (int)(_vertexBufferSize * 1.5f);    // To increase capacity
                _indexBufferSize = (int)(_vertexBufferSize * 1.5f);     // To have 2:3 ratio from vertex to index

                if (_vertexBufferSize > MAX_BATCH_SIZE * 4)
                    _vertexBufferSize = MAX_BATCH_SIZE * 4;
                
                if (_indexBufferSize > MAX_BATCH_SIZE * 6)
                    _indexBufferSize = MAX_BATCH_SIZE * 6;
                
                // Increase capacity of _vertexData
                var tempVertexData = new VertexPosition2DTextureColor[_vertexData.Length];

                fixed (void* vertexSourcePtr = &_vertexData[0])
                fixed (void* vertexDestPtr = &tempVertexData[0])
                {
                    Buffer.MemoryCopy(vertexSourcePtr,
                                      vertexDestPtr,
                                      tempVertexData.Length * sizeof(VertexPosition2DTextureColor),
                                      _vertexData.Length * sizeof(VertexPosition2DTextureColor));
                }
                
                // Increase vertexData capacity
                _vertexData = new VertexPosition2DTextureColor[_vertexBufferSize];
                
                fixed (void* vertexSourcePtr = &tempVertexData[0])
                fixed (void* vertexDestPtr = &_vertexData[0])
                {
                    // Store vertex data from temp array back into _vertexData
                    Buffer.MemoryCopy(vertexSourcePtr,
                                      vertexDestPtr,
                                      tempVertexData.Length * sizeof(VertexPosition2DTextureColor),
                                      tempVertexData.Length * sizeof(VertexPosition2DTextureColor));
                }
                
                // Increase capacity of _indexData
                
                var tempIndexData = new ushort[_indexData.Length];

                fixed (void* indexSourcePtr = &_indexData[0])
                fixed (void* indexDestPtr = &tempIndexData[0])
                {
                    // Store vertex data in temp array
                    Buffer.MemoryCopy(indexSourcePtr,
                                      indexDestPtr,
                                      _indexData.Length * sizeof(ushort),
                                      _indexData.Length * sizeof(ushort));
                }
                
                // Increase vertexData capacity
                _indexData = new ushort[_indexBufferSize];
                
                fixed (void* indexSourcePtr = &tempIndexData[0])
                fixed (void* indexDestPtr = &_indexData[0])
                {
                    // Store vertex data from temp array back into _vertexData
                    Buffer.MemoryCopy(indexSourcePtr,
                                      indexDestPtr,
                                      tempIndexData.Length * sizeof(ushort),
                                      tempIndexData.Length * sizeof(ushort));
                }

                CreateVertexBuffer();
                CreateIndexBuffer();
            }
        }
        
        private void CreateIndexBuffer()
        {
            _vertexBuffer?.Dispose();
            _vertexBuffer = VertexBuffer.Create<VertexPosition2DTextureColor>(RenderContext,
                                                                              _vertexBufferSize,
                                                                              BufferUsageHint.StreamDraw,
                                                                              true);
            _vertexBuffer.Label = "SpriteBatch VertexBuffer";
        }

        private void CreateVertexBuffer()
        {
            _indexBuffer?.Dispose();
            _indexBuffer = IndexBuffer.Create<ushort>(RenderContext,
                                                      _indexBufferSize,
                                                      BufferUsageHint.StreamDraw,
                                                      true);
            _indexBuffer.Label = "SpriteBatch IndexBuffer";
        }

        private void TransformSourceRectangle(ref Rectangle rectangle, int height)
        {
            rectangle.Y = height - (rectangle.Y + rectangle.Height);
        }
    }
}