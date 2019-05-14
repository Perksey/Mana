using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Mana.Graphics.Buffers;
using Mana.Graphics.Shaders;
using Mana.Graphics.Vertex;
using Mana.Graphics.Vertex.Types;
using Mana.Logging;
using OpenTK.Graphics.OpenGL4;
using Buffer = System.Buffer;

namespace Mana.Graphics
{
    public class SpriteBatch : IGraphicsResource
    {
        private const int MAX_BATCH_SIZE = 3000;
        
        private static Logger _log = Logger.Create();
        
        private VertexBuffer _vertexBuffer;
        private int _vertexBufferSize;
        private VertexPosition2DTextureColor[] _vertexData;

        private IndexBuffer _indexBuffer;
        private int _indexBufferSize;
        private ushort[] _indexData;

        private Texture2D _lastTexture;
        private ShaderProgram _lastShader;
        
        private int _storedItems = 0;
        private bool _began = false;
        
        public SpriteBatch(GraphicsDevice graphicsDevice)
        {
            unsafe
            {
                GraphicsDevice = graphicsDevice;
                GraphicsDevice.Resources.Add(this);

                _vertexBufferSize = 64;
                _indexBufferSize = 96;

                CreateVertexBuffer();
                _vertexData = new VertexPosition2DTextureColor[_vertexBufferSize];

                CreateIndexBuffer();
                _indexData = new ushort[_indexBufferSize];
            }
        }
        
        public GraphicsDevice GraphicsDevice { get; }
        
        public ShaderProgram Shader { get; set; }

        public void Begin()
        {
            if (_began)
                throw new InvalidOperationException("SpriteBatch already began.");
            
            _storedItems = 0;
            _began = true;
        }

        public void End()
        {
            if (!_began)
                throw new InvalidOperationException("End() must be called before Begin() may be called.");
            
            if (_storedItems != 0)
                Flush(true);
            
            _began = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(Texture2D texture, Rectangle destination)
        {
            Draw(texture, destination, new Rectangle(0, 0, texture.Width, texture.Height), Color.White);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]   
        public void Draw(Texture2D texture, Rectangle destination, Rectangle source)
        {
            Draw(texture, destination, source, Color.White);
        }

        public void Draw(Texture2D texture, Rectangle destination, Rectangle source, Color color)
        {
            if (!_began)
                throw new InvalidOperationException("Begin() must be called before SpriteBatch may be used for drawing.");
            
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));
            
            unsafe
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
            
                if (((_storedItems ) * 4) + 3 > ushort.MaxValue || _storedItems >= MAX_BATCH_SIZE)
                {
                    Flush();
                }

                _storedItems++;
                EnsureBufferLargeEnough();

                int firstVertexIndex = (_storedItems - 1) * 4;
                int firstIndexIndex = (_storedItems - 1) * 6;
                
                fixed (VertexPosition2DTextureColor* dest = &_vertexData[firstVertexIndex])
                {
                    float l = source.X / (float)texture.Width;
                    float r = source.Right / (float)texture.Width;
                    float t = source.Y / (float)texture.Height;
                    float b = source.Bottom / (float)texture.Height;
                    
                    dest[0].Position.X = destination.Left;
                    dest[0].Position.Y = destination.Bottom;
                    dest[0].TexCoord.X = l;
                    dest[0].TexCoord.Y = t;
                    dest[0].Color.R = color.R;
                    dest[0].Color.G = color.G;
                    dest[0].Color.B = color.B;
                    dest[0].Color.A = color.A;
                    // dest[0].Color = color;
                    
                    dest[1].Position.X = destination.Right;
                    dest[1].Position.Y = destination.Bottom;
                    dest[1].TexCoord.X = r;
                    dest[1].TexCoord.Y = t;
                    dest[1].Color.R = color.R;
                    dest[1].Color.G = color.G;
                    dest[1].Color.B = color.B;
                    dest[1].Color.A = color.A;
                    // dest[1].Color = color;
                    
                    dest[2].Position.X = destination.Right;
                    dest[2].Position.Y = destination.Top;
                    dest[2].TexCoord.X = r;
                    dest[2].TexCoord.Y = b;
                    dest[2].Color.R = color.R;
                    dest[2].Color.G = color.G;
                    dest[2].Color.B = color.B;
                    dest[2].Color.A = color.A;
                    // dest[2].Color = color;
                    
                    dest[3].Position.X = destination.Left;
                    dest[3].Position.Y = destination.Top;
                    dest[3].TexCoord.X = l;
                    dest[3].TexCoord.Y = b;
                    dest[3].Color.R = color.R;
                    dest[3].Color.G = color.G;
                    dest[3].Color.B = color.B;
                    dest[3].Color.A = color.A;
                    // dest[3].Color = color;
                }
                
