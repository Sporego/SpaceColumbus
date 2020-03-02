half3 rgb2yuv(half3 rgb)
{
    half3 yuv;
    yuv.x = rgb.r * 0.299 + rgb.g * 0.587 + rgb.b * 0.114;
    yuv.y = rgb.r * -0.147 + rgb.g * -0.289 + rgb.b * 0.436;
    yuv.z = rgb.r * 0.615 + rgb.g * -0.515 + rgb.b * 0.100;
    return yuv;
}

half3 yuv2rgb(half3 yuv)
{
    half3 rgb;
    rgb.x = yuv.x + yuv.z * 1.140;
    rgb.y = yuv.x + yuv.y * -0.395 + yuv.z * -0.581;
    rgb.z = yuv.x + yuv.y * 2.032;
    return rgb;
}

// take luminance from A and color information from B
half4 yuvBlendMode(half3 a, half3 b)
{
    half3 yuv1 = rgb2yuv(a);
    half3 yuv2 = rgb2yuv(b);
    half3 yuv3 = half3(yuv1.x, yuv2.y, yuv2.z);
    return half4(yuv2rgb(yuv3), 1);
}


float3 rgb2hsv(float3 c)
{
    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
    float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

float3 hsv2rgb(float3 c)
{
    c = float3(c.x, clamp(c.yz, 0.0, 1.0));
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// take value from A and hue/saturation from B
half4 hsvBlendMode(half3 a, half3 b)
{
    half3 hsv1 = rgb2hsv(a);
    half3 hsv2 = rgb2hsv(b);
    half3 hsv3 = half3(hsv2.x, hsv2.y, hsv1.z);
    return half4(hsv2rgb(hsv3), 1);
}

// take max value from A/B and hue/saturation from B
half4 hsvMaxBlendMode(half3 a, half3 b)
{
    half3 hsv1 = rgb2hsv(a);
    half3 hsv2 = rgb2hsv(b);
    half3 hsv3 = half3(max(hsv1.x, hsv2.x), hsv2.y, hsv1.z);
    return half4(hsv2rgb(hsv3), 1);
}
