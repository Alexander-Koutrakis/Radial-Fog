Shader "Custom/RadialFog"
{
   Properties
    {
        _MainTex ("Texture", 2D) = "white"
    }
   SubShader {

		Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}

		Cull Off
		ZWrite Off

		Pass {

			 Name "RadialFogPass"

			 CGPROGRAM

			#pragma vertex VertexProgram
			#pragma fragment frag

			#pragma multi_compile FOG_LINEAR FOG_EXPONENTIAL FOG_EXPONENTIAL_SQUARED
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;
			float4 _Color;
			float3 _FrustumCorners[4];
			float _Density;
			float _Start;
			float _End;
			
			struct VertexData {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Interpolators {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 ray : TEXCOORD1;
			};

			half ComputeFogFactor(float coord)
			{
				float fog = 0.0;
				#if FOG_LINEAR
					// factor = (end-z)/(end-start) = z * (-1/(end-start)) + (end/(end-start))
					fog=(_End-coord)/(_End-_Start);
				#elif FOG_EXPONENTIAL
					// factor = exp(-density*z)
					fog = _Density * coord;
					fog = exp2(-fog);
				#else // FOG_EXP2
					// factor = exp(-(density*z)^2)
					fog = _Density * coord;
					fog = exp2(-fog * fog);
				#endif 
				return saturate(fog);
			}


			Interpolators VertexProgram (VertexData v) {
				Interpolators i;
				i.pos = UnityObjectToClipPos(v.vertex);
				i.uv = v.uv;
				i.ray = _FrustumCorners[v.uv.x + 2 * v.uv.y];
				return i;
			}

			float4 frag (Interpolators i) : SV_Target {
				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
				depth = Linear01Depth(depth);
				
				//float3 ray=_FrustumCorners[i.uv.x + 2 * i.uv.y];
				float viewDistance = length(i.ray * depth);
				viewDistance-= _ProjectionParams.y;
				half fog=ComputeFogFactor(max(0.0,viewDistance));

				if (depth > 0.9999) {
					fog = 1;
				}
				
				float3 sourceColor = tex2D(_MainTex, i.uv).rgb;
				float3 foggedColor =lerp(_Color.rgb,sourceColor,fog);
				return float4(foggedColor, 1);
			}

			ENDCG
		}
	}
	}