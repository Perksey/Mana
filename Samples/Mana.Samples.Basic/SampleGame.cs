using System;
using System.Numerics;
using ImGuiNET;
using Mana.Graphics;
using Mana.Graphics.Buffers;
using Mana.Graphics.Shaders;
using Mana.Graphics.Vertex.Types;
using Mana.IMGUI;

namespace Mana.Samples.Basic
{
    public class SampleGame : Game
    {
        private VertexPositionColor[] _a = new VertexPositionColor[]
        {
            new VertexPositionColor(new Vector3(-0.7f, 0.5f, 0f), Color.Red),
            new VertexPositionColor(new Vector3(-0.7f, -0.5f, 0f), Color.Green),
            new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0f), Color.Blue),
        };
        
        private VertexPositionColor[] _b = new VertexPositionColor[]
        {
            new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0f), Color.Orange),
            new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0f), Color.Yellow),
            new VertexPositionColor(new Vector3(-0.3f, -0.5f, 0f), Color.Magenta),
        };
        
        private VertexPositionColor[] _c = 
        {
            new VertexPositionColor(new Vector3(-0.3f, 0.5f, 0f), Color.Purple),
            new VertexPositionColor(new Vector3(-0.3f, -0.5f, 0f), Color.Gray),
            new VertexPositionColor(new Vector3(-0.1f, -0.5f, 0f), Color.White),
        };
        
        private VertexPositionColor[] _data = 
        {
            new VertexPositionColor(new Vector3(-0.1f, 0.5f, 0f), Color.Magenta),
            new VertexPositionColor(new Vector3(-0.1f, -0.5f, 0f), Color.Magenta),
            new VertexPositionColor(new Vector3(0.1f, -0.5f, 0f), Color.Magenta),
            new VertexPositionColor(new Vector3(0.1f, 0.5f, 0f), Color.Magenta),
            new VertexPositionColor(new Vector3(0.1f, -0.5f, 0f), Color.Magenta),
            new VertexPositionColor(new Vector3(0.3f, -0.5f, 0f), Color.Magenta),
            new VertexPositionColor(new Vector3(0.3f, 0.5f, 0f), Color.Magenta),
            new VertexPositionColor(new Vector3(0.3f, -0.5f, 0f), Color.Magenta),
            new VertexPositionColor(new Vector3(0.5f, -0.5f, 0f), Color.Magenta),
        };

        private ushort[] _indices = 
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8,
        };

        private ShaderProgram _shader;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        
        private Texture2D _texture;
        
        protected override void Initialize()
        {
            Window.Title = "Hello";

            _shader = AssetManager.Load<ShaderProgram>("./Assets/Shaders/shader1.json");

            _texture = AssetManager.Load<Texture2D>("./Assets/Textures/cat.jpeg");
            
            Components.Add(new ImGuiRenderer());

            _vertexBuffer = VertexBuffer.Create(GraphicsDevice, _data, BufferUsage.StaticDraw, dynamic: true);
            
            _indexBuffer = IndexBuffer.Create(GraphicsDevice, _indices, BufferUsage.StaticDraw, dynamic: true);
        }

        protected override unsafe void Update(float time, float deltaTime)
        {
            if (Input.WasKeyPressed(Key.Escape))
            {
                Quit();
            }

            if (Input.WasKeyPressed(Key.Number1))
            {
                _vertexBuffer.SubData(_a, IntPtr.Zero);
            }
            
            if (Input.WasKeyPressed(Key.Number2))
            {
                _vertexBuffer.SubData(_b, new IntPtr(sizeof(VertexPositionColor) * 3));
            }
            
            if (Input.WasKeyPressed(Key.Number3))
            {
                _vertexBuffer.SubData(_c, new IntPtr(sizeof(VertexPositionColor) * 6));
            }
        }

        protected override void Render(float time, float deltaTime)
        {
            GraphicsDevice.Clear(Color.Gray);
            //GraphicsDevice.Render(_vertexBuffer, _shader);
            GraphicsDevice.Render(_vertexBuffer, _indexBuffer, _shader);

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
                    // void WindowToggle(string label, ref bool toggle)
                    // {
                    //     if (ImGui.MenuItem(label, null, toggle))
                    //     {
                    //         toggle = !toggle;
                    //     }
                    // }
                    //
                    // WindowToggle("Scene", ref _sceneWindowOpen);
                    // WindowToggle("Inspector", ref _inspectorWindowOpen);
                    //
                    // ImGui.Separator();
                    //
                    // WindowToggle("Metrics", ref _metricsWindowOpen);
                    // WindowToggle("Graphics Resources", ref _graphicsResourcesWindowOpen);

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