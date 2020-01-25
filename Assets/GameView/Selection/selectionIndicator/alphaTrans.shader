// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit alpha - blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Unlit/TransparentAlphaMask" {
    Properties{
        [Toggle] _IsCircle("Is Circle (or Square)", Float) = 0
        _Color("Tint Color", Color) = (1,1,1,1)
        _OpacityBoost("Opacity Boost", Range(0.01, 10.0)) = 2
        _OutlineDepth("Outline Depth", Range(0.01, 1.0)) = 0.2
        _OutlineThickness("Outline Thickness", Range(0.01, 1.0)) = 0.02
    }

        SubShader{
            Tags {"Queue" = "Transparent"}
            LOD 100

            ZWrite Off

            Blend SrcAlpha OneMinusSrcAlpha

            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog

                #include "UnityCG.cginc"

                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    UNITY_FOG_COORDS(1)
                    float4 vertex : SV_POSITION;
                };

                fixed4 _Color;
                float _OutlineDepth;
                float _OutlineThickness;
                float _OpacityBoost;

                UNITY_INSTANCING_BUFFER_START(Props)
                    UNITY_DEFINE_INSTANCED_PROP(float, _IsCircle)
                UNITY_INSTANCING_BUFFER_END(Props)

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    UNITY_TRANSFER_FOG(o, o.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float2 uv = -1.0 + i.uv * 2.0;
                    uv = uv * uv;

                    float alpha = uv.x + uv.y - _OutlineDepth * _OutlineDepth;
                    alpha = clamp(alpha, 0, _OutlineThickness) / _OutlineThickness;

                    // add external check
                    if (UNITY_ACCESS_INSTANCED_PROP(Props, _IsCircle))
                    {
                        alpha = clamp(2 * alpha, 0, 1) - alpha;
                    }

                    return fixed4(_Color.r, _Color.g, _Color.b, _OpacityBoost * alpha);
                }
                ENDCG
            }
    }
}