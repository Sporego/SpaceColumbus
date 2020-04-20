Shader "UI/Default With Line"
{
    Properties
    {
        [PerRendererData] _SizeX("Size X", Float) = 200
        [PerRendererData] _SizeY("Size Y", Float) = 200

        [PerRendererData] _LineDirection("Line Direction", Vector) = (1,1,1,1)
        [PerRendererData] _LineSize("Line Size", Float) = 5
        [PerRendererData] _LineThickness("Line Thickness", Float) = 5
        [PerRendererData] _LineColor("Line Color", Color) = (1,1,1,1)

        [PerRendererData][Toggle] _ApplyRepeat("Apply Repeat", Float) = 1
        [PerRendererData] _RepeatFrequence("Repeat Frequency", Float) = 1

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
                    float4 Line   : Output;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                fixed4 _Color;
                sampler2D _MainTex;
                float4 _MainTex_ST;
                fixed4 _TextureSampleAdd;
                float4 _ClipRect;

                float4 _LineDirection;
                float _LineSize;
                float _LineThickness;
                fixed4 _LineColor;

                float _RepeatFrequency;
                float _ApplyRepeat;

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

                    _LineDirection = max(FLOAT_MIN, _LineDirection);

                    OUT.Line.x = _SizeX / 2;
                    OUT.Line.y = _SizeY / 2;
                    OUT.Line.z =  -1.0 / _LineThickness;
                    OUT.Line.w = (_LineSize + _LineThickness) / _LineThickness;

                    OUT.color = v.color * _Color;
                    return OUT;
                }

                fixed4 frag(v2f IN) : SV_Target
                {
                    UNITY_SETUP_INSTANCE_ID(IN);

                    fixed4 color = fixed4(0,0,0,0);

                    if (_LineSize != 0 || _LineThickness != 0)
                    {
                        // distance to the line in pixel space
                        float d = abs(_LineDirection.x * ((IN.uv.x * _SizeX) - IN.Line.x) - ((IN.uv.y * _SizeY) - IN.Line.y) + _LineDirection.y) / _LineDirection.z;

                        if (_ApplyRepeat)
                            d = abs((d % (2 * _RepeatFrequency)) - _RepeatFrequency);

                        float isLine = _LineThickness == 0 ? d <= _LineSize : saturate(IN.Line.z * d + IN.Line.w);

                        float mask = tex2D(_MaskTex, IN.uv).x;

                        fixed4 colorL = _LineColor;
                        colorL.a = min(colorL.a, lerp(colorL.a, mask, _MaskWeight)); // weighted with mask alpha, but not lower than color alpha

                        color = lerp(IN.color, colorL, smoothstep(0, 1, isLine));

                        APPLY_DITHER(color);
                    }

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
