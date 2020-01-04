using System;
using System.Numerics;
using Mana.Graphics.Buffers;
using Mana.Graphics.Shaders;
using Mana.Graphics.Vertex.Types;
using Mana.Utilities;
using osuTK.Graphics.OpenGL4;

namespace Mana.Graphics
{
    public class LineBatch : GraphicsResource
    {
        private static Logger _log = Logger.Create();

        private const int MAX_BATCH_SIZE = 3000;

        private VertexBuffer _vertexBuffer;

        private int _vertexBufferSize;
        private VertexPosition2Color[] _vertexData;

        private int _vertexCount = 0;
        private bool _active = false;

        public LineBatch(RenderContext renderContext)
            : base(renderContext)
        {
            _vertexBufferSize = 64;

            CreateVertexBuffer();
            _vertexData = new VertexPosition2Color[_vertexBufferSize];
        }

        public ShaderProgram Shader { get; set; }

        protected override void Dispose(bool disposing)
        {
            _vertexBuffer.Dispose();
        }

        public void Begin()
        {
            if (_active)
                throw new InvalidOperationException("SpriteBatch already began.");

            _vertexCount = 0;
            _active = true;
        }

        public void End()
        {
            if (!_active)
                throw new InvalidOperationException("End() must be called before Begin() may be called.");

            if (_vertexCount != 0)
                Flush();

            _active = false;
        }

        public void DrawLine(Vector2 a, Vector2 b, Color color)
        {
            if (!_active)
                throw new InvalidOperationException("Begin() must be called before SpriteBatch may be used for drawing.");

            FlushIfNeeded();

            int vertexOffset = _vertexCount;

            _vertexCount += 2;
            EnsureBufferLargeEnough();

            _vertexData[vertexOffset + 0].Position.X = a.X;
            _vertexData[vertexOffset + 0].Position.Y = a.Y;
            _vertexData[vertexOffset + 0].Color = color;

            _vertexData[vertexOffset + 1].Position.X = b.X;
            _vertexData[vertexOffset + 1].Position.Y = b.Y;
            _vertexData[vertexOffset + 1].Color = color;
        }

        public void DrawLine(Vector2 a, Color colorA, Vector2 b, Color colorB)
        {
            if (!_active)
                throw new InvalidOperationException("Begin() must be called before SpriteBatch may be used for drawing.");

            FlushIfNeeded();

            int vertexOffset = _vertexCount;

            _vertexCount += 2;
            EnsureBufferLargeEnough();

            _vertexData[vertexOffset + 0].Position.X = a.X;
            _vertexData[vertexOffset + 0].Position.Y = a.Y;
            _vertexData[vertexOffset + 0].Color = colorA;

            _vertexData[vertexOffset + 1].Position.X = b.X;
            _vertexData[vertexOffset + 1].Position.Y = b.Y;
            _vertexData[vertexOffset + 1].Color = colorB;
        }

        private void FlushIfNeeded()
        {
            if (_vertexCount + 2 > ushort.MaxValue || _vertexCount >= MAX_BATCH_SIZE)
            {
                Flush();
            }
        }

        private void Flush()
        {
            Assert.That(Shader != null);

            unsafe
            {
                fixed (VertexPosition2Color* vertexPtr = &_vertexData[0])
                {
                    _vertexBuffer.SubData<VertexPosition2Color>((IntPtr)vertexPtr, 0, _vertexCount);
                }
            }

            bool prevDepthTest = ParentContext.DepthTest;
            ParentContext.DepthTest = false;

            ParentContext.BindVertexBuffer(_vertexBuffer);
            ParentContext.BindShaderProgram(Shader);

            _vertexBuffer.VertexTypeInfo.Apply(Shader);

            GL.DrawArrays(PrimitiveType.Lines, 0, _vertexCount);

            ParentContext.DepthTest = prevDepthTest;

            // unchecked
            // {
            //     Metrics._drawCalls++;
            //     Metrics._primitiveCount += _count * 6;
            // }

            _vertexCount = 0;
        }

        private unsafe void EnsureBufferLargeEnough()
        {
            if (_vertexCount > _vertexBufferSize)
            {
                _vertexBufferSize = (int)(_vertexBufferSize * 1.5f);    // To increase capacity

                if (_vertexBufferSize > MAX_BATCH_SIZE)
                    _vertexBufferSize = MAX_BATCH_SIZE;

                // Increase capacity of _vertexData
                var tempVertexData = new VertexPosition2Color[_vertexData.Length];

                fixed (void* vertexSourcePtr = &_vertexData[0])
                fixed (void* vertexDestPtr = &tempVertexData[0])
                {
                    System.Buffer.MemoryCopy(vertexSourcePtr,
                                      vertexDestPtr,
                                      tempVertexData.Length * sizeof(VertexPosition2Color),
                                      _vertexData.Length * sizeof(VertexPosition2Color));
                }

                // Increase vertexData capacity
                _vertexData = new VertexPosition2Color[_vertexBufferSize];

                fixed (void* vertexSourcePtr = &tempVertexData[0])
                fixed (void* vertexDestPtr = &_vertexData[0])
                {
                    // Store vertex data from temp array back into _vertexData
                    System.Buffer.MemoryCopy(vertexSourcePtr,
                                      vertexDestPtr,
                                      tempVertexData.Length * sizeof(VertexPosition2Color),
                                      tempVertexData.Length * sizeof(VertexPosition2Color));
                }

                CreateVertexBuffer();
            }
        }

        private void CreateVertexBuffer()
        {
            _vertexBuffer?.Dispose();
            _vertexBuffer = VertexBuffer.Create<VertexPosition2Color>(ParentContext,
                                                                      _vertexBufferSize,
                                                                      BufferUsageHint.StreamDraw,
                                                                      true);
            _vertexBuffer.Label = "PrimitiveBatch VertexBuffer";
        }
    }
}
