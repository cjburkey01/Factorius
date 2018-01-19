#version 330 core

layout (location = 0) in vec3 pos;
layout (location = 1) in vec2 uv;

uniform mat4 projectionMatrix;
uniform mat4 posMatrix;

out vec2 texCoords;

void main() {
	gl_Position = projectionMatrix * posMatrix * vec4(pos.x, pos.y, pos.z - 0.1, 1.0);
	texCoords = uv;
}