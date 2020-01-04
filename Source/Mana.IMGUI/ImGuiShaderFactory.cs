using Mana.Graphics;
using Mana.Graphics.Shaders;

namespace Mana.IMGUI
{
    public static class ImGuiShaderFactory
    {
        public static ShaderProgram CreateShaderProgram(RenderContext renderContext)
        {
            VertexShader vertexShader = CreateVertexShader(renderContext);
            FragmentShader fragmentShader = CreateFragmentShader(renderContext);

            ShaderProgram shaderProgram = new ShaderProgram(renderContext);

            shaderProgram.AttachShader(vertexShader);
            shaderProgram.AttachShader(fragmentShader);

            shaderProgram.Link();

            shaderProgram.DetachShader(vertexShader);
            shaderProgram.DetachShader(fragmentShader);

            vertexShader.Dispose();
            fragmentShader.Dispose();

            shaderProgram.Label = "IMGUI ShaderProgram";

            return shaderProgram;
        }

        private static FragmentShader CreateFragmentShader(RenderContext renderContext)
        {
            return new FragmentShader(renderContext, @"#version 330 core

                out vec4 FragColor;

                in vec2 TexCoord;
                in vec4 Color;

                uniform sampler2D texture0;

                void main()
                {
                    FragColor = texture(texture0, TexCoord) * Color;
                }"
            );
        }

        private static VertexShader CreateVertexShader(RenderContext renderContext)
        {
            return new VertexShader(renderContext, @"#version 330 core

                layout (location = 0) in vec2 aPos;
                layout (location = 1) in vec2 aTexCoord;
                layout (location = 2) in vec4 aColor;

                out vec2 TexCoord;
                out vec4 Color;

                uniform mat4 projection;

                void main()
                {
                    gl_Position = projection * vec4(aPos, 1.0, 1.0);
                    TexCoord = aTexCoord;
                    Color = aColor;
                }"
            );
        }
    }
}
