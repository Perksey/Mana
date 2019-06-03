using Mana.Graphics;
using Mana.Graphics.Shader;

namespace Mana.IMGUI
{
    public static class ImGuiShaderFactory
    {
        public static ShaderProgram CreateShaderProgram(ResourceManager resourceManager)
        {
            VertexShader vertexShader = CreateVertexShader(resourceManager);
            FragmentShader fragmentShader = CreateFragmentShader(resourceManager);
            
            ShaderProgram shaderProgram = new ShaderProgram(resourceManager);

            shaderProgram.Link(vertexShader, fragmentShader);
            
            vertexShader.Dispose();
            fragmentShader.Dispose();
            
            ShaderHelper.BuildShaderInfo(shaderProgram);

            shaderProgram.Label = "IMGUI ShaderProgram";
            
            return shaderProgram;
        }

        private static FragmentShader CreateFragmentShader(ResourceManager resourceManager)
        {
            return new FragmentShader(resourceManager, @"#version 330 core
                
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

        private static VertexShader CreateVertexShader(ResourceManager resourceManager)
        {
            return new VertexShader(resourceManager, @"#version 330 core

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