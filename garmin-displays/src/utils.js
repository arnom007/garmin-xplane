// ─── Utility Functions ────────────────────────────────────────────────────

export const DEG2RAD = Math.PI / 180;
export const RAD2DEG = 180 / Math.PI;

export function clamp(v, lo, hi) { return v < lo ? lo : v > hi ? hi : v; }

export function lerp(a, b, t) { return a + (b - a) * t; }

/** Frame-rate independent exponential damping */
export function damp(current, target, smoothing, dt) {
  return lerp(current, target, 1 - Math.exp(-smoothing * dt));
}

/** Angle-aware damping (handles 0↔360 wrap) */
export function dampAngle(current, target, smoothing, dt) {
  let diff = target - current;
  // Normalize to [-180, 180]
  while (diff > 180) diff -= 360;
  while (diff < -180) diff += 360;
  return current + diff * (1 - Math.exp(-smoothing * dt));
}

/** Normalize angle to [0, 360) */
export function normAngle(a) {
  a = a % 360;
  return a < 0 ? a + 360 : a;
}

/** Draw text with alignment */
export function text(ctx, str, x, y, font, color, align = 'left', baseline = 'middle') {
  ctx.font = font;
  ctx.fillStyle = color;
  ctx.textAlign = align;
  ctx.textBaseline = baseline;
  ctx.fillText(str, x, y);
}

/** Draw outlined text */
export function textOutline(ctx, str, x, y, font, fill, stroke, lineW, align = 'left', baseline = 'middle') {
  ctx.font = font;
  ctx.textAlign = align;
  ctx.textBaseline = baseline;
  if (stroke) {
    ctx.strokeStyle = stroke;
    ctx.lineWidth = lineW || 2;
    ctx.strokeText(str, x, y);
  }
  ctx.fillStyle = fill;
  ctx.fillText(str, x, y);
}

/** Draw a rounded rectangle path */
export function roundRect(ctx, x, y, w, h, r) {
  r = Math.min(r, w / 2, h / 2);
  ctx.beginPath();
  ctx.moveTo(x + r, y);
  ctx.lineTo(x + w - r, y);
  ctx.arcTo(x + w, y, x + w, y + r, r);
  ctx.lineTo(x + w, y + h - r);
  ctx.arcTo(x + w, y + h, x + w - r, y + h, r);
  ctx.lineTo(x + r, y + h);
  ctx.arcTo(x, y + h, x, y + h - r, r);
  ctx.lineTo(x, y + r);
  ctx.arcTo(x, y, x + r, y, r);
  ctx.closePath();
}

/** Draw a line */
export function line(ctx, x1, y1, x2, y2, color, width = 1) {
  ctx.strokeStyle = color;
  ctx.lineWidth = width;
  ctx.beginPath();
  ctx.moveTo(x1, y1);
  ctx.lineTo(x2, y2);
  ctx.stroke();
}

/** Format altitude label (e.g., 3500 -> "35", 400 -> "4") */
export function fmtAltLabel(alt) {
  return String(Math.round(alt / 100));
}
