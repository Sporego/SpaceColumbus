float depthAlongRay(fixed3 worldPos, fixed3 rootPos)
{
    float3 localPos = worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
    return distance(rootPos, localPos);
}


