using System;
using System.Drawing;
using System.Numerics;
using ImGuiNET;
using Mana.Graphics;
using Mana.Graphics.Buffers;
using Mana.Graphics.Shader;
using Mana.Graphics.Textures;
using Mana.Graphics.Vertex.Types;
using OpenTK.Graphics.OpenGL4;

namespace Mana.IMGUI
{
    public class ImGuiRenderer
    {
        private VertexPosition2DTextureColor[] _vertexData;
        private VertexBuffer _vertexBuffer;
        private int _vertexBufferSize;

        private ushort[] _indexData;
        private IndexBuffer _indexBuffer;
        private int _indexBufferSize;

        private ImGuiSystem _imGuiSystem;
        private ManaWindow _window;
        
        internal Matrix4x4 _projection;
        internal ShaderProgram _shaderProgram;
        
        public ImGuiRenderer(ImGuiSystem imguiSystem, ManaWindow window)
        {
            _imGuiSystem = imguiSystem;
            _window = window;
            _shaderProgram = ImGuiShaderFactory.CreateShaderProgram(ManaWindow.MainWindow.ResourceManager);
            
            var offset = _window.Location - ((Size)ManaWindow.MainWindow.Location + new Size(8, 31));
            _projection = Matrix4x4.CreateOrthographicOffCenter(offset.X, _window.Width + offset.X, _window.Height + offset.Y, offset.Y, -1f, 1f);
        }
        
        public void RenderDrawData(RenderContext renderContext, ImDrawDataPtr drawData, int windowWidth, int windowHeight)
        {
            // If width or height is zero, game is minimized, so don't draw anything.
            if (windowWidth == 0 || windowHeight == 0)
                return;
            
            Rectangle lastViewport = renderContext.ViewportRectangle;
            Rectangle lastScissorRectangle = renderContext.ScissorRectangle;
            bool lastBlend = renderContext.Blend;
            bool lastDepthTest = renderContext.DepthTest;
            bool lastScissorTest = renderContext.ScissorTest;
            
            renderContext.Blend = true;
            renderContext.CullBackfaces = false;
            renderContext.DepthTest = false;
            renderContext.ScissorTest = true;
            
            renderContext.ScissorRectangle = new Rectangle(0, 0, windowWidth, windowHeight);
            renderContext.ViewportRectangle = new Rectangle(0, 0, windowWidth, windowHeight); 
            
            UpdateBuffers(renderContext, drawData);

            RenderCommandLists(renderContext, drawData);
         
            renderContext.ViewportRectangle = lastViewport;
            renderContext.ScissorRectangle = lastScissorRectangle;
            renderContext.Blend = lastBlend;
            renderContext.DepthTest = lastDepthTest;
            renderContext.ScissorTest = lastScissorTest;
        }
        
        public unsafe void UpdateBuffers(RenderContext renderContext, ImDrawDataPtr drawData)
        {
            if (drawData.TotalVtxCount == 0)
                return;
            
            // Expand buffers if we need more room.
            if (drawData.TotalVtxCount > _vertexBufferSize)
            {
                _vertexBuffer?.Dispose();

                _vertexBufferSize = (int)(drawData.TotalVtxCount * 1.5f);
                _vertexBuffer = VertexBuffer.Create<VertexPosition2DTextureColor>(renderContext,
                                                                                  _vertexBufferSize,
                                                                                  BufferUsageHint.StreamDraw,
                                                                                  immutable: true);
                _vertexBuffer.Label = "IMGUI VertexBuffer";
                _vertexData = new VertexPosition2DTextureColor[_vertexBufferSize];
            }

            if (drawData.TotalIdxCount > _indexBufferSize)
            {
                _indexBuffer?.Dispose();

                _indexBufferSize = (int)(drawData.TotalIdxCount * 1.5f);
                _indexBuffer = IndexBuffer.Create<ushort>(renderContext,
                                                          _indexBufferSize,
                                                          BufferUsageHint.StreamDraw,
                                                          immutable: true);
                _indexBuffer.Label = "IMGUI IndexBuffer";
                _indexData = new ushort[_indexBufferSize];
            }
            
            // Copy ImGui's vertices and indices to a set of managed byte arrays
            int vtxOffset = 0;
            int idxOffset = 0;

            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr cmdList = drawData.CmdListsRange[n];

                fixed (void* vtxDstPtr = &_vertexData[vtxOffset])
                fixed (void* idxDstPtr = &_indexData[idxOffset])
                {
                    System.Buffer.MemoryCopy((void*)cmdList.VtxBuffer.Data, 
                                      vtxDstPtr, 
                                      _vertexData.Length * sizeof(ImDrawVert), 
                                      cmdList.VtxBuffer.Size * sizeof(ImDrawVert));
                    System.Buffer.MemoryCopy((void*)cmdList.IdxBuffer.Data, 
                                      idxDstPtr, 
                                      _indexData.Length  * sizeof(ushort), 
                                      cmdList.IdxBuffer.Size * sizeof(ushort));
                }

                vtxOffset += cmdList.VtxBuffer.Size;
                idxOffset += cmdList.IdxBuffer.Size;
            }
            
