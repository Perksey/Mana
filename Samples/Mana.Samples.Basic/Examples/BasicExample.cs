using System;
using System.Numerics;
using ImGuiNET;
using Mana.Graphics;
using Mana.Graphics.Buffers;
using Mana.Graphics.Shaders;
using Mana.IMGUI;

namespace Mana.Samples.Basic.Examples
{
    public class BasicExample : Example
    {
        private Texture2D _texture;
        private SpriteBatch _spriteBatch;
        private ShaderProgram _spriteShader;

        private FrameBuffer _frameBuffer;

        private IntPtr _frameBufferHandle;
        
        public BasicExample(SampleGame game) : base(game)
        {
        }

        public override void Initialize()
        {
            _spriteShader = AssetManager.Load<ShaderProgram>("./Assets/Shaders/sprite.json");
            _texture = AssetManager.Load<Texture2D>("./Assets/Textures/mittens.png");
            _frameBuffer = new FrameBuffer(GraphicsDevice, Game.Window.Width, Game.Window.Height, FrameBufferFlags.Color);
            
            _spriteBatch = new SpriteBatch(GraphicsDevice)
            {
                Shader = _spriteShader,
            };

            _frameBufferHandle = ImGuiRenderer.Instance.BindTexture(_frameBuffer);
        }

        public override void Dispose()
        {
            _spriteBatch.Dispose();
            _frameBuffer.Dispose();

            _texture.Dispose();
            _spriteShader.Dispose();

            ImGuiRenderer.Instance.UnbindTexture(_frameBufferHandle);
        }

        public override void Update(float time, float deltaTime)
        {
            var proj = Matrix4x4.CreateOrthographicOffCenter(0f, Game.Window.Width, Game.Window.Height, 0, -1f, 1f);
            _spriteShader.SetUniform("projection", ref proj);

            _clear = !Input.IsMouseDown(MouseButton.Left);
        }

        private bool _clear = true;
        
        public override void Render(float time, float deltaTime)
        {
            if (_texture == null)
                return;
            
            GraphicsDevice.BindFrameBuffer(_frameBuffer);
            
            if (_clear)
            {
                GraphicsDevice.Clear(Color.Magenta);
            }
            
            _spriteBatch.Begin();
            var m = Input.MousePosition;
            _spriteBatch.Draw(_texture, new Rectangle(m.X, m.Y, 25, 25));
            _spriteBatch.End();
            
            GraphicsDevice.BindFrameBuffer(null);

            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_frameBuffer, new Rectangle(50, 50, 1280 / 3, 720 / 3));
            _spriteBatch.End();
            
            ImGuiHelper.MetricsWindow();

            ImGui.Begin("Viewport");
            
            ImGuiHelper.Image(_frameBufferHandle, 1280 / 3f, 720 / 3f);
            
            ImGui.End();
        }
    }
}