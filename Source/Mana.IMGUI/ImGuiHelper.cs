using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using ImGuiNET;
using Mana.Graphics.Buffers;
using Mana.Graphics.Textures;

namespace Mana.IMGUI
{
    public class ImGuiHelper
    {
        internal static ImGuiSystem System;
        
        public static void BeginGlobalDocking(bool menuBar = true)
        {
            ImGuiViewportPtr viewport = ImGui.GetMainViewport();

            ImGui.SetNextWindowPos(viewport.Pos);
            ImGui.SetNextWindowSize(viewport.Size);
            ImGui.SetNextWindowViewport(viewport.ID);
            ImGui.SetNextWindowBgAlpha(0.0f);

            ImGuiWindowFlags windowFlags = ImGuiWindowFlags.NoDocking;

            if (menuBar)
            {
                windowFlags |= ImGuiWindowFlags.MenuBar;
            }
            
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

        public static unsafe void DragRectangle(string label, ref Rectangle rectangle)
        {
            var ints = stackalloc int[4];
            ints[0] = rectangle.X;
            ints[1] = rectangle.Y;
            ints[2] = rectangle.Width;
            ints[3] = rectangle.Height;

            ImGui.DragInt4(label, ref ints[0]);

            rectangle.X = ints[0];
            rectangle.Y = ints[1];
            rectangle.Width = ints[2];
            rectangle.Height = ints[3];
        }
        
        public static void Image(IntPtr texture, Vector2 size)
        {
            ImGui.Image(texture, size, new Vector2(0, 1), new Vector2(1, 0));
        }
        
        public static void Image(IntPtr texture, float width, float height)
        {
            ImGui.Image(texture, new Vector2(width, height), new Vector2(0, 1), new Vector2(1, 0));
        }

        public static void Button(string text, int width, int height, Color color)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, color.ToUint());

            ImGui.Button(text, new Vector2(width, height));

            ImGui.PopStyleColor();
        }

        public static IntPtr BindTexture(Texture2D texture)
        {
            return System.BindTexture(texture);
        }

        public static void UnbindTexture(IntPtr handle)
        {
            System.UnbindTexture(handle);
        }

        public static Vector2 GetCursorScreenPos()
        {
            igGetCursorScreenPos(out var output);
            return output;
        }
        
        public static Vector2 GetContentRegionAvail()
        {
            igGetContentRegionAvail(out var output);
            return output;
        }
        
        public static Vector2 GetWindowPos()
        {
            igGetWindowPos(out var output);
            return output;
        }
        
        public static Vector2 GetWindowSize()
        {
            igGetWindowSize(out var output);
            return output;
        }

        public static Vector2 GetMousePos()
        {
            igGetMousePos(out var output);
            return output;
        }
        
        [DllImport("cimgui", EntryPoint = "igGetMousePos_nonUDT", CallingConvention = CallingConvention.Cdecl)]
        static extern void igGetMousePos(out Vector2 output);
        
        [DllImport("cimgui", EntryPoint = "igGetCursorScreenPos_nonUDT", CallingConvention = CallingConvention.Cdecl)]
        static extern void igGetCursorScreenPos(out Vector2 output);
        
        [DllImport("cimgui", EntryPoint = "igGetContentRegionAvail_nonUDT", CallingConvention = CallingConvention.Cdecl)]
        static extern void igGetContentRegionAvail(out Vector2 output);
        
        [DllImport("cimgui", EntryPoint = "igGetWindowPos_nonUDT", CallingConvention = CallingConvention.Cdecl)]
        static extern void igGetWindowPos(out Vector2 output);

        [DllImport("cimgui", EntryPoint = "igGetWindowSize_nonUDT", CallingConvention = CallingConvention.Cdecl)]
        static extern void igGetWindowSize(out Vector2 output);
    }
}