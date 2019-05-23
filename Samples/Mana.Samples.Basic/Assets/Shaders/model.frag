#version 330 core

struct Material {
    float shininess;
};

uniform Material material;

out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoord;

uniform sampler2D texture_diffuse1;
uniform sampler2D texture_specular1;
uniform sampler2D texture_ambient_roughness_metalness;

uniform vec3 lightPos;
uniform vec3 cameraPos;
uniform vec3 lightColor;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

//#define VISUALIZE_NORMALS
//#define VISUALIZE_SPECULAR
//#define VISUALIZE_DIFFUSE
//#define VISUALIZE_DIFFUSE_ALPHA

void main()
{
//    {
//        #ifdef VISUALIZE_SPECULAR
//        vec4 s = texture(texture_specular1, TexCoord);
//        FragColor = vec4(s.r, s.g, s.b, 1.0);
//        return;
//        #endif
//    }
//
//    {
//        #ifdef VISUALIZE_DIFFUSE
//        FragColor = texture(texture_diffuse1, TexCoord);
//        return;
//        #endif
//    }
//
//    {
//        #ifdef VISUALIZE_DIFFUSE_ALPHA
//        vec4 s = texture(texture_diffuse1, TexCoord);
//        float r = s.r;
//        FragColor = vec4(r, r, r, 1.0);
//        return;
//        #endif
//    }
//
//    {
//        #ifdef VISUALIZE_NORMALS
//        FragColor = vec4((Normal.xyz + vec3(1.0, 1.0, 1.0)) / 2.0 , 1.0);
//        return;
//        #endif
//    }

    FragColor = texture(texture_diffuse1, TexCoord);
}
