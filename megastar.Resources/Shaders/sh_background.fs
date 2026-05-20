precision highp float;   // All floats will be 32-bit by default
precision mediump int;   // All integers will be 16-bit by default

layout (std140, set = 0, binding = 0) uniform m_Parameters
{
    float g_Time;
    vec2 g_Resoltuion;
};

layout (location = 0) in vec2 v_TexCoord;
layout (location = 0) out vec4 FragColor;

void main(void)
{
    vec2 uv = v_TexCoord / g_Resoltuion;
    uv.y = 1 - sin(uv.y + g_Time) ;
    uv.x = sin(uv.x + g_Time);
    FragColor = vec4(vec2(uv), 0.0, 1.0);
}