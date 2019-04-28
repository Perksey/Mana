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
        private VertexPositionTexture[] _vertices = 
        {
            new VertexPositionTexture(new Vector3(-0.3f, -0.5f, 0f), new Vector2(0.0f, 0.0f)),    // BL
            new VertexPositionTexture(new Vector3( 0.3f, -0.5f, 0f), new Vector2(1.0f, 0.0f)),    // BR
            new VertexPositionTexture(new Vector3( 0.3f,  0.5f, 0f), new Vector2(1.0f, 1.0f)),    // TR
            new VertexPositionTexture(new Vector3(-0.3f,  0.5f, 0f), new Vector2(0.0f, 1.0f)),    // TL
        };
        
        private ushort[] _indices = 
        {
            0, 1, 2, 0, 2, 3, 
        };

        private ShaderProgram _shader;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        
        private Texture2D _texture;

        private bool _metricsWindowOpen = true;
        
        protected override void Initialize()
        {
            Window.Title = "Hello";

            _shader = AssetManager.Load<ShaderProgram>("./Assets/Shaders/shader1.json");
            _texture = AssetManager.Load<Texture2D>("./Assets/Textures/cat.jpeg");
            
            Components.Add(new ImGuiRenderer());

            _vertexBuffer = VertexBuffer.Create(GraphicsDevice, _vertices, BufferUsage.StaticDraw, dynamic: true);
            _indexBuffer = IndexBuffer.Create(GraphicsDevice, _indices, BufferUsage.StaticDraw, dynamic: true);
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
            GraphicsDevice.Clear(Color.Gray);
            GraphicsDevice.BindTexture(0, _texture);
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
                    if (ImGui.MenuItem("Metrics", null, _metricsWindowOpen))
                    {
                        _metricsWindowOpen = !_metricsWindowOpen;
                    }
                    
                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }

            if (_metricsWindowOpen && ImGui.Begin("Metrics", ref _metricsWindowOpen))
            {
                ImGui.Text($"FPS:          {Metrics.FramesPerSecond} fps");
                ImGui.Text($"Frame Time:   {Metrics.MillisecondsPerFrame:F3} ms");
                ImGui.Text($"Memory:       {Metrics.TotalMegabytes} MB");
                ImGui.Separator();
                ImGui.Text($"Clears:       {Metrics.ClearCount}");
                ImGui.Text($"Draw Calls:   {Metrics.DrawCalls}");
                ImGui.Text($"Triangles:    {Metrics.PrimitiveCount / 3}");
                
                ImGui.End();
            }
        }
    }
}