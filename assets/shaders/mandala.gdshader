shader_type canvas_item;

uniform int divisions = 12;
uniform float radius = 0.3f;
uniform int[64] activeMasks;

void vertex() {
    // Called for every vertex the material is visible on.
}

float circleFragF(float T, vec2 coords) {
    float offset = 0.5 - radius;
    float distance = pow((coords.x - 0.5 - offset * cos(T * 6.283185)), 2) + 
                     pow((coords.y - 0.5 - offset * sin(T * 6.283185)), 2);
    float circleRadius = radius * radius;

    // Use smoothstep to create a gradient effect
    float edgeWidth = 0.05; // Adjust for gradient smoothness
    return smoothstep(circleRadius, circleRadius - edgeWidth, distance);
}
float segmentMaskF(float T, vec2 coords) {
    float before = T - 1.0 / float(divisions);
    float after = T + 1.0 / float(divisions);
    
    // Get the gradient values from circleFrag
    float currentAlpha = circleFragF(T, coords);
    float beforeAlpha = circleFragF(before, coords);
    float afterAlpha = circleFragF(after, coords);
    
    // Calculate the segment mask using the gradients
    float alpha = currentAlpha * (1.0 - max(beforeAlpha, afterAlpha));

    return alpha;
}

bool circleFrag(float T, vec2 coords) {
    float offset = 0.5 - radius;
    return pow((coords.x - 0.5 - offset * cos(T * 6.283185)), 2) + 
           pow((coords.y - 0.5 - offset * sin(T * 6.283185)), 2) < pow(radius, 2);
}
bool segmentMask(float T, vec2 coords) {
	float before = T - 1.0f / float(divisions);
    float after = T + 1.0f / float(divisions);
	bool alpha = (circleFrag(T, coords) &&
			(!circleFrag(after, coords) ||
             !circleFrag(before, coords)));
	return alpha;
}
bool segmentNib(float T, vec2 coords) {
	float before = T - 1.0f / float(divisions);
    float after = T + 1.0f / float(divisions);
	bool alpha = (circleFrag(T, coords) &&
			!(circleFrag(after, coords) ||
             circleFrag(before, coords)));
	return alpha;
}


float mandalaF(vec2 coords) {
	// Ensure t and division are defined and initialized appropriately
	float divi = float(divisions);
	float alpha = 0.0f; //float(segmentMask(t, UV));
	float beta = 1.0f;
	float rotateOffSet = 0.0f; // TIME / 4.0f;
	int iInt;
	int jInt;
	for (int i = 0; i < 64; i++) {		
		if (activeMasks[i] == 0) continue; // Skip inactive masks
		alpha += float(segmentNib(float(activeMasks[i])/divi + rotateOffSet, coords));
		for (int j = i+1; j < 64; j++) {
			if (activeMasks[j] == 0) {continue;}
			iInt = activeMasks[i];
			jInt = activeMasks[j];
			while (iInt < 0) {iInt += divisions;}
			while (jInt < 0) {jInt += divisions;}
			if (iInt % divisions == jInt % divisions) {continue;}
			
			beta = 1.0f;
			beta *= float(segmentMask(float(activeMasks[i])/divi - rotateOffSet, coords));
			beta *= float(segmentMask(float(activeMasks[j])/divi + rotateOffSet, coords));
			alpha += beta;
		}
	}
	return alpha;
}

void fragment() {
	COLOR.rg = UV;
	COLOR.b = 1.0f-UV.r;
	COLOR.a = clamp(mandalaF(UV), 0.0f, 1.0f);
}

// void light() {
//     // Called for every pixel for every light affecting the material.
//     // Uncomment to replace the default light processing function with this one.
// }