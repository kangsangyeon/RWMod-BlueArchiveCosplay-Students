Shader "Custom/Particle Additive Glow Fixed"
{
    Properties
    {
        _MainTex ("Particle Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _GlowColor ("Glow Color", Color) = (1,1,1,1)
        _GlowStrength ("Glow Strength", Range(0,10)) = 2.0
        _GlowSize ("Glow Size", Range(0.1, 5)) = 1.5

        // Hidden properties
        [HideInInspector] _Mode ("__mode", Float) = 0.0
        [HideInInspector] _ColorMode ("__colormode", Float) = 0.0
        [HideInInspector] _FlipbookMode ("__flipbookmode", Float) = 0.0
        [HideInInspector] _LightingEnabled ("__lightingenabled", Float) = 0.0
        [HideInInspector] _DistortionEnabled ("__distortionenabled", Float) = 0.0
        [HideInInspector] _EmissionEnabled ("__emissionenabled", Float) = 0.0
        [HideInInspector] _BlendOp ("__blendop", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0
        [HideInInspector] _Cull ("__cull", Float) = 2.0
        [HideInInspector] _SoftParticlesEnabled ("__softparticlesenabled", Float) = 0.0
        [HideInInspector] _CameraFadingEnabled ("__camerafadingenabled", Float) = 0.0
        [HideInInspector] _SoftParticleFadeParams ("__softparticlefadeparams", Vector) = (0,0,0,0)
        [HideInInspector] _CameraFadeParams ("__camerafadeparams", Vector) = (0,0,0,0)
        [HideInInspector] _ColorAddSubDiff ("__coloraddsubdiff", Vector) = (0,0,0,0)
        [HideInInspector] _DistortionStrengthScaled ("__distortionstrengthscaled", Float) = 0.0
    }

    Category
    {
        SubShader
        {
            Tags
            {
                "Queue"="Transparent"
                "RenderType"="Transparent"
                "IgnoreProjector"="True"
                "PreviewType"="Plane"
            }

            BlendOp [_BlendOp]
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            Cull [_Cull]
            ColorMask RGB

            Pass
            {
                Tags
                {
                    "LightMode" = "Always"
                }
                CGPROGRAM
                #pragma target 2.5
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_particles
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _Color;
                float4 _GlowColor;
                float _GlowStrength;
                float _GlowSize;

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float4 texcoord : TEXCOORD0;
                    float4 color : COLOR;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    fixed4 color : COLOR;
                };

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                    o.color = v.color * _Color;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float2 center = float2(0.5, 0.5);
                    float dist = distance(i.uv, center) * _GlowSize;
                    float glow = pow(saturate(1.0 - dist), 2.0);

                    fixed4 tex = tex2D(_MainTex, i.uv);
                    fixed4 baseCol = tex * i.color;
                    fixed4 glowCol = _GlowColor * glow * _GlowStrength * tex.a;

                    // return baseCol + glowCol;
                    return _Color;
                }
                ENDCG
            }
        }
    }

}