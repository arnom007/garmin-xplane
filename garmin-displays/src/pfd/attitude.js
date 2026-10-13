// ─── Attitude Indicator ───────────────────────────────────────────────────
// Faithful reproduction of Garmin TXi PFD attitude indicator
// Sky/Ground fills entire PFD upper area (tapes overlay on top)
// Pitch ladder, roll arc, aircraft symbol

import { C, FONT, PFD, CONTENT_Y } from '../config.js';
import { DEG2RAD, clamp, text } from '../utils.js';

const ROLL_MARKS = [10, 20, 30, 45, 60];

export function drawAttitude(ctx, data, ox, oy) {
  // Full PFD width for the attitude background
  const fullW = PFD.IAS_W + PFD.ATT_W + PFD.ALT_W + PFD.VSI_W;
  const ah = PFD.ATT_H;

  // Center of the attitude display (center of PFD width)
  const cx = ox + PFD.ATT_X + PFD.ATT_W / 2;
  const cy = oy + ah / 2 + 12;

  const pitch = data.pitch || 0;
  const roll  = data.roll || 0;
  const slip  = data.slip || 0;

  const PPD = 5.5; // pixels per degree of pitch

  ctx.save();
  // Clip to FULL PFD upper area (attitude is the background behind tapes)
  ctx.beginPath();
  ctx.rect(ox, oy, fullW, ah);
  ctx.clip();

  // ─── SKY & GROUND ──────────────────────────────────────────────────
  ctx.save();
  ctx.translate(cx, cy);
  ctx.rotate(-roll * DEG2RAD);

  const pitchOffset = pitch * PPD;
  const horizonY = -pitchOffset;
  const bigH = 900;

  // Sky gradient
  const skyGrad = ctx.createLinearGradient(0, horizonY - bigH, 0, horizonY);
  skyGrad.addColorStop(0, '#0A2E88');
  skyGrad.addColorStop(1, '#4488CC');
  ctx.fillStyle = skyGrad;
  ctx.fillRect(-700, horizonY - bigH, 1400, bigH);

  // Ground gradient
  const gndGrad = ctx.createLinearGradient(0, horizonY, 0, horizonY + bigH);
  gndGrad.addColorStop(0, '#8A6B20');
  gndGrad.addColorStop(1, '#5A3A08');
  ctx.fillStyle = gndGrad;
  ctx.fillRect(-700, horizonY, 1400, bigH);

  // Horizon line (white)
  ctx.strokeStyle = '#FFFFFF';
  ctx.lineWidth = 1.5;
  ctx.beginPath();
  ctx.moveTo(-700, horizonY);
  ctx.lineTo(700, horizonY);
  ctx.stroke();

  // ─── PITCH LADDER ──────────────────────────────────────────────────
  ctx.strokeStyle = '#FFFFFF';
  ctx.fillStyle = '#FFFFFF';

  for (let deg = -30; deg <= 30; deg += 2.5) {
    if (deg === 0) continue;
    const py = horizonY - deg * PPD;
    const absDeg = Math.abs(deg);
    let halfW;

    if (absDeg % 10 === 0) {
      halfW = 50;
    } else if (absDeg % 5 === 0) {
      halfW = 28;
    } else {
      halfW = 12;
    }

    ctx.lineWidth = absDeg % 10 === 0 ? 1.5 : 1;
    ctx.beginPath();
    ctx.moveTo(-halfW, py);
    ctx.lineTo(halfW, py);
    ctx.stroke();

    // Degree labels at 10° intervals
    if (absDeg % 10 === 0 && absDeg > 0) {
      const lbl = String(absDeg);
      ctx.font = FONT.PITCH_LABEL;
      ctx.textAlign = 'right';
      ctx.textBaseline = 'middle';
      ctx.fillText(lbl, -halfW - 5, py);
      ctx.textAlign = 'left';
      ctx.fillText(lbl, halfW + 5, py);
    }

    // End caps (pointing toward horizon) on 5° and 10° lines
    if (absDeg % 5 === 0) {
      const capDir = deg > 0 ? 1 : -1; // positive pitch caps point down, negative point up
      const capLen = 5;
      ctx.lineWidth = 1;
      ctx.beginPath();
      ctx.moveTo(-halfW, py);
      ctx.lineTo(-halfW, py + capDir * capLen);
      ctx.moveTo(halfW, py);
      ctx.lineTo(halfW, py + capDir * capLen);
      ctx.stroke();
    }
  }

  ctx.restore(); // un-rotate for pitch/roll

  // ─── ROLL ARC ──────────────────────────────────────────────────────
  const rollArcR = 110;
  const rollArcCy = oy + 26 + rollArcR;

  ctx.save();
  ctx.translate(cx, rollArcCy);

  // Arc (60° each side from top)
  ctx.strokeStyle = '#FFFFFF';
  ctx.lineWidth = 1.2;
  ctx.beginPath();
  ctx.arc(0, 0, rollArcR, (-90 - 60) * DEG2RAD, (-90 + 60) * DEG2RAD);
  ctx.stroke();

  // Roll tick marks
  for (const mark of ROLL_MARKS) {
    for (const sign of [-1, 1]) {
      const ang = (-90 + sign * mark) * DEG2RAD;
      const tickLen = (mark === 30 || mark === 60) ? 12 : (mark === 45 ? 8 : 7);
      const inner = rollArcR - tickLen;

      ctx.strokeStyle = '#FFFFFF';
      ctx.lineWidth = (mark === 30 || mark === 60) ? 1.5 : 1;
      ctx.beginPath();
      ctx.moveTo(Math.cos(ang) * inner, Math.sin(ang) * inner);
      ctx.lineTo(Math.cos(ang) * rollArcR, Math.sin(ang) * rollArcR);
      ctx.stroke();
    }
  }

  // Zero mark (triangle at top)
  ctx.fillStyle = '#FFFFFF';
  ctx.beginPath();
  ctx.moveTo(0, -rollArcR);
  ctx.lineTo(-5, -rollArcR - 9);
  ctx.lineTo(5, -rollArcR - 9);
  ctx.closePath();
  ctx.fill();

  // Roll pointer (rotates with bank)
  ctx.save();
  ctx.rotate(-roll * DEG2RAD);
  ctx.fillStyle = '#FFFFFF';
  ctx.beginPath();
  ctx.moveTo(0, -rollArcR + 1);
  ctx.lineTo(-5, -rollArcR + 10);
  ctx.lineTo(5, -rollArcR + 10);
  ctx.closePath();
  ctx.fill();

  // Slip/skid indicator
  const slipPx = clamp(slip * 4, -15, 15);
  ctx.fillStyle = '#FFFFFF';
  ctx.fillRect(-6 + slipPx, -rollArcR + 12, 12, 4);
  ctx.restore();

  ctx.restore();

  // ─── AIRCRAFT REFERENCE SYMBOL (fixed, yellow) ─────────────────────
  // Garmin style: two horizontal wing bars + center dot, hooks on inner ends
  ctx.strokeStyle = '#C8B000';
  ctx.fillStyle = '#C8B000';
  ctx.lineWidth = 2.5;
  ctx.lineCap = 'round';

  // Left wing
  ctx.beginPath();
  ctx.moveTo(cx - 55, cy);
  ctx.lineTo(cx - 15, cy);
  ctx.lineTo(cx - 15, cy + 6);
  ctx.stroke();

  // Right wing
  ctx.beginPath();
  ctx.moveTo(cx + 55, cy);
  ctx.lineTo(cx + 15, cy);
  ctx.lineTo(cx + 15, cy + 6);
  ctx.stroke();

  // Center dot
  ctx.beginPath();
  ctx.arc(cx, cy, 3, 0, Math.PI * 2);
  ctx.fill();

  ctx.lineCap = 'butt';

  // ─── RA (Radio Altitude) ───────────────────────────────────────────
  if (data.alt_agl !== undefined && data.alt_agl < 2500) {
    ctx.font = FONT.INFO_VALUE || '16px "Roboto Condensed"';
    ctx.fillStyle = '#FFFFFF';
    ctx.textAlign = 'left';
    ctx.fillText('RA ' + Math.round(data.alt_agl), cx + 70, cy + 55);
  }

  ctx.restore(); // un-clip
}
