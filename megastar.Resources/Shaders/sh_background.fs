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
    
    // Create a beautiful, clean pinkish-purple gradient background
    vec3 deepPinkPurple = vec3(0.25, 0.08, 0.28); // Rich dark magenta-purple base
    vec3 softPinkPurple = vec3(0.48, 0.16, 0.44); // Vibrant upper gradient
    vec3 finalColor     = mix(deepPinkPurple, softPinkPurple, uv.y);
    
    // Wave colors (ranging from soft rose to light pastel pink)
    vec3 waveColor1 = vec3(0.68, 0.28, 0.52);
    vec3 waveColor2 = vec3(0.85, 0.44, 0.66);
    vec3 waveColor3 = vec3(0.96, 0.62, 0.78); // Lightest pink
    
    // Layer 1: Slow, deep background wave
    // (Combining two sine frequencies makes the liquid motion look organic and non-repetitive)
    float w1 = sin(uv.x * 2.5 - g_Time * 0.3) * 0.07 + 
              sin(uv.x * 5.0 - g_Time * 0.5) * 0.02 + 0.50;
    float mask1 = smoothstep(w1 + 0.005, w1 - 0.005, uv.y);
    finalColor = mix(finalColor, waveColor1, mask1 * 0.35); // 35% opacity blend
    
    // Layer 2: Midground wave (slightly faster, offset phase)
    float w2 = sin(uv.x * 3.5 - g_Time * 0.5 + 2.0) * 0.05 + 
              sin(uv.x * 1.8 - g_Time * 0.2) * 0.03 + 0.35;
    float mask2 = smoothstep(w2 + 0.005, w2 - 0.005, uv.y);
    finalColor = mix(finalColor, waveColor2, mask2 * 0.45); // 45% opacity blend
    
    // Layer 3: Foreground wave (lightest pink, crispest movement)
    float w3 = sin(uv.x * 2.0 - g_Time * 0.7 + 4.0) * 0.08 + 
              sin(uv.x * 6.0 - g_Time * 0.9) * 0.01 + 0.20;
    float mask3 = smoothstep(w3 + 0.005, w3 - 0.005, uv.y);
    finalColor = mix(finalColor, waveColor3, mask3 * 0.60); // 60% opacity blend

    FragColor = vec4(finalColor, 1.0);
}