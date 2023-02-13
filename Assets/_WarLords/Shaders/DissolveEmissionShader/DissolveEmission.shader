Shader "DissolverShader/DissolveShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}

		_DissolveMap ("Dissolve Map", 2D) = "white" {}
		_DissolveAmount ("DissolveAmount", Range(0,1)) = 0
		_DissolveColor ("DissolveColor", Color) = (1,1,1,1)
		_DissolveEmission ("DissolveEmission", Range(0,1)) = 1
		_DissolveWidth ("DissolveWidth", Range(0,0.1)) = 0.05
		_DissolveThreshold ("DissolveThreshold", Range(0,1)) = 0.3
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf SimpleLambert noambient
		#pragma target 2.0

		sampler2D _MainTex;
		sampler2D _NormalMap;
		sampler2D _DissolveMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_NormalMap;
			float2 uv_DissolveMap;
		};

		half _DissolveThreshold;
		half _DissolveAmount;
		half _NormalStrenght;
		half _DissolveEmission;
		half _DissolveWidth;
		fixed4 _Color;
		fixed4 _DissolveColor;

		half4 LightingSimpleLambert (SurfaceOutput s, half3 lightDir, half atten) {
			half NdotL = dot (s.Normal, lightDir);
			half4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

		void surf (Input IN, inout SurfaceOutput  o) {
			_DissolveAmount = _DissolveAmount - _DissolveThreshold;

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;			
			fixed4 mask = tex2D (_DissolveMap, IN.uv_DissolveMap);

			if(mask.r < _DissolveAmount)
				discard;

			o.Albedo = c.rgb;

			if(mask.r < _DissolveAmount + _DissolveWidth) {
				o.Albedo = _DissolveColor;
				o.Emission = _DissolveColor * _DissolveEmission;
			}
			
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
