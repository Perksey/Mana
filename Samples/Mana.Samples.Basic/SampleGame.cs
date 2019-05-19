using System;
using System.Globalization;
using System.Numerics;
using ImGuiNET;
using Mana.Graphics;
using Mana.Graphics.Shaders;
using Mana.IMGUI;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Samples.Basic
{
    public class SampleGame : Game
    {
        private ShaderProgram _shader;
        
        private Texture2D _texture;
        private Texture2D _bigImage;

        private SpriteBatch _spriteBatch;
        private ShaderProgram _spriteShader;

        private bool _metricsWindowOpen = true;
        private bool _check = true;

        private bool _useLargeImage = false;

        private int _count = 5000;
        private int _size = 100;

        private float _maxDeltaTime = 0f;

        protected override void Initialize()
        {
            Window.Title = "Mana | Basic Sample Game";

            _shader = AssetManager.Load<ShaderProgram>("./Assets/Shaders/shader1.json");
            _shader.Label = "Shader1";
            
            _spriteShader = AssetManager.Load<ShaderProgram>("./Assets/Shaders/sprite.json");
            _spriteShader.Label = "SpriteBatch ShaderProgram";
            
            _texture = AssetManager.Load<Texture2D>("./Assets/Textures/mittens.png");
            _texture.Label = "Mittens";
            
            Components.Add(new ImGuiRenderer());

            _spriteBatch = new SpriteBatch(GraphicsDevice)
            {
                Shader = _spriteShader
            };
            
            var proj = Matrix4x4.CreateOrthographicOffCenter(0f, Window.Width, Window.Height, 0, -1f, 1f);
            _spriteShader.SetUniform("projection", ref proj);

            ImGui.GetStyle().Alpha = 0.78f;
        }

        public override void Dispose()
        {
            _shader.Dispose();
            _spriteShader.Dispose();
            _spriteBatch.Dispose();
            _texture.Dispose();
            
            base.Dispose();
        }

        protected override void Update(float time, float deltaTime)
        {
            if (Input.WasKeyPressed(Key.Escape))
            {
                Quit();
            }

            if (deltaTime > _maxDeltaTime)
            {
                _maxDeltaTime = deltaTime;
            }
        }

        protected override void Render(float time, float deltaTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateGray);

            if (_check)
            {
                _spriteBatch.Begin();
                
                for (int i = 0; i < _count; i++)
                {
                    float x = (float)((Math.Sin((time + (i / 1000.012f)) * 4.5f) * 100) + 250);
                    float y = (float)((Math.Cos((time + (i / 1000.012f)) * 4.5f) * 100) + 250);

                    _spriteBatch.Draw(_useLargeImage ? _bigImage : _texture, new Rectangle((int)x, (int)y, _size, _size));
                }
                
                _spriteBatch.End();
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
                ImGui.Text($"FPS:          {Metrics.FramesPerSecond.ToString()} fps");
                ImGui.Text($"Frame Time:   {Metrics.MillisecondsPerFrame.ToString(CultureInfo.InvariantCulture)} ms");
                ImGui.Text($"Memory:       {Metrics.TotalMegabytes.ToString(CultureInfo.InvariantCulture)} MB");
                
                ImGui.Separator();
                
                ImGui.Text($"Clears:       {Metrics.ClearCount.ToString(CultureInfo.InvariantCulture)}");
                ImGui.Text($"Draw Calls:   {Metrics.DrawCalls.ToString(CultureInfo.InvariantCulture)}");
                ImGui.Text($"Triangles:    {(Metrics.PrimitiveCount / 3).ToString()}");
                
                ImGui.Separator();
                
                ImGui.Text($"Max deltaTime:    {_maxDeltaTime.ToString(CultureInfo.InvariantCulture)}");
                
                ImGui.Checkbox("Draw with SpriteBatch", ref _check);

                ImGui.DragInt("Sprites", ref _count);
                ImGui.DragInt("Size", ref _size);
                
                ImGui.Separator();

                ImGui.Checkbox("Use large image", ref _useLargeImage);
                
                if (ImGui.Button("Load large image"))
                {
                    AssetManager.CreateAsyncBatch()
                                .Load(() => _bigImage, "./Assets/Textures/big-image.jpg")
                                .Load(() => _shader, "./Assets/Shaders/shader1.json")
                                .Load(() => _texture, "./Assets/Textures/mittens.png")
                                .Run();
                    
                    // AssetManager.CreateAsyncBatch()
                    //             .Load<Texture2D>(r => _bigImage = r, "./Assets/Textures/big-image.jpg")
                    //             .Load<ShaderProgram>(r => _shader = r, "./Assets/Shaders/shader1.json")
                    //             .Load<Texture2D>(r => _texture = r, "./Assets/Textures/mittens.png")
                    //             .Run();
                }

                ImGui.Separator();
                
                if (ImGui.Button("Perform invalid GL call"))
                {
                    GL.Enable((EnableCap)(-20));
                }

                ImGui.End();
            }
        }
    }
}