                fixed (ushort* dest = &_indexData[firstIndexIndex])
                {
                    dest[0 + 0] = (ushort)(firstVertexIndex + 0);
                    dest[0 + 1] = (ushort)(firstVertexIndex + 1);
                    dest[0 + 2] = (ushort)(firstVertexIndex + 2);
                    dest[0 + 3] = (ushort)(firstVertexIndex + 0);
                    dest[0 + 4] = (ushort)(firstVertexIndex + 2);
                    dest[0 + 5] = (ushort)(firstVertexIndex + 3);
                }
            }
        }
        
        public void Dispose()
        {
            GraphicsDevice.Resources.Remove(this);
            
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
        }

        private void Flush(bool inEnd = false)
        {
            Debug.Assert(Shader != null);
            Debug.Assert(_lastTexture != null);

            unsafe
            {
                fixed (VertexPosition2DTextureColor* vertexPtr = &_vertexData[0])
                fixed (ushort* indexPtr = &_indexData[0])
                {
                    //_vertexBuffer.DiscardData();
                    _vertexBuffer.SubData<VertexPosition2DTextureColor>((IntPtr)vertexPtr, 0, _storedItems * 4);
                    
                    //_indexBuffer.DiscardData();
                    _indexBuffer.SubData<ushort>((IntPtr)indexPtr, 0, _storedItems * 6);
                }
            }

            GraphicsDevice.BindVertexBuffer(_vertexBuffer);
            GraphicsDevice.BindIndexBuffer(_indexBuffer);
            GraphicsDevice.BindShaderProgram(Shader);
            GraphicsDevice.BindTexture(0, _lastTexture);

            _vertexBuffer.VertexTypeInfo.Apply(Shader);

            GL.DrawRangeElements(PrimitiveType.Triangles,
                                 0,
                                 _storedItems * 4,
                                 _storedItems * 6,
                                 DrawElementsType.UnsignedShort,
                                 IntPtr.Zero);
            GLHelper.CheckLastError();

            unchecked
            {
                Metrics._drawCalls++;
                Metrics._primitiveCount += _storedItems * 6;
            }
            
            _storedItems = 0;
        }
        
        private unsafe void EnsureBufferLargeEnough()
        {
            if (_storedItems * 4 > _vertexBufferSize)
            {
                // _log.Debug("SpriteBatch capacity increased from " + _vertexBufferSize + " to " + (int)(_vertexBufferSize * 1.5f));
                _vertexBufferSize = (int)(_vertexBufferSize * 1.5f);    // To increase capacity
                _indexBufferSize = (int)(_vertexBufferSize * 1.5f);     // To have 2:3 ratio from vertex to index

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

        private void CreateVertexBuffer()
        {
            _vertexBuffer?.Dispose();
            _vertexBuffer = VertexBuffer.Create<VertexPosition2DTextureColor>(GraphicsDevice,
                                                                              _vertexBufferSize,
                                                                              BufferUsage.StaticDraw,
                                                                              immutable: true,
                                                                              dynamic: true);
        }

        private void CreateIndexBuffer()
        {
            _indexBuffer?.Dispose();
            _indexBuffer = IndexBuffer.Create<ushort>(GraphicsDevice,
                                                      _indexBufferSize,
                                                      BufferUsage.StaticDraw,
                                                      immutable: true,
                                                      dynamic: true);
        }
    }
}