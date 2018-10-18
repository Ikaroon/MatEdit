float sampleCurve(sampler2D curve, float time)
{
	float4 values = tex2Dlod(curve, float4(time, 0.5, 0, 0));
	return values.x + values.y + values.z + values.w;
}