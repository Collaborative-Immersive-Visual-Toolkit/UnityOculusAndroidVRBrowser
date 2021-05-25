// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ForceField" 
{
    Properties
    {
		_MainTex("Bump Texture", 2D) = "white" {}

		[NoScaleOffset]
		_BumpTex("Bump Normal", 2D) = "white" {}

		[HDR]
		_Color("Base Color", Color) = (1, 1, 1, 0.25)

		[HDR]
		_HighlightColor("Intersection Color", Color) = (1, 1, 1, .5)
		
		_IntersectionThreshold("Intersection Threshold ", Range(0, 1)) = 0.25
		_Distortion("Distortion", Range(0, 128)) = 10
		_BumpStrength("Bump Strength", Range(0, 10)) = 3
		_SilhouetteEnhancement("Silhouette Enhancement", Range(-2, 2)) = 1

		_XSpeed("X Speed", Range(-10, 10)) = 1
		_YSpeed("Y Speed", Range(-10, 10)) = 1

	}
    SubShader
    {
		Tags
		{ 
			"Queue" = "Transparent" 
			"IgnoreProjector" = "True" 
			"RenderType" = "Transparent" 
		}

		GrabPass
		{
			Name "BASE"
			Tags{ "LightMode" = "Always" }
		}

        Pass
        {
			Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off 
 
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
			uniform sampler2D _GrabTexture;
			uniform float4 _GrabTexture_TexelSize;

            uniform sampler2D _CameraDepthTexture;
					
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_TexelSize;
			uniform sampler2D _BumpTex;
			uniform float4 _BumpTex_TexelSize;

            float4 _Color;
            float4 _HighlightColor;

            float _IntersectionThreshold;
			float _Distortion;
			float _BumpStrength;
			float _SilhouetteEnhancement;
			
			float _XSpeed;
			float _YSpeed;

			float4 _BumpTex_ST;
			float4 _MainTex_ST;

            struct v2f
            {
                float4 pos : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float4 uvgrab : TEXCOORD3;
				float4 projPos : TEXCOORD4;
            };
 
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

				o.normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
				o.viewDir = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex).xyz);
				
#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
#else
				float scale = 1.0;
#endif
				o.uvgrab.xy = (float2(o.pos.x, o.pos.y*scale) + o.pos.w) * 0.5;
				o.uvgrab.zw = o.pos.zw;

				o.projPos = ComputeScreenPos(o.pos);
				UNITY_TRANSFER_FOG(o, o.pos);

                return o;
            }
 
            half4 frag(v2f i) : COLOR
            {
				float2 texCoord = i.texcoord + float2(_XSpeed * _Time.x, _YSpeed * _Time.x);

				float heightSampleCenter = tex2D(_MainTex, texCoord).r;
				float heightSampleRight = tex2D(_MainTex, texCoord + float2(_MainTex_TexelSize.x, 0)).r;
				float heightSampleUp = tex2D(_MainTex, texCoord + float2(0, _MainTex_TexelSize.y)).r;

				float sampleDeltaRight = heightSampleRight - heightSampleCenter;
				float sampleDeltaUp = heightSampleUp - heightSampleCenter;

				float3 normal = UnpackNormal(tex2D(_BumpTex, texCoord));
				normal = cross(float3(1, 0, sampleDeltaRight * _BumpStrength), float3(0, 1, sampleDeltaUp * _BumpStrength));
				normal = normalize(normal);
				
				float2 offset = normal * _Distortion * _GrabTexture_TexelSize.xy;
#ifdef UNITY_Z_0_FAR_FROM_CLIPSPACE 
				i.uvgrab.xy = offset * UNITY_Z_0_FAR_FROM_CLIPSPACE(i.uvgrab.z) + i.uvgrab.xy;
#else
				i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
#endif

				half4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab)) * _Color;

				float alpha = tex2D(_MainTex, texCoord);

				float3 normalDirection = normalize(i.normal);
				float3 viewDirection = normalize(i.viewDir);

				float opacity = min(1.0, _Color.a / pow(abs(dot(viewDirection, normalDirection)), _SilhouetteEnhancement));
				col.a = opacity;

				float world_z = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
				float projPos = i.projPos.w;
				float distance = world_z - projPos;
				float multiplier = pow(1 - saturate(distance / _IntersectionThreshold), 3);

				return lerp(col, _HighlightColor, multiplier);
            }
 
            ENDCG
        }
    }
    FallBack "VertexLit"
}