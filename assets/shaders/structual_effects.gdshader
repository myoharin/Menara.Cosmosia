shader_type canvas_item;

uniform float uv_scale = 1.0f;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Get the UV coordinates
    vec2 uv = UV * uv_scale;

    // Normalize UV coordinates to [-1, 1]
    vec2 uv_normalized = (uv - 0.5) * 2.0;

    // Convert UV to spherical coordinates
    float theta = uv_normalized.x * 3.14159265359; // Longitude
    float phi = uv_normalized.y * 1.57079632679; // Latitude

    // Calculate the normal vector
    float x = cos(phi) * cos(theta);
    float y = sin(phi);
    float z = cos(phi) * sin(theta);

    // Normalize the normal vector
    vec3 normal = normalize(vec3(x, y, z));

    // Convert normal vector to RGB
    vec3 normal_rgb = normal * 0.5 + 0.5; // Scale to [0, 1]
	
	NORMAL_MAP = normal_rgb;
	COLOR = vec4(normal_rgb,1.0f);
    // Output the normal map color
    //COLOR = texture(NORMAL_MAP, uv) * vec4(normal_rgb, 1.0);
	// NORMAL_MAP = vec3(UV.r, -UV.g, UV.g);
	// Called for every pixel the material is visible on.
	// COLOR = 0.5f * vec4(UV.r,UV.g,1.0f,1.0f);
}

void light() {
	
	float cNdotL = max(0.0, dot(NORMAL, LIGHT_DIRECTION));
	LIGHT = vec4(LIGHT_COLOR.rgb * COLOR.rgb * LIGHT_ENERGY * cNdotL, LIGHT_COLOR.a);
}
