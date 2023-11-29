Shader "Unlit/Skybox shader"
{
    Properties
    {
        [NoScaleOffset] _SunZenithGrad ("Sun-Zenith gradient", 2D) = "white" {}
        [NoScaleOffset] _ViewZenithGrad ("View-Zenith gradient", 2D) = "white" {}
        [NoScaleOffset] _SunViewGrad ("Sun-View gradient", 2D) = "white" {}
        _SunRadius ("Sun radius", Range(0,1)) = 0.05
        _MoonRadius ("Moon radius", Range(0,1)) = 0.05
        _MoonExposure ("Moon exposure", Range(-16, 16)) = 0
    }
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 posOS    : POSITION;
            };

            struct v2f
            {
                float4 posCS        : SV_POSITION;
                float3 viewDirWS    : TEXCOORD0;
            };

            v2f Vertex(Attributes IN)
            {
                v2f OUT = (v2f)0;
    
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.posOS.xyz);
    
                OUT.posCS = vertexInput.positionCS;
                OUT.viewDirWS = vertexInput.positionWS;

                return OUT;
            }

            TEXTURE2D(_SunZenithGrad);      SAMPLER(sampler_SunZenithGrad);
            TEXTURE2D(_ViewZenithGrad);     SAMPLER(sampler_ViewZenithGrad);
            TEXTURE2D(_SunViewGrad);        SAMPLER(sampler_SunViewGrad);

            float3 _SunDir, _MoonDir;
            float3 _MoonPhaseMask;
            float _SunRadius, _MoonRadius;
            float _MoonExposure;

            float GetSunMask(float sunViewDot, float sunRadius)
            {
                float stepRadius = 1 - sunRadius * sunRadius;
                return step(stepRadius, sunViewDot);
            }

            // From Inigo Quilez, https://www.iquilezles.org/www/articles/intersectors/intersectors.htm
            float sphIntersect(float3 rayDir/*ray direction*/, float3 spherePos/*ray origin, however Moon direction is put here?*/, float sphereRadius/*sphere radius*/)
            {
                float3 sphereRayOriginDistance = -spherePos; // (distance between ray origin and sphere origin on page) inversed ray origin (or in this case, inversed Moon direction)?
                float sphereRayOriginDot = dot(sphereRayOriginDistance, rayDir); // dot value between sphere origin and ray direction.
                
                float intersectionDistance = dot(sphereRayOriginDistance, sphereRayOriginDistance) - sphereRadius * sphereRadius; //difference between magnitude squared and radius squared (distance from vector to intersectedSphereSurface(when all values are squared)).
                // dot value between sphere origin and sphere origin (basically: dot(vector3(x,y,z)) = (x*x)+(y*y)+(z*z) which gives squared magnitude of vector3) subtracted with sphere radius squared to get difference between magnitude and radius squared?
                
                float sphereRayIntersectionDistance = sphereRayOriginDot * sphereRayOriginDot - intersectionDistance; // squared distance from the closest ray point to the sphere.
                if(sphereRayIntersectionDistance < 0.0) return -1.0; // no intersection when sphereRayIntersectionDistance < 0.
                sphereRayIntersectionDistance = sqrt(sphereRayIntersectionDistance);
                return -sphereRayOriginDot - sphereRayIntersectionDistance; // return distance to closest intersection with ray.
            }

            float4 Fragment (v2f IN) : SV_TARGET
            {
                float3 viewDir = normalize(IN.viewDirWS);

                // Main angles
                float sunViewDot = dot(_SunDir, viewDir);
                float sunZenithDot = _SunDir.y;
                float viewZenithDot = viewDir.y;
                float sunMoonDot = dot(_SunDir, _MoonDir);

                float sunViewDot01 = (sunViewDot + 1.0) * 0.5;
                float sunZenithDot01 = (sunZenithDot + 1.0) * 0.5;

                // Sky colours
                float3 sunZenithColor = SAMPLE_TEXTURE2D(_SunZenithGrad, sampler_SunZenithGrad, float2(sunZenithDot01, 0.5)).rgb;
                float3 viewZenithColor = SAMPLE_TEXTURE2D(_ViewZenithGrad, sampler_ViewZenithGrad, float2(sunZenithDot01, 0.5)).rgb;
                float vzMask = pow(saturate(1.0 - viewZenithDot), 4);
                float3 sunViewColor = SAMPLE_TEXTURE2D(_SunViewGrad, sampler_SunViewGrad, float2(sunZenithDot01, 0.5)).rgb;
                float svMask = pow(saturate(sunViewDot), 4);

                float3 skyColor = sunZenithColor + vzMask * viewZenithColor + svMask * sunViewColor;

                // The sun
                float sunMask = GetSunMask(sunViewDot, _SunRadius);
                float3 sunColor = _MainLightColor.rgb * sunMask;

                // The moon
                float moonIntersect = sphIntersect(viewDir, _MoonDir, _MoonRadius);
                float moonMask = moonIntersect > -1 ? 1 : 0;
                float3 moonNormal = normalize((_MoonDir - viewDir * moonIntersect)/* * _MoonPhaseMask*/);
                float moonNdotL = saturate(dot(moonNormal, /*-_SunDir*/_MoonPhaseMask));
                [SerializeField]float3 moonColor = moonMask * moonNdotL * exp2(_MoonExposure);

                float3 col = skyColor + sunColor + moonColor;
                return float4(col, 1);
            }
            ENDHLSL
        }
    }
}
