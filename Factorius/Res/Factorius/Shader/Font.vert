#version 330 core

layout (location = 0) in vec3 pos;
layout (location = 1) in vec2 uv;

uniform vec2 screenSize;
uniform float fontSize;

out vec2 texCoord;

void main() {
	gl_Position = vec4((pos.x / screenSize.x) * 2.0 - 1.0, ((pos.y / screenSize.y) * 2.0 - 1.0), 0.0, 1.0);
	texCoord = uv;
}