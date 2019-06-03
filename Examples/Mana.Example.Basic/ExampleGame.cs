using ImGuiNET;
using Mana.IMGUI;

namespace Mana.Example.Basic
{
    class ExampleGame : Game
    {
        protected override void Initialize()
        {
            AddSystem(new ImGuiSystem());
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
            }
            ImGui.End();
        }
    }
}