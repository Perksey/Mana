using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using ImGuiNET;
using Mana.Audio;
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
        private Sound _ritual;
        private Sound _fanfare;
        private Sound _loop;
        private Sound _sound;
        private SoundInstance _instance;

        protected override void Initialize()
        {
            AddSystem(new ImGuiSystem());
            
            _textEdit = new TextEditor();

            _ritual = AssetManager.Load<Sound>("./Assets/Sounds/ritual.wav");
            _fanfare = AssetManager.Load<Sound>("./Assets/Sounds/fanfare.wav");
            _loop = AssetManager.Load<Sound>("./Assets/Sounds/loop.wav");
            _sound = AssetManager.Load<Sound>("./Assets/Sounds/sound.wav");
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

                if (ImGui.Begin("Tests"))
                {
                    if (_instance == null)
                    {
                        if (ImGui.Button("Play Ritual"))
                        {
                            _instance = _ritual.Play();
                        }
                        
                        if (ImGui.Button("Play Fanfare"))
                        {
                            _instance = _fanfare.Play();
                        }
                        
                        if (ImGui.Button("Play Loop"))
                        {
                            _instance = _loop.Play();
                            _instance.Looping = true;
                        }
                        
                        if (ImGui.Button("Play Sound"))
                        {
                            _instance = _sound.Play();
                        }
                    }
                    else
                    {
                        if (ImGui.Button("Play"))
                        {
                            _instance.Play();
                        }
                        
                        if (ImGui.Button("Pause"))
                        {
                            _instance.Pause();
                        }
                        
                        if (ImGui.Button("Stop"))
                        {
                            _instance.Stop();
                            _instance = null;
                        }

                        if (_instance != null)
                        {
                            var vol = _instance.Volume;
                            ImGui.DragFloat("Volume", ref vol, 0.02f, 0.0f, 1.0f);
                            _instance.Volume = vol;

                            var pitch = _instance.Pitch;
                            ImGui.DragFloat("Pitch", ref pitch, 0.02f, 0.0f, 4.0f);
                            _instance.Pitch = pitch;
                        }
                    }
                }
                ImGui.End();

                _textEdit.ShowWindow();
            }
            ImGui.End();
        }
    }
}
