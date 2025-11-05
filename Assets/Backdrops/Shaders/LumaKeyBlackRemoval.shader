Shader "Custom/LumaKeyBlackRemoval"
{
    Properties{
        _MainTex("Video Texture", 2D) = "white" {}   // <-- keep this name for VideoPlayer
        _Cutoff("Luma Threshold", Range(0,1)) = 0.15 // raise to remove more black
        _Smooth("Feather", Range(0,0.5)) = 0.08      // soft edge
        _Despill("Shadow Lift", Range(0,0.3)) = 0.04 // lifts near-black fringe
    }
    SubShader{
        Tags{ "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha

        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Cutoff, _Smooth, _Despill;

            struct appdata { float4 vertex: POSITION; float2 uv: TEXCOORD0; };
            struct v2f     { float4 pos: SV_POSITION; float2 uv: TEXCOORD0; };

            v2f vert(appdata v){
                v2f o; o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i): SV_Target{
                fixed4 col = tex2D(_MainTex, i.uv);

                // Perceptual luminance
                float luma = dot(col.rgb, float3(0.299, 0.587, 0.114));

                // Alpha from luma: below cutoff -> transparent
                float a = smoothstep(_Cutoff, _Cutoff + _Smooth, luma);

                // Lightly lift very dark colors near the edge to reduce black fringe
                col.rgb = max(col.rgb, _Despill);

                col.a = a;
                return col;
            }
            ENDCG
        }
    }
}
