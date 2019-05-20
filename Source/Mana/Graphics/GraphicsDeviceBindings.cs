namespace Mana.Graphics
{
    internal struct GraphicsDeviceBindings
    {
        public GLHandle VertexBuffer;
        public GLHandle IndexBuffer;
        public GLHandle FrameBuffer;
        public GLHandle PixelBuffer;
        public GLHandle ShaderProgram;

        public int ActiveTexture;
        public GLHandle[] TextureUnits;

        public GLHandle Texture
        {
            get => TextureUnits[ActiveTexture];
            set => TextureUnits[ActiveTexture] = value;
        }
    }
}