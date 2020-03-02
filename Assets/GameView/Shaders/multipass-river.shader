// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Exploration/River" {
    Properties{
        _Color("Main Color", Color) = (1,1,1,1)
        _DepthColor("Depth Color", Color) = (1,1,1,1)
        _WaterDepth("Water Depth", Range(0, 10)) = 1
        _BumpMap("Normal Shading (Normal)", 2D) = "bump" {}
        _WaterSpeed("Water Speed", Range(0, 10)) = 1
        _WaterSpeed2("Water Speed", Range(0, 10)) = 0.37
        _Fresnel("Fresnel Value", Float) = 0.028
    }
        SubShader{
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            Blend One One

            Pass
            {
                Name "RiverDepth"
                Blend SrcAlpha OneMinusSrcAlpha
                CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    #pragma fragmentoption ARB_precision_hint_fastest
                    #include "UnityCG.cginc"

                    struct v2f {
                        float4 pos          : POSITION;
                        float4 screenPos    : TEXCOORD0;
                    };

                    v2f vert(appdata_full v)
                    {
                        v2f o;
                        o.pos = UnityObjectToClipPos(v.vertex);
                        o.screenPos = ComputeScreenPos(o.pos);
                        return o;
                    }

                    sampler2D _CameraDepthTexture;
                    float4 _DepthColor;
                    float _WaterDepth;

                    half4 frag(v2f i) : COLOR
                    {
                        float depth = 1 - saturate(_WaterDepth - (LinearEyeDepth(tex2D(_CameraDepthTexture, i.screenPos.xy / i.screenPos.w).r) - i.screenPos.z));
                        return half4(_DepthColor.rgb, depth * _DepthColor.a);
                    }
                ENDCG
            }

            GrabPass {
                Name "RiverGrab"
            }

            Pass
            {
                Name "RiverDistortion"
                Blend Off
                CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    #pragma fragmentoption ARB_precision_hint_fastest
                    #include "UnityCG.cginc"

                    struct v2f {
                        float4 pos          : POSITION;
                        float4 uvgrab       : TEXCOORD0;
                        float2 uv           : TEXCOORD1;
                        float4 screenPos    : TEXCOORD2;
                    };

                    v2f vert(appdata_full v)
                    {
                        v2f o;
                        o.pos = UnityObjectToClipPos(v.vertex);
                        #if UNITY_UV_STARTS_AT_TOP
                        float scale = -1.0;
                        #else
                        float scale = 1.0;
                        #endif
                        o.uvgrab.xy = (float2(o.pos.x, o.pos.y * scale) + o.pos.w) * 0.5;
                        o.uvgrab.zw = o.pos.zw;
                        o.uv = v.texcoord.xy;
                        return o;
                    }

                    sampler2D _BumpMap;
                    float _WaterSpeed, _WaterSpeed2;
                    sampler2D _GrabTexture;
                    float4 _GrabTexture_TexelSize;

                    half4 frag(v2f i) : COLOR
                    {
                        float2 riverUVs = i.uv;
                        riverUVs.y += _Time * _WaterSpeed;
                        float3 normal1 = UnpackNormal(tex2D(_BumpMap, riverUVs));
                        riverUVs = i.uv;
                        riverUVs.x *= -1;
                        riverUVs.y += 0.3 + _Time * _WaterSpeed2;
                        float3 normal2 = UnpackNormal(tex2D(_BumpMap, riverUVs));
                        normal2 *= float3(1, 1, 0.5);

                        float3 combinedNormal = normalize(normal1 * normal2);

                        float2 offset = combinedNormal.xy * 5 * _GrabTexture_TexelSize.xy;
                        i.uvgrab.xy = (offset * i.uvgrab.z) + i.uvgrab.xy;
                        return half4(tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab)).rgb, 1);
                    }
                ENDCG
            }

            CGPROGRAM
                #include "ExplorationLighting.cginc"
                #pragma surface surf ExplorationRiver noambient novertexlights nolightmap
                #pragma target 3.0

                struct Input
                {
                    float2 uv_BumpMap;
                };

                sampler2D _BumpMap, _CameraDepthTexture;
                float _Specular, _Gloss, _WaterSpeed, _WaterSpeed2;

                void surf(Input IN, inout SurfaceOutput o)
                {
                    float2 riverUVs = IN.uv_BumpMap;
                    riverUVs.y += _Time * _WaterSpeed;
                    float3 normal1 = UnpackNormal(tex2D(_BumpMap, riverUVs));
                    riverUVs = IN.uv_BumpMap;
                    riverUVs.x *= -1;
                    riverUVs.y += 0.3 + _Time * _WaterSpeed2;
                    float3 normal2 = UnpackNormal(tex2D(_BumpMap, riverUVs));
                    normal2 *= float3(1, 1, 0.5);

                    float3 combinedNormal = normalize(normal1 * normal2);

                    o.Albedo = fixed3(1,1,1);
                    o.Normal = combinedNormal;
                    o.Alpha = 0;
                }
            ENDCG
        }

            Fallback "Transparent/VertexLit"
}