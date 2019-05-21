using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using ImGuiNET;
using Mana.Graphics;
using Mana.Graphics.Buffers;
using Mana.Graphics.Shaders;
using Mana.Graphics.Vertex;
using Mana.Graphics.Vertex.Types;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Buffer = System.Buffer;
using Vector2 = System.Numerics.Vector2;

//
// Modern OpenGL IMGUI Renderer adapted from Eric Mellino's IMGUI renderer for XNA: 
//     https://github.com/mellinoe/ImGui.NET/blob/master/src/ImGui.NET.SampleProgram.XNA/ImGuiRenderer.cs
//

namespace Mana.IMGUI
{
    public class ImGuiRenderer : GameComponent
    {
        private ImGuiViewportManager _viewportManager;
        private List<int> _keys = new List<int>();
        
        private Dictionary<IntPtr, Texture2D> _boundTextures = new Dictionary<IntPtr, Texture2D>();
        private int _textureID;
        
        private VertexPosition2DTextureColor[] _vertexData;
        private VertexBuffer _vertexBuffer;
        private int _vertexBufferSize;

        private ushort[] _indexData;
        private IndexBuffer _indexBuffer;
        private int _indexBufferSize;
        
        private ImGuiIOPtr _io;

        private Matrix4x4 _projection;
        private float _displayX = float.MinValue;
        private float _displayY = float.MinValue;
        private ShaderProgram _shaderProgram;
        
        private int _drawCalls;
        private int _primitiveCount;
        
        public ImGuiRenderer()
        {
            ImGuiHelper._imguiRenderer = this;
            
            _viewportManager = new ImGuiViewportManager(this);
        }

        public int DrawCalls => _drawCalls;
        public int PrimitiveCount => _primitiveCount;
        
        public override void OnAddedToGame(Game game)
        {
            base.OnAddedToGame(game);
            
            ImGui.SetCurrentContext(ImGui.CreateContext());

            _io = ImGui.GetIO();
            
            SetupInput();
            RebuildFontAtlas();
            SetStyleDefaults();

            _shaderProgram = CreateShaderProgram();

            _io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            _io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;

            _io.BackendFlags |= ImGuiBackendFlags.PlatformHasViewports;
            _io.BackendFlags |= ImGuiBackendFlags.HasMouseHoveredViewport;

            //_viewportManager.Initialize(game);
        }

        public override void Dispose()
        {
            foreach (var kvp in _boundTextures)
            {
                kvp.Value.Dispose();
            }
            
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
            _shaderProgram.Dispose();
        }

        private unsafe void RebuildFontAtlas()
        {
            _io.Fonts.GetTexDataAsRGBA32(out byte* pixelData, out int width, out int height, out _);

            Texture2D fontTexture = Texture2D.CreateFromRGBAPointer(GraphicsDevice, width, height, pixelData);
            fontTexture.Label = "IMGUI Font Texture";

            _io.Fonts.SetTexID(BindTexture(fontTexture));
            _io.Fonts.ClearTexData();
        }

        public IntPtr BindTexture(Texture2D texture)
        {
            IntPtr id = new IntPtr(_textureID++);
            _boundTextures.Add(id, texture);
            return id;
        }

        public void UnbindTexture(IntPtr id)
        {
            _boundTextures.Remove(id);
        }

        private void SetupInput()
        {
            var keyMap = _io.KeyMap;
            
            _keys.Add(keyMap[(int)ImGuiKey.Tab] = (int)Key.Tab);
            _keys.Add(keyMap[(int)ImGuiKey.LeftArrow] = (int)Key.Left);
            _keys.Add(keyMap[(int)ImGuiKey.RightArrow] = (int)Key.Right);
            _keys.Add(keyMap[(int)ImGuiKey.UpArrow] = (int)Key.Up);
            _keys.Add(keyMap[(int)ImGuiKey.DownArrow] = (int)Key.Down);
            _keys.Add(keyMap[(int)ImGuiKey.PageUp] = (int)Key.PageUp);
            _keys.Add(keyMap[(int)ImGuiKey.PageDown] = (int)Key.PageDown);
            _keys.Add(keyMap[(int)ImGuiKey.Home] = (int)Key.Home);
            _keys.Add(keyMap[(int)ImGuiKey.End] = (int)Key.End);
            _keys.Add(keyMap[(int)ImGuiKey.Delete] = (int)Key.Delete);
            _keys.Add(keyMap[(int)ImGuiKey.Backspace] = (int)Key.Backspace);
            _keys.Add(keyMap[(int)ImGuiKey.Enter] = (int)Key.Enter);
            _keys.Add(keyMap[(int)ImGuiKey.Escape] = (int)Key.Escape);
            _keys.Add(keyMap[(int)ImGuiKey.A] = (int)Key.A);
            _keys.Add(keyMap[(int)ImGuiKey.C] = (int)Key.C);
            _keys.Add(keyMap[(int)ImGuiKey.V] = (int)Key.V);
            _keys.Add(keyMap[(int)ImGuiKey.X] = (int)Key.X);
            _keys.Add(keyMap[(int)ImGuiKey.Y] = (int)Key.Y);
            _keys.Add(keyMap[(int)ImGuiKey.Z] = (int)Key.Z);

            Input.KeyTyped += (c) =>
            {
                if (c == '\t')
                {
                    return;
                }

                _io.AddInputCharacter(c);
            };

            ImGui.GetIO().Fonts.AddFontDefault();
        }

