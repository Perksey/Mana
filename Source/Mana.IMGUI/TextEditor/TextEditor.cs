using System;
using System.Numerics;
using ImGuiNET;

namespace Mana.IMGUI.TextEditor
{
    public class TextEditor
    {
        internal TextEditorBuffer Buffer;
        
        private bool _initialized = false;
        
        public void ShowWindow()
        {
            if (!_initialized)
                Initialize();

            if (ImGui.Begin("Text Editor"))
            {
                Buffer.Begin();

                var coord = Buffer.GetMouseCoordinate();
                Buffer.DrawBackground(coord.X, coord.Y, Color.Blue);
            
                Buffer.DrawBuffer();
            
                Buffer.End();                
            }
            
            ImGui.End();
        }

        private void Initialize()
        {
            Buffer = new TextEditorBuffer();
            Buffer.SetText(@"using Mana.Utilities;
using Mana.Utilities.Extensions;
using OpenTK.Input;

namespace Mana.IMGUI
{
    public class ImGuiSystem : GameSystem, IDisposable
    {
        private static Logger _log = new Logger();

        public readonly bool UseViewports;

        internal ImGuiIOPtr IO;
        internal int[] Keys;

        internal readonly Dictionary<IntPtr, Texture2D> BoundTextures = new Dictionary<IntPtr, Texture2D>();
        internal int TextureID;

        private Game _game;
        private ImGuiRenderer _renderer;
        private ImGuiViewportManager _viewportManager;

        public ImGuiSystem(bool useViewports = false)
        {
            ImGuiHelper.System = this;
            UseViewports = useViewports;
        }
    }
}");
            
            _initialized = true;
        } 
    }
}