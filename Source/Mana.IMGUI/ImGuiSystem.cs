using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using Mana.Graphics;
using Mana.Graphics.Textures;
using Mana.IO;
using Mana.Utilities;
using Mana.Utilities.Extensions;
using osuTK.Input;

namespace Mana.IMGUI
{
    public class ImGuiSystem : IGameSystem, IDisposable
    {
        private static Logger _log = new Logger("IMGUI");

        public readonly bool UseViewports;

        internal ImGuiIOPtr IO;
        internal int[] Keys;

        internal readonly Dictionary<IntPtr, Texture2D> BoundTextures = new Dictionary<IntPtr, Texture2D>();
        internal int TextureID;

        private Game _game;
        private ImGuiRenderer _renderer;

        public static ImGuiSystem Instance { get; private set; }

        public ImFontPtr ProggyClean;
        public ImFontPtr RobotoMedium;

        public ImGuiSystem(bool useViewports = false)
        {
            if (Instance != null)
            {
                throw new InvalidOperationException("Only one ImGuiSystem instance may be created.");
            }

            Instance = this;
            ImGuiHelper.System = this;
            UseViewports = useViewports;

            Input.KeyTyped += c => IO.AddInputCharacter(c);
        }

        public void OnAddedToGame(Game game)
        {
            _game = game;
            ImGui.SetCurrentContext(ImGui.CreateContext());
            IO = ImGui.GetIO();

            Keys = EnumHelper.GetValues<ImGuiKey>()
                             .Where(x => x != ImGuiKey.COUNT)
                             .Select(x => IO.KeyMap[(int)x] = (int)ImGuiKeyHelper.ToOpenTKKey(x))
                             .ToArray();

            InitializeFonts();
            InitializeStyle();

            // IO.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            // IO.ConfigDockingTransparentPayload = true;
            //
            // if (UseViewports)
            // {
            //     //_io.BackendFlags |= ImGuiBackendFlags.HasMouseHoveredViewport;
            //
            //     IO.BackendFlags |= ImGuiBackendFlags.PlatformHasViewports;
            //     IO.BackendFlags |= ImGuiBackendFlags.RendererHasViewports;
            //     IO.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
            //     _viewportManager = new ImGuiViewportManager(this);
            // }

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

        public void EarlyUpdate(float time, float deltaTime)
        {
        }

        public void LateUpdate(float time, float deltaTime)
        {
        }

        public void EarlyRender(float time, float deltaTime, RenderContext renderContext)
        {
            ImGui.GetIO().DeltaTime = deltaTime;
            UpdateInput();
            ImGui.NewFrame();
        }

        public void LateRender(float time, float deltaTime, RenderContext renderContext)
        {
            ImGui.Render();
            _renderer.RenderDrawData(_game.RenderContext, ImGui.GetDrawData(), _game.Window.Width, _game.Window.Height);
            ImGui.EndFrame();
        }

        private void InitializeFonts()
        {
            var fonts = IO.Fonts;

            RobotoMedium = LoadEmbeddedFont("Roboto-Medium.ttf", 16f);
            ProggyClean = fonts.AddFontDefault();

            RebuildFontAtlas(_game.RenderContext);
        }

        private void InitializeStyle()
        {
            var style = ImGui.GetStyle();

            float rounding = 3.5f;

            style.WindowRounding = rounding;
            style.ChildRounding = rounding;
            style.FrameRounding = rounding;
            style.GrabRounding = rounding;
            style.PopupRounding = rounding;
            style.ScrollbarRounding = rounding;
            style.TabRounding = rounding;
        }

        private void UpdateInput()
        {
            IO.DisplaySize = new Vector2(_game.Window.Width, _game.Window.Height);
            IO.DisplayFramebufferScale = Vector2.One;

            for (int i = 0; i < Keys.Length; i++)
            {
                IO.KeysDown[Keys[i]] = Input.IsKeyDown((Key)Keys[i]);
            }

            IO.KeysDown[(int)Key.Enter] = Input.IsKeyDown(Key.Enter) || Input.IsKeyDown(Key.KeypadEnter);

            IO.MousePos = Input.MousePosition.ToVector2();

            IO.KeyShift = Input.IsKeyDown(Key.ShiftLeft) || Input.IsKeyDown(Key.ShiftRight);
            IO.KeyCtrl = Input.IsKeyDown(Key.ControlLeft) || Input.IsKeyDown(Key.ControlRight);
            IO.KeyAlt = Input.IsKeyDown(Key.AltLeft) || Input.IsKeyDown(Key.AltRight);
            IO.KeySuper = Input.IsKeyDown(Key.WinLeft) || Input.IsKeyDown(Key.WinRight);

            IO.MouseDown[0] = Input.MouseLeft;
            IO.MouseDown[1] = Input.MouseRight;
            IO.MouseDown[2] = Input.MouseMiddle;
            IO.MouseWheel = Input.MouseWheelDelta > 0 ? 1 : Input.MouseWheelDelta < 0 ? -1 : 0;
        }

        private unsafe void RebuildFontAtlas(RenderContext renderContext)
        {
            IO.Fonts.GetTexDataAsRGBA32(out byte* pixelData, out int width, out int height, out _);

            Texture2D fontTexture = Texture2D.CreateFromRGBAPointer(renderContext, width, height, pixelData);
            fontTexture.Label = "IMGUI Font Texture";

            IO.Fonts.SetTexID(BindTexture(fontTexture));
            IO.Fonts.ClearTexData();
        }

        private unsafe ImFontPtr LoadEmbeddedFont(string resourceName, float sizePixels)
        {
            var data = EmbeddedResource.LoadFromAssembly(typeof(ImGuiSystem).Assembly, $"Mana.IMGUI.Fonts.{resourceName}");

            fixed (byte* fontPtr = &data[0])
            {
                return IO.Fonts.AddFontFromMemoryTTF(new IntPtr(fontPtr), data.Length, sizePixels);
            }
        }
    }
}
