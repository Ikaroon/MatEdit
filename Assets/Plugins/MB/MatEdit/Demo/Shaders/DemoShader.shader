Shader "MatEdit/DemoShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ColorGradient("Gradient", 2D) = "white" {}
		_CurveHeightY("Curve", 2D) = "black" {}
		_CurveHeightX("Curve", 2D) = "black" {}
		_MinHeight("Min Height", Float) = 0
		_MaxHeight("Max Height", Float) = 1
		_Brightness("Brightness", Float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// shader feature
			#pragma multi_compile __ HEIGHT_MOD
			
			#include "UnityCG.cginc"
			#include "MatEditCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _ColorGradient;

			sampler2D _CurveHeightX;
			float4 _CurveHeightX_ST;
			sampler2D _CurveHeightY;
			float4 _CurveHeightY_ST;
			float _MinHeight;
			float _MaxHeight;

			float _Brightness;
			
			v2f vert (appdata v)
			{ 
				v2f o;
				#if HEIGHT_MOD
				float height = clamp(sampleCurve(_CurveHeightY, v.uv.y) * sampleCurve(_CurveHeightX, v.uv.x), _MinHeight, _MaxHeight);
				v.vertex.xyz += v.normal * height;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv * _MainTex_ST.xy + _MainTex_ST.zw);
				col *= sampleGradient(_ColorGradient, i.uv.y) * _Brightness;

				return col;
			}
			ENDCG
		}
	}
    CustomEditor "DemoShader_GUI"
}