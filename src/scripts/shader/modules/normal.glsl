// https://iquilezles.org/articles/normalsSDF/

vec3 calcNormal(vec3 p) {
  const float h = 0.0001;
  const vec2 k = vec2(1, -1);
  return normalize(
    k.xyy * sdf(p + k.xyy * h) +
    k.yyx * sdf(p + k.yyx * h) +
    k.yxy * sdf(p + k.yxy * h) +
    k.xxx * sdf(p + k.xxx * h));
}