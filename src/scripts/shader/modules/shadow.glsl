float softShadow(vec3 ro, vec3 lp, float k) {
  const int maxIterationsShad = 24;
  vec3 rd = lp - ro;
  float shade = 1.0;
  float dist = 0.002;
  float end = max(length(rd), 0.001);
  float stepDist = end / float(maxIterationsShad);

  rd /= end;

  for (int i = 0; i < maxIterationsShad; i++) {
    float h = sdf(ro + rd * dist);
    shade = min(shade, smoothstep(0.0, 1.0, k * h / dist));
    dist += clamp(h, 0.02, 0.25);
    if (h < 0.0 || dist > end)
      break;
  }

  return min(max(shade, 0.0) + 0.25, 1.0);
}