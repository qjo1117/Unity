Shader "Unlit/Decal"
{
    Properties
    {
        [HDR] _Color("Tint", Color) = (0,0,0,1)
        _MainTex("Texture",2D) = "while" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent-400" "DisableBatching" = "True" }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 position : POSITION;
                // 투영값
                float4 screenPos : TEXCOORD0;
                // 붙여질 좌표
                float3 ray : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color;
            sampler2D_float _CameraDepthTexture;

            v2f vert (appdata input)
            {
                v2f output;
                float3 worldPos = mul(unity_ObjectToWorld, input.vertex);

                output.position = UnityWorldToClipPos(worldPos);
                output.ray = worldPos - _WorldSpaceCameraPos;
                output.screenPos = ComputeScreenPos(output.position);

                return output;
            }

            float3 GetProjectedObjectPos(float2 screenPos, float3 worldRay)
            {
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,screenPos);
                depth = Linear01Depth(depth) * _ProjectionParams.z;
                worldRay = normalize(worldRay);

                worldRay /= dot(worldRay, - UNITY_MATRIX_V[2].xyz);

                float3 worldPos = _WorldSpaceCameraPos + worldRay * depth;
                float3 objectPos = mul(unity_WorldToObject, float4(worldPos,1)).xyz;

                clip(0.5 - abs(objectPos));
                objectPos += 0.5;
                return objectPos;

            }

            float4 frag (v2f input) : SV_Target
            {
                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                float2 uv = GetProjectedObjectPos(screenUV, input.ray).xz;

                float4 result = tex2D(_MainTex, uv);
                result *= _Color;
                return result;

            }
            ENDCG
        }
    }
}
