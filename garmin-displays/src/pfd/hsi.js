// ─── HSI (Horizontal Situation Indicator) ─────────────────────────────────
// Pixel-accurate reproduction of Garmin TXi HSI
// Compass rose with course pointer, CDI, heading bug, GPS label, clock

import { C, FONT, PFD, CONTENT_Y, CONTENT_H } from '../config.js';
import { DEG2RAD, clamp, text, normAngle } from '../utils.js';

export function drawHSI(ctx, data, ox, oy) {
  const hsiY = oy + PFD.ATT_H + PFD.INFO_H;
  const hsiH = PFD.HSI_H;
  const hsiW = PFD.IAS_W + PFD.ATT_W + PFD.ALT_W + PFD.VSI_W;
  const hsiX = ox;

  const centerX = ox + PFD.IAS_W + PFD.ATT_W / 2;
  const centerY = hsiY + hsiH / 2 + 5;
  const radius = Math.min(hsiW, hsiH) / 2 - 22;

  const hdg = data.hdg || 0;
  const crs = data.crs || 0;
  const cdi = data.cdi || 0;

  ctx.save();
  ctx.beginPath();
  ctx.rect(hsiX, hsiY, hsiW, hsiH);
  ctx.clip();

  // ─── Background ────────────────────────────────────────────────────
  ctx.fillStyle = '#0A0A0E';
  ctx.fillRect(hsiX, hsiY, hsiW, hsiH);

  // ─── Compass Rose ──────────────────────────────────────────────────
  ctx.save();
  ctx.translate(centerX, centerY);
  ctx.rotate(-hdg * DEG2RAD);

  // Compass circle
  ctx.strokeStyle = 'rgba(255,255,255,0.25)';
  ctx.lineWidth = 1.2;
  ctx.beginPath();
  ctx.arc(0, 0, radius, 0, Math.PI * 2);
  ctx.stroke();

  // Tick marks and labels
  const LABELS = {
    0: 'N', 30: '3', 60: '6', 90: 'E',
    120: '12', 150: '15', 180: 'S', 210: '21',
    240: '24', 270: 'W', 300: '30', 330: '33'
  };

  for (let deg = 0; deg < 360; deg += 5) {
    const ang = deg * DEG2RAD;
    const cos = Math.cos(ang);
    const sin = Math.sin(ang);

    const is30 = deg % 30 === 0;
    const is10 = deg % 10 === 0;

    const tickOuter = radius;
    const tickInner = is30 ? radius - 14 : is10 ? radius - 9 : radius - 5;

    ctx.strokeStyle = is30 ? '#FFFFFF' : 'rgba(255,255,255,0.5)';
    ctx.lineWidth = is30 ? 1.5 : 0.7;
    ctx.beginPath();
    ctx.moveTo(sin * tickInner, -cos * tickInner);
    ctx.lineTo(sin * tickOuter, -cos * tickOuter);
    ctx.stroke();

    // Labels at 30° intervals
    if (LABELS[deg] !== undefined) {
      const labelR = radius - 22;
      const lx = sin * labelR;
      const ly = -cos * labelR;

      ctx.save();
      ctx.translate(lx, ly);
      ctx.rotate(deg * DEG2RAD); // rotate label to be upright relative to display

      const label = LABELS[deg];
      const isCardinal = label === 'N' || label === 'S' || label === 'E' || label === 'W';
      const fontSize = isCardinal ? '700 13px' : '500 11px';
      const color = label === 'N' ? '#FF0000' : '#FFFFFF';

      text(ctx, label, 0, 0, `${fontSize} "Roboto Condensed"`, color, 'center');
      ctx.restore();
    }
  }

  // ─── Course Line (magenta) ─────────────────────────────────────────
  const crsRel = (crs - hdg) * DEG2RAD;
  // Course pointer line from center
  ctx.save();
  ctx.rotate(crsRel);

  // Arrow head at top
  ctx.fillStyle = C.MAGENTA;
  ctx.beginPath();
  ctx.moveTo(0, -(radius - 12));
  ctx.lineTo(-5, -(radius - 24));
  ctx.lineTo(5, -(radius - 24));
  ctx.closePath();
  ctx.fill();

  // Course line from arrowhead to center area
  ctx.strokeStyle = C.MAGENTA;
  ctx.lineWidth = 2;
  ctx.beginPath();
  ctx.moveTo(0, -(radius - 24));
  ctx.lineTo(0, -22);
  ctx.stroke();

  // Course line from center to bottom
  ctx.beginPath();
  ctx.moveTo(0, 22);
  ctx.lineTo(0, radius - 12);
  ctx.stroke();

  // CDI dots (5 hollow circles)
  const dotSpacing = 12;
  for (let i = -2; i <= 2; i++) {
    ctx.strokeStyle = 'rgba(255,255,255,0.35)';
    ctx.lineWidth = 0.8;
    ctx.beginPath();
    ctx.arc(i * dotSpacing, 0, 3, 0, Math.PI * 2);
    ctx.stroke();
  }

  // CDI needle (magenta, offset by cdi value)
  const cdiPx = clamp(cdi, -2.5, 2.5) * dotSpacing;
  ctx.strokeStyle = C.MAGENTA;
  ctx.lineWidth = 2.5;
  ctx.beginPath();
  ctx.moveTo(cdiPx, -20);
  ctx.lineTo(cdiPx, 20);
  ctx.stroke();

  ctx.restore(); // un-rotate course

  ctx.restore(); // un-rotate compass

  // ─── Fixed Aircraft Symbol (white cross at center) ─────────────────
  ctx.strokeStyle = '#FFFFFF';
  ctx.lineWidth = 2;
  // Vertical bar (fuselage)
  ctx.beginPath();
  ctx.moveTo(centerX, centerY - 14);
  ctx.lineTo(centerX, centerY + 10);
  ctx.stroke();
  // Wings
  ctx.beginPath();
  ctx.moveTo(centerX - 10, centerY - 2);
  ctx.lineTo(centerX + 10, centerY - 2);
  ctx.stroke();
  // Tail
  ctx.beginPath();
  ctx.moveTo(centerX - 5, centerY + 8);
  ctx.lineTo(centerX + 5, centerY + 8);
  ctx.stroke();

  // ─── Fixed Heading Triangle (white, at top of compass) ─────────────
  ctx.fillStyle = '#FFFFFF';
  ctx.beginPath();
  ctx.moveTo(centerX, centerY - radius - 2);
  ctx.lineTo(centerX - 5, centerY - radius - 10);
  ctx.lineTo(centerX + 5, centerY - radius - 10);
  ctx.closePath();
  ctx.fill();

  // ─── Heading Bug (cyan triangle on compass edge) ────────────────────
  if (data.sel_hdg !== undefined) {
    const bugAng = (data.sel_hdg - hdg) * DEG2RAD;
    const bugX = centerX + Math.sin(bugAng) * radius;
    const bugY = centerY - Math.cos(bugAng) * radius;

    ctx.save();
    ctx.translate(bugX, bugY);
    ctx.rotate(bugAng);
    ctx.fillStyle = C.BUG_CYAN;
    ctx.beginPath();
    ctx.moveTo(0, 0);
    ctx.lineTo(-5, -10);
    ctx.lineTo(5, -10);
    ctx.closePath();
    ctx.fill();
    ctx.restore();
  }

  // ─── GPS1 Label ─────────────────────────────────────────────────────
  text(ctx, 'GPS1', centerX - radius + 15, centerY - 10,
       '500 11px "Roboto Condensed"', '#00DD00', 'left');

  // ─── TAWS TEST label ────────────────────────────────────────────────
  // Box with white border
  const tawsX = centerX + radius - 60;
  const tawsY = centerY - 15;
  ctx.fillStyle = '#000000';
  ctx.strokeStyle = '#666666';
  ctx.lineWidth = 0.5;
  ctx.fillRect(tawsX, tawsY - 7, 55, 15);
  ctx.strokeRect(tawsX, tawsY - 7, 55, 15);
  text(ctx, 'TAWS TEST', tawsX + 27, tawsY,
       '500 9px "Roboto Condensed"', '#FFFFFF', 'center');

  // ─── Clock (bottom-left of HSI) ────────────────────────────────────
  const now = new Date();
  const utcStr = String(now.getUTCHours()).padStart(2, '0') + ':' +
                 String(now.getUTCMinutes()).padStart(2, '0') + ' UTC';
  const clockX = hsiX + 6;
  const clockY = hsiY + hsiH - 18;
  ctx.fillStyle = '#0A0A14';
  ctx.fillRect(clockX, clockY - 7, 68, 14);
  text(ctx, 'Clock', clockX + 2, clockY, '400 8px "Roboto Condensed"', '#555555', 'left');
  text(ctx, utcStr, clockX + 66, clockY, '500 9px "Roboto Condensed"', '#FFFFFF', 'right');

  // ─── CDI label (bottom center, between HSI and buttons) ────────────
  const cdiLabelY = hsiY + hsiH - 5;
  ctx.fillStyle = '#0A0A14';
  ctx.fillRect(centerX - 20, cdiLabelY - 7, 40, 14);
  ctx.strokeStyle = '#444444';
  ctx.lineWidth = 0.5;
  ctx.strokeRect(centerX - 20, cdiLabelY - 7, 40, 14);
  text(ctx, 'CDI', centerX, cdiLabelY, '500 10px "Roboto Condensed"', '#FFFFFF', 'center');

  // ─── Menu label (bottom left) ──────────────────────────────────────
  text(ctx, 'Menu', hsiX + 65, hsiY + hsiH - 5,
       '400 9px "Roboto Condensed"', '#FFFFFF', 'center');

  ctx.restore(); // un-clip
}