            // Copy the managed byte arrays to the gpu vertex and index buffers
            _vertexBuffer.SubData(renderContext, _vertexData, 0, drawData.TotalVtxCount);
            _indexBuffer.SubData(renderContext, _indexData, 0, drawData.TotalIdxCount);
        }
        
        public void UpdateShader(RenderContext renderContext, Texture2D texture)
        {
            if (_window == ManaWindow.MainWindow)
            {
                _projection = Matrix4x4.CreateOrthographicOffCenter(0f, _window.Width, _window.Height, 0, -1f, 1f);                
            }
            else
            {
                var offset = _window.Location - ((Size)ManaWindow.MainWindow.Location + new Size(8, 31));
                _projection = Matrix4x4.CreateOrthographicOffCenter(offset.X, _window.Width + offset.X, _window.Height + offset.Y, offset.Y, -1f, 1f);
            }
            
            _shaderProgram.SetUniform("projection", ref _projection);
            renderContext.BindTexture(0, texture);
        }
        
        public void RenderCommandLists(RenderContext renderContext, 
                                        ImDrawDataPtr drawData)
        {
            if (drawData.TotalVtxCount == 0)
                return;
            
            renderContext.BindVertexBuffer(_vertexBuffer);
            renderContext.BindIndexBuffer(_indexBuffer);
            renderContext.BindShaderProgram(_shaderProgram);

            int vtxOffset = 0;
            int idxOffset = 0;
            
            _vertexBuffer.VertexTypeInfo.Apply(_shaderProgram);

            int drawCalls = 0;
            int primitiveCount = 0;
            
            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr cmdList = drawData.CmdListsRange[n];

                for (int cmdi = 0; cmdi < cmdList.CmdBuffer.Size; cmdi++)
                {
                    ImDrawCmdPtr drawCmd = cmdList.CmdBuffer[cmdi];

                    if (!_imGuiSystem.BoundTextures.ContainsKey(drawCmd.TextureId))
                    {
                        throw new InvalidOperationException($"Could not find a texture with id '{drawCmd.TextureId}', please check your bindings");
                    }

                    if (_window == ManaWindow.MainWindow)
                    {
                        renderContext.ScissorRectangle = new Rectangle((int)drawCmd.ClipRect.X,
                                                                       (int)(_window.Height - drawCmd.ClipRect.W),
                                                                       (int)(drawCmd.ClipRect.Z - drawCmd.ClipRect.X),
                                                                       (int)(drawCmd.ClipRect.W - drawCmd.ClipRect.Y));
                    }
                    else
                    {
                        var offset = _window.Location - ((Size)ManaWindow.MainWindow.Location + new Size(8, 31));
                        renderContext.ScissorRectangle = new Rectangle((int)drawCmd.ClipRect.X - offset.X,
                                                                       (int)(_window.Height - drawCmd.ClipRect.W) + offset.Y,
                                                                       (int)(drawCmd.ClipRect.Z - drawCmd.ClipRect.X),
                                                                       (int)(drawCmd.ClipRect.W - drawCmd.ClipRect.Y));
                    }

                    UpdateShader(renderContext, _imGuiSystem.BoundTextures[drawCmd.TextureId]);

                    int baseVertex = vtxOffset;
                    int minVertexIndex = 0;
                    int numVertices = cmdList.VtxBuffer.Size;
                    int numIndices = cmdList.IdxBuffer.Size;
                    int startIndex = idxOffset;
                    
                    GL.DrawRangeElementsBaseVertex(PrimitiveType.Triangles,
                                                   minVertexIndex,
                                                   minVertexIndex + numVertices - 1,
                                                   (int)drawCmd.ElemCount,
                                                   DrawElementsType.UnsignedShort,
                                                   (IntPtr)(startIndex * sizeof(ushort)),
                                                   baseVertex);
                    
                    unchecked
                    {
                        drawCalls++;
                        primitiveCount += numIndices;
                    }

                    idxOffset += (int)drawCmd.ElemCount;
                }

                vtxOffset += cmdList.VtxBuffer.Size;
            }
            
            // _drawCalls = drawCalls;
            // _primitiveCount = primitiveCount;
        }

        public void Dispose()
        {
            {
                
            }
            
            _vertexBuffer?.Dispose();
            _indexBuffer?.Dispose();
            _shaderProgram?.Dispose();
        }
    }
}