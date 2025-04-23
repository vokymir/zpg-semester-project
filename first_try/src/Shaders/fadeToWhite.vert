#version 330 core

layout(location = 0) in vec3 aPos;
layout(location = 2) in vec2 aTexCoords;

out vec2 TexCoords;

uniform mat4 projection;
uniform float Xover2;
uniform float Yover2;
uniform float Zdist;

void main()
{
    // scaling the whiteScreen
    vec4 posVS = vec4(aPos.x * Xover2,
            aPos.y * Yover2,
            -Zdist, // push the whitescreen a bit from the camera
            1.0);

    // project into clip space
    gl_Position = projection * posVS;

    TexCoords = aTexCoords;
}
