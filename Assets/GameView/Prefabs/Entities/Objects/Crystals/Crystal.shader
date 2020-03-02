// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Crystal"
{
    Properties
    {
        [Header(Main Material Parameters)]

        _Color("Tint", Color) = (1,1,1,1)
        _Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
        _Metallic("Metallic", Range(0.0, 1.0)) = 1.0
        _BumpScale("Normal Scale", Float) = 1.0
        [Normal] _BumpMap("Normal Map", 2D) = "bump" {}
        _Parallax("Height Scale", Range(0.005, 0.1)) = 0.02
        _ParallaxMap("Height Map", 2D) = "black" {}
        _DistortionAmount("Distortion Amount", float) = 0
        _DistortionInfluence("Distortion Influence", Range(0.0, 1.0)) = 0
        _MinAngleFactor("Distortion Angle Min Influence", Range(0.001, 0.999)) = 0
        _RootPos("Root Position", Vector) = (1,1,1,1)
        _EmissionFalloff("Distance Emission Falloff", Range(0.01, 10.0)) = 1
        _EmissionPower("Distance Emission Power Scaling", Range(0, 3)) = 0.5
        [HDR] _EmissionColor("Emission Color", Color) = (1,1,1,1)
        _EmissionMapInfluence("Emission Map Influence", Range(0, 1)) = 0.75
        _EmissionMap("Emission Map", 2D) = "black" {}
    }
        SubShader
    {
        Tags { "Queue" = "Transparent+100" }
        //LOD 200
        //Blend SrcAlpha OneMinusSrcAlpha

        // Grab the screen behind the object into _BackgroundTexture
        GrabPass
        {
            "_CrystalBackgroundTexture1"
        }

        //Pass
        //{
        //    Name "BackFaceDepthPass"

        //    Cull Front
        //    ZWrite On
        //    ColorMask 0
        //}

        // Surface Shader Passes

        ZWrite On

        CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Standard fullforwardshadows

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0

            #include "Assets/GameView/Shaders/ParallaxMapping.cginc"

            sampler2D _MainTex;

            struct Input
            {
                float2 uv_BumpMap;
                float2 uv_ParallaxMap;
                float3 viewDir;
                float3 worldPos;
                float4 screenPos;
            };

            // main texture
            fixed4 _Color;
            sampler2D _Tex;
            float _Glossiness;
            float _Metallic;
            float _BumpScale;
            sampler2D _BumpMap;
            sampler2D _ParallaxMap;
            fixed4 _RootPos;
            float _Parallax;
            fixed4 _EmissionColor;
            half _EmissionFalloff;
            half _EmissionMapInfluence;
            half _EmissionPower;
            sampler2D _EmissionMap;

            // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
            // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
            // #pragma instancing_options assumeuniformscaling
            UNITY_INSTANCING_BUFFER_START(Props)
                // put more per-instance properties here
            UNITY_INSTANCING_BUFFER_END(Props)

            void surf (Input IN, inout SurfaceOutputStandard o)
            {
                IN.uv_BumpMap += getParallaxOffset(_ParallaxMap, _Parallax, IN.uv_ParallaxMap, IN.viewDir);
                o.Normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_BumpMap), _BumpScale);
                
                o.Albedo = _Color.rgb;

                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;

                float angleFactor = abs(dot(IN.viewDir, o.Normal)); // TODO: VERIFY WORLD VS OBJECT NORMALS?
                
                float4 emission = lerp(1, tex2D(_EmissionMap, IN.uv_BumpMap), _EmissionMapInfluence);

                float3 localPos = IN.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
                float distanceFactor = pow(min(1, distance(_RootPos.xyz, localPos) / _EmissionFalloff), _EmissionPower);

                o.Emission = _EmissionColor * smoothstep(0, 1, emission * angleFactor * (1 - distanceFactor));
                o.Alpha = _Color.a;
            }
        ENDCG

        GrabPass
        {
            //"_CrystalBackgroundTexture2"
        }

        Pass
        {
            Name "CrystalDistortion"
            //Cull On
            ZWrite On

            //Blend One One

            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"
                #include "UnityStandardUtils.cginc"
                #include "Assets/GameView/Shaders/colorspaces.cginc"
                #include "Assets/GameView/Shaders/ParallaxMapping.cginc"
                #include "Crystal.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float4 uv : TEXCOORD0;
                    float3 normal : NORMAL;
                };

                struct v2f
                {
                    float4 uvGrab : TEXCOORD0;
                    float2 uvBump : TEXCOORD1;
                    float2 uvParallax : TEXCOORD2;
                    float3 normal : NORMAL0;
                    float3 viewDir : NORMAL1;
                    float4 pos : POSITION0;
                    float4 worldPos : POSITION1;
                    float angleFactor : Output;
                };

                float _DistortionAmount;
                float _DistortionInfluence;

                float _MinAngleFactor;

                sampler2D _BumpMap;
                float4 _BumpMap_ST;
                float _BumpScale;
                sampler2D _ParallaxMap;
                float4 _ParallaxMap_ST;
                float _Parallax;

                sampler2D _CrystalBackgroundTexture1;
                sampler2D _GrabTexture;

                fixed4 _RootPos;

                v2f vert(appdata v)
                {
                    v2f o;

                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uvGrab = ComputeGrabScreenPos(o.pos);

                    o.uvBump = TRANSFORM_TEX(v.uv, _BumpMap);
                    o.uvParallax = TRANSFORM_TEX(v.uv, _ParallaxMap);

                    //float3 normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_BumpMap), _BumpScale);

                    o.normal = v.normal;

                    o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                    float3 worldNormal = mul(unity_ObjectToWorld, float4(v.normal, 0.0)).xyz;
                    o.viewDir = normalize(WorldSpaceViewDir(v.vertex));

                    float angleFactor = 1 - abs(dot(o.viewDir, worldNormal)); // TODO: VERIFY WORLD VS OBJECT NORMALS?

                    o.angleFactor = smoothstep(0, 1, max(_MinAngleFactor, angleFactor) / (1 - _MinAngleFactor));

                    return o;
                }
                
                // BLENDING IS DONE HERE
                half4 frag(v2f i) : SV_Target
                {
                    i.uvBump += getParallaxOffset(_ParallaxMap, _Parallax, i.uvParallax, i.viewDir);

                    float3 normal = UnpackScaleNormal(tex2D(_BumpMap, i.uvBump), _BumpScale);
                    //normal.y = 0;

                    float crystalDepthFactor = smoothstep(0, 1, pow(depthAlongRay(i.worldPos, _RootPos), 0.1));

                    fixed4 distortedUv = i.uvGrab;
                    distortedUv.xy += crystalDepthFactor * normal.xy * i.angleFactor * _DistortionAmount;

                    half4 bgcolorDistorted = tex2Dproj(_CrystalBackgroundTexture1, distortedUv); // distorted only
                    half4 bgcolorCrystal = tex2Dproj(_GrabTexture, i.uvGrab); // with crystal

                    return 
                        lerp(
                            bgcolorCrystal,
                            bgcolorCrystal + hsvMaxBlendMode(bgcolorDistorted, bgcolorCrystal),
                            _DistortionInfluence * (_DistortionInfluence + Luminance(bgcolorDistorted) * (1 - _DistortionInfluence))
                        );

                    //return fixed4(crystalDepthFactor, crystalDepthFactor, crystalDepthFactor, 1);
                    //return bgcolor1 + 0.25;
                }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
