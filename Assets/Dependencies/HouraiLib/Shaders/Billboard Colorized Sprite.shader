﻿Shader "Sprites/Billboard Colorized"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_Scale("Scale", Float) = 1.0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord  : TEXCOORD0;
			};

			fixed4 _Color;
			fixed _Scale;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_P,
							 mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
						     + _Scale * float4(IN.vertex.x, IN.vertex.y, IN.vertex.z, 0.0));
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 targetColor = IN.color;
				fixed4 texColor = tex2D(_MainTex, IN.texcoord);
				fixed greyScale = (texColor.r + texColor.g + texColor.b) / 3;
				fixed4 a = (1, 1, 1, texColor.a * targetColor.a);
				fixed4 b = lerp(targetColor, _Color, greyScale);
				return a*b;
			}
			ENDCG
		}
	}
}
