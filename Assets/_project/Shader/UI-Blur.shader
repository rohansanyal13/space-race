Shader "UI/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _BlurSize ("Blur Size", Range(0,10)) = 2.0
        
        // Required for UI.Mask
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }
    
    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp] 
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        GrabPass { "_GrabTexture" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 grabPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            sampler2D _GrabTexture;
            float4 _Color;
            float _BlurSize;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                o.color = v.color * _Color;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float blur = _BlurSize / 100.0;
                
                fixed4 col = fixed4(0,0,0,0);
                float weight_total = 0;
                
                // Sample the grab texture using offsets to create a simple blur
                for(float x = -4; x <= 4; x++)
                {
                    for(float y = -4; y <= 4; y++)
                    {
                        float weight = 1.0 - abs(x) * 0.125 - abs(y) * 0.125;
                        float2 offset = float2(x * blur, y * blur);
                        col += tex2Dproj(_GrabTexture, i.grabPos + float4(offset, 0, 0)) * weight;
                        weight_total += weight;
                    }
                }
                
                col /= weight_total;
                col *= i.color;
                return col;
            }
            ENDCG
        }
    }
}