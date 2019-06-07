using System;
using System.Drawing;
using System.Numerics;
using ImGuiNET;

namespace Mana.IMGUI.TextEditor
{
    public class TextEditor
    {
        internal TextEditorBuffer Buffer;
        
        private bool _initialized = false;

        private Point _cursorLocation = new Point(1, 28);
        private float _time;
        private float _clickTime;
        private string _str = "";
        
        public void ShowWindow()
        {
            _time += ImGui.GetIO().DeltaTime;
            
            if (!_initialized)
                Initialize();

            if (ImGui.Begin("Text Editor"))
            {
                ImGui.InputText("Temp", ref _str, 128);
                
                Buffer.Begin();

                Buffer.Render();
            
                if (ImGui.GetIO().MouseClicked[0] && Buffer.IsFocused)
                {
                    _cursorLocation = Buffer.GetMouseTextCursorCoordinate();
                    _clickTime = _time;
                }
                
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