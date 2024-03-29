#version 330 core

out vec4 FragColor;

in vec2 TexCoord;
in vec4 Color;

uniform sampler2D texture0;

void main()
{
    FragColor = texture(texture0, TexCoord) * Color; 
    //FragColor = vec4(1.0, 1.0, 0.0, 1.0);
}