// ─── Airspeed & Altimeter Tapes + VSI ─────────────────────────────────────
// Pixel-accurate reproduction of Garmin TXi PFD tapes
// Based on high-res screenshot analysis

import { C, FONT, PFD, CONTENT_Y, T27 } from '../config.js';
import { clamp, text, roundRect } from '../utils.js';

// ═══════════════════════════════════════════════════════════════════════════
// AIRSPEED TAPE (left side)
// ═══════════════════════════════════════════════════════════════════════════
export function drawAirspeedTape(ctx, data, ox, oy) {
  const x = ox + PFD.IAS_X;
  const y = oy;
  const w = PFD.IAS_W;           // 54px
  const h = PFD.ATT_H;           // 350px
  const rightEdge = x + w;       // right edge where ticks start
  const midY = y + h / 2 + 12;   // vertical center aligned with attitude

  const ias = data.ias || 0;
  const PPK = 3.2;               // pixels per knot
  const visRange = h / PPK / 2 + 25;

  ctx.save();
  ctx.beginPath();
  ctx.rect(x, y, w, h);
  ctx.clip();

  // ─── Semi-transparent dark background ──────────────────────────────
  ctx.fillStyle = 'rgba(0,0,10,0.75)';
  ctx.fillRect(x, y, w, h);

  // ─── Speed Color Bands (left edge, thin stripe) ────────────────────
  const bandW = 3;
  const bx = x + 1;

  function spdToY(spd) { return midY - (spd - ias) * PPK; }

  // White arc (Vs1 to Vfe)
  _drawBand(ctx, bx, spdToY(T27.VFE), spdToY(T27.VS1), bandW, C.SPD_WHITE, y, y + h);
  // Green arc (Vs0 to Vno)
  _drawBand(ctx, bx, spdToY(T27.VNO), spdToY(T27.VS0), bandW, C.SPD_GREEN, y, y + h);
  // Yellow arc (Vno to Vne)
  _drawBand(ctx, bx, spdToY(T27.VNE), spdToY(T27.VNO), bandW, C.SPD_YELLOW, y, y + h);
  // Red line at Vne
  const vneY = spdToY(T27.VNE);
  if (vneY > y && vneY < y + h) {
    ctx.fillStyle = C.SPD_RED;
    ctx.fillRect(bx, vneY - 1, bandW + 2, 3);
  }

  // ─── Tick marks and numbers ─────────────────────────────────────────
  const minSpd = Math.floor((ias - visRange) / 10) * 10;
  const maxSpd = Math.ceil((ias + visRange) / 10) * 10;

  for (let spd = minSpd; spd <= maxSpd; spd += 5) {
    const py = spdToY(spd);
    if (py < y - 5 || py > y + h + 5) continue;

    const isMajor = spd % 10 === 0;
    const tickLen = isMajor ? 10 : 5;

    // Tick marks from RIGHT edge toward center
    ctx.strokeStyle = C.TAPE_TICK;
    ctx.lineWidth = isMajor ? 1.2 : 0.7;
    ctx.beginPath();
    ctx.moveTo(rightEdge, py);
    ctx.lineTo(rightEdge - tickLen, py);
    ctx.stroke();

    // Number label at major ticks (LEFT of tick)
    if (isMajor && spd >= 0) {
      text(ctx, String(spd), rightEdge - tickLen - 3, py, FONT.TAPE_NUM, C.TAPE_TEXT, 'right');
    }
  }

  // Right edge separator line
  ctx.strokeStyle = 'rgba(255,255,255,0.15)';
  ctx.lineWidth = 1;
  ctx.beginPath();
  ctx.moveTo(rightEdge, y);
  ctx.lineTo(rightEdge, y + h);
  ctx.stroke();

  ctx.restore();

  // ─── Current Speed Pointer (outside clip) ──────────────────────────
  _drawSpeedPointer(ctx, rightEdge, midY, ias);

  // ─── Speed Trend Vector (magenta bar on right edge) ────────────────
  if (data.ias_trend) {
    const trendPx = data.ias_trend * PPK;
    if (Math.abs(trendPx) > 3) {
      ctx.fillStyle = C.TREND_MAGENTA;
      const ty = trendPx > 0 ? midY - trendPx : midY;
      ctx.fillRect(rightEdge - 3, ty, 3, Math.abs(trendPx));
    }
  }
}

