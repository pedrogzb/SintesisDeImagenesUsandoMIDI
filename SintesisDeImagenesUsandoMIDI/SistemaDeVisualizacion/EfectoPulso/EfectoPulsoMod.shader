Shader "PFG/EfectoPulsoMod"{
	Properties{
		_Color("Color",Color) = (1,1,1,1)
		_Color2("Color2",Color) = (1,1,1,1)
		_Alpha("Rotacion",Range(0,360)) = 0
		_Scale("Escala",Range(2,15)) = 5
		_Velocidad("Velocidad Modulacion",Range(0,10)) = 5
		_Grosor("Grosor",Range(0,1)) = 0.5
		_Difuminado("Difuminado",Range(0,1)) = 0.5
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
			
			#define pi 3.14159
			#define t _Time.y
			

			float4 _Color;
			float4 _Color2;
			float _Alpha;
			float _Scale;
			float _Velocidad;
			float _Grosor;
			float _Difuminado;

			struct MeshData {
				float3 positionOS : POSITION;
				float2 uv : TEXCOORD0;
				
			};
			struct Interpolators {
				float4 positionCS : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			Interpolators Vertex(MeshData v) {
				Interpolators o;
				VertexPositionInputs posnInputs = GetVertexPositionInputs(v.positionOS);

				o.positionCS = posnInputs.positionCS;
				o.uv = v.uv;
				return o;
			}
					
			float4 Fragment(Interpolators i) : SV_TARGET{
				i.uv -= 0.5;
				i.uv *= _Scale;
				i.uv.x = cos(radians(_Alpha)) * i.uv.x - sin(radians(_Alpha)) * i.uv.y;
				i.uv.y = sin(radians(_Alpha)) * i.uv.x + cos(radians(_Alpha)) * i.uv.y;
			
				float r = length(i.uv) + sin(_Velocidad * i.uv.x)- sin(_Velocidad * i.uv.y)-sin(t * _Velocidad);
				r = fmod(r, 1.5);
				float a = 0;
				float difuminado = 0.48*_Difuminado+0.02;
				float grosor = 0.48*_Grosor + 0.02;
				float3 col = (smoothstep(a, a+difuminado, r)- smoothstep(a + grosor + difuminado, a + grosor + 2*difuminado, r));
				col = lerp(_Color.xyz, _Color2.xyz, 1-col);
				return float4(col,1);
			}
			ENDHLSL
		}
	}
}