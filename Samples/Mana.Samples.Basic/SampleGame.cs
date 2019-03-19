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
        private VertexPositionColor[] _vertexData = new VertexPositionColor[]
        {
            new VertexPositionColor(new Vector3(0f, 0.5f, 0f), Color.Black),
            new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0f), Color.Black),
            new VertexPositionColor(new Vector3(0.5f, -0.5f, 0f), Color.Black),
        };

        private ShaderProgram _shader;
        
        protected override void Initialize()
        {
            this.Window.Title = "Hello";

            _shader = AssetManager.Load<ShaderProgram>("./Assets/Shaders/shader1.json");
        }

        protected override void Update(float time, float deltaTime)
        {
        }

        protected override void Render(float time, float deltaTime)
        {
            GraphicsDevice.Clear(Color.White);
            GraphicsDevice.Render(_vertexData, _shader);
        }
    }
}