#version 330 core
layout(location = 0) in vec2 aPos;
layout(location = 1) in vec2 aTex;
uniform mat4 uModel;
uniform mat4 uProj;
uniform vec2 uSize;
out vec2 vTex;
void main()
{
    vTex = aTex;
    gl_Position = uProj * uModel * vec4(aPos * uSize, 0.0, 1.0);
}