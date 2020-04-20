#define TEXEL_SIZE(TEX) TEX##_TexelSize

#if defined(BLUR_PASS_1)
    static float2  blurMultiplyVec = float2(0.0f, 1.0f);
#elif defined(BLUR_PASS_2)
    static float2  blurMultiplyVec = float2(1.0f, 0.0f);
#else
    static float2  blurMultiplyVec = float2(0.0f, 0.0f);
#endif

fixed4 _Color;
float4 _ClipRect;

float _UseMainTexColor;

float _GausBlurSigma;     // The _GausBlurSigma value for the gaussian function: higher value means more blur
                                  // A good value for 9x9 is around 3 to 5
                                  // A good value for 7x7 is around 2.5 to 4
                                  // A good value for 5x5 is around 2 to 3.5
                                  // ... play around with this based on what you need :)

float _GausBlurSize;  // This should usually be equal to
                              // 1.0f / texture_pixel_width for a horizontal blur, and
                              // 1.0f / texture_pixel_height for a vertical blur.

float _GausBlurSamples; // basically = Kernel size width - 2; ex: for 3x3, use 1 sample; 5x5, use 3 samples

static float _PI = 3.14159265f;

inline bool uvInBounds(half2 uv)
{
    return uv.x >= 0 && uv.x <= 1 && uv.y >= 0 && uv.y <= 1;
}

sampler2D _MainTex;

float4 TEXEL_SIZE(_GRAB_TEX);

// Incremental Gaussian Coefficent Calculation (See GPU Gems 3 pp. 877 - 889)
// blurSampler: Texture that will be blurred by this shader
fixed4 fastGaussianBlur (sampler2D blurSampler, float4 uv)
{
    float3 incrementalGaussian = float3(0, 0, 0);
    incrementalGaussian.x = 1.0f / (sqrt(2.0f * _PI) * _GausBlurSigma);
    incrementalGaussian.y = exp(-0.5f / (_GausBlurSigma * _GausBlurSigma));
    incrementalGaussian.z = incrementalGaussian.y * incrementalGaussian.y;

    float4 avgValue = float4(0,0,0,0);
    float coefficientSum = 0.0f;

    // Take the central sample first...
    avgValue += tex2Dproj(blurSampler, uv) * incrementalGaussian.x;
    coefficientSum += incrementalGaussian.x;
    incrementalGaussian.xy *= incrementalGaussian.yz;

    float4 texelSize = TEXEL_SIZE(_GRAB_TEX);

#if defined(BLUR_PASS_1)
    float blurSize = _GausBlurSize * texelSize.y;
#elif defined(BLUR_PASS_2)
    float blurSize = _GausBlurSize * texelSize.x;
#else
    float blurSize = 0;
#endif

    float2 offset = blurSize * blurMultiplyVec * uv.w; // * uv.w, if not added, surfaces closer to camera will appear blurrier
    float2 offset_cur = offset;
    // Go through the remaining 8 vertical samples (4 on each side of the center)
    for (float i = 1.0f; i <= _GausBlurSamples; i++)
    {
        float4 uv1 = uv;
        uv1.xy -= offset_cur;
        float4 uv2 = uv;
        uv2.xy += offset_cur;

        // increment offset
        offset_cur += offset;

        avgValue += tex2Dproj(blurSampler, uv1) * incrementalGaussian.x;
        avgValue += tex2Dproj(blurSampler, uv2) * incrementalGaussian.x;
        coefficientSum += 2 * incrementalGaussian.x;

        incrementalGaussian.xy *= incrementalGaussian.yz;
    }

    return avgValue / coefficientSum;
}

#if defined (BLUR_SHADER_FUNCS)
struct appdata_t
{
    float4 vertex   : POSITION;
    float2 uv : TEXCOORD0;
    fixed4 color    : COLOR;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
    fixed4 color : COLOR;
    float4 pos : POSITION0;
    float2 uv : TEXCOORD0;
    float4 uvGrab : TEXCOORD1;
    float4 worldPosition : TEXCOORD2;
};

sampler2D _GRAB_TEX;

v2f vert(appdata_t IN)
{
    v2f OUT;
    OUT.color = IN.color;
    OUT.pos = UnityObjectToClipPos(IN.vertex);
    OUT.uv = IN.uv;
    OUT.uvGrab = ComputeGrabScreenPos(OUT.pos);
    OUT.worldPosition = IN.vertex;
    return OUT;
}

fixed4 frag(v2f IN) : SV_Target
{
    half4 maintex = tex2D(_MainTex, IN.uv);

    half4 color = fastGaussianBlur(_GRAB_TEX, IN.uvGrab);

    color *= IN.color * _Color; // apply tint

#ifdef BLUR_PASS_2 // only apply texture color on the second pass to avoid maintex blurring
    if (_UseMainTexColor)
        color *= maintex;
#endif

    color.a = maintex.a;
    color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

#ifdef UNITY_UI_ALPHACLIP
    clip(color.a - 0.001);
#endif

    return color;
}
#endif
