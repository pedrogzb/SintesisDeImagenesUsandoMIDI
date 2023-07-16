Shader "PFG/ModulacionVideo"
{	Properties{
		[MainTexture] _ColorMap("Color", 2D) = "white" {}
		_Color("Color",Color) = (1,1,1,1)
		_Filt("Filtrado",Range(0,10)) = 1
		_Selector("Selector de Efecto",Integer) = 0
	}
SubShader{
	Tags{"RenderPipeline" = "UniversalPipeline"}
	Pass {
		Name "ForwardLit"
		Tags{"LightMode" = "UniversalForward"}

		HLSLPROGRAM
		#pragma vertex Vertex
		#pragma fragment Fragment

		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

		TEXTURE2D(_ColorMap); SAMPLER(sampler_ColorMap);
		float4 _ColorMap_ST;
		float4 _Color;
		float _Filt;
		int _Selector;


		struct MeshData {
			float3 positionOS : POSITION;
			float3 normalOS : NORMAL;
			float2 uv : TEXCOORD0;

		};
		struct Interpolators {
			float4 positionCS : SV_POSITION;
			float2 uv : TEXCOORD0;
			float3 normalWS : TEXCOORD1;
		};
		Interpolators Vertex(MeshData v) {
			Interpolators o;
			VertexPositionInputs posnInputs = GetVertexPositionInputs(v.positionOS);
			VertexNormalInputs normalInputs = GetVertexNormalInputs(v.normalOS);

			o.positionCS = posnInputs.positionCS;
			o.uv = v.uv;
			o.normalWS = normalInputs.normalWS;
			return o;
		}
		float3 aberracionCromatica(float2 uv, float2 offset)
		{
			float colorSampleR = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, uv + offset).r;
			float colorSampleG = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, uv).g;
			float colorSampleB = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, uv - offset).b;
			return float3(colorSampleR, colorSampleG, colorSampleB);
		}

		float4 Fragment(Interpolators i) : SV_TARGET{
			float LuzDifusa = saturate(dot(i.normalWS,normalize(_MainLightPosition.xyz)));
			/*								Efecto1							*/
			if (_Selector == 0) {
				float2 offset = float2(0.03, -0.03) * _Filt - float2(sin(-_Time.y * 6) * 0.1, cos(-_Time.y * 6) * 0.1);
				float2 uv = i.uv;
				float3 col = aberracionCromatica(uv, offset);
				return float4(col* LuzDifusa, 1);
			}
			/*								Efecto2							*/
			if (_Selector == 1) {
				float3 colorSample = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, i.uv).rgb;
				float2 uv = (i.uv - 0.5)*50;
				float3 col = (1-pow(0.5 * sin(uv.y+_Time.y*10+ (0.5-_Filt)*sin(uv.y*_Time.y*25*_Filt)) + 0.5,10)+0.3) * _Color.xyz;
				col *=saturate(dot(i.normalWS, normalize(_MainLightPosition.xyz)));
				colorSample *= col;
				return float4(colorSample* LuzDifusa,1) ;
			}
			/*								Efecto3							*/
			if (_Selector == 2) {
				float offset = 0.03* sqrt(_Filt);
				float2 uv;
				float3 maxCol = float3(0, 0, 0);
				float numPasos = 10;
				for(int a = -numPasos; a<numPasos+1; a++){
					for (int b = -numPasos; b < numPasos + 1; b++){
						uv = i.uv + float2(a / numPasos, b / numPasos) * offset;
						maxCol = max(SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, uv).rgb, maxCol);
					}
				}
				return float4(maxCol* LuzDifusa, 1);
			}
			/*								Efecto4							*/
			if (_Selector == 3) {
				float offset = 0.001*_Filt;
				float3 pos11 = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, i.uv + float2(-offset, offset)).rgb;
				float3 pos12 = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, i.uv + float2(	     0, offset)).rgb;
				float3 pos13 = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, i.uv + float2( offset, offset)).rgb;
				float3 pos21 = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, i.uv + float2(-offset,	     0)).rgb;
				float3 pos22 = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, i.uv).rgb;
				float3 pos23 = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, i.uv + float2( offset,	     0)).rgb;
				float3 pos31 = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, i.uv + float2(-offset, offset)).rgb;
				float3 pos32 = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, i.uv + float2(      0,-offset)).rgb;
				float3 pos33 = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, i.uv + float2( offset,-offset)).rgb;
				float filt = 0.4;
				float3 dif_x = ((pos11 +  2*pos21 + pos31) - (pos13 +  2*pos23 + pos13) + filt * pos22);
				float3 dif_y = ((pos11 +  2*pos12 + pos13) - (pos31 +  2*pos32 + pos33) + filt * pos22);
				float3 mag = sqrt(dif_x * dif_x + dif_y * dif_y);
				bool umbral = (mag.r * 0.299 + mag.g * 0.587 + mag.b * 0.114) > 0.75;
				float3 col = (umbral) ? _Color.xyz : pos22;
				return float4(col* LuzDifusa, 1);
			}
			/*								Error							*/
			return float4(255, 0, 255, 1);
		}
		ENDHLSL
	}
}
}
