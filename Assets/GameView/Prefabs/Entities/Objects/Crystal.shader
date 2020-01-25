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
        _RootPos("Root Position", Vector) = (1,1,1,1)
        _EmissionFalloff("Distance Emission Falloff", Range(0.01, 10.0)) = 1
        _EmissionMultiplier("Distance Emission Multiplier", Range(0, 1)) = 0.5
        [HDR] _EmissionColor("Emission Color", Color) = (1,1,1,1)
        _EmissionMap("Emission Map", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        //Blend SrcAlpha OneMinusSrcAlpha
        //ZWrite On

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_BumpMap;
            float2 uv_ParallaxMap;
            float3 viewDir;
            float3 worldPos;
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
        half _EmissionMultiplier;
        sampler2D _EmissionMap;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        float2 getParallaxOffset(sampler2D Tex, float parallaxAmount, float2 UV, float3 viewDir) {
            float h = tex2D(Tex, UV).x;
            return ParallaxOffset(h, parallaxAmount, viewDir);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            IN.uv_BumpMap += getParallaxOffset(_ParallaxMap, _Parallax, IN.uv_ParallaxMap, IN.viewDir);
            o.Normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_BumpMap), _BumpScale);
            
            fixed4 c = _Color;
            o.Albedo = c.rgb;

            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

            float angleFactor = abs(dot(IN.viewDir, o.Normal));
            
            float4 emission = lerp(1, tex2D(_EmissionMap, IN.uv_BumpMap), _EmissionMultiplier);

            float3 localPos = IN.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
            float distanceFactor = min(1, distance(_RootPos.xyz, localPos) / _EmissionFalloff);

            o.Emission = _EmissionColor * smoothstep(0, 1, emission * angleFactor * (1 - distanceFactor));
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
