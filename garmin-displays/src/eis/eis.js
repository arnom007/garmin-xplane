import { C, FONT, EIS_X, EIS_W, CONTENT_Y, CONTENT_H, T27 } from '../config.js';
import { clamp, text, DEG2RAD } from '../utils.js';

// ─── Arc Gauge Component ──────────────────────────────────────────────────
function drawArcGauge(ctx, cx, cy, radius, value, min, max, bands, label, unit, valueStr, options = {}) {
  // Canvas 2D: 0° = 3 o'clock, increases clockwise
  // Garmin gauge: starts at 7 o'clock (150°), sweeps 240° CW to 5 o'clock (30°=390°)
  const START_DEG = 150;
  const SWEEP_DEG = 240;

  function valToAngle(v) {
    const norm = clamp((v - min) / (max - min), 0, 1);
    return (START_DEG + norm * SWEEP_DEG) * DEG2RAD;
  }

  const arcW = 5;

  // Background arc
  ctx.strokeStyle = '#333333';
  ctx.lineWidth = arcW;
  ctx.lineCap = 'butt';
  ctx.beginPath();
  ctx.arc(cx, cy, radius, START_DEG * DEG2RAD, (START_DEG + SWEEP_DEG) * DEG2RAD);
  ctx.stroke();

  // Color bands
  for (const band of bands) {
    const [bMin, bMax, color] = band;
    const a1 = valToAngle(bMin);
    const a2 = valToAngle(bMax);
    ctx.strokeStyle = color;
    ctx.lineWidth = arcW;
    ctx.beginPath();
    ctx.arc(cx, cy, radius, a1, a2);
    ctx.stroke();
  }

  // Tick marks
  const ticks = options.ticks || [];
  for (const tick of ticks) {
    const ang = valToAngle(tick.value);
    const cos = Math.cos(ang);
    const sin = Math.sin(ang);
    const inner = radius - (tick.major ? 8 : 4);
    const outer = radius + 2;

    ctx.strokeStyle = '#FFFFFF';
    ctx.lineWidth = tick.major ? 1.5 : 1;
    ctx.beginPath();
    ctx.moveTo(cx + cos * inner, cy + sin * inner);
    ctx.lineTo(cx + cos * outer, cy + sin * outer);
    ctx.stroke();
  }

  // Triangle markers
  const markers = options.markers || [];
  for (const marker of markers) {
    const ang = valToAngle(marker.value);
    const cos = Math.cos(ang);
    const sin = Math.sin(ang);
    
    const mR = radius - 8;
    ctx.fillStyle = marker.color || '#FFFFFF';
    ctx.beginPath();
    ctx.moveTo(cx + cos * mR, cy + sin * mR);
    const perpCos = Math.cos(ang + Math.PI / 2);
    const perpSin = Math.sin(ang + Math.PI / 2);
    ctx.lineTo(cx + cos * (mR - 6) + perpCos * 4, cy + sin * (mR - 6) + perpSin * 4);
    ctx.lineTo(cx + cos * (mR - 6) - perpCos * 4, cy + sin * (mR - 6) - perpSin * 4);
    ctx.fill();
  }

  // Pointer needle (white pointer from center to arc edge)
  const needleAngle = valToAngle(clamp(value, min, max));
  const cosN = Math.cos(needleAngle);
  const sinN = Math.sin(needleAngle);

  ctx.strokeStyle = '#FFFFFF';
  ctx.lineWidth = 2;
  ctx.beginPath();
  ctx.moveTo(cx, cy);
  ctx.lineTo(cx + cosN * (radius - 2), cy + sinN * (radius - 2));
  ctx.stroke();

  // Label & unit (Small gray text INSIDE the arc, centered)
  text(ctx, label, cx, cy - 8, '400 14px "Roboto Condensed"', '#AAAAAA', 'center');
  if (unit) {
    text(ctx, unit, cx, cy + 8, '400 10px "Roboto Condensed"', '#AAAAAA', 'center');
  }

  // Digital Value (LARGE, about 22px, RIGHT of the arc gauge)
  let valueColor = '#00FF00';
  for (const band of bands) {
    if (value >= band[0] && value <= band[1]) {
      valueColor = band[2];
    }
  }

  text(ctx, valueStr, cx + radius + 25, cy, '600 22px "Roboto Condensed"', valueColor, 'left');

  // Left text (e.g. SHP)
  if (options.leftText) {
    text(ctx, options.leftText, cx - radius - 15, cy, '400 12px "Roboto Condensed"', '#FFFFFF', 'right');
  }
}

// ─── Horizontal Bar Gauge ─────────────────────────────────────────────────
function drawBarGauge(ctx, x, y, w, value, min, max, bands, label, valueStr) {
  const barH = 6;
  const barY = y;

  // Label
  text(ctx, label, x, barY - 4, '400 12px "Roboto Condensed"', '#AAAAAA', 'left', 'bottom');

  // Background bar
  ctx.fillStyle = '#333333';
  ctx.fillRect(x, barY, w, barH);

  // Color bands
  for (const band of bands) {
    const [bMin, bMax, color] = band;
    const x1 = x + clamp((bMin - min) / (max - min), 0, 1) * w;
    const x2 = x + clamp((bMax - min) / (max - min), 0, 1) * w;
    ctx.fillStyle = color;
    ctx.fillRect(x1, barY, x2 - x1, barH);
  }

  // Pointer (triangle below bar)
  const ptrX = x + clamp((value - min) / (max - min), 0, 1) * w;
  ctx.fillStyle = '#FFFFFF';
  ctx.beginPath();
  ctx.moveTo(ptrX, barY + barH + 1);
  ctx.lineTo(ptrX - 4, barY + barH + 6);
  ctx.lineTo(ptrX + 4, barY + barH + 6);
  ctx.fill();

  // Digital value (right of bar)
  let valueColor = '#00FF00';
  for (const band of bands) {
    if (value >= band[0] && value <= band[1]) {
      valueColor = band[2];
    }
  }
  text(ctx, valueStr, x + w + 15, barY + barH / 2, '600 18px "Roboto Condensed"', valueColor, 'left', 'middle');
}

