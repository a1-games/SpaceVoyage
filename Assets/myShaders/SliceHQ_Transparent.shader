Shader "Custom/SliceHQ_Transparent"
{
    Properties
    {
        // Slicing Variables
        [HideInInspector] sliceNormal("normal", Vector) = (0,0,0,0)
        [HideInInspector] sliceCentre("centre", Vector) = (0,0,0,0)
        [HideInInspector] sliceOffsetDst("offset", Float) = 0.0

        // My Addition To Standard Variables
        _MapScale("Scale All Maps", Range(0.0, 2.0)) = 1.0

        _AcidMap("Acid Trip Map", 2D) = "white" {}
        _AcidMapStrength("Acid Trip Map Strength", Range(0,1)) = 0.0
            // Standard Surface Shader Variables


            _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
            //_GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
            //[Enum(Metallic Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel("Smoothness Texture Channel", Float) = 0

            //[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
            //[ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0

            //_Parallax("Height Scale", Range(0.005, 0.08)) = 0.02
            //_ParallaxMap("Height Map", 2D) = "black" {}

            _MainTex("Albedo Texture", 2D) = "white" {}
            _Color("Albedo Color", Color) = (1,1,1,1)

            _MetallicGlossMap("Metallic Map", 2D) = "white" {}
            [Gamma] _Metallic("Metallic Strength", Range(0.0, 2.0)) = 0.0

            [Normal] _BumpMap("Normal Map", 2D) = "bump" {}
            _BumpScale("Normal Strength", Range(-2.0, 2.0)) = 1.0

            _HeightMap("Height Map", 2D) = "white" {}
            _HeightMapStrength("Height Map Strength", Range(0,0.12)) = 0.0

            _OcclusionMap("Occlusion Map", 2D) = "white" {}
            _OcclusionStrength("OcclusionStrength", Range(-2.0, 2.0)) = 1.0

            _EmissionMap("Emission Map", 2D) = "white" {}
            [HDR] _EmissionColor("Emission Color", Color) = (0,0,0)

                /*
            _DetailScale("Scale All Details", Range(0.0, 2.0)) = 1.0
            _DetailMask("Detail Mask", 2D) = "white" {}
            _DetailMaskStrength("Detail Mask Strength", Range(-2.0, 2.0)) = 1.0
            _DetailAlbedoMap("Detail Albedo Texture", 2D) = "grey" {}
            _DetailAlbedoStrength("Detail Mask Strength", Range(-2.0, 2.0)) = 1.0

            [Normal] _DetailNormalMap("Detail Normal Map", 2D) = "bump" {}
            _DetailNormalMapStrength("Detail Normal Strength", Range(-2.0, 2.0)) = 1.0
                */
                //[Enum(UV0,0,UV1,1)] _UVSec("UV Set for secondary textures", Float) = 0

                    // Blending state
                    [HideInInspector] _Mode("__mode", Float) = 3.0/*
                    [HideInInspector] _SrcBlend("__src", Float) = 1.0
                    [HideInInspector] _DstBlend("__dst", Float) = 0.0
                    [HideInInspector] _ZWrite("__zw", Float) = 1.0*/

    }
        SubShader
            {
                Tags { "Queue" = "Transparent" "IgnoreProjector" = "True"  "RenderType" = "Transparent" }

                //Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
                Blend SrcAlpha OneMinusSrcAlpha

                LOD 100

                CGPROGRAM
                // Physically based Standard lighting model, and enable shadows on all light types
                #pragma surface surf Standard addshadow keepalpha

                #pragma target 3.0
                // Use shader model 3.0 target, to get nicer looking lighting

                struct Input
                {
                    float2 uv_MainTex;
                    float2 uv_BumpMap;
                    float2 uv_Detail;
                    float2 uv_OcclusionMap;
                    float2 uv_MetallicGlossMap;
                    float2 uv_EmissionMap;
                    float2 uv_HeightMap;
                    float2 uv_AcidMap;
                    //float2 uv_DetailMask;
                    //float2 uv_DetailNormal;

                    float3 viewDir;
                    float3 worldPos;
                };


                sampler2D _MainTex;
                sampler2D _BumpMap;
                sampler2D _Detail;
                sampler2D _OcclusionMap;
                sampler2D _MetallicGlossMap;
                sampler2D _EmissionMap;
                sampler2D _HeightMap;
                sampler2D _AcidMap;
                //sampler2D _DetailMask;
                //sampler2D _DetailNormal;


                //half3 _Albedo; not needed bc we have fixed4 _Color
                //fixed3 _Normal;
                //half _Emission;
                half _Smoothness;
                half _MapScale;
                half _DetailScale;
                // strength values - have weird names so they match the standard shader which makes you able to copy and paste the values in the editor
                half _Metallic;
                half _BumpScale;
                half _OcclusionStrength;
                half _HeightMapStrength;
                half _AcidMapStrength;
                //half _DetailMaskStrength;
                fixed4 _Color;
                fixed4 _EmissionColor;

                // World space normal of slice, anything along this direction from centre will be invisible
                float3 sliceNormal;
                // World space centre of slice
                float3 sliceCentre;
                // Increasing makes more of the mesh visible, decreasing makes less of the mesh visible
                float sliceOffsetDst;

                void surf(Input IN, inout SurfaceOutputStandard o)
                {
                    // Slicing
                    float3 adjustedCentre = sliceCentre + sliceNormal * sliceOffsetDst;
                    float3 offsetToSliceCentre = adjustedCentre - IN.worldPos;
                    clip(dot(offsetToSliceCentre, sliceNormal));

                    // Map Modifiers Calculation
                    float2 acidEffectOffset = ParallaxOffset(tex2D(_AcidMap, IN.uv_AcidMap).r, _AcidMapStrength, IN.viewDir);
                    float2 heightOffset = ParallaxOffset(tex2D(_HeightMap, IN.uv_HeightMap).r, _HeightMapStrength, IN.viewDir);
                    fixed4 c = tex2D(_MainTex, IN.uv_MainTex * _MapScale) * _Color;

                    // Albedo comes from a texture tinted by color
                    //o.Albedo *= tex2D(_Detail, IN.uv_Detail).rgb * 2; // detail tex map

                    // Standard variables
                    o.Albedo = c;
                    o.Smoothness = _Smoothness;
                    o.Alpha = c.a;


                    // a1 attempt at stuff
                    // emission map
                    o.Emission = tex2D(_EmissionMap, (IN.uv_MainTex + acidEffectOffset) * _MapScale + heightOffset) * _EmissionColor;
                    // metallic map
                    o.Metallic = tex2D(_MetallicGlossMap, (IN.uv_MetallicGlossMap + acidEffectOffset) * _MapScale + heightOffset) * _Metallic;
                    // normal map
                    o.Normal = UnpackScaleNormal(tex2D(_BumpMap, (IN.uv_BumpMap + acidEffectOffset) * _MapScale + heightOffset), _BumpScale);
                    // occlusion map
                    o.Occlusion = tex2D(_OcclusionMap, (IN.uv_OcclusionMap + acidEffectOffset) * _MapScale + heightOffset) * _OcclusionStrength;
                    //o.Occlusion = UnpackNormal(tex2D(_OcclusionMap, (IN.uv_OcclusionMap + acidEffectOffset) * _MapScale + heightOffset)) * _OcclusionStrength;
                    // Detail map
                    //o.DetailMask = UnpackNormal(tex2D(_DetailMask, (IN.uv_DetailMask) * _DetailScale + heightOffset)) * _DetailMaskStrength;
                }
                ENDCG
            }
                FallBack "VertexLit"
}
