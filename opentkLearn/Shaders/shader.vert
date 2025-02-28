﻿#version 330 core
layout (location = 0) in vec3 aPos;
//layout (location = 1) in vec3 aColor;
layout (location = 1) in vec2 aTexCoord;

out vec2 texCoord;

uniform mat4 transform;

void main()
{

	texCoord = aTexCoord;

	gl_Position = vec4(aPos, 1.0) * transform;
}
