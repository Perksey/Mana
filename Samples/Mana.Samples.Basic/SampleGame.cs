using System.Globalization;
using System.Numerics;
using ImGuiNET;
using Mana.Asset.Async;
using Mana.Graphics;
using Mana.Graphics.Shaders;
using Mana.IMGUI;
using Mana.Logging;

namespace Mana.Samples.Basic
{
    public class SampleGame : Game
    {
        private static Logger _log = Logger.Create();
        
        private ShaderProgram _shader;
        
        private Texture2D _texture;
        private Texture2D _bigImage;
        private Texture2D _bigImage2;
        private Texture2D _bigImage3;
        private Texture2D _bigImage4;
        private Texture2D _bigImage5;

        private SpriteBatch _spriteBatch;
        private ShaderProgram _spriteShader;

        private bool _metricsWindowOpen = true;
        private bool _check = true;

        private bool _useLargeImage = false;

        private int _count = 5000;
        private int _size = 100;

        private float _maxDeltaTime;

        private AssetLoadingTask _task;

        protected override void Initialize()
        {
            Window.Title = "Mana - Sample Game";

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

            var proj = Matrix4x4.CreateOrthographicOffCenter(0f, Window.Width, Window.Height, 0, -1f, 1f);
            _spriteShader.SetUniform("projection", ref proj);
            
            if (_check && _texture != null)
            {
                _spriteBatch.Begin();
                
                int x = (int)Input.MousePosition.X;
                int y = (int)Input.MousePosition.Y;
                _spriteBatch.Draw(_useLargeImage ? _bigImage : _texture, new Rectangle(x, y, _size, _size));
                
                _spriteBatch.End();
            }

            ImGuiHelper.BeginGlobalDocking();
            
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

            if (ImGui.Begin("Metrics2"))
            {
                ImGui.Text($"FPS:          {Metrics.FramesPerSecond.ToString()} fps");
                ImGui.Text($"Frame Time:   {Metrics.MillisecondsPerFrame.ToString(CultureInfo.InvariantCulture)} ms");
                ImGui.Text($"Memory:       {Metrics.TotalMegabytes.ToString(CultureInfo.InvariantCulture)} MB");
            }
            
            ImGui.End();

            if (_metricsWindowOpen)
            {
                if (ImGui.Begin("Metrics", ref _metricsWindowOpen))
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
                    
                    if (ImGui.Button("Load single bigimage"))
                    {
                        if (_bigImage != null)
                        {
                            AssetManager.Unload(_bigImage);
                        }

                        _task = AssetManager.CreateAsyncBatch()
                                            .Load(() => _bigImage, "./Assets/Textures/big-image.jpg")
                                            .OnCompleted(() => { _log.Debug("Batch loading job completed!"); })
                                            .Start();
                    }
                    
                    if (ImGui.Button("Load five bigimages"))
                    {
                        if (_bigImage != null)
                            AssetManager.Unload(_bigImage);
                        
                        _task = AssetManager.CreateAsyncBatch()
                                            .Load(() => _bigImage, "./Assets/Textures/big-image.jpg")
                                            .Load(() => _bigImage2, "./Assets/Textures/big-image-2.jpg")
                                            .Load(() => _bigImage3, "./Assets/Textures/big-image-3.jpg")
                                            .Load(() => _bigImage4, "./Assets/Textures/big-image-4.jpg")
                                            .Load(() => _bigImage5, "./Assets/Textures/big-image-5.jpg")
                                            .OnCompleted(() => { _log.Debug("Batch loading job completed!"); })
                                            .Start();
                    }

                    if (_task != null)
                    {
                        ImGui.Text($"Items Loaded: {_task.ProgressCount}/{_task.Count} ({(_task.Progress):P})");
                    }
                    ImGui.Separator();
                }
            
                ImGui.End();
            }

            ImGui.End();
        }
    }
}
