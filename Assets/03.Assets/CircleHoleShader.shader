Shader "UI/CircleHole"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,0.7)
        _Center ("Center", Vector) = (0.5,0.5,0,0)
        _Radius ("Radius", Float) = 0.1
        _Softness ("Softness", Float) = 0.02
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _Color;
            float4 _Center;
            float _Radius;
            float _Softness;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 center = _Center.xy;

                // 화면 비율 보정
                float aspect = _ScreenParams.x / _ScreenParams.y;

                uv.x *= aspect;
                center.x *= aspect;

                // 거리 계산
                float dist = distance(uv, center);

                // 원 안은 투명
                float alpha = smoothstep(
                    _Radius,
                    _Radius + _Softness,
                    dist
                );

                return fixed4(
                    _Color.rgb,
                    _Color.a * alpha
                );
            }

            ENDCG
        }
    }
}