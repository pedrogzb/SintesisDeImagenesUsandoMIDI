Shader "PFG/EfectoPulsoMultiple"{
	Properties{
		_Onda1("Onda 1",Vector)=(0,0,0.1,0.3)
		_Color1("Color Onda 1",Color) = (1,1,1,1)
		_Onda2("Onda 2",Vector) = (0,0,0.1,0.3)
		_Color2("Color Onda 2",Color) = (1,1,1,1)
		_Onda3("Onda 3",Vector) = (0,0,0.1,0.3)
		_Color3("Color Onda 3",Color) = (1,1,1,1)
		_ColorFondo("Color Fondo",Color) = (1,1,1,1)
		_Scale("Escala",Range(2,15)) = 2
		_Velocidad("Velocidad Modulacion",Range(0,10)) = 5
		_Alpha("Rotacion",Range(0,360)) = 0
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
				#define ModoDeLuz true

				float4 _Color1;
				float4 _Onda1;
				float4 _Color2;
				float4 _Onda2;
				float4 _Color3;
				float4 _Onda3;
				float4 _ColorFondo;
				float _Scale;
				float _Velocidad;
				float _Alpha;

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
				float CrearTextura(float2 uv,float4 Onda) {
					float r = length(uv+Onda.xy)-sin(t*_Velocidad);
					r = fmod(r, 1);
					float a = 0;
					float difuminado = 0.49 * saturate(Onda.z) + 0.01;
					float grosor = 0.5 * saturate(Onda.w) + 0.01;
					float col = (smoothstep(a, a + difuminado, r) - smoothstep(a + grosor + difuminado, a + grosor + 2 * difuminado, r));
					return col;

				}
				float4 Fragment(Interpolators i) : SV_TARGET{

					i.uv -= 0.5;
					i.uv = rotar(i.uv, radians(_Alpha));
					i.uv *= _Scale;
					
					float tex1 = CrearTextura(i.uv, _Onda1);
					float tex2 = CrearTextura(i.uv, _Onda2);
					float tex3 = CrearTextura(i.uv, _Onda3);

					float3 col =   lerp(_ColorFondo.rgb, _Color1.rgb, tex1 );
					col = max(col, lerp(_ColorFondo.rgb, _Color2.rgb, tex2));
					col = max(col, lerp(_ColorFondo.rgb, _Color3.rgb, tex3));
					col = col * saturate(dot(i.normalWS, normalize(_MainLightPosition.xyz)));
					return float4(col,1);

			}
				ENDHLSL
			}
	}
}