using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using ImGuiNET;
using Mana.Graphics;
using Mana.Graphics.Shader;
using Mana.Graphics.Textures;
using Mana.IMGUI;
using Mana.IMGUI.TextEditor;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace Mana.Example.Basic
{
    class ExampleGame : Game
    {
        private TextEditor _textEdit;

        protected override void Initialize()
        {
            AddSystem(new ImGuiSystem());
            
            _textEdit = new TextEditor();
        }
        
        protected override void Update(float time, float deltaTime)
        {
        }

        protected override void Render(float time, float deltaTime)
        {
            RenderContext.Clear(Color.DarkCyan);

            ImGuiHelper.BeginGlobalDocking();
            {
                if (ImGui.BeginMainMenuBar())
                {
                    if (ImGui.BeginMenu("File"))
                    {
                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Edit"))
                    {
                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Window"))
                    {
                        ImGui.EndMenu();
                    }

                    ImGui.EndMainMenuBar();
                }

                ImGui.ShowMetricsWindow();

                _textEdit.ShowWindow();
            }
            ImGui.End();
        }
    }
}