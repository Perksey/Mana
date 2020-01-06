namespace Mana.Graphics.Shaders
{
    public static class BasicShaderFactory
    {
        public static ShaderProgram CreateSpriteShaderProgram(RenderContext renderContext, string label = "Sprite ShaderProgram")
        {
            VertexShader vertexShader = new VertexShader(renderContext,
@"#version 330 core

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
}");

            FragmentShader fragmentShader = new FragmentShader(renderContext,
@"#version 330 core

out vec4 FragColor;

in vec2 TexCoord;
in vec4 Color;

uniform sampler2D texture0;

void main()
{
    FragColor = texture(texture0, TexCoord) * Color;
}");

            ShaderProgram shaderProgram = new ShaderProgram(renderContext);

            shaderProgram.AttachShader(vertexShader);
            shaderProgram.AttachShader(fragmentShader);

            shaderProgram.Link();

            shaderProgram.DetachShader(vertexShader);
            shaderProgram.DetachShader(fragmentShader);

            vertexShader.Dispose();
            fragmentShader.Dispose();

            shaderProgram.Label = label;

            return shaderProgram;
        }

        public static ShaderProgram CreateLineShaderProgram(RenderContext renderContext, string label = "Line ShaderProgram")
        {
            VertexShader vertexShader = new VertexShader(renderContext,
@"#version 330 core

layout (location = 0) in vec2 aPos;
layout (location = 1) in vec4 aColor;

out vec4 Color;

uniform mat4 projection;

void main()
{
    gl_Position = projection * vec4(aPos, 1.0, 1.0);
    Color = aColor;
}");

            FragmentShader fragmentShader = new FragmentShader(renderContext,
@"#version 330 core

out vec4 FragColor;

in vec4 Color;

void main()
{
    FragColor = Color;
}");

            ShaderProgram shaderProgram = new ShaderProgram(renderContext);

            shaderProgram.AttachShader(vertexShader);
            shaderProgram.AttachShader(fragmentShader);

            shaderProgram.Link();

            shaderProgram.DetachShader(vertexShader);
            shaderProgram.DetachShader(fragmentShader);

            vertexShader.Dispose();
            fragmentShader.Dispose();

            shaderProgram.Label = label;

            return shaderProgram;
        }
    }
}