        public override void EarlyRender(float time, float deltaTime)
        {
            BeforeLayout(deltaTime);
        }

        public override void LateRender(float time, float deltaTime)
        {
            AfterLayout();
        }

        private void BeforeLayout(float deltaTime)
        {
            ImGui.GetIO().DeltaTime = deltaTime;
            UpdateInput();
            ImGui.NewFrame();
        }

        private void AfterLayout()
        {
            if (!Visible)
                return;

            ImGui.Render();

            RenderDrawData(ImGui.GetDrawData(), Game.Window.Width, Game.Window.Height);
            
            ImGui.UpdatePlatformWindows();
            ImGui.RenderPlatformWindowsDefault();
        }

        private void UpdateInput()
        {
            var keysDown = _io.KeysDown;
            
            for (int i = 0; i < _keys.Count; i++)
            {
                keysDown[_keys[i]] = Input.IsKeyDown((Key)_keys[i]);
            }
            
            _io.KeyShift = Input.IsKeyDown(Key.LeftShift) || Input.IsKeyDown(Key.RightShift);
            _io.KeyCtrl = Input.IsKeyDown(Key.LeftControl) || Input.IsKeyDown(Key.RightControl);
            _io.KeyAlt = Input.IsKeyDown(Key.LeftAlt) || Input.IsKeyDown(Key.RightAlt);
            // TODO: Windows key.
            
            _io.DisplaySize = new Vector2(Game.Window.Width, Game.Window.Height);
            _io.DisplayFramebufferScale = Vector2.One;
            
            _io.MousePos = new Vector2(Input.MousePosition.X, Input.MousePosition.Y);
            
            var mouseDown = _io.MouseDown;
            
            mouseDown[0] = Input.IsMouseDown(MouseButton.Left);
            mouseDown[1] = Input.IsMouseDown(MouseButton.Right);
            mouseDown[2] = Input.IsMouseDown(MouseButton.Middle);
            
            int scrollDelta = Input.MouseWheelDelta;
            _io.MouseWheel = scrollDelta > 0 ? 1 : scrollDelta < 0 ? -1 : 0;
        }

        private void RenderDrawData(ImDrawDataPtr drawData, int windowWidth, int windowHeight)
        {
            // If width or height is zero, game is minimized, so don't draw anything.
            if (windowWidth == 0 || windowHeight == 0)
                return;

            Rectangle lastViewport = GraphicsDevice.ViewportRectangle;
            Rectangle lastScissorRectangle = GraphicsDevice.ScissorRectangle;
            bool lastBlend = GraphicsDevice.Blend;
            bool lastDepthTest = GraphicsDevice.DepthTest;

            GraphicsDevice.Blend = true;
            GraphicsDevice.CullBackfaces = false;
            GraphicsDevice.DepthTest = false;
            GraphicsDevice.ScissorTest = true;
            
            drawData.ScaleClipRects(_io.DisplayFramebufferScale);
            
            GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, windowWidth, windowHeight);
            GraphicsDevice.ViewportRectangle = new Rectangle(0, 0, windowWidth, windowHeight);

            UpdateBuffers(drawData);

            RenderCommandLists(drawData);
            
            GraphicsDevice.ViewportRectangle = lastViewport;
            GraphicsDevice.ScissorRectangle = lastScissorRectangle;
            GraphicsDevice.Blend = lastBlend;
            GraphicsDevice.DepthTest = lastDepthTest;
        }

