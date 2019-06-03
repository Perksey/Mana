using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Shader
{
    public readonly struct ShaderAttributeInfo
    {
        public readonly string Name;
        public readonly int Location;
        public readonly int Size;
        public readonly ActiveAttribType Type;

        public ShaderAttributeInfo(string name, int location, int size, ActiveAttribType type)
        {
            Name = name;
            Location = location;
            Size = size;
            Type = type;
        }
    }
}