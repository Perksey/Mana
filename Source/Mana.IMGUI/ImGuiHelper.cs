using System;
using System.Globalization;
using System.Numerics;
using ImGuiNET;

namespace Mana.IMGUI
{
    public static class ImGuiHelper
    {
        internal static ImGuiRenderer _imguiRenderer; 
        
        public static bool ColorPicker(string label, ref Color color)
        {
            Vector4 cast = color.ToVector4();
            bool ret = ImGui.ColorEdit4(label, ref cast);
            color = Color.FromVector4(cast);
            return ret;
        }

        public static void BeginGlobalDocking()
        {
            ImGuiViewportPtr viewport = ImGui.GetMainViewport();

            ImGui.SetNextWindowPos(viewport.Pos);
            ImGui.SetNextWindowSize(viewport.Size);
            ImGui.SetNextWindowViewport(viewport.ID);
            ImGui.SetNextWindowBgAlpha(0.0f);

            ImGuiWindowFlags windowFlags = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;
            windowFlags |= ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse;
            windowFlags |= ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
            windowFlags |= ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;

            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
            ImGui.Begin("Docking Demo", windowFlags);
            ImGui.PopStyleVar(3);

            uint dockspaceID = ImGui.GetID("default-dockspace");
            ImGuiDockNodeFlags dockspaceFlags = ImGuiDockNodeFlags.PassthruCentralNode;
            ImGui.DockSpace(dockspaceID, Vector2.Zero, dockspaceFlags);
        }

        public static void MetricsWindow()
        {
            if (ImGui.Begin("Metrics"))
            {
                ImGui.Text($"FPS:          {Metrics.FramesPerSecond.ToString()} fps");
                ImGui.Text($"Frame Time:   {Metrics.MillisecondsPerFrame.ToString(CultureInfo.InvariantCulture)} ms");
                ImGui.Text($"Memory:       {Metrics.TotalMegabytes.ToString(CultureInfo.InvariantCulture)} MB");

                ImGui.Separator();

                ImGui.Text($"Clears:       {Metrics.ClearCount.ToString(CultureInfo.InvariantCulture)}");
                ImGui.Text($"Draw Calls:   {Metrics.DrawCalls.ToString(CultureInfo.InvariantCulture)}");
                ImGui.Text($"Triangles:    {(Metrics.PrimitiveCount / 3).ToString()}");
                
                ImGui.Separator();
                
                ImGui.Text($"IMGUI Draw Calls: {_imguiRenderer.DrawCalls.ToString()}");
                ImGui.Text($"IMGUI Triangles:  {(_imguiRenderer.PrimitiveCount / 3).ToString()}");
            }

            ImGui.End();
        }

        public static void Image(IntPtr texture, Vector2 size)
        {
            ImGui.Image(texture, size, new Vector2(0, 1), new Vector2(1, 0));
        }
        
        public static void Image(IntPtr texture, float width, float height)
        {
            ImGui.Image(texture, new Vector2(width, height), new Vector2(0, 1), new Vector2(1, 0));
        }
    }
}