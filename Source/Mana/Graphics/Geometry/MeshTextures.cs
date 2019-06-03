using Mana.Graphics.Shader;
using Mana.Graphics.Textures;

namespace Mana.Graphics.Geometry
{
    public class MeshTextures
    {
        public Texture2D Diffuse;
        public Texture2D Specular;
        public Texture2D MetallicRoughness;

        public void Apply(RenderContext renderContext, ShaderProgram shaderProgram)
        {
            int i = 0;

            void SetTexture(string uniform, Texture2D texture)
            {
                if (texture != null)
                {
                    renderContext.BindTexture(i, Diffuse);
                    shaderProgram.TrySetUniform(uniform, i++);
                }
            }

            SetTexture("texture_diffuse1", Diffuse);
            SetTexture("texture_specular1", Specular);
            SetTexture("texture_ambient_roughness_metalness", MetallicRoughness);
        }
    }
}