Shader "Custom/RegionShader"
{

    Properties
    {
        [Header(Global Material Parameters)]

        [HDR] _Color("Tint", Color) = (1,1,1,1)
        _BumpScale("Normal Scale", Float) = 1.0
        [Normal] _BumpMap("Normal Map", 2D) = "bump" {}

        [Header(Main Material Parameters)]

        [HDR] _Color1("Tint", Color) = (1,1,1,1)
        _Tex1("Albedo", 2D) = "white" {}
        _Glossiness1("Smoothness", Range(0.0, 1.0)) = 0.5
        _MetallicGlossMap1("Metallic", 2D) = "white" {}
        _BumpScale1("Normal Scale", Float) = 1.0
        [Normal] _BumpMap1("Normal Map", 2D) = "bump" {}
        _Parallax1("Height Scale", Range(0.005, 0.08)) = 0.02
        _ParallaxMap1("Height Map", 2D) = "black" {}
        _OcclusionStrength1("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionMap1("Occlusion", 2D) = "white" {}

        [Header(Slope Material Parameters)]

        [HDR] _Color2("Tint", Color) = (1,1,1,1)
        _Tex2("Albedo", 2D) = "white" {}
        _Glossiness2("Smoothness", Range(0.0, 1.0)) = 0.5
        _MetallicGlossMap2("Metallic", 2D) = "white" {}
        _BumpScale2("Normal Scale", Float) = 1.0
        [Normal] _BumpMap2("Normal Map", 2D) = "bump" {}
        _Parallax2("Height Scale", Range(0.005, 0.08)) = 0.02
        _ParallaxMap2("Height Map", 2D) = "black" {}
        _OcclusionStrength2("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionMap2("Occlusion", 2D) = "white" {}

        [Header(Slope Parameters)]

        _SlopeWeight("Slope Weight", Range(0.0001,1)) = 0.1
        _SlopeAmount("Slope Amount", Range(0.0001,1)) = 0.1
        _SlopeAmountVar("Slope Amount Var", Range(0.0001,1)) = 0.1
        _SlopeAmountRand("Slope Amount Random", Range(0.0001,1)) = 0.1
        _SlopeNormalInfluence("Slope Normal Influence", Range(0.0001, 0.9999)) = 1.0
        _SlopeDir("Slope Direction", Vector) = (0, 1, 0, 0)
        
        [Header(Noise Texture)]

        _Noise("Noise", 2D) = "white" {}
    }

        CGINCLUDE
        //@TODO: should this be pulled into a shader_feature, to be able to turn it off?
#define _GLOSSYENV 1
#define UNITY_SETUP_BRDF_INPUT MetallicSetup
        ENDCG

        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200
        ZWrite On

        CGPROGRAM

        #include "UnityPBSLighting.cginc"

        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

            // global tint
            float4 _Color;
            half _BumpScale;
            sampler2D _BumpMap;
            half _Glossiness;

            // main texture
            float4 _Color1;
            sampler2D _Tex1;
            half _Glossiness1;
            sampler2D _MetallicGlossMap1;
            half _BumpScale1;
            sampler2D _BumpMap1;
            half _Parallax1;
            sampler2D _ParallaxMap1;
            half _OcclusionStrength1;
            sampler2D _OcclusionMap1;

            // secondary texture
            float4 _Color2;
            sampler2D _Tex2;
            half _Glossiness2;
            sampler2D _MetallicGlossMap2;
            half _BumpScale2;
            sampler2D _BumpMap2;
            half _Parallax2;
            sampler2D _ParallaxMap2;
            half _OcclusionStrength2;
            sampler2D _OcclusionMap2;

            // slope paramaters
            float4 _SlopeDir;
            half _SlopeAmount;
            half _SlopeWeight;
            half _SlopeAmountVar;
            half _SlopeAmountRand;
            half _SlopeNormalInfluence;

            sampler2D _Noise;

            struct Input
            {
                float2 uv_Tex1;
                float2 uv_Tex2;
                float2 uv_BumpMap;
                float2 uv_Noise;
                float3 viewDir;
                float3 worldNormal;
                INTERNAL_DATA
            };

            // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
            // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
            // #pragma instancing_options assumeuniformscaling
            UNITY_INSTANCING_BUFFER_START(Props)
                // put more per-instance properties here
            UNITY_INSTANCING_BUFFER_END(Props)

                float getOcclusion(sampler2D Tex, float occlusionStrength, float2 UV) {
                    float occ = tex2D(Tex, UV);
                    return LerpOneTo(occ, occlusionStrength);
                }

                //void blendMaterials(Input IN, inout SurfaceOutputStandard o, float alpha)
                //{
                //    float4 color1 = _Color1 * tex2D(_Tex1, IN.uv_Tex1);
                //    float4 color2 = _Color2 * tex2D(_Tex2, IN.uv_Tex2);

                //    float occlusion1 = getOcclusion(_OcclusionMap1, _OcclusionStrength1, IN.uv_Tex1);
                //    float occlusion2 = getOcclusion(_OcclusionMap2, _OcclusionStrength2, IN.uv_Tex2);
                //    o.Occlusion = lerp(occlusion1, occlusion2, alpha);

                //    o.Albedo = lerp(color1, color2, alpha) * o.Occlusion;

                //    float metallic1 = tex2D(_MetallicGlossMap1, IN.uv_Tex1);
                //    float metallic2 = tex2D(_MetallicGlossMap2, IN.uv_Tex2);
                //    o.Metallic = lerp(metallic1, metallic2, alpha);

                //    o.Smoothness = lerp(_Glossiness1, _Glossiness2, alpha);

                //    float3 normal1 = UnpackScaleNormal(tex2D(_BumpMap1, IN.uv_Tex1), _BumpScale1);
                //    float3 normal2 = UnpackScaleNormal(tex2D(_BumpMap2, IN.uv_Tex2), _BumpScale2);

                //    o.Normal = lerp(normal1, normal2, alpha); // pick the highest values to maintain the important components of all
                //}

                SurfaceOutputStandard applySlopeTexture(Input IN, inout SurfaceOutputStandard o, float3 worldNormal)
                {
                    float4 noise = tex2D(_Noise, IN.uv_Noise);

                    float slopeRatio = abs(dot(worldNormal, normalize(_SlopeDir).xyz)); // 0 - 1, 1: perfectly in line
                    slopeRatio = 1 - slopeRatio;

                    float slopeAmount = 1 - _SlopeAmount;
                    //slopeRatio = clamp(1 + (slopeRatio - slopeAmount) / _SlopeAmountVar, 0, _SlopeWeight);
                    slopeRatio = clamp(1 + (slopeRatio - slopeAmount * (_SlopeAmountRand) / noise.x) / (_SlopeAmountVar * noise.y ), 0, _SlopeWeight);
                    //slopeRatio = clamp(1 + (slopeRatio - slopeAmount * noise.y) / (_SlopeAmountVar * noise.z), 0, _SlopeWeight);
                    slopeRatio = smoothstep(0, 1, slopeRatio);

                    float alpha = slopeRatio;

                    // normals
                    float3 normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_BumpMap), _BumpScale);
                    float3 normal1 = UnpackScaleNormal(tex2D(_BumpMap1, IN.uv_Tex1), _BumpScale1);
                    float3 normal2 = UnpackScaleNormal(tex2D(_BumpMap2, IN.uv_Tex2), _BumpScale2);
                    o.Normal = normal + lerp(normal1, normal2, alpha * _SlopeNormalInfluence);

                    // occlusion
                    float occlusion1 = getOcclusion(_OcclusionMap1, _OcclusionStrength1, IN.uv_Tex1);
                    float occlusion2 = getOcclusion(_OcclusionMap2, _OcclusionStrength2, IN.uv_Tex2);
                    o.Occlusion = lerp(occlusion1, occlusion2, alpha);

                    // albedo
                    float4 color1 = _Color1 * tex2D(_Tex1, IN.uv_Tex1);
                    float4 color2 = _Color2 * tex2D(_Tex2, IN.uv_Tex2);
                    o.Albedo = lerp(color1, color2, alpha) * o.Occlusion;

                    // metalic and smoothness
                    float metallic1 = tex2D(_MetallicGlossMap1, IN.uv_Tex1);
                    float metallic2 = tex2D(_MetallicGlossMap2, IN.uv_Tex2);
                    o.Metallic = lerp(metallic1, metallic2, alpha);
                    o.Smoothness = lerp(_Glossiness1, _Glossiness2, alpha);

                    return o;
                }

                float2 getParallaxOffset(sampler2D Tex, float parallaxAmount, float2 UV, float3 viewDir) {
                    float h = tex2D(Tex, UV).x;
                    return ParallaxOffset(h, parallaxAmount, viewDir);
                }

                void surf(Input IN, inout SurfaceOutputStandard o)
                {
                    IN.uv_Tex1 += getParallaxOffset(_ParallaxMap1, _Parallax1, IN.uv_Tex1, IN.viewDir);
                    IN.uv_Tex2 += getParallaxOffset(_ParallaxMap2, _Parallax2, IN.uv_Tex2, IN.viewDir);

                    // get world normal vector
                    float3 worldNormal = WorldNormalVector(IN, o.Normal);

                    //blendMaterials(IN, o, alpha);

                    ////float4 noise = tex2D(_Noise, IN.uv_Noise);

                    o = applySlopeTexture(IN, o, worldNormal);

                    o.Albedo *= _Color;
                    o.Alpha = _Color.a;
                }
            ENDCG


    }
        FallBack "Diffuse"
                //CustomEditor "StandardShaderGUI"
}