        private unsafe void UpdateBuffers(ImDrawDataPtr drawData)
        {
            if (drawData.TotalVtxCount == 0)
                return;
            
            // Expand buffers if we need more room.
            if (drawData.TotalVtxCount > _vertexBufferSize)
            {
                _vertexBuffer?.Dispose();

                _vertexBufferSize = (int)(drawData.TotalVtxCount * 1.5f);
                _vertexBuffer = VertexBuffer.Create<VertexPosition2DTextureColor>(GraphicsDevice,
                                                                                  _vertexBufferSize,
                                                                                  BufferUsage.StreamDraw,
                                                                                  immutable: true);
                _vertexBuffer.Label = "IMGUI VertexBuffer";
                _vertexData = new VertexPosition2DTextureColor[_vertexBufferSize];
            }

            if (drawData.TotalIdxCount > _indexBufferSize)
            {
                _indexBuffer?.Dispose();

                _indexBufferSize = (int)(drawData.TotalIdxCount * 1.5f);
                _indexBuffer = IndexBuffer.Create<ushort>(GraphicsDevice,
                                                          _indexBufferSize,
                                                          BufferUsage.StreamDraw,
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
                    Buffer.MemoryCopy((void*)cmdList.VtxBuffer.Data, 
                                      vtxDstPtr, 
                                      _vertexData.Length * sizeof(ImDrawVert), 
                                      cmdList.VtxBuffer.Size * sizeof(ImDrawVert));
                    Buffer.MemoryCopy((void*)cmdList.IdxBuffer.Data, 
                                      idxDstPtr, 
                                      _indexData.Length  * sizeof(ushort), 
                                      cmdList.IdxBuffer.Size * sizeof(ushort));
                }

                vtxOffset += cmdList.VtxBuffer.Size;
                idxOffset += cmdList.IdxBuffer.Size;
            }
            
            // Copy the managed byte arrays to the gpu vertex and index buffers
            _vertexBuffer.SubData(_vertexData, 0, drawData.TotalVtxCount);
            _indexBuffer.SubData(_indexData, 0, drawData.TotalIdxCount);
        }

        private void RenderCommandLists(ImDrawDataPtr drawData)
        {
            if (drawData.TotalVtxCount == 0)
                return;
            
            GraphicsDevice.BindVertexBuffer(_vertexBuffer);
            GraphicsDevice.BindIndexBuffer(_indexBuffer);
            GraphicsDevice.BindShaderProgram(_shaderProgram);

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

                    if (!_boundTextures.ContainsKey(drawCmd.TextureId))
                    {
                        throw new InvalidOperationException($"Could not find a texture with id '{drawCmd.TextureId}', please check your bindings");
                    }
                    
                    GraphicsDevice.ScissorRectangle = new Rectangle((int)drawCmd.ClipRect.X, 
                                                                    (int)(_io.DisplaySize.Y - drawCmd.ClipRect.W),
                                                                    (int)(drawCmd.ClipRect.Z - drawCmd.ClipRect.X),
                                                                    (int)(drawCmd.ClipRect.W - drawCmd.ClipRect.Y));

                    UpdateShader(_boundTextures[drawCmd.TextureId]);

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
            
            _drawCalls = drawCalls;
            _primitiveCount = primitiveCount;
        }

        private void UpdateShader(Texture2D texture)
        {
            if (_io.DisplaySize.X != _displayX ||
                _io.DisplaySize.Y != _displayY)
            {
                _projection = Matrix4x4.CreateOrthographicOffCenter(0f, _io.DisplaySize.X, _io.DisplaySize.Y, 0, -1f, 1f);
                _displayX = _io.DisplaySize.X;
                _displayY = _io.DisplaySize.Y;
                _shaderProgram.SetUniform("projection", ref _projection);
            }

            GraphicsDevice.BindTexture(0, texture);
        }

        private VertexShader CreateVertexShader()
        {
            return new VertexShader(GraphicsDevice, @"#version 330 core

                layout (location = 0) in vec2 aPos;
                layout (location = 1) in vec2 aTexCoord;
                layout (location = 2) in vec4 aColor;
                
                out vec2 TexCoord;
                out vec4 Color;

                uniform mat4 projection;
                
                void main()
                {
                    gl_Position = projection * vec4(aPos, 1.0, 1.0);
                    TexCoord = aTexCoord;
                    Color = aColor;
                }"
            );
        }
        
        private FragmentShader CreateFragmentShader()
        {
            return new FragmentShader(GraphicsDevice, @"#version 330 core
                
                out vec4 FragColor;
                
                in vec2 TexCoord;
                in vec4 Color;
                
                uniform sampler2D texture0;
                
                void main()
                {
                    FragColor = texture(texture0, TexCoord) * Color;
                }"
            );
        }

        private ShaderProgram CreateShaderProgram()
        {
            VertexShader vertexShader = CreateVertexShader();
            FragmentShader fragmentShader = CreateFragmentShader();
            
            ShaderProgram shaderProgram = new ShaderProgram(GraphicsDevice);

            shaderProgram.Link(vertexShader, fragmentShader);
            
            vertexShader.Dispose();
            fragmentShader.Dispose();
            
            ShaderHelper.BuildShaderInfo(shaderProgram);

            shaderProgram.Label = "IMGUI ShaderProgram";
            
            return shaderProgram;
        }

        public void SetStyleDefaults()
        {
            ImGuiStylePtr style = ImGui.GetStyle();

            // style.WindowRounding = 0.0f;
            // style.ChildRounding = 0.0f;
            // style.FrameRounding = 0.0f;
            // style.GrabRounding = 0.0f;
            // style.PopupRounding = 0.0f;
            // style.ScrollbarRounding = 0.0f;
            // style.TabRounding = 0.0f;
        }
    }
}