using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using Mana.Graphics;
using Mana.Graphics.Textures;
using Mana.IMGUI.Viewport;
using Mana.Input;
using Mana.Utilities;
using Mana.Utilities.Extensions;
using OpenTK.Input;

namespace Mana.IMGUI
{
    public class ImGuiSystem : GameSystem, IDisposable
    {
        private static Logger _log = new Logger("IMGUI");
        
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
        
        public override void OnAddedToGame(Game game)
        {
            _game = game;
            ImGui.SetCurrentContext(ImGui.CreateContext());
            IO = ImGui.GetIO();
            
            Keys = EnumHelper.GetValues<ImGuiKey>()
                              .Where(x => x != ImGuiKey.COUNT)
                              .Select(x => IO.KeyMap[(int)x] = (int)ImGuiKeyHelper.ToOpenTKKey(x))
                              .ToArray();

            InitializeFonts();
            
            //InitializeStyle();

            IO.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            IO.ConfigDockingTransparentPayload = true;

            if (UseViewports)
            {
                //_io.BackendFlags |= ImGuiBackendFlags.HasMouseHoveredViewport;
                
                IO.BackendFlags |= ImGuiBackendFlags.PlatformHasViewports;
                IO.BackendFlags |= ImGuiBackendFlags.RendererHasViewports;
                IO.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
                _viewportManager = new ImGuiViewportManager(this);
            }

            _renderer = new ImGuiRenderer(this, ManaWindow.MainWindow);
        }
        
        public IntPtr BindTexture(Texture2D texture)
        {
            IntPtr id = new IntPtr(TextureID++);
            BoundTextures.Add(id, texture);
            return id;
        }

        public void UnbindTexture(IntPtr id)
        {
            BoundTextures.Remove(id);
        }

        public void Dispose()
        {
            _renderer.Dispose();
        }

        public override void EarlyUpdate(float time, float deltaTime)
        {
        }

        public override void LateUpdate(float time, float deltaTime)
        {
        }

        public override void EarlyRender(float time, float deltaTime, RenderContext renderContext)
        {
            ImGui.GetIO().DeltaTime = deltaTime;
            UpdateInput();
            ImGui.NewFrame();
        }

        public override void LateRender(float time, float deltaTime, RenderContext renderContext)
        {
            ImGui.Render();
            _renderer.RenderDrawData(_game.RenderContext, ImGui.GetDrawData(), _game.Window.Width, _game.Window.Height);
            ImGui.EndFrame();

            if (UseViewports)
            {
                ManaWindow.MainWindow.RenderContext.Release();

                ImGui.UpdatePlatformWindows();
                ImGui.RenderPlatformWindowsDefault();

                ManaWindow.MainWindow.RenderContext.MakeCurrent();
            }
        }
        
        private void InitializeFonts()
        {
            var fonts = IO.Fonts;
            
            fonts.AddFontDefault();
            RebuildFontAtlas(_game.ResourceManager.MainContext);
        }
        
        private void InitializeStyle()
        {
            var style = ImGui.GetStyle();
            
            style.WindowRounding = 0.0f;
            style.ChildRounding = 0.0f;
            style.FrameRounding = 0.0f;
            style.GrabRounding = 0.0f;
            style.PopupRounding = 0.0f;
            style.ScrollbarRounding = 0.0f;
            style.TabRounding = 0.0f;
        }

        private void UpdateInput()
        {
            IO.DisplaySize = new Vector2(_game.Window.Width, _game.Window.Height);
            IO.DisplayFramebufferScale = Vector2.One;

            InputManager input;
            ManaWindow focusedWindow = null;
            ManaWindow hoveredWindow = null;
            ImGuiViewportPtr? focusedViewport = null;
            
            if (_viewportManager != null)
            {
                int focusedCount = 0;
                
                var platformIO = ImGui.GetPlatformIO();
                for (int i = platformIO.Viewports.Size - 1; i >= 0; i--)
                {
                    ImGuiViewportPtr viewport = platformIO.Viewports[i];
                
                    ManaWindow window = _viewportManager.GetWindow(viewport.PlatformHandle);

                    if (window.Focused)
                    {
                        focusedCount++;
                        focusedWindow = window;
                        focusedViewport = viewport;
                    }
                    
                    if (window.IsHovered)
                    {
                        hoveredWindow = window;
                    }
                }
                
                if (focusedWindow != null)
                    input = focusedWindow.Input;
                else
                    input = _game.Window.Input;
            }
            else
            {
                input = _game.Window.Input;
            }

            if (input == null) 
                return;
            
            for (int i = 0; i < Keys.Length; i++)
                IO.KeysDown[Keys[i]] = input.IsKeyDown((Key)Keys[i]);


            var offset = ManaWindow.MainWindow.Location.ToVector2() + new Vector2(8, 31);

            if (_viewportManager == null)
                IO.MousePos = input.MousePosition.ToVector2();
            else
                IO.MousePos = _viewportManager.GlobalMouseLocation - offset;
            
            IO.KeyShift = input.IsKeyDown(Key.ShiftLeft) || input.IsKeyDown(Key.ShiftRight);
            IO.KeyCtrl = input.IsKeyDown(Key.ControlLeft) || input.IsKeyDown(Key.ControlRight);
            IO.KeyAlt = input.IsKeyDown(Key.AltLeft) || input.IsKeyDown(Key.AltRight);
            IO.KeySuper = input.IsKeyDown(Key.WinLeft) || input.IsKeyDown(Key.WinRight);

            IO.MouseDown[0] = _viewportManager?.GlobalMouseLeft ?? input.MouseLeft;
            IO.MouseDown[1] = input.MouseRight;
            IO.MouseDown[2] = input.MouseMiddle;

            IO.MouseWheel = input.MouseWheelDelta > 0 ? 1 : input.MouseWheelDelta < 0 ? -1 : 0;
            
            while (input.CharacterBuffer.Count > 0)
                IO.AddInputCharacter(input.CharacterBuffer.Dequeue());
        }

        private unsafe void RebuildFontAtlas(RenderContext renderContext)
        {
            IO.Fonts.GetTexDataAsRGBA32(out byte* pixelData, out int width, out int height, out _);

            Texture2D fontTexture = Texture2D.CreateFromRGBAPointer(renderContext, width, height, pixelData);
            fontTexture.Label = "IMGUI Font Texture";

            IO.Fonts.SetTexID(BindTexture(fontTexture));
            IO.Fonts.ClearTexData();
        }
    }
}