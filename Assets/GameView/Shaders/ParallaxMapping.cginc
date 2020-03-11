float2 getParallaxOffset(sampler2D Tex, float parallaxAmount, float2 UV, float3 viewDir)
{
    float h = tex2D(Tex, UV).x;
    return ParallaxOffset(h, parallaxAmount, viewDir);
}
