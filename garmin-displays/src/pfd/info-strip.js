// ─── PFD Info Strip ───────────────────────────────────────────────────────
// Between attitude indicator and HSI
// Pixel-accurate reproduction of Garmin TXi info strip
// Shows: TAS, GS, G-load, SAT, GPS1 box, CDI dots, HDG box, CRS box, VS

import { C, FONT, PFD, CONTENT_Y } from '../config.js';
import { text, roundRect, normAngle } from '../utils.js';

export function drawInfoStrip(ctx, data, ox, oy) {
  const sy = oy + PFD.ATT_H;
  const sw = PFD.IAS_W + PFD.ATT_W + PFD.ALT_W + PFD.VSI_W;
  const sh = PFD.INFO_H;

  // Dark background
  ctx.fillStyle = '#0A0A0E';
  ctx.fillRect(ox, sy, sw, sh);

  // Top border line
  ctx.strokeStyle = 'rgba(255,255,255,0.15)';
  ctx.lineWidth = 0.5;
  ctx.beginPath();
  ctx.moveTo(ox, sy + 0.5);
  ctx.lineTo(ox + sw, sy + 0.5);
  ctx.stroke();

  // Row positions
  const row1Y = sy + 12;    // TAS/GS row
  const row2Y = sy + 26;    // GPS1 + CDI dots row
  const row3Y = sy + 42;    // HDG/CRS/VS row

  // ─── Row 1: TAS / GS (left column) ────────────────────────────────
  const tas = data.tas || 0;
  const gs = data.gs || 0;

  text(ctx, 'TAS', ox + 3, row1Y, '400 10px "Roboto Condensed"', '#888888', 'left');
  text(ctx, String(Math.round(tas)), ox + 26, row1Y, '500 11px "Roboto Condensed"', C.WHITE, 'left');

  text(ctx, 'GS', ox + 3, row2Y, '400 10px "Roboto Condensed"', '#888888', 'left');
  text(ctx, String(Math.round(gs)), ox + 20, row2Y, '500 11px "Roboto Condensed"', C.WHITE, 'left');

  // ─── Row 3 left: G-load, SAT ───────────────────────────────────────
  const gLoad = data.g_load || 1.0;
  const sat = data.sat || 15;

  text(ctx, gLoad.toFixed(1) + 'G', ox + 3, row3Y, '500 11px "Roboto Condensed"', C.WHITE, 'left');
  text(ctx, 'SAT', ox + 36, row3Y, '400 10px "Roboto Condensed"', '#888888', 'left');
  text(ctx, Math.round(sat) + '°C', ox + 56, row3Y, '500 11px "Roboto Condensed"', C.WHITE, 'left');

  // ─── GPS1 indicator box (green border) ─────────────────────────────
  const gpsX = ox + 95;
  ctx.strokeStyle = '#00DD00';
  ctx.lineWidth = 1;
  roundRect(ctx, gpsX, row2Y - 7, 32, 14, 2);
  ctx.stroke();
  text(ctx, 'GPS1', gpsX + 16, row2Y, '500 9px "Roboto Condensed"', '#00DD00', 'center');

  // ─── CDI scale dots (5 hollow circles) ─────────────────────────────
  const dotsX = gpsX + 40;
  const dotSpacing = 13;
  for (let i = -2; i <= 2; i++) {
    ctx.strokeStyle = '#555555';
    ctx.lineWidth = 0.8;
    ctx.beginPath();
    ctx.arc(dotsX + i * dotSpacing, row2Y, 2.5, 0, Math.PI * 2);
    ctx.stroke();
  }
  // Center diamond mark
  ctx.strokeStyle = '#555555';
  ctx.lineWidth = 1;
  ctx.beginPath();
  ctx.moveTo(dotsX, row2Y - 5);
  ctx.lineTo(dotsX, row2Y + 5);
  ctx.stroke();

  // ─── Baro (right side of strip, cyan) ──────────────────────────────
  const baro = data.baro || 29.92;
  text(ctx, baro.toFixed(2) + ' IN', ox + sw - 6, row2Y,
       '500 11px "Roboto Condensed"', C.BARO_CYAN, 'right');

  // ─── HDG / Heading / CRS / VS (row 3, center area) ────────────────
  const hdg = normAngle(data.hdg || 0);
  const crs = normAngle(data.crs || 0);
  const vs = data.vs || 0;

  // HDG box (cyan border)
  const hdgBoxX = ox + 88;
  ctx.strokeStyle = C.HDG_CYAN;
  ctx.lineWidth = 1;
  roundRect(ctx, hdgBoxX, row3Y - 8, 48, 16, 2);
  ctx.stroke();
  text(ctx, 'HDG', hdgBoxX + 2, row3Y, '400 9px "Roboto Condensed"', C.HDG_CYAN, 'left');
  text(ctx, Math.round(hdg) + '°', hdgBoxX + 46, row3Y, '600 11px "Roboto Condensed"', C.HDG_CYAN, 'right');

  // Heading value (large, white, centered in PFD)
  const hsiCenterX = ox + PFD.IAS_W + PFD.ATT_W / 2;
  const hdgStr = String(Math.round(hdg)).padStart(3, '0') + '°';
  text(ctx, hdgStr, hsiCenterX, row3Y, '700 18px "Roboto Condensed"', C.WHITE, 'center');

  // Heading bug triangle (small cyan inverted triangle below heading value)
  ctx.fillStyle = C.HDG_CYAN;
  ctx.beginPath();
  ctx.moveTo(hsiCenterX, row3Y + 10);
  ctx.lineTo(hsiCenterX - 4, row3Y + 16);
  ctx.lineTo(hsiCenterX + 4, row3Y + 16);
  ctx.closePath();
  ctx.fill();

  // CRS box (green border)
  const crsBoxX = hsiCenterX + 38;
  ctx.strokeStyle = C.CRS_GREEN;
  ctx.lineWidth = 1;
  roundRect(ctx, crsBoxX, row3Y - 8, 48, 16, 2);
  ctx.stroke();
  text(ctx, 'CRS', crsBoxX + 2, row3Y, '400 9px "Roboto Condensed"', C.CRS_GREEN, 'left');
  text(ctx, Math.round(crs) + '°', crsBoxX + 46, row3Y, '600 11px "Roboto Condensed"', C.CRS_GREEN, 'right');

  // VS box
  const vsBoxX = crsBoxX + 55;
  ctx.strokeStyle = '#555555';
  ctx.lineWidth = 0.5;
  roundRect(ctx, vsBoxX, row3Y - 8, 44, 16, 2);
  ctx.stroke();
  text(ctx, 'VS', vsBoxX + 2, row3Y, '400 9px "Roboto Condensed"', '#888888', 'left');
  text(ctx, String(Math.round(vs)), vsBoxX + 42, row3Y, '500 11px "Roboto Condensed"', C.WHITE, 'right');

  // ─── "No Wind Data" (bottom left) ──────────────────────────────────
  text(ctx, 'No Wind', ox + 3, sy + sh - 8, '400 8px "Roboto Condensed"', '#555555', 'left');
  text(ctx, 'Data', ox + 42, sy + sh - 8, '400 8px "Roboto Condensed"', '#555555', 'left');
}
