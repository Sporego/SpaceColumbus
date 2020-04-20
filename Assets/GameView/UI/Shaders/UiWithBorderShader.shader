Shader "UI/Default With Border"
{
    Properties
    {
        [PerRendererData] _SizeX("Size X", Float) = 200
        [PerRendererData] _SizeY("Size Y", Float) = 200

        [PerRendererData] _BorderSize("Border Size", Float) = 5
        [PerRendererData] _BorderThickness("Border Thickness", Float) = 5
        [PerRendererData] _BorderColor("Border Color", Color) = (1,1,1,1)
        [PerRendererData] _MaskTex("Mask Texture", 2D) = "black" {}
        [PerRendererData] _MaskWeight("Mask Weight", Range(0,1)) = 1

        [PerRendererData][Toggle] _ApplyDither("Apply Dither", Float) = 1
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
                #include "Assets/GameView/Shaders/constants.cginc"

                #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
                #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    float4 color    : COLOR;
                    float2 uv       : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 uv       : TEXCOORD0;
                    float4 worldPos : TEXCOORD1;
                    float4 border   : Output;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                fixed4 _Color;
                sampler2D _MainTex;
                float4 _MainTex_ST;
                fixed4 _TextureSampleAdd;
                float4 _ClipRect;

                float _BorderSize;
                float _BorderThickness;
                fixed4 _BorderColor;
                float _BorderOnly;

                float _SizeX;
                float _SizeY;

                sampler2D _MaskTex;
                float _MaskWeight;

                DITHER_CONSTANTS;

                v2f vert(appdata_t v)
                {
                    v2f OUT;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_TRANSFER_INSTANCE_ID(v, OUT); // necessary only if you want to access instanced properties in the fragment Shader.

                    OUT.worldPos = v.vertex;
                    OUT.vertex = UnityObjectToClipPos(OUT.worldPos);

                    OUT.uv = TRANSFORM_TEX(v.uv, _MainTex);

                    OUT.border.x = _BorderThickness / _SizeX;
                    OUT.border.y = _BorderThickness / _SizeY;
                    OUT.border.z = OUT.border.x + _BorderSize / _SizeX;
                    OUT.border.w = OUT.border.y + _BorderSize / _SizeY;

                    OUT.color = v.color * _Color;
                    return OUT;
                }

                fixed4 frag(v2f IN) : SV_Target
                {
                    UNITY_SETUP_INSTANCE_ID(IN);

                    float isBorderX =
                        max(0, IN.border.z - IN.uv.x) +
                        max(0, IN.uv.x - (1 - IN.border.z));
                    float isBorderY =
                        max(0, IN.border.w - IN.uv.y) +
                        max(0, IN.uv.y - (1 - IN.border.w));

                    float isBorder;
                    if (_BorderThickness == 0)
                    {
                        isBorderX = isBorderX == 0 ? 0 : 1;
                        isBorderY = isBorderY == 0 ? 0 : 1;
                        isBorder = isBorderX + isBorderY;
                    }
                    else
                    {
                        isBorderX /= IN.border.x;
                        isBorderY /= IN.border.y;
                        isBorder = sqrt(isBorderX * isBorderX + isBorderY * isBorderY);
                    }

                    fixed4 colorB = _BorderColor;
                    float maskB = tex2D(_MaskTex, IN.uv).x;
                    colorB.a = min(colorB.a, lerp(colorB.a, maskB, _MaskWeight)); // weighted with mask alpha, but not lower than color alpha

                    fixed4 color;
                    if (_BorderOnly)
                    {
                        color = fixed4(0, 0, 0, 0);
                    }
                    else
                    {
                        color = (tex2D(_MainTex, IN.uv) + _TextureSampleAdd) * IN.color;
                    }

                    color = lerp(color, colorB, smoothstep(0, 1, isBorder));

                    APPLY_DITHER(color);

                    #ifdef UNITY_UI_CLIP_RECT
                    color.a *= UnityGet2DClipping(IN.worldPos.xy, _ClipRect);
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