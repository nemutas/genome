// https://www.shadertoy.com/view/4sdGWN

//Random number [0:1] without sine
#define HASHSCALE1 .1031
float hash(float p) {
    vec3 p3 = fract(vec3(p) * HASHSCALE1);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.x + p3.y) * p3.z);
}

vec3 randomSphereDir(vec2 rnd) {
    float s = rnd.x * PI * 2.;
    float t = rnd.y * 2. - 1.;
    return vec3(sin(s), cos(s), t) / sqrt(1.0 + t * t);
}
vec3 randomHemisphereDir(vec3 dir, float i) {
    vec3 v = randomSphereDir(vec2(hash(i + 1.), hash(i + 2.)));
    return v * sign(dot(v, dir));
}

float ambientOcclusion(in vec3 p, in vec3 n, in float maxDist, in float falloff) {
    const int nbIte = 32;
    const float nbIteInv = 1. / float(nbIte);
    const float rad = 1. - 1. * nbIteInv; //Hemispherical factor (self occlusion correction)

    float ao = 0.0;

    for (int i = 0; i < nbIte; i++) {
        float l = hash(float(i)) * maxDist;
        vec3 rd = normalize(n + randomHemisphereDir(n, l) * rad) * l; // mix direction with the normal
        													    // for self occlusion problems!

        ao += (l - max(sdf(p + rd), 0.)) / maxDist * falloff;
    }

    return clamp(1. - ao * nbIteInv, 0., 1.);
}