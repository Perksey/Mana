using System;
using System.Diagnostics;
using Mana.Graphics.Buffers;
using Mana.Graphics.Shaders;
using Mana.Graphics.Vertex;
using Mana.Graphics.Vertex.Types;
using Mana.Logging;
using OpenTK.Graphics.OpenGL4;
using Buffer = System.Buffer;

namespace Mana.Graphics.Batch
{
    public class SpriteBatch : IGraphicsResource
    {
        private static Logger _log = Logger.Create();

        private const int MAX_BATCH_SIZE = 3000; 
        private int _storedItems = 0;
        
        private VertexBuffer _vertexBuffer;
        private int _vertexBufferSize;
        private VertexPosition2DTextureColor[] _vertexData;

        private IndexBuffer _indexBuffer;
        private int _indexBufferSize;
        private ushort[] _indexData;

        private Texture2D _lastTexture;
        private ShaderProgram _lastShader;
        
        public ShaderProgram Shader { get; set; }
        
        public SpriteBatch(GraphicsDevice graphicsDevice)
        {
            unsafe
            {
                GraphicsDevice = graphicsDevice;
                GraphicsDevice.Resources.Add(this);

                _vertexBufferSize = 64;
                _indexBufferSize = 96;
                
                _vertexBuffer = VertexBuffer.Create(GraphicsDevice,
                                                    _vertexBufferSize * sizeof(VertexPosition2DTextureColor),
                                                    VertexTypeInfo.Get<VertexPosition2DTextureColor>(),
                                                    BufferUsage.DynamicDraw,
                                                    clear: false,
                                                    dynamic: true,
                                                    mutable: true);
                _vertexData = new VertexPosition2DTextureColor[_vertexBufferSize];

                _indexBuffer = IndexBuffer.Create(GraphicsDevice,
                                                  _indexBufferSize,
                                                  sizeof(ushort),
                                                  DrawElementsType.UnsignedShort,
                                                  BufferUsage.DynamicDraw,
                                                  clear: false,
                                                  dynamic: true,
                                                  mutable: true);
                _indexData = new ushort[_indexBufferSize];
            }
        }
        
        public GraphicsDevice GraphicsDevice { get; }

        public void Begin()
        {
            _storedItems = 0;
        }

        public void End()
        {
            if (_storedItems == 0)
                return;
            
            Flush(true);
        }

        public void Draw(Texture2D texture, Rectangle rectangle)
        {
            unsafe
            {
                if (_lastTexture == null)
                {
                    _lastTexture = texture;
                }
                else if (_lastTexture != texture)
                {
                    Flush();
                    _lastTexture = texture;                
                }
                
                if (Shader != _lastShader)
                {
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
                    dest[0].Position.X = rectangle.Left;
                    dest[0].Position.Y = rectangle.Bottom;
                    dest[0].TexCoord.X = 0;
                    dest[0].TexCoord.Y = 0;
                    dest[0].Color.R = byte.MaxValue;
                    dest[0].Color.G = byte.MaxValue;
                    dest[0].Color.B = byte.MaxValue;
                    dest[0].Color.A = byte.MaxValue;
                    
                    dest[1].Position.X = rectangle.Right;
                    dest[1].Position.Y = rectangle.Bottom;
                    dest[1].TexCoord.X = 1;
                    dest[1].TexCoord.Y = 0;
                    dest[1].Color.R = byte.MaxValue;
                    dest[1].Color.G = byte.MaxValue;
                    dest[1].Color.B = byte.MaxValue;
                    dest[1].Color.A = byte.MaxValue;
                    
                    dest[2].Position.X = rectangle.Right;
                    dest[2].Position.Y = rectangle.Top;
                    dest[2].TexCoord.X = 1;
                    dest[2].TexCoord.Y = 1;
                    dest[2].Color.R = byte.MaxValue;
                    dest[2].Color.G = byte.MaxValue;
                    dest[2].Color.B = byte.MaxValue;
                    dest[2].Color.A = byte.MaxValue;
                    
                    dest[3].Position.X = rectangle.Left;
                    dest[3].Position.Y = rectangle.Top;
                    dest[3].TexCoord.X = 0;
                    dest[3].TexCoord.Y = 1;
                    dest[3].Color.R = byte.MaxValue;
                    dest[3].Color.G = byte.MaxValue;
                    dest[3].Color.B = byte.MaxValue;
                    dest[3].Color.A = byte.MaxValue;
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
        }

        private void Flush(bool inEnd = false)
        {
            Debug.Assert(Shader != null);
            Debug.Assert(_lastTexture != null);

            unsafe
            {
                fixed (VertexPosition2DTextureColor* vtx = &_vertexData[0])
                fixed (ushort* idx = &_indexData[0])
                {
                    _vertexBuffer.DiscardData();
                    _vertexBuffer.SetDataPointer((IntPtr)vtx, _storedItems * 4 * sizeof(VertexPosition2DTextureColor));
                    _indexBuffer.DiscardData();
                    _indexBuffer.SetDataPointer((IntPtr)idx, _storedItems * 6 * sizeof(ushort));
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
            
            _storedItems = 0;
        }
        
        private unsafe void EnsureBufferLargeEnough()
        {
            if (_storedItems * 4 > _vertexBufferSize)
            {
                _log.Debug("SpriteBatch capacity increased from " + _vertexBufferSize + " to " + (int)(_vertexBufferSize * 1.5f));
                _vertexBufferSize = (int)(_vertexBufferSize * 1.5f);    // To increase capacity
                _indexBufferSize = (int)(_vertexBufferSize * 1.5f);     // To have 2:3 ratio from vertex to index

                // Increase capacity of _vertexData
                
                var tempVertexData = new VertexPosition2DTextureColor[_vertexData.Length];

                fixed (void* vertexSourcePtr = &_vertexData[0])
                fixed (void* vertexDestPtr = &tempVertexData[0])
                {
                    // Store vertex data in temp array
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

                _vertexBuffer = VertexBuffer.Create(GraphicsDevice,
                                                    _vertexBufferSize * sizeof(VertexPosition2DTextureColor),
                                                    VertexTypeInfo.Get<VertexPosition2DTextureColor>(),
                                                    BufferUsage.DynamicDraw,
                                                    clear: false,
                                                    dynamic: true,
                                                    mutable: true);
                
                _indexBuffer = IndexBuffer.Create(GraphicsDevice,
                                                  _indexBufferSize,
                                                  sizeof(ushort),
                                                  DrawElementsType.UnsignedShort,
                                                  BufferUsage.DynamicDraw,
                                                  clear: false,
                                                  mutable: true);
            }
        }
    }
}