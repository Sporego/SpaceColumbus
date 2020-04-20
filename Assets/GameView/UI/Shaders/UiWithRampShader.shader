
Shader "UI/Default With Ramp"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}

        [PerRendererData] _SizeX("Size X", Float) = 200
        [PerRendererData] _SizeY("Size Y", Float) = 200

        [PerRendererData] _Color1("Tint 1", Color) = (1,1,1,1)
        [PerRendererData] _Color2("Tint 2", Color) = (1,1,1,1)
        [PerRendererData] _RampPower("Ramp Power", Range(0.1, 10)) = 1
        [PerRendererData] _RampScale("Ramp Scale", Range(0.01, 10)) = 1
        [PerRendererData] _RampDirection("Ramp Direction", Vector) = (1,1,1,1)
        [PerRendererData, Toggle] _Radial("Radial", float) = 1
        [PerRendererData, Toggle] _Invert("Invert ramp", float) = 1
        [PerRendererData, Toggle] _ApplyAlpha("Apply alpha", float) = 1

        [PerRendererData, Toggle] _ApplyDither("Apply Dither", Float) = 1
        [PerRendererData] _DitherStrength("Dither Strength", Range(0, 1)) = 0.001

        [PerRendererData] _SrcBlend("__src", Float) = 1.0
        [PerRendererData] _DstBlend("__dst", Float) = 0.0

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
            Blend[_SrcBlend][_DstBlend]
            ColorMask[_ColorMask]

            Pass
            {
                Name "Default"
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0

                #include "UnityCG.cginc"
                #include "UnityUI.cginc"
                #include "Assets/GameView/Shaders/dithering.cginc"

                #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
                #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    float4 color    : COLOR;
                    float2 uv : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 uv  : TEXCOORD0;
                    float4 worldPosition : TEXCOORD1;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _MainTex_TexelSize;
                fixed4 _TextureSampleAdd;
                float4 _ClipRect;

                float _SizeX;
                float _SizeY;

                fixed4 _Color1;
                fixed4 _Color2;
                float2 _RampDirection;
                float _RampPower;
                float _RampScale;
                float _Invert;
                float _Radial;
                float _ApplyAlpha;

                float _ApplyDither;
                float _DitherStrength;

                v2f vert(appdata_t v)
                {
                    v2f OUT;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_TRANSFER_INSTANCE_ID(v, OUT); // necessary only if you want to access instanced properties in the fragment Shader.

                    OUT.worldPosition = v.vertex;
                    OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                    OUT.uv = TRANSFORM_TEX(v.uv, _MainTex);

                    OUT.color = v.color;
                    return OUT;
                }

                fixed4 frag(v2f IN) : SV_Target
                {
                    UNITY_SETUP_INSTANCE_ID(IN);

                    fixed4 color = (tex2D(_MainTex, IN.uv) + _TextureSampleAdd) * IN.color;

                    float ramp;
                    if (_Radial)
                    {
                        ramp = (0.5 - IN.uv.x) * (0.5 - IN.uv.x) + (0.5 - IN.uv.y) * (0.5 - IN.uv.y);
                        ramp = sqrt(4 * ramp);
                    } 
                    else 
                    {
                        ramp = (IN.uv.x * _RampDirection.x + IN.uv.y * _RampDirection.y); // TODO: scaling with _SizeX, _SizeY; maybe use _ScreenParams
                    }

                    ramp = pow(ramp, _RampPower) * _RampScale; // modify ramp paramater

                    if (_Invert)
                        ramp = 1 - ramp;

                    color *= lerp(_Color1, _Color2, ramp);

                    if (_ApplyAlpha)
                        color.a *= ramp;

                    APPLY_DITHER(color);

                    #ifdef UNITY_UI_CLIP_RECT
                    color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                    #endif

                    #ifdef UNITY_UI_ALPHACLIP
                    clip(color.a - 0.001);
                    #endif

                    return color;
                }
            ENDCG
            }
        }
}