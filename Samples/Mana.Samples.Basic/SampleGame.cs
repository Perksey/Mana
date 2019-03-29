using System.Numerics;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Mana.Graphics;
using Mana.Graphics.Shaders;
using Mana.Graphics.Vertex.Types;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Samples.Basic
{
    public class SampleGame : Game
    {
        private VertexPositionTexture[] _vertexData = new VertexPositionTexture[]
        {
            new VertexPositionTexture(new Vector3(0f, 0.5f, 0f), new Vector2(0, 0)),
            new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0f), new Vector2(1, 0)),
            new VertexPositionTexture(new Vector3(0.5f, -0.5f, 0f), new Vector2(1, 1)),
        };

        private ShaderProgram _shader;
        
        protected override void Initialize()
        {
            this.Window.Title = "Hello";

            _shader = AssetManager.Load<ShaderProgram>("./Assets/Shaders/shader1.json");
        }

        protected override void Update(float time, float deltaTime)
        {
            if (Input.WasKeyPressed(Key.Escape))
            {
                Quit();
            }
        }

        protected override void Render(float time, float deltaTime)
        {
            GraphicsDevice.Clear(Color.White);
            GraphicsDevice.Render(_vertexData, _shader);
        }
    }
}