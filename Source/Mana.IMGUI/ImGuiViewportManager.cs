using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ImGuiNET;
using Mana.Logging;
using OpenTK;

namespace Mana.IMGUI
{
    public class ImGuiViewportManager
    {
        private static Logger _log = Logger.Create();
        
        public delegate void CreateWindowFunc(ImGuiViewportPtr ptr);
        
        private ImGuiRenderer _imguiRenderer;
        private Game _game;
        
        private Dictionary<IntPtr, GameWindow> _boundWindows = new Dictionary<IntPtr, GameWindow>();
        private int _windowID;

        private CreateWindowFunc _createWindowDelegate;
        
        public ImGuiViewportManager(ImGuiRenderer imguiRenderer)
        {
            _imguiRenderer = imguiRenderer;
            _createWindowDelegate = CreateWindow;
        }

        public void Initialize(Game game)
        {
            _game = game;
            
            var viewport = ImGui.GetMainViewport();
            viewport.PlatformHandle = BindWindow(game.GraphicsDevice.Window.GameWindow);

            var platformIO = ImGui.GetPlatformIO();

            platformIO.Platform_CreateWindow = Marshal.GetFunctionPointerForDelegate(_createWindowDelegate);
        }
        
        public IntPtr BindWindow(GameWindow window)
        {
            IntPtr id = new IntPtr(_windowID++);
            _boundWindows.Add(id, window);
            return id;
        }

        public void UnbindWindow(IntPtr id)
        {
            _boundWindows.Remove(id);
        }

        public void CreateWindow(ImGuiViewportPtr viewport)
        {
            _log.Debug("Create Window Viewport!");
        }
    }
}