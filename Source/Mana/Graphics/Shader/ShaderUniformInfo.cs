using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Shader
{
    public readonly struct ShaderUniformInfo
    {
        public readonly string Name;
        public readonly int Location;
        public readonly int Size;
        public readonly ActiveUniformType Type;

        public ShaderUniformInfo(string name, int location, int size, ActiveUniformType type)
        {
            Name = name;
            Location = location;
            Size = size;
            Type = type;
        }
    }
}