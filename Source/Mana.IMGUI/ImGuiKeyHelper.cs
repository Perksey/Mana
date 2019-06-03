using System;
using ImGuiNET;
using OpenTK.Input;

namespace Mana.IMGUI
{
    public class ImGuiKeyHelper
    {
        public static Key ToOpenTKKey(ImGuiKey key)
        {
            switch (key)
            {
                case ImGuiKey.Tab:
                    return Key.Tab;
                case ImGuiKey.LeftArrow:
                    return Key.Left;
                case ImGuiKey.RightArrow:
                    return Key.Right;
                case ImGuiKey.UpArrow:
                    return Key.Up;
                case ImGuiKey.DownArrow:
                    return Key.Down;
                case ImGuiKey.PageUp:
                    return Key.PageUp;
                case ImGuiKey.PageDown:
                    return Key.PageDown;
                case ImGuiKey.Home:
                    return Key.Home;
                case ImGuiKey.End:
                    return Key.End;
                case ImGuiKey.Insert:
                    return Key.Insert;
                case ImGuiKey.Delete:
                    return Key.Delete;
                case ImGuiKey.Backspace:
                    return Key.BackSpace;
                case ImGuiKey.Space:
                    return Key.Space;
                case ImGuiKey.Enter:
                    return Key.Enter;
                case ImGuiKey.Escape:
                    return Key.Escape;
                case ImGuiKey.A:
                    return Key.A;
                case ImGuiKey.C:
                    return Key.C;
                case ImGuiKey.V:
                    return Key.V;
                case ImGuiKey.X:
                    return Key.X;
                case ImGuiKey.Y:
                    return Key.Y;
                case ImGuiKey.Z:
                    return Key.Z;
                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, null);
            }
        }
    }
}