using System.Numerics;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using ImGuiNET;
using Mana.Graphics;
using Mana.Graphics.Shaders;
using Mana.Graphics.Vertex.Types;
using Mana.IMGUI;
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
            
            Components.Add(new ImGuiRenderer());
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
            
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Open"))
                    {
                    }

                    if (ImGui.MenuItem("Save"))
                    {
                    }

                    if (ImGui.MenuItem("Quit"))
                        Quit();

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("View"))
                {
                    void WindowToggle(string label, ref bool toggle)
                    {
                        if (ImGui.MenuItem(label, null, toggle))
                        {
                            toggle = !toggle;
                        }
                    }

                    //WindowToggle("Scene", ref _sceneWindowOpen);
                    //WindowToggle("Inspector", ref _inspectorWindowOpen);

                    ImGui.Separator();

                    //WindowToggle("Metrics", ref _metricsWindowOpen);
                    //WindowToggle("Graphics Resources", ref _graphicsResourcesWindowOpen);

                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }

            if (ImGui.Begin("Hello"))
            {
                for (int i = 0; i < 100; i++)
                {
                    ImGui.Text("Hello.");                    
                }
                
                ImGui.End();
            }
        }
    }
}