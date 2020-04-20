// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "UI/Background Gaussian Blur"
{
    Properties
    {
        [PerRendererData] _GausBlurSigma("Gaussian Blur Sigma", Range(0.1, 10.0)) = 3
        [PerRendererData] _GausBlurSize("Gaussian Blur Size", Range(0, 5.0)) = 1
        [PerRendererData] _GausBlurSamples("Gaussian Blur Samples", Range(1, 50.0)) = 5

        [Toggle] _UseMainTexColor("Use Texture Color", Float) = 0

        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)

        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _ColorMask("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }

            Stencil
            {
                Ref[_Stencil]
                Comp[_StencilComp]
                Pass[_StencilOp]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
            }

            Cull Off
            Lighting Off
            ZWrite Off
            ZTest[unity_GUIZTestMode]
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask[_ColorMask]

            GrabPass
            {
                "_GRAB_TEX_1"
            }
            
            Pass
            {
                Name "First Blur Pass"

            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 3.0
                #pragma exclude_renderers d3d11_9x

                #include "UnityCG.cginc"
                #include "UnityUI.cginc"
                #include "UnityStandardUtils.cginc"

                #define BLUR_PASS_1
                #define BLUR_SHADER_FUNCS // remove this line, if want to define vert and frag functions
                #define _GRAB_TEX _GRAB_TEX_1 // must match texture name in most recent grabpass
                #include "Assets/GameView/Shaders/fast-gaussian-blur.cginc"
            ENDCG
            }

            GrabPass
            {
                "_GRAB_TEX_2"
            }

            Pass
            {
                Name "Second Blur Pass"

            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 3.0
                #pragma exclude_renderers d3d11_9x

                #include "UnityCG.cginc"
                #include "UnityUI.cginc"
                #include "UnityStandardUtils.cginc"

                #define BLUR_PASS_2
                #define BLUR_SHADER_FUNCS // remove this line, if want to define vert and frag functions
                #define _GRAB_TEX _GRAB_TEX_2 // must match texture name in most recent grabpass
                #include "Assets/GameView/Shaders/fast-gaussian-blur.cginc"
            ENDCG
            }
        }
}