// ═══════════════════════════════════════════════════════════════════════════
// MAIN EIS DRAW FUNCTION
// ═══════════════════════════════════════════════════════════════════════════
export function drawEIS(ctx, data) {
  const x = EIS_X;
  const y = CONTENT_Y;
  const w = EIS_W;
  const h = CONTENT_H;

  // Black Background
  ctx.fillStyle = '#000000';
  ctx.fillRect(x, y, w, h);

  const gaugeR = 50;
  // Position slightly left of center to leave room for the values on the right
  const colCenter = x + w / 2 - 40;

  const trq = data.trq || 0;
  const nh = data.nh || 0;
  const ng = data.ng || 0;
  const t5 = data.t5 || 0;
  const oilT = data.oil_temp || 0;
  const oilP = data.oil_psi || 0;
  const shp = Math.round(trq * 3.58);

  const GREEN = '#00FF00';
  const YELLOW = '#FFFF00';
  const RED = '#FF2222';

  // ─── 1. TRQ Gauge ──────────────────────────────────────────────────
  const trqY = y + 70;
  drawArcGauge(ctx, colCenter, trqY, gaugeR, trq, 0, 133,
    [
      [0, 100, GREEN],
      [100, 115, YELLOW],
      [115, 133, RED]
    ],
    'TRQ', '%', trq.toFixed(1),
    {
      ticks: [
        { value: 0, major: true },
        { value: 50, major: false },
        { value: 100, major: true },
        { value: 133, major: true }
      ],
      leftText: 'SHP ' + shp
    }
  );

  // ─── 2. NP RPM Gauge ───────────────────────────────────────────────
  const nhY = trqY + 115;
  const npRPM = nh * 22; // approx RPM
  drawArcGauge(ctx, colCenter, nhY, gaugeR, npRPM, 200, 2200,
    [
      [200, 2000, GREEN],
      [2000, 2100, YELLOW],
      [2100, 2200, RED]
    ],
    'NP', 'RPM', Math.round(npRPM).toString()
  );

  // ─── 3. NG % Gauge ─────────────────────────────────────────────────
  const ngY = nhY + 115;
  drawArcGauge(ctx, colCenter, ngY, gaugeR, ng, 0, 114,
    [
      [55, 101.5, GREEN],
      [101.5, 104, YELLOW],
      [104, 114, RED]
    ],
    'NG', '%', ng.toFixed(1)
  );

  // ─── 4. ITT °C Gauge ───────────────────────────────────────────────
  const t5Y = ngY + 115;
  drawArcGauge(ctx, colCenter, t5Y, gaugeR, t5, 200, 1190,
    [
      [200, 770, GREEN],
      [770, 810, YELLOW],
      [810, 1190, RED]
    ],
    'ITT', '°C', Math.round(t5).toString(),
    {
      markers: [{ value: 770, color: '#FFFFFF' }]
    }
  );

  // ─── Oil Bars ──────────────────────────────────────────────────────
  const barX = x + 35;
  const barW = 200;
  
  const oilTY = t5Y + 80;
  drawBarGauge(ctx, barX, oilTY, barW, oilT, 0, 150,
    [
      [0, 110, GREEN],
      [110, 130, YELLOW],
      [130, 150, RED]
    ],
    'Oil °C', Math.round(oilT).toString()
  );

  const oilPY = oilTY + 45;
  drawBarGauge(ctx, barX, oilPY, barW, oilP, 0, 150,
    [
      [0, 30, RED],
      [30, 120, GREEN],
      [120, 150, RED]
    ],
    'Oil PSI', Math.round(oilP).toString()
  );

  // ─── Text Readouts ─────────────────────────────────────────────────
  const textY = oilPY + 50;
  const tX1 = x + 40;
  const tX2 = x + w - 40;
  const fontSmall = '400 16px "Roboto Condensed"';
  const fontLabel = '400 14px "Roboto Condensed"';
  
  // Electrical
  text(ctx, '11A      1 ALT 2      13A', x + w / 2, textY, fontSmall, '#FFFFFF', 'center');
  
  text(ctx, 'BAT', tX1, textY + 25, fontLabel, '#AAAAAA', 'left');
  text(ctx, (data.bus_volts || 29.2).toFixed(1) + 'v', tX2, textY + 25, fontSmall, '#FFFFFF', 'right');
  
  // Fuel
  const fuelQty = Math.round(data.fuel_qty || 150);
  text(ctx, `${fuelQty}    M Fuel QTY GAL    ${fuelQty}`, x + w / 2, textY + 55, fontSmall, '#FFFFFF', 'center');
  
  text(ctx, 'FF GPH', tX1, textY + 80, fontLabel, '#AAAAAA', 'left');
  text(ctx, Math.round(data.fuel_flow || 25).toString(), tX2, textY + 80, fontSmall, '#FFFFFF', 'right');

  text(ctx, 'Fuel PSI', tX1, textY + 105, fontLabel, '#AAAAAA', 'left');
  text(ctx, '25.5', tX2, textY + 105, fontSmall, '#FFFFFF', 'right');

  text(ctx, 'VAC IN', tX1, textY + 130, fontLabel, '#AAAAAA', 'left');
  text(ctx, '5.1', tX2, textY + 130, fontSmall, '#FFFFFF', 'right');
}
