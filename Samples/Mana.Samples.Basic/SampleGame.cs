using System;
using System.Diagnostics;
using System.Numerics;
using ImGuiNET;
using Mana.Graphics;
using Mana.Graphics.Batch;
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

        private SpriteBatch _spriteBatch;
        private ShaderProgram _spriteShader;

        private bool _metricsWindowOpen = true;
        private bool _check = true;

        private int _count = 1;

        private float _x = 0;
        private float _y = 0;
        
        protected override void Initialize()
        {
            Window.Title = "Hello";

            _shader = AssetManager.Load<ShaderProgram>("./Assets/Shaders/shader1.json");
            _spriteShader = AssetManager.Load<ShaderProgram>("./Assets/Shaders/sprite.json");
            _texture = AssetManager.Load<Texture2D>("./Assets/Textures/cat.jpeg");
            
            Components.Add(new ImGuiRenderer());

            _vertexBuffer = VertexBuffer.Create(GraphicsDevice, _vertices, BufferUsage.StaticDraw, dynamic: true);
            _indexBuffer = IndexBuffer.Create(GraphicsDevice, _indices, BufferUsage.StaticDraw, dynamic: true);

            _spriteBatch = new SpriteBatch(GraphicsDevice)
            {
                Shader = _spriteShader
            };
            
            var proj = Matrix4x4.CreateOrthographicOffCenter(0f, Window.Width, Window.Height, 0, -1f, 1f);
            _spriteShader.SetUniform("projection", ref proj);

            ImGui.GetStyle().Alpha = 0.78f;
        }

        protected override void Update(float time, float deltaTime)
        {
            if (Input.WasKeyPressed(Key.Escape))
            {
                Quit();
            }

            _y = (float)((Math.Cos(time * 4.5f) * 100) + 250);
            _x = (float)((Math.Sin(time * 4.5f) * 100) + 250);
        }

        private Stopwatch _stopwatch = new Stopwatch();

        private TimeSpan _a;
        private TimeSpan _b;

        protected override void Render(float time, float deltaTime)
        {
            GraphicsDevice.Clear(Color.Gray);
            //GraphicsDevice.BindTexture(0, _texture);
            //GraphicsDevice.Render(_vertexBuffer, _indexBuffer, _shader);


            if (_check)
            {
                _spriteBatch.Begin();

                _stopwatch.Restart();
                
                for (int i = 0; i < _count; i++)
                {
                    var x = (float)((Math.Sin((time + (i / 5000.012f)) * 4.5f) * 100) + 250);
                    var y = (float)((Math.Cos((time + (i / 5000.012f)) * 4.5f) * 100) + 250);
                    
                    _spriteBatch.Draw(_texture, new Rectangle((int)x, (int)y, 25, 25));
                }

                _stopwatch.Stop();

                _a = _stopwatch.Elapsed;
                
                _stopwatch.Restart();
                
                _spriteBatch.End();

                _stopwatch.Stop();
                _b = _stopwatch.Elapsed;
            }
            
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
                
                ImGui.Separator();
                
                ImGui.Checkbox("Draw with SpriteBatch", ref _check);
                
                ImGui.DragInt("Number of Boxes", ref _count);
                
                ImGui.Separator();

                ImGui.Text($"CPU Time: {_a.TotalMilliseconds:F3}");
                ImGui.Text($"GPU Time: {_b.TotalMilliseconds:F3}");

                ImGui.Separator();
                
                // ImGui.Text($"Prep: {SpriteBatch.A.Ticks:F3}");
                // ImGui.Text($"vtx send: {SpriteBatch.B.Ticks:F3}");
                // ImGui.Text($"idx send: {SpriteBatch.C.Ticks:F3}");
                //
                // ImGui.Separator();
                //
                // long sum = SpriteBatch.ASum + SpriteBatch.BSum + SpriteBatch.CSum;
                //
                // float a = (SpriteBatch.ASum / (float)sum) * 100.0f;
                // float b = (SpriteBatch.BSum / (float)sum) * 100.0f;
                // float c = (SpriteBatch.CSum / (float)sum) * 100.0f;
                //
                // ImGui.Text($"PrepSum:     {SpriteBatch.ASum:F3}");
                // ImGui.Text($"vtx sendSum: {SpriteBatch.BSum:F3}");
                // ImGui.Text($"idx sendSum: {SpriteBatch.CSum:F3}");
                //
                // ImGui.Separator();
                //
                // ImGui.Text($"prp%: {a:F3}");
                // ImGui.Text($"vtx%: {b:F3}");
                // ImGui.Text($"idx%: {c:F3}");

                ImGui.End();
            }
        }
    }
}