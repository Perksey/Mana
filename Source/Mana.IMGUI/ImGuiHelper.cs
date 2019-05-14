using System.Numerics;
using ImGuiNET;

namespace Mana.IMGUI
{
    public static class ImGuiHelper
    {
        public static bool ColorPicker(string label, ref Color color)
        {
            Vector4 cast = color.ToVector4();
            bool ret = ImGui.ColorEdit4(label, ref cast);
            color = Color.FromVector4(cast);
            return ret;
        }
    }
}