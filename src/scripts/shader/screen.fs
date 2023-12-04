precision mediump float;

#define repeat(p, span) mod(p, span) - (0.5 * span)

uniform vec2 uResolution;
uniform float uTime;

const int MAX_STEP = 60;
const float MIN_DIST = 0.0001;
const float MAX_DIST = 30.0;
const float PI = acos(-1.0);
const float TAU = PI * 2.0;

#include './modules/primitives.glsl'
#include './modules/combinations.glsl'

mat2 rotate(float a) {
  float s = sin(a), c = cos(a);
  return mat2(c, s, -s, c);
}

vec2 pmod(vec2 p, float r) {
  float a = atan(p.x, p.y) + PI / r;
  float n = TAU / r;
  a = floor(a / n) * n;
  return p * rotate(-a);
}

float sdf(vec3 _p) {
  float final = 10.0;
  const int LOOP = 5;

  for (int i = 0; i < LOOP; i++) {
    float ratio = float(i) / float(LOOP);
    vec3 p = _p;
    p.z += float(i) * 3.0;

    p.xy = rotate(PI * 0.25 + ratio * PI) * p.xy;
    p.xz = rotate(p.y - uTime * (1.0 / (ratio + 1.0)) * 0.5 ) * p.xz;
    p.x = abs(p.x);
    p.y = repeat(p.y, 0.5);

    float c1 = sdCapsule(p + vec3(-0.7, 0.0, 0.0), vec3(0.0, 0.5, 0.0), vec3(0.0, -0.5, 0.0), 0.12);
    float c2 = sdCapsule(p, vec3(0.2, 0.0, 0.0), vec3(0.6, 0.0, 0.0), 0.1);
    final = min(final, opSmoothUnion(c1, c2, 0.05));
  }

  return final * 0.9;
}

float rayMarch(vec3 ro, vec3 rd) {
  float total = 0.0;
  float accum = 0.0;
  for (int i = 0; i < MAX_STEP; i++) {
    vec3 p = ro + rd * total;
    float d = sdf(p);
    d = max(abs(d), 0.01);
    accum += exp(-d * 10.0); 
    total += d * 0.9;
    if (MAX_DIST < total) break;
  }
  return accum;
}

void main() {
  vec2 p = (gl_FragCoord.xy * 2.0 - uResolution) / min(uResolution.x, uResolution.y);
  vec3 rd = normalize(vec3(p, -2.0));
  vec3 ro = vec3(0.0, 0.0, 3.5);

  float accum = rayMarch(ro, rd);

  vec3 color = vec3(accum);
  color *= 0.015;
  color = smoothstep(0.02, 0.8, color);
  color *= mix(vec3(0.00, 0.92, 1.00), vec3(0.82, 0.93, 0.99), color);

  gl_FragColor = vec4(color, 1.0);
}