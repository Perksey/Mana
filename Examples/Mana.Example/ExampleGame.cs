using System;
using System.Drawing;
using System.Numerics;
using ImGuiNET;
using Mana.Graphics;
using Mana.Graphics.Shaders;
using Mana.Graphics.Textures;
using Mana.IMGUI;
using Mana.Utilities;
using Color = Mana.Graphics.Color;

namespace Mana.Example
{
    public class ExampleGame : Game
    {
        private ShaderProgram _spriteShader;
        private ShaderProgram _testShader;

        private Texture2D _mittens;
        private SpriteBatch _spriteBatch;

        private Color _backgroundColor = Color.CornflowerBlue;

        public override void Initialize()
        {
#if DEBUG
            AssetManager.RootPath = "../../../Assets";
#else
            AssetManager.RootPath = "./Assets";
#endif

            AddGameSystem(new ImGuiSystem());

            _spriteShader = BasicShaderFactory.CreateSpriteShaderProgram(RenderContext);

            _testShader = AssetManager.Load<ShaderProgram>("./Shaders/shader.json", true);
            _testShader.Label = "test shader label";
            _mittens = AssetManager.Load<Texture2D>("./Textures/mittens.png", true);

            _spriteBatch = new SpriteBatch(RenderContext)
            {
                Shader = _testShader
            };
        }

        public override void Update(float time, float deltaTime)
        {
        }

        public override void Render(float time, float deltaTime)
        {
            _testShader.SetUniform("projection", ref Window.ProjectionMatrix);
            _spriteShader.SetUniform("projection", ref Window.ProjectionMatrix); //

            RenderContext.Clear(_backgroundColor);

            _spriteBatch.Begin();

            Vector2 a = new Vector2(20, 20);
            Vector2 b = new Vector2(300, 300);

            float x = MathHelper.Lerp(a.X, b.X, (MathF.Sin(time * 4f) + 1f) / 2f);
            float y = MathHelper.Lerp(a.Y, b.Y, (MathF.Cos(time * 4f) + 1f) / 2f);

            _spriteBatch.Draw(_mittens, new Rectangle((int)x, (int)y, 400, 400));

            _spriteBatch.End();
        }
    }
}
