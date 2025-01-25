shader_type canvas_item;
uniform float timeMutiplier = 1.0f;
uniform float warpAmplitude = 1.0f;

uniform float outerCircleDiameter = 1.0f;
uniform float innerCircleDiameter = 0.93f;
uniform float manaCircleDiameter = 0.85f;

uniform float fillPercentage = 0.5f;

uniform vec3 color;

void vertex() {
	VERTEX += vec2(warpAmplitude*sin(TIME*timeMutiplier+VERTEX.x+VERTEX.y));
	// Called for every vertex the material is visible on.
}
float isInCirlce(float diameter, vec2 uv, vec2 origin) {
	float a = pow(uv.x-origin.x,2) + pow(uv.y-origin.y,2) - pow(diameter/2.0f,2);
	return a/abs(a);
}
float manaCircle(float diameter, vec2 uv, vec2 origin, float percentage) {
	float a = pow(uv.x-origin.x,2) + pow(uv.y-origin.y,2) - pow(diameter/2.0f,2);
	float b = a/abs(a);
	float c = uv.y - 1.0f + percentage
		+ 0.04 * sin(uv.y * 100.0f * PI)
		+ 0.01 * sin(uv.x * 20.0f * PI + TIME * 10.0f);
	float d = (c/abs(c))/2.0f + 0.5f;
	return clamp(b * -1.0f * d,0,1);
}

void fragment() {
	float alpha = -1.0f;
	alpha *= isInCirlce(max(outerCircleDiameter,1), UV, vec2(0.5f));
	alpha *= isInCirlce(min(innerCircleDiameter,outerCircleDiameter), UV, vec2(0.5f));

	alpha = clamp(alpha, 0, 1);

	alpha += manaCircle(manaCircleDiameter, UV, vec2(0.5f), fillPercentage);
	// Called for every pixel the material is visible on.
	COLOR.a = alpha;
	COLOR.rgb = color;
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
