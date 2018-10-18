float4 sampleGradientlod(sampler2D gradient, float value)
{
	return tex2Dlod(gradient, float4(value, 0.5, 0, 0));
}

float4 sampleGradient(sampler2D gradient, float value)
{
	return tex2D(gradient, float2(value, 0.5));
}