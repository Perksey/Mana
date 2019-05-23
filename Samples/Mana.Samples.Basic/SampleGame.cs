using System;
using System.Collections.Generic;
using ImGuiNET;
using Mana.IMGUI;
using Mana.Logging;
using Mana.Samples.Basic.Examples;

namespace Mana.Samples.Basic
{
    public class SampleGame : Game
    {
        private static Logger _log = Logger.Create();

        private Dictionary<string, Func<Example>> _exampleFactories = new Dictionary<string, Func<Example>>();

        private Example _currentExample;
        
        protected override void Initialize()
        {
            Window.Title = "Mana - Examples";
            Components.Add(new ImGuiRenderer());
            
            _exampleFactories.Add("Basic Example", () => new BasicExample(this));

            //SwitchToExample(new BasicExample(this));
            SwitchToExample(new ModelExample(this));
        }

        protected override void Update(float time, float deltaTime)
        {
            if (Input.WasKeyPressed(Key.Escape))
            {
                if (_currentExample != null)
                {
                    SwitchToExample(null);
                    Window.Title = "Mana - Examples";
                }
                else
                {
                    Quit();
                }
            }

            _currentExample?.Update(time, deltaTime);
        }

        protected override void Render(float time, float deltaTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateGray);

            ImGuiHelper.BeginGlobalDocking();
            
            if (_currentExample != null)
            {
                _currentExample.Render(time, deltaTime);
            }
            else
            {
                if (ImGui.Begin("Example Selector"))
                {
                    foreach (var kvp in _exampleFactories)
                    {
                        if (ImGui.Button(kvp.Key))
                        {
                            SwitchToExample(kvp.Value.Invoke());
                            Window.Title = $"Mana - {kvp.Key} - Press escape to return to menu.";
                        }
                    }
                }
                
                ImGui.End();
            }

            ImGui.End();
        }

        private void SwitchToExample(Example example)
        {
            if (_currentExample != null)
            {
                _currentExample.Dispose();
                _currentExample = null;
            }

            example?.Initialize();
            _currentExample = example;
        }
    }
}
