#define sampleCurve(curve, time) (sampleCurveDirect(curve, curve##_ST, time))

float matedit_InverseLerp(float a, float b, float x)
{
	return (x - a)*(1 / (b - a));
}

float sampleCurveDirect(sampler2D curve, float4 borders, float time)
{
	float4 values = tex2Dlod(curve, float4(matedit_InverseLerp(borders.x, borders.z, time), 0.5, 0, 0));
	return lerp(borders.y, borders.w, (values.x + values.y + values.z + values.w) / 4);
}