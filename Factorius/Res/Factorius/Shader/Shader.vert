#version 330 core

layout (location = 0) in vec3 pos;
layout (location = 1) in vec2 uv;

uniform mat4 transformMatrix;

out vec2 texCoord;

void main() {
	gl_Position = transformMatrix * vec4(pos, 1.0);
	texCoord = uv;
}