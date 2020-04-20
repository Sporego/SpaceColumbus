#ifndef EXPLORATIONLIGHTING
#define EXPLORATIONLIGHTING

struct SurfaceOutputCharacter {
    fixed3 Albedo;
    fixed3 Normal;
    fixed4 AnisoDir;
    fixed3 Emission;
    fixed3 Specular;
    fixed Alpha;
};
 
sampler2D _AnisoRamp, _RampTex;
float _Cutoff;
inline fixed4 LightingExplorationCharacter (SurfaceOutputCharacter s, fixed3 lightDir, fixed3 viewDir, fixed atten)
{
    fixed3 h = normalize(lightDir + viewDir);
    fixed NdotL = saturate(dot(s.Normal, lightDir));
    float nh = saturate(dot(s.Normal, h));
   
    fixed HdotA = dot(normalize(s.Normal + s.AnisoDir.rgb), h);
    float2 anisoUV = float2(HdotA, 0.5);
    anisoUV -= 0.2;
    fixed3 aniso = pow(tex2D(_AnisoRamp, anisoUV), s.Specular.g * 64) * s.Specular.r;
   
    float spec = pow(nh, s.Specular.g * 128) * s.Specular.r;
    spec = saturate(lerp(spec, aniso, s.AnisoDir.a));
 
    float temp = (NdotL * 0.5 + 0.5) * atten;
    float3 ramp = tex2D(_RampTex, float2(temp, temp)).rgb * s.Specular.b;
 
    fixed4 c;
    c.rgb = pow((((pow(s.Albedo + ramp, 2.2) * _LightColor0.rgb * NdotL) * (_LightColor0.rgb * (1 - spec))) + (_LightColor0.rgb * spec)) * (atten * 2), 0.45);
    c.a = 1;
    clip(s.Alpha - _Cutoff);
   
    return c;
}


inline fixed4 LightingExplorationStandard (SurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
{
    fixed3 h = normalize(lightDir + viewDir);
   
    fixed NdotL = saturate(dot(s.Normal, lightDir));
   
    float nh = saturate(dot(s.Normal, h));
    float spec = pow(nh, s.Gloss * 128) * s.Specular;
 
    fixed4 c;
    c.rgb = pow(((pow(s.Albedo, 2.2) * _LightColor0.rgb * NdotL) * ((1 - spec) * _LightColor0.rgb) + (_LightColor0.rgb * spec)) * (atten * 2), 0.45);
    c.a = s.Alpha;
    clip(s.Alpha - _Cutoff);
   
    return c;
}
 
struct SurfaceOutputAniso {
    fixed3 Albedo;
    fixed3 Normal;
    fixed4 AnisoDir;
    fixed3 Emission;
    half Specular;
    fixed Gloss;
    fixed Alpha;
};
 
float _AnisoOffset;
inline fixed4 LightingExplorationAniso (SurfaceOutputAniso s, fixed3 lightDir, fixed3 viewDir, fixed atten)
{
    fixed3 h = normalize(lightDir + viewDir);
    float nh = saturate(dot(s.Normal, h));
 
    fixed NdotL = saturate(dot(s.Normal, lightDir));
    fixed HdotA = dot(normalize(s.Normal + s.AnisoDir.rgb), h);
 
    float2 anisoUV = float2(HdotA, 0.5);
    anisoUV.x += _AnisoOffset;
    fixed3 aniso = pow(tex2D(_AnisoRamp, anisoUV), s.Gloss * 128) * s.Specular;
 
    fixed3 spec = pow(nh, s.Gloss * 128) * s.Specular;
    spec = lerp(spec, aniso, s.AnisoDir.a);
 
    fixed4 c;
    c.rgb = pow(((pow(s.Albedo, 2.2) * _LightColor0.rgb * NdotL) * ((1 - spec) * _LightColor0.rgb) + (_LightColor0.rgb * spec)) * (atten * 2), 0.45);
    c.a = 1;
    clip(s.Alpha - _Cutoff);
    return c;
}
 
struct SurfaceOutputEyes {
    fixed3 Albedo;
    fixed3 Normal;
    fixed3 Normal2;
    fixed3 Emission;
    half Specular;
    fixed Gloss;
    fixed Alpha;
};
 
inline fixed4 LightingExplorationEyes (SurfaceOutputEyes s, fixed3 lightDir, fixed3 viewDir, fixed atten)
{
    fixed3 h = normalize(lightDir + viewDir);
   
    fixed NdotL = saturate(dot(s.Normal, lightDir));
   
    float nh = saturate(dot(s.Normal2, h));
    float spec = pow (nh, s.Gloss * 128) * s.Specular;
   
    fixed4 c;
    c.rgb = pow(((pow(s.Albedo, 2.2) * _LightColor0.rgb * NdotL) * ((1 - spec) * _LightColor0.rgb) + (_LightColor0.rgb * spec)) * (atten * 2), 0.45);
    c.a = 1;
    return c;
}
 
inline fixed4 LightingExplorationRiver (SurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
{  
    fixed3 h = normalize(lightDir + viewDir);
    float nh = saturate(dot (s.Normal, h));
    float spec = pow(nh, 128);
   
    fixed4 c;
    c.rgb = _LightColor0.rgb * spec * (atten * 2);
    c.a = 1;
    return c;
}

#endif /* EXPLORATIONLIGHTING */