function _drawSpeedPointer(ctx, rightEdge, midY, ias) {
  const pw = 48;     // pointer width
  const ph = 24;     // pointer height
  const arrowW = 9;  // arrow point width
  const px = rightEdge - pw;

  // Pointer shape: black rectangle with arrow pointing right
  ctx.fillStyle = '#000000';
  ctx.strokeStyle = '#AAAAAA';
  ctx.lineWidth = 1;
  ctx.beginPath();
  ctx.moveTo(px, midY - ph / 2);
  ctx.lineTo(rightEdge - arrowW, midY - ph / 2);
  ctx.lineTo(rightEdge - 1, midY);
  ctx.lineTo(rightEdge - arrowW, midY + ph / 2);
  ctx.lineTo(px, midY + ph / 2);
  ctx.closePath();
  ctx.fill();
  ctx.stroke();

  // Speed value in white
  const spdStr = ias < 0 ? '---' : String(Math.round(ias));
  text(ctx, spdStr, rightEdge - arrowW - 4, midY, FONT.TAPE_PTR, C.WHITE, 'right');
}

// ═══════════════════════════════════════════════════════════════════════════
// ALTIMETER TAPE (right side of attitude)
// ═══════════════════════════════════════════════════════════════════════════
export function drawAltimeterTape(ctx, data, ox, oy) {
  const x = ox + PFD.ALT_X;
  const y = oy;
  const w = PFD.ALT_W;           // 68px
  const h = PFD.ATT_H;
  const leftEdge = x;            // left edge where ticks start
  const midY = y + h / 2 + 12;

  const alt = data.alt || 0;
  const PPF = 0.27;              // pixels per foot
  const visRange = h / PPF / 2 + 200;

  ctx.save();
  ctx.beginPath();
  ctx.rect(x, y, w, h);
  ctx.clip();

  // ─── Semi-transparent dark background ──────────────────────────────
  ctx.fillStyle = 'rgba(0,0,10,0.75)';
  ctx.fillRect(x, y, w, h);

  // ─── Tick marks and numbers ─────────────────────────────────────────
  function altToY(a) { return midY - (a - alt) * PPF; }

  const minAlt = Math.floor((alt - visRange) / 100) * 100;
  const maxAlt = Math.ceil((alt + visRange) / 100) * 100;

  for (let a = minAlt; a <= maxAlt; a += 20) {
    const py = altToY(a);
    if (py < y - 5 || py > y + h + 5) continue;

    const isMajor = a % 100 === 0;
    const tickLen = isMajor ? 10 : 5;

    // Tick marks from LEFT edge toward center
    ctx.strokeStyle = C.TAPE_TICK;
    ctx.lineWidth = isMajor ? 1.2 : 0.6;
    ctx.beginPath();
    ctx.moveTo(leftEdge, py);
    ctx.lineTo(leftEdge + tickLen, py);
    ctx.stroke();

    // Number label at major ticks (RIGHT of tick)
    if (isMajor) {
      // Format: show as hundreds (e.g., 8400 → "84", 400 → "4", -200 → "-2")
      const hundreds = Math.round(a / 100);
      text(ctx, String(hundreds), leftEdge + tickLen + 4, py, FONT.TAPE_NUM, C.TAPE_TEXT, 'left');
    }
  }

  // Left edge separator line
  ctx.strokeStyle = 'rgba(255,255,255,0.15)';
  ctx.lineWidth = 1;
  ctx.beginPath();
  ctx.moveTo(leftEdge, y);
  ctx.lineTo(leftEdge, y + h);
  ctx.stroke();

  // ─── Selected Altitude Bug ──────────────────────────────────────────
  if (data.sel_alt !== undefined) {
    const bugY = altToY(data.sel_alt);
    if (bugY > y - 5 && bugY < y + h + 5) {
      ctx.fillStyle = C.BUG_CYAN;
      ctx.strokeStyle = C.BUG_CYAN;
      ctx.lineWidth = 2;
      // Bug marker: horizontal line with two small filled rectangles
      ctx.beginPath();
      ctx.moveTo(leftEdge, bugY);
      ctx.lineTo(leftEdge + 8, bugY);
      ctx.stroke();
      ctx.fillRect(leftEdge, bugY - 4, 3, 8);
    }
  }

  ctx.restore();

  // ─── Selected Altitude Box (above tape, cyan) ──────────────────────
  if (data.sel_alt !== undefined) {
    const selAltStr = String(Math.round(data.sel_alt));
    ctx.fillStyle = '#000008';
    ctx.strokeStyle = C.BUG_CYAN;
    ctx.lineWidth = 1;
    ctx.fillRect(x, y - 1, w, 17);
    ctx.strokeRect(x, y - 1, w, 17);
    text(ctx, selAltStr, x + w / 2, y + 7, FONT.TAPE_PTR_SM, C.BUG_CYAN, 'center');
  }

  // ─── Current Altitude Pointer ───────────────────────────────────────
  _drawAltPointer(ctx, leftEdge, midY, w, alt);

  // ─── Altitude Trend Vector ──────────────────────────────────────────
  if (data.vs) {
    const trendFt = data.vs * 0.1; // 6-second prediction
    const trendPx = trendFt * PPF;
    if (Math.abs(trendPx) > 3) {
      ctx.fillStyle = C.TREND_MAGENTA;
      const ty = trendPx > 0 ? midY - trendPx : midY;
      ctx.fillRect(leftEdge + 1, ty, 3, Math.abs(trendPx));
    }
  }

  // ─── Baro Setting (below tape, cyan text) ──────────────────────────
  const baro = data.baro || 29.92;
  text(ctx, baro.toFixed(2) + ' IN', x + w / 2, y + h + 14, FONT.INFO_VALUE, C.BARO_CYAN, 'center');
}

