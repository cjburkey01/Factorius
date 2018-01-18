﻿#version 330 core

layout (location = 0) in vec3 pos;
layout (location = 1) in vec2 uv;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;

out vec2 texCoord;

void main() {
	gl_Position = modelMatrix * viewMatrix * projectionMatrix * vec4(pos, 1.0);
	texCoord = uv;
}