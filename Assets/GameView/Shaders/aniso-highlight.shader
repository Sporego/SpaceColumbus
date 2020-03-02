Shader "Exploration/Hair Soft Edge Surface"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Diffuse (RGB) Alpha (A)", 2D) = "white" {}
        _SpecularTex("Specular (R) Gloss (G) Null (B)", 2D) = "gray" {}
        _BumpMap("Normal (Normal)", 2D) = "bump" {}
        _AnisoTex("Anisotropic Direction (RGB)", 2D) = "bump" {}
        _AnisoOffset("Anisotropic Highlight Offset", Range(-0.5,0.5)) = -0.2
        _Cutoff("Alpha Cut-Off Threshold", Range(0,1)) = 0.5
        _Fresnel("Fresnel Value", Float) = 0.028
    }

        SubShader{
            Tags { "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" }

            CGPROGRAM
                #pragma surface surf Lambert fullforwardshadows exclude_path:prepass
                #pragma target 3.0

                struct Input
                {
                    float2 uv_MainTex;
                };

                sampler2D _MainTex;
                sampler2D _SpecularTex;
                sampler2D _BumpMap;
                sampler2D _AnisoDirection;

                void surf(Input IN, inout SurfaceOutput o)
                {
                    fixed4 albedo = tex2D(_MainTex, IN.uv_MainTex);
                    o.Albedo = albedo.rgb;
                    o.Alpha = albedo.a;
                    //o.AnisoDir = tex2D(_AnisoDirection, IN.uv_MainTex);
                    o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
                    o.Specular = tex2D(_SpecularTex, IN.uv_MainTex).rgb;
                }
            ENDCG

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
                #pragma surface surf Lambert fullforwardshadows exclude_path:prepass noforwardadd
                #pragma target 3.0

                struct Input
                {
                    float2 uv_MainTex;
                };

                sampler2D _MainTex, _SpecularTex, _BumpMap, _AnisoDirection;

                void surf(Input IN, inout SurfaceOutput o)
                {
                    fixed4 albedo = tex2D(_MainTex, IN.uv_MainTex);
                    o.Albedo = albedo.rgb;
                    o.Alpha = albedo.a;
                    //o.AnisoDir = tex2D(_AnisoDirection, IN.uv_MainTex);
                    o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
                    o.Specular = tex2D(_SpecularTex, IN.uv_MainTex).rgb;
                }
            ENDCG
        }
            FallBack "Transparent/Cutout/VertexLit"
}


//
///*
//Alpha tested pass.
//
//Alpha blended pass w/ ZWrite off and alphatest greater than _Cutoff.
//
//Anisotropic highlight.
//*/
//
//Shader "Exploration/Hair Soft Edge Surface" {
//    Properties{
//        _Color("Main Color", Color) = (1,1,1,1)
//        _MainTex("Diffuse (RGB) Alpha (A)", 2D) = "white" {}
//        _SpecularTex("Specular (R) Gloss (G) Null (B)", 2D) = "gray" {}
//        _BumpMap("Normal (Normal)", 2D) = "bump" {}
//        _AnisoTex("Anisotropic Direction (RGB)", 2D) = "bump" {}
//        _AnisoOffset("Anisotropic Highlight Offset", Range(-0.5,0.5)) = -0.2
//        _Cutoff("Alpha Cut-Off Threshold", Range(0,1)) = 0.5
//        _Fresnel("Fresnel Value", Float) = 0.028
//    }
//
//        SubShader{
//            Tags { "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" }
//
//            CGPROGRAM
//                #include "ExplorationLighting.cginc"
//                #pragma surface surf ExplorationSoftHairFirst fullforwardshadows exclude_path:prepass
//                #pragma target 3.0
//
//                struct Input
//                {
//                    float2 uv_MainTex;
//                };
//
//                sampler2D _MainTex, _SpecularTex, _BumpMap, _AnisoDirection;
//
//                void surf(Input IN, inout SurfaceOutputCharacter o)
//                {
//                    fixed4 albedo = tex2D(_MainTex, IN.uv_MainTex);
//                    o.Albedo = albedo.rgb;
//                    o.Alpha = albedo.a;
//                    o.AnisoDir = tex2D(_AnisoDirection, IN.uv_MainTex);
//                    o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
//                    o.Specular = tex2D(_SpecularTex, IN.uv_MainTex).rgb;
//                }
//            ENDCG
//
//            Blend SrcAlpha OneMinusSrcAlpha
//            ZWrite Off
//
//            CGPROGRAM
//                #include "ExplorationLighting.cginc"
//                #pragma surface surf ExplorationSoftHairSecond fullforwardshadows exclude_path:prepass noforwardadd
//                #pragma target 3.0
//
//                struct Input
//                {
//                    float2 uv_MainTex;
//                };
//
//                sampler2D _MainTex, _SpecularTex, _BumpMap, _AnisoDirection;
//
//                void surf(Input IN, inout SurfaceOutputCharacter o)
//                {
//                    fixed4 albedo = tex2D(_MainTex, IN.uv_MainTex);
//                    o.Albedo = albedo.rgb;
//                    o.Alpha = albedo.a;
//                    o.AnisoDir = tex2D(_AnisoDirection, IN.uv_MainTex);
//                    o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
//                    o.Specular = tex2D(_SpecularTex, IN.uv_MainTex).rgb;
//                }
//            ENDCG
//        }
//            FallBack "Transparent/Cutout/VertexLit"
//}
