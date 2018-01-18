#version 330 core

layout (location = 0) in vec3 pos;

uniform mat4 transformMatrix;

void main() {
	gl_Position = transformMatrix * vec4(pos, 1.0);
}