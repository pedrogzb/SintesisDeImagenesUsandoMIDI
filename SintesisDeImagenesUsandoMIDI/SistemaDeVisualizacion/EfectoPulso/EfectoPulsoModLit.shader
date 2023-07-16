Shader "PFG/EfectoPulsoModLit"{
	Properties{
		_Color("Color",Color) = (1,1,1,1)
		_Color2("Color2",Color) = (1,1,1,1)
		_Alpha("Rotacion",Range(0,360)) = 0
		_Scale("Escala",Range(2,15)) = 2
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
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float2 uv : TEXCOORD0;
				
			};
			struct Interpolators {
				float4 positionCS : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
				float3 tangentWS : TEXCOORD2;
			};
			Interpolators Vertex(MeshData v) {
				Interpolators o;
				VertexPositionInputs posnInputs = GetVertexPositionInputs(v.positionOS);
				VertexNormalInputs normalInputs = GetVertexNormalInputs(v.normalOS,v.tangentOS);
				
				o.positionCS = posnInputs.positionCS;
				o.uv = v.uv;
				o.normalWS = normalInputs.normalWS;
				o.tangentWS = normalInputs.tangentWS;
				return o;
			}
			float2 rotar(float2 uv,float angulo)
			{
				float2 rotado;
				rotado.x = cos(angulo) * uv.x - sin(angulo) * uv.y;
				rotado.y = sin(angulo) * uv.x + cos(angulo) * uv.y;
				return rotado;
			}
			float CrearTextura(float2 uv){
				float r = length(uv) + sin(_Velocidad * uv.x) - sin(_Velocidad * uv.y) - sin(t * _Velocidad);
				r = fmod(r, 1.5);
				float a = 0;
				float difuminado = 0.48 * _Difuminado + 0.02;
				float grosor = 0.48 * _Grosor + 0.02;
				float col = (smoothstep(a, a + difuminado, r) - smoothstep(a + grosor + difuminado, a + grosor + 2 * difuminado, r));
				return col;
			}
			float3 GetNormalTextura(float2 uv, float textura, float3 normal, float3 tangente) {
				float3 bitangente = cross(normal, tangente);
				
				float e = 1e-2;

				float dx = textura - CrearTextura(float2(uv.x - e, uv.y));
				float dy = textura - CrearTextura(float2(uv.x, uv.y - e));
				
				float escala = pow(5,2);
				
				float3 va = float3(1, 0, dx*escala);
				float3 vb = float3(0, 1, dy*escala);
				float3 resul = normalize(cross(va, vb));
				resul = (normal     * dot(resul, float3(0, 0, 1))  +
						 tangente   * dot(resul, float3(1, 0, 0))  +
						 bitangente * dot(resul, float3(0, 1, 0)))/3;
				return normalize(resul);
			}
			float4 Fragment(Interpolators i) : SV_TARGET{
				
				i.uv -= 0.5;
				i.uv *= _Scale;
				i.uv = rotar(i.uv, radians(_Alpha));

				float tex = CrearTextura(i.uv);
				float3 col = lerp(_Color2.xyz, _Color.xyz,  tex);

				float3 normalTex = GetNormalTextura(i.uv, tex, i.normalWS, i.tangentWS);
				float LuzTextura = saturate(dot(normalTex, normalize(_MainLightPosition.xyz)));

				float LuzDifusa = saturate(dot(i.normalWS, normalize(_MainLightPosition.xyz)));
				
				col = col * LuzTextura * LuzDifusa;
				return float4(col,1);
			
		}
			ENDHLSL
		}
	}
}