function _drawAltPointer(ctx, leftEdge, midY, tapeW, alt) {
  const pw = 58;     // pointer width
  const ph = 24;     // pointer height
  const arrowW = 9;

  const right = leftEdge + pw;

  // Pointer shape: black box with arrow pointing left
  ctx.fillStyle = '#000000';
  ctx.strokeStyle = '#AAAAAA';
  ctx.lineWidth = 1;
  ctx.beginPath();
  ctx.moveTo(leftEdge + 1, midY);
  ctx.lineTo(leftEdge + arrowW, midY - ph / 2);
  ctx.lineTo(right, midY - ph / 2);
  ctx.lineTo(right, midY + ph / 2);
  ctx.lineTo(leftEdge + arrowW, midY + ph / 2);
  ctx.closePath();
  ctx.fill();
  ctx.stroke();

  // Diagonal hatching inside the pointer (Garmin style)
  ctx.save();
  ctx.beginPath();
  ctx.moveTo(leftEdge + 1, midY);
  ctx.lineTo(leftEdge + arrowW, midY - ph / 2);
  ctx.lineTo(right, midY - ph / 2);
  ctx.lineTo(right, midY + ph / 2);
  ctx.lineTo(leftEdge + arrowW, midY + ph / 2);
  ctx.closePath();
  ctx.clip();

  ctx.strokeStyle = 'rgba(255,255,255,0.12)';
  ctx.lineWidth = 1;
  for (let i = -10; i < 20; i++) {
    const sx = leftEdge + arrowW + i * 6;
    ctx.beginPath();
    ctx.moveTo(sx, midY - ph / 2 - 2);
    ctx.lineTo(sx + ph, midY + ph / 2 + 2);
    ctx.stroke();
  }
  ctx.restore();

  // Altitude value
  const altVal = Math.round(alt);
  // Split into thousands and sub-thousands
  const thousands = Math.floor(Math.abs(altVal) / 1000);
  const hundreds = Math.abs(altVal) % 1000;
  const prefix = altVal < 0 ? '-' : '';

  // Show as composite: large thousands, small hundreds
  if (Math.abs(altVal) >= 1000) {
    const thousStr = prefix + String(thousands);
    const hundStr = String(hundreds).padStart(3, '0');
    // Thousands - larger
    text(ctx, thousStr, leftEdge + arrowW + 3, midY - 1, FONT.TAPE_PTR, C.WHITE, 'left');
    // Hundreds separator
    const tw = ctx.measureText(thousStr).width || (thousStr.length * 10);
    // Simplified: just show full altitude
    text(ctx, prefix + String(Math.abs(altVal)), leftEdge + arrowW + 3, midY, FONT.TAPE_PTR, C.WHITE, 'left');
  } else {
    const displayStr = prefix + String(Math.abs(altVal));
    text(ctx, displayStr, leftEdge + arrowW + 3, midY, FONT.TAPE_PTR, C.WHITE, 'left');
  }
}

