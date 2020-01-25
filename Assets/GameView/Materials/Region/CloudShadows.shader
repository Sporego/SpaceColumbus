Shader "Projector/CloudShadowsHDR"
{
    Properties
    {
        [Header(Opacity)]
        _Opacity("Color", Color) = (0.5,0.5,0.5,0.5)
        [Header(Textures and Size)]
        _ShadowTex("Shadow Texture", 2D) = "black" { }
        _NoiseTex("Distortion Texture", 2D) = "black" { }
        [Header(Movement)]
        _Magnitude("Magnitude", Float) = 0.25
        _Freq("Frequency", Float) = 100.0
        [Header(Height)]
        _EdgeBlend("Edge Blend", Range(0.1,100)) = 20.0
        _Height("Height", Float) = 10.0
    }

        Subshader{
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent+200" }
            Pass {
                ZWrite Off
                //ColorMask RGB
                Blend DstColor One

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog
                #include "UnityCG.cginc"

                struct v2f {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float2 uvNoise : TEXCOORD1;
                    float3 wPos : TEXCOORD2; // added for height comparisons.
                    UNITY_FOG_COORDS(3)
                };

                float4x4 unity_Projector;
                sampler2D _ShadowTex;
                sampler2D _NoiseTex;
                float4 _ShadowTex_ST;
                float4 _NoiseTex_ST;
                float4 _Opacity;
                float _Magnitude;
                float _Freq;
                float _Height;
                float _EdgeBlend;

                v2f vert(appdata_tan v) {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    o.uv = TRANSFORM_TEX(mul(unity_Projector, v.vertex).xy, _ShadowTex);
                    o.uvNoise = TRANSFORM_TEX(mul(unity_Projector, v.vertex).xy, _NoiseTex);
                    UNITY_TRANSFER_FOG(o,o.pos);
                    return o;
                }

                fixed4 frag(v2f i) : COLOR {
                    float time = _Time[1];
                    fixed4 noise = tex2D(_NoiseTex, i.uvNoise - frac(time / _Freq));
                    float2 circ = 3.1415 * 5.0 / _Freq;
                    float2 waterDisplacement = _Magnitude + _Magnitude;
                    waterDisplacement = waterDisplacement / 2.0 * (1.0 + (float2(time, -time) + noise.xy) * circ) - _Magnitude;
                    fixed4 c = tex2D(_ShadowTex, i.uv * 0.1 + waterDisplacement);
                    c = clamp(lerp(c,0,(i.wPos.y - _Height) / _EdgeBlend),-1,0);
                    c = c * _Opacity;
                    UNITY_APPLY_FOG_COLOR(i.fogCoord, c, fixed4(0,0,0,0));
                    return c;
                }
                ENDCG
            }
        }
}