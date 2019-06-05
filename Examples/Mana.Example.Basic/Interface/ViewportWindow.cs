using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using Mana.Asset;
using Mana.Graphics;
using Mana.Graphics.Buffers;
using Mana.Graphics.Shader;
using Mana.Graphics.Textures;
using Mana.IMGUI;
using Mana.Utilities.Extensions;

namespace Mana.Example.Basic.Interface
{
    public class ViewportWindow : EditorWindow
    {
        private Texture2D _texture;
        private FrameBuffer _viewport;
        private SpriteBatch _spriteBatch;
        private ShaderProgram _spriteShader;
        
        private IntPtr _viewportHandle;
        
        public ViewportWindow(RenderContext renderContext, AssetManager assetManager)
        {
            RenderContext = renderContext;

            _texture = assetManager.Load<Texture2D>("./Assets/Textures/mittens.png");
            _viewport = new FrameBuffer(renderContext, 500, 500, FrameBufferFlags.Color);
            _spriteBatch = new SpriteBatch(renderContext);
            _spriteShader = ImGuiShaderFactory.CreateShaderProgram(renderContext.ResourceManager);

            _spriteBatch.Shader = _spriteShader;
            
            _viewportHandle = ImGuiHelper.BindTexture(_viewport.ColorTexture);
        }

        public override string Name => "Viewport";
        
        public RenderContext RenderContext { get; }
        
        public override void Render(float time, float deltaTime)
        {
            RenderContext.BindFrameBuffer(_viewport);
            
            var proj = Matrix4x4.CreateOrthographicOffCenter(0, 500, 500, 0, -1.0f, 1.0f);
            _spriteShader.SetUniform("projection", ref proj);

            var prevViewport = RenderContext.ViewportRectangle;
            var prevScissor = RenderContext.ScissorRectangle;

            RenderContext.ViewportRectangle = new Rectangle(0, 0, 500, 500);
            RenderContext.ScissorRectangle = new Rectangle(0, 0, 500, 500);
            
            var p = RenderToViewport(time, deltaTime);
            
            RenderContext.BindFrameBuffer(null);

            RenderContext.ViewportRectangle = prevViewport;
            RenderContext.ScissorRectangle = prevScissor;
            
            ImGuiHelper.Image(_viewportHandle, 500, 500);
        }

        private Point RenderToViewport(float time, float deltaTime)
        {
            RenderContext.Clear(Color.Red);

            _spriteBatch.Begin();

            Point mouse = (ManaWindow.MainWindow.Input.MousePosition.ToVector2() - ImGuiHelper.GetCursorScreenPos()).ToPoint();
            
            _spriteBatch.Draw(_texture, new Rectangle(0, 0, 500, 500));
            _spriteBatch.Draw(_texture, new Rectangle(mouse.X, mouse.Y, 50, 50));
            
            _spriteBatch.End();

            return mouse;
        }

        protected override void PreRender()
        {
            // ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
        }

        protected override void PostRender()
        {
            // ImGui.PopStyleVar();
        }
    }
}