// ═══════════════════════════════════════════════════════════════════════════
// VSI (Vertical Speed Indicator) - thin strip to the right of altimeter
// ═══════════════════════════════════════════════════════════════════════════
export function drawVSI(ctx, data, ox, oy) {
  const x = ox + PFD.VSI_X;
  const y = oy;
  const w = PFD.VSI_W;           // 32px
  const h = PFD.ATT_H;
  const midY = y + h / 2 + 12;

  const vs = data.vs || 0;       // fpm

  ctx.save();
  ctx.beginPath();
  ctx.rect(x, y, w, h);
  ctx.clip();

  // ─── Semi-transparent dark background ──────────────────────────────
  ctx.fillStyle = 'rgba(0,0,10,0.75)';
  ctx.fillRect(x, y, w, h);

  // Non-linear scale: sqrt compression
  // Marks at 0, ±1, ±2, ±4, ±6 (×1000 fpm)
  const marks = [0, 1, 2, 4, 6];
  const maxPx = (h / 2) - 18;

  function vsiToY(v) {
    const sign = v >= 0 ? -1 : 1;
    const absV = Math.abs(v) / 1000;
    const norm = Math.sqrt(absV / 6);
    return midY + sign * norm * maxPx;
  }

  // Scale marks and labels
  for (const m of marks) {
    for (const sign of m === 0 ? [1] : [-1, 1]) {
      const fpm = sign * m * 1000;
      const my = vsiToY(fpm);

      // Tick mark
      ctx.strokeStyle = m === 0 ? 'rgba(255,255,255,0.5)' : 'rgba(255,255,255,0.3)';
      ctx.lineWidth = m === 0 ? 1.5 : 0.7;
      ctx.beginPath();
      ctx.moveTo(x + 2, my);
      ctx.lineTo(x + (m === 0 ? w - 2 : 10), my);
      ctx.stroke();

      // Labels
      if (m > 0) {
        text(ctx, String(m), x + 13, my, '400 8px "Roboto Condensed"', '#555555', 'left');
      }
    }
  }

  // ─── VSI pointer triangle ──────────────────────────────────────────
  const ptrY = vsiToY(vs);
  const clampedY = clamp(ptrY, y + 6, y + h - 6);

  ctx.fillStyle = C.WHITE;
  ctx.beginPath();
  ctx.moveTo(x + 2, clampedY);
  ctx.lineTo(x + 10, clampedY - 4);
  ctx.lineTo(x + 10, clampedY + 4);
  ctx.closePath();
  ctx.fill();

  // ─── Digital readout (only if VS > 100 fpm) ────────────────────────
  if (Math.abs(vs) > 100) {
    const rounded = Math.round(vs / 50) * 50;
    text(ctx, String(Math.abs(rounded)), x + 13, clampedY,
         '500 8px "Roboto Condensed"', C.WHITE, 'left');
  }

  // Left separator
  ctx.strokeStyle = 'rgba(255,255,255,0.1)';
  ctx.lineWidth = 0.5;
  ctx.beginPath();
  ctx.moveTo(x, y);
  ctx.lineTo(x, y + h);
  ctx.stroke();

  ctx.restore();
}

// ─── Helper ──────────────────────────────────────────────────────────────
function _drawBand(ctx, x, y1, y2, w, color, clipTop, clipBottom) {
  const top = clamp(Math.min(y1, y2), clipTop, clipBottom);
  const bot = clamp(Math.max(y1, y2), clipTop, clipBottom);
  if (bot - top < 1) return;
  ctx.fillStyle = color;
  ctx.fillRect(x, top, w, bot - top);
}
