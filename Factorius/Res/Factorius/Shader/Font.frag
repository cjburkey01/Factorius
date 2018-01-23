#version 330 core

uniform sampler2D sampler;
uniform vec4 fontColor;

in vec2 texCoord;

out vec4 fragColor;

void main() {
	fragColor = vec4(1.0, 1.0, 1.0, texture(sampler, texCoord).r) * fontColor;
}