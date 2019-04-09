using System;
using System.Collections.Generic;
using System.Drawing;
using ImGuiNET;
using Mana.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Mana.IMGUI
{
    public class ImGuiRenderer : GameComponent
    {
        private int _textureID;
        private Dictionary<IntPtr, Texture2D> _boundTextures = new Dictionary<IntPtr, Texture2D>();
        private List<int> _keys = new List<int>();
        
        private ImGuiIOPtr _io;
        
        public ImGuiRenderer()
        {
        }

        public override void OnAddedToGame(Game game)
        {
            base.OnAddedToGame(game);
            
            ImGui.SetCurrentContext(ImGui.CreateContext());

            _io = ImGui.GetIO();
            
            SetupInput();
            RebuildFontAtlas();
            SetStyleDefaults();
        }

        private unsafe void RebuildFontAtlas()
        {
            _io.Fonts.GetTexDataAsRGBA32(out byte* pixelData, out int width, out int height, out _);
            
            Texture2D fontTexture = new Texture2D(GraphicsDevice);
            fontTexture.SetDataFromAlpha(pixelData, width, height);

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
            // TODO: Windows key?
            
            _io.DisplaySize = new System.Numerics.Vector2(Game.Window.Width, Game.Window.Height);
            _io.DisplayFramebufferScale = System.Numerics.Vector2.One;
            
            _io.MousePos = new System.Numerics.Vector2(Input.MousePosition.X, Input.MousePosition.Y);

            var mouseDown = _io.MouseDown;
            
            mouseDown[0] = Input.IsMouseDown(MouseButton.Left);
            mouseDown[1] = Input.IsMouseDown(MouseButton.Right);
            mouseDown[2] = Input.IsMouseDown(MouseButton.Middle);
            
            int scrollDelta = Input.MouseWheelDelta;
            _io.MouseWheel = scrollDelta > 0 ? 1 : scrollDelta < 0 ? -1 : 0;
        }

        private void AfterLayout()
        {
            if (!Visible)
                return;

            ImGui.Render();

            RenderDrawData(ImGui.GetDrawData(), Game.Window.Width, Game.Window.Height);
        }

        private void RenderDrawData(ImDrawDataPtr drawData, int windowWidth, int windowHeight)
        {
            if (windowWidth == 0 || windowHeight == 0)
            {
                // Game is minimized, so don't draw anything.
                return;
            }
            
            Rectangle oldScissorRectangle = GraphicsDevice.ScissorRectangle;
            //bool oldWireframeRendering = GraphicsDevice.WireframeRendering;
            Rectangle oldViewport = GraphicsDevice.ViewportRectangle;
            bool oldBlend = GraphicsDevice.Blend;
            bool oldCullBackfaces = GraphicsDevice.CullBackfaces;
            bool oldDepthTest = GraphicsDevice.DepthTest;
            bool oldScissorTest = GraphicsDevice.ScissorTest;

            GraphicsDevice.DepthTest = false;
            //GraphicsDevice.WireframeRendering = false;
            GraphicsDevice.BindShaderProgram(null);
            GraphicsDevice.BindTexture(0, (Texture2D)null);
            //GraphicsDevice.BindVertexBuffer(null);
            //GraphicsDevice.BindIndexBuffer(null);
            GraphicsDevice.ViewportRectangle = new Rectangle(0, 0, windowWidth, windowHeight);
            GL.UseProgram(0);

            GL.PushAttrib(AttribMask.EnableBit | AttribMask.ColorBufferBit | AttribMask.TransformBit);
            GraphicsDevice.Blend = true;
            GraphicsDevice.CullBackfaces = false;
            GraphicsDevice.DepthTest = false;
            GraphicsDevice.ScissorTest = true;
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.Enable(EnableCap.Texture2D);
            GLHelper.CheckLastError();

            for (int i = 0; i < 3; i++)
            {
                GL.DisableVertexAttribArray(i);
            }

            // Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
            //ImGuiIOPtr io = ImGui.GetIO();
            drawData.ScaleClipRects(_io.DisplayFramebufferScale);

            // Setup orthographic projection matrix
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Ortho(
                     0.0f,
                     _io.DisplaySize.X / _io.DisplayFramebufferScale.X,
                     _io.DisplaySize.Y / _io.DisplayFramebufferScale.Y,
                     0.0f,
                     -1.0f,
                     1.0f);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();
            GLHelper.CheckLastError();

            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                unsafe
                {
                    ImDrawListPtr cmdList = drawData.CmdListsRange[n];
                    IntPtr vtxBufferData = cmdList.VtxBuffer.Data;
                    ushort* idxBufferData = (ushort*)cmdList.IdxBuffer.Data;

                    GL.VertexPointer(2, VertexPointerType.Float, sizeof(ImDrawVert), vtxBufferData);
                    GL.TexCoordPointer(2, TexCoordPointerType.Float, sizeof(ImDrawVert), vtxBufferData + 8);
                    GL.ColorPointer(4, ColorPointerType.UnsignedByte, sizeof(ImDrawVert), vtxBufferData + 16);
                    GLHelper.CheckLastError();

                    for (int cmdIndex = 0; cmdIndex < cmdList.CmdBuffer.Size; cmdIndex++)
                    {
                        ImDrawCmdPtr pcmd = cmdList.CmdBuffer[cmdIndex];
                        {
                            IntPtr textureId = pcmd.TextureId;

                            GraphicsDevice.BindTexture(0, _boundTextures[textureId]);

                            GraphicsDevice.ScissorRectangle = new Rectangle((int)pcmd.ClipRect.X,
                                                                            (int)(_io.DisplaySize.Y - pcmd.ClipRect.W),
                                                                            (int)(pcmd.ClipRect.Z - pcmd.ClipRect.X),
                                                                            (int)(pcmd.ClipRect.W - pcmd.ClipRect.Y));
                            ushort[] indices = new ushort[pcmd.ElemCount];
                            for (int i = 0; i < indices.Length; i++)
                            {
                                indices[i] = idxBufferData[i];
                            }

                            GL.DrawElements(OpenTK.Graphics.OpenGL.PrimitiveType.Triangles, (int)pcmd.ElemCount,
                                            DrawElementsType.UnsignedShort, new IntPtr(idxBufferData));
                            GLHelper.CheckLastError();
                        }

                        idxBufferData += pcmd.ElemCount;
                    }
                }
            }

            // Restore modified state
            GL.DisableClientState(ArrayCap.ColorArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.PopAttrib();

            GraphicsDevice.BindShaderProgram(null);
            GraphicsDevice.BindTexture(0, null);
            //GraphicsDevice.BindVertexBuffer(null);
            //GraphicsDevice.BindIndexBuffer(null);

            GraphicsDevice.ScissorRectangle = oldScissorRectangle;
            //GraphicsDevice.WireframeRendering = oldWireframeRendering;
            GraphicsDevice.ViewportRectangle = oldViewport;
            GraphicsDevice.Blend = oldBlend;
            GraphicsDevice.CullBackfaces = oldCullBackfaces;
            GraphicsDevice.DepthTest = oldDepthTest;
            GraphicsDevice.ScissorTest = oldScissorTest;

            GLHelper.CheckLastError();
        }
        
        public void SetStyleDefaults()
        {
            ImGuiStylePtr style = ImGui.GetStyle();

            style.WindowRounding = 0.0f;
            style.ChildRounding = 0.0f;
            style.FrameRounding = 0.0f;
            style.GrabRounding = 0.0f;
            style.PopupRounding = 0.0f;
            style.ScrollbarRounding = 0.0f;
            style.TabRounding = 0.0f;
        }
    }
}