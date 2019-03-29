using ImGuiNET;
using Mana.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Mana.IMGUI
{
    public class ImGuiRenderer : GameComponent
    {
        public ImGuiRenderer()
        {
        }

        public override void OnAddedToGame(Game game)
        {
            base.OnAddedToGame(game);
            
            ImGui.SetCurrentContext(ImGui.CreateContext());
            SetupInput();
            RebuildFontAtlas();
        }

        private void SetupInput()
        {
        }

        private unsafe void RebuildFontAtlas()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.Fonts.GetTexDataAsAlpha8(out byte* pixelData, out int width, out int height, out _);

            Texture2D fontTexture = new Texture2D(GraphicsDevice);
            //fontTexture.SetDataFromAlpha(pixelData, width, height, TextureFilterMode.Nearest, TextureWrapMode.Repeat);

            //io.Fonts.SetTexID(BindTexture(fontTexture));
            io.Fonts.ClearTexData();
        }

        public override void EarlyRender(float time, float deltaTime)
        {
            BeforeLayout(deltaTime);
        }

        public override void LateRender(float time, float deltaTime)
        {
            AfterLayout();
        }

        private void BeforeLayout(float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        private void AfterLayout()
        {
            throw new System.NotImplementedException();
        }
    }
}