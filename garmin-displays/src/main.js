// ─── GDU TXi 1060 — Main Entry Point ──────────────────────────────────────
// Full display with PFD + MFD placeholder + EIS
// Demo mode with simulated flight data

import './style.css';
import { C, FONT, W, H, HEADER_H, BTN_BAR_H, STATUS_H,
         CONTENT_Y, CONTENT_H, PFD_X, PFD_W, MFD_X, MFD_W, EIS_X, EIS_W, PFD } from './config.js';
import { damp, dampAngle, normAngle, text } from './utils.js';
import { drawAttitude } from './pfd/attitude.js';
import { drawAirspeedTape, drawAltimeterTape, drawVSI } from './pfd/tapes.js';
import { drawHSI } from './pfd/hsi.js';
import { drawInfoStrip } from './pfd/info-strip.js';
import { drawEIS } from './eis/eis.js';

// ─── Canvas Setup ─────────────────────────────────────────────────────────
const canvas = document.getElementById('gdu');
const ctx = canvas.getContext('2d');
canvas.width = W;
canvas.height = H;

// ─── Flight Data (smoothed values for display) ───────────────────────────
const display = {
  ias: 0, tas: 0, gs: 0,
  alt: 0, vs: 0, baro: 29.92,
  pitch: 0, roll: 0, slip: 0,
  hdg: 305, crs: 185, cdi: 0,
  g_load: 1.0, sat: 15,
  sel_alt: 8000, sel_hdg: 305,
  alt_agl: 0,
  ias_trend: 0,
  // EIS
  trq: 0, nh: 0, ng: 0, t5: 0,
  oil_temp: 0, oil_psi: 0,
  bus_volts: 29.2, fuel_qty: 150, fuel_flow: 0,
};

// Target values (from WebSocket or demo)
const target = { ...display };

// ─── Demo Mode ────────────────────────────────────────────────────────────
let demoMode = true;
let demoTime = 0;

function updateDemo(dt) {
  demoTime += dt;
  const t = demoTime;

  // Simulate circling flight
  target.hdg = normAngle(t * 12); // 12°/s = 30s per circle
  target.ias = 120 + Math.sin(t * 0.3) * 15;
  target.tas = target.ias * 1.08;
  target.gs = target.ias * 1.05;
  target.alt = 3000 + Math.sin(t * 0.15) * 800;
  target.vs = Math.cos(t * 0.15) * 800 * 0.15 * 60; // derivative of alt
  target.pitch = Math.sin(t * 0.4) * 5;
  target.roll = Math.sin(t * 0.2) * 20;
  target.slip = Math.sin(t * 0.7) * 1.5;
  target.crs = 185;
  target.cdi = Math.sin(t * 0.1) * 1.5;
  target.g_load = 1.0 + Math.sin(t * 0.3) * 0.15;
  target.sat = 15;
  target.baro = 29.92;
  target.sel_alt = 4000;
  target.sel_hdg = 305;
  target.alt_agl = target.alt - 2200;

  // IAS trend (6-second prediction)
  target.ias_trend = Math.cos(t * 0.3) * 15 * 0.3 * 6;

  // EIS - T-27 engine running
  target.trq = 57 + Math.sin(t * 0.25) * 3;
  target.nh = 79.5 + Math.sin(t * 0.18) * 1.5; // ~1750 RPM (×22)
  target.ng = 79 + Math.sin(t * 0.22) * 2;
  target.t5 = 500 + Math.sin(t * 0.12) * 30;
  target.oil_temp = 80 + Math.sin(t * 0.08) * 5;
  target.oil_psi = 102 + Math.sin(t * 0.1) * 3;
  target.bus_volts = 29.2;
  target.fuel_qty = 150 - t * 0.05;
  target.fuel_flow = 25 + Math.sin(t * 0.15) * 2;
}

// ─── WebSocket ────────────────────────────────────────────────────────────
let ws = null;
let wsConnected = false;

function connectWebSocket() {
  try {
    ws = new WebSocket('ws://localhost:9877');
    ws.onopen = () => { wsConnected = true; demoMode = false; };
    ws.onclose = () => { wsConnected = false; demoMode = true; ws = null; setTimeout(connectWebSocket, 3000); };
    ws.onerror = () => { ws?.close(); };
    ws.onmessage = (e) => {
      try {
        const d = JSON.parse(e.data);
        if (d.type === 'flight_data') {
          Object.assign(target, d);
        }
      } catch (_) {}
    };
  } catch (_) {
    setTimeout(connectWebSocket, 3000);
  }
}
connectWebSocket();

// ─── Smooth Data Updates ──────────────────────────────────────────────────
const SMOOTH = 8; // smoothing factor

function smoothData(dt) {
  display.ias = damp(display.ias, target.ias, SMOOTH, dt);
  display.tas = damp(display.tas, target.tas, SMOOTH, dt);
  display.gs = damp(display.gs, target.gs, SMOOTH, dt);
  display.alt = damp(display.alt, target.alt, SMOOTH, dt);
  display.vs = damp(display.vs, target.vs, SMOOTH, dt);
  display.pitch = damp(display.pitch, target.pitch, SMOOTH, dt);
  display.roll = damp(display.roll, target.roll, SMOOTH, dt);
  display.slip = damp(display.slip, target.slip, SMOOTH, dt);
  display.hdg = dampAngle(display.hdg, target.hdg, SMOOTH, dt);
  display.crs = dampAngle(display.crs, target.crs, SMOOTH, dt);
  display.cdi = damp(display.cdi, target.cdi, SMOOTH, dt);
  display.g_load = damp(display.g_load, target.g_load, SMOOTH, dt);
  display.baro = target.baro;
  display.sat = target.sat;
  display.sel_alt = target.sel_alt;
  display.sel_hdg = target.sel_hdg;
  display.alt_agl = damp(display.alt_agl, target.alt_agl, SMOOTH, dt);
  display.ias_trend = damp(display.ias_trend, target.ias_trend, SMOOTH, dt);

  // EIS
  display.trq = damp(display.trq, target.trq, SMOOTH, dt);
  display.nh = damp(display.nh, target.nh, SMOOTH, dt);
  display.ng = damp(display.ng, target.ng, SMOOTH, dt);
  display.t5 = damp(display.t5, target.t5, SMOOTH, dt);
  display.oil_temp = damp(display.oil_temp, target.oil_temp, SMOOTH, dt);
  display.oil_psi = damp(display.oil_psi, target.oil_psi, SMOOTH, dt);
  display.bus_volts = target.bus_volts;
  display.fuel_qty = target.fuel_qty;
  display.fuel_flow = damp(display.fuel_flow, target.fuel_flow, SMOOTH, dt);
}

// ─── Header Bar ───────────────────────────────────────────────────────────
function drawHeader() {
  ctx.fillStyle = '#0C0C0C';
  ctx.fillRect(0, 0, W, HEADER_H);

  // GARMIN text centered
  text(ctx, 'GARMIN', W / 2, HEADER_H / 2, FONT.GARMIN_TITLE, C.WHITE, 'center');

  // Bottom border
  ctx.strokeStyle = '#333';
  ctx.lineWidth = 0.5;
  ctx.beginPath();
  ctx.moveTo(0, HEADER_H);
  ctx.lineTo(W, HEADER_H);
  ctx.stroke();
}

// ─── Bottom Bars ──────────────────────────────────────────────────────────
function drawBottomBars() {
  const btnY = H - BTN_BAR_H - STATUS_H;
  const statusY = H - STATUS_H;

  // Button bar background
  ctx.fillStyle = '#0A0A0A';
  ctx.fillRect(0, btnY, W, BTN_BAR_H);

  // PFD buttons
  const buttons = ['Menu', '', '', '', 'CDI', '', 'Full'];
  const btnW = PFD_W / buttons.length;
  for (let i = 0; i < buttons.length; i++) {
    if (!buttons[i]) continue;
    const bx = PFD_X + i * btnW;
    // Button box
    ctx.strokeStyle = '#444';
    ctx.lineWidth = 0.5;
    ctx.strokeRect(bx + 4, btnY + 4, btnW - 8, BTN_BAR_H - 8);
    text(ctx, buttons[i], bx + btnW / 2, btnY + BTN_BAR_H / 2, FONT.BTN_LABEL, C.WHITE, 'center');
  }

  // Demo indicator
  if (demoMode) {
    ctx.fillStyle = '#1A1A2A';
    ctx.fillRect(PFD_W - 60, btnY + 3, 56, BTN_BAR_H - 6);
    ctx.strokeStyle = '#555';
    ctx.strokeRect(PFD_W - 60, btnY + 3, 56, BTN_BAR_H - 6);
    text(ctx, 'Demo', PFD_W - 32, btnY + BTN_BAR_H / 2, FONT.BTN_LABEL, C.WHITE, 'center');
  }

  // MFD buttons
  const mfdBtns = ['Full', 'Back', 'Home', 'Advisory', 'Menu'];
  const mbw = MFD_W / mfdBtns.length;
  for (let i = 0; i < mfdBtns.length; i++) {
    const bx = MFD_X + i * mbw;
    ctx.strokeStyle = '#444';
    ctx.lineWidth = 0.5;
    ctx.strokeRect(bx + 4, btnY + 4, mbw - 8, BTN_BAR_H - 8);
    text(ctx, mfdBtns[i], bx + mbw / 2, btnY + BTN_BAR_H / 2, FONT.BTN_LABEL, C.WHITE, 'center');
  }

  // EIS buttons
  const eisBtns = ['ENG', ''];
  const ebw = EIS_W / eisBtns.length;
  for (let i = 0; i < eisBtns.length; i++) {
    if (!eisBtns[i]) continue;
    const bx = EIS_X + i * ebw;
    ctx.strokeStyle = '#444';
    ctx.lineWidth = 0.5;
    ctx.strokeRect(bx + 4, btnY + 4, ebw - 8, BTN_BAR_H - 8);
    text(ctx, eisBtns[i], bx + ebw / 2, btnY + BTN_BAR_H / 2, FONT.BTN_LABEL, C.WHITE, 'center');
  }

  // ─── Status bar (Push Sync) ─────────────────────────────────────────
  ctx.fillStyle = '#060606';
  ctx.fillRect(0, statusY, W, STATUS_H);
  ctx.strokeStyle = '#222';
  ctx.lineWidth = 0.5;
  ctx.beginPath();
  ctx.moveTo(0, statusY);
  ctx.lineTo(W, statusY);
  ctx.stroke();

  text(ctx, 'Push Sync →', 6, statusY + STATUS_H / 2, FONT.STATUS_LABEL, C.DIM, 'left');

  // Status indicators
  const indicators = [
    { label: 'HDG', color: C.HDG_CYAN },
    { label: 'ALT', color: C.WHITE },
    { label: 'VS', color: C.WHITE },
    { label: 'IAS', color: C.WHITE },
    { label: 'CRS', color: C.WHITE },
    { label: 'BARO', color: C.WHITE },
  ];
  let ix = 80;
  for (const ind of indicators) {
    ctx.fillStyle = '#1A1A28';
    ctx.fillRect(ix, statusY + 2, 34, STATUS_H - 4);
    ctx.strokeStyle = ind.color;
    ctx.lineWidth = 0.5;
    ctx.strokeRect(ix, statusY + 2, 34, STATUS_H - 4);
    text(ctx, ind.label, ix + 17, statusY + STATUS_H / 2, FONT.STATUS_LABEL, ind.color, 'center');
    ix += 38;
  }

  // Right-side MFD indicators
  const rightInds = ['MAP', 'TRFC', 'TERR', 'CHRT', 'SXM', 'ENG'];
  let rx = W - 10;
  for (let i = rightInds.length - 1; i >= 0; i--) {
    const lw = ctx.measureText ? 32 : 32;
    rx -= 36;
    ctx.fillStyle = '#1A1A28';
    ctx.fillRect(rx, statusY + 2, 34, STATUS_H - 4);
    text(ctx, rightInds[i], rx + 17, statusY + STATUS_H / 2, FONT.STATUS_LABEL,
         i === 0 ? C.CYAN : C.DIM, 'center');
  }
}

// ─── MFD Placeholder ──────────────────────────────────────────────────────
function drawMFDPlaceholder() {
  ctx.fillStyle = '#050508';
  ctx.fillRect(MFD_X, CONTENT_Y, MFD_W, CONTENT_H);

  // Border
  ctx.strokeStyle = C.PANEL_BORDER;
  ctx.lineWidth = 1;
  ctx.beginPath();
  ctx.moveTo(MFD_X, CONTENT_Y);
  ctx.lineTo(MFD_X, CONTENT_Y + CONTENT_H);
  ctx.stroke();

  // "Home" title
  text(ctx, '🏠 Home', MFD_X + MFD_W / 2, CONTENT_Y + 20, '500 14px "Roboto Condensed"', C.WHITE, 'center');

  // Home page icon grid (like the real Garmin)
  const icons = [
    { label: 'Map', emoji: '🌐' },
    { label: 'Traffic', emoji: '◆' },
    { label: 'Terrain', emoji: '⛰️' },
    { label: 'Charts', emoji: '📋' },
    { label: 'Flight Plan', emoji: '✈️' },
    { label: 'Weather', emoji: '⛈️' },
    { label: 'Waypoint\nInfo', emoji: '📍' },
    { label: 'Music', emoji: '🎵' },
    { label: 'System', emoji: '⚙️' },
    { label: 'Engine', emoji: '🔧' },
  ];

  const cols = 3;
  const rows = Math.ceil(icons.length / cols);
  const cellW = 110;
  const cellH = 90;
  const gridX = MFD_X + (MFD_W - cols * cellW) / 2;
  const gridY = CONTENT_Y + 50;

  for (let i = 0; i < icons.length; i++) {
    const col = i % cols;
    const row = Math.floor(i / cols);
    const cx = gridX + col * cellW + cellW / 2;
    const cy = gridY + row * cellH + cellH / 2;

    // Icon background box
    ctx.fillStyle = '#1A1A24';
    ctx.strokeStyle = '#333344';
    ctx.lineWidth = 0.5;
    const boxW = 90, boxH = 72;
    ctx.fillRect(cx - boxW / 2, cy - boxH / 2, boxW, boxH);
    ctx.strokeRect(cx - boxW / 2, cy - boxH / 2, boxW, boxH);

    // Emoji / icon
    text(ctx, icons[i].emoji, cx, cy - 10, '24px sans-serif', C.WHITE, 'center');

    // Label
    const lines = icons[i].label.split('\n');
    for (let l = 0; l < lines.length; l++) {
      text(ctx, lines[l], cx, cy + 18 + l * 12, FONT.BTN_LABEL, C.WHITE, 'center');
    }
  }
}

// ─── Main Render Loop ─────────────────────────────────────────────────────
let lastTime = 0;

function render(timestamp) {
  const dt = lastTime ? (timestamp - lastTime) / 1000 : 1 / 60;
  lastTime = timestamp;

  // Update demo if active
  if (demoMode) updateDemo(dt);

  // Smooth all values
  smoothData(dt);

  // Clear canvas
  ctx.fillStyle = C.BG;
  ctx.fillRect(0, 0, W, H);

  // ─── Draw all sections ──────────────────────────────────────────────

  // Header
  drawHeader();

  // PFD components
  const pfdOx = PFD_X;
  const pfdOy = CONTENT_Y;

  drawAttitude(ctx, display, pfdOx, pfdOy);
  drawAirspeedTape(ctx, display, pfdOx, pfdOy);
  drawAltimeterTape(ctx, display, pfdOx, pfdOy);
  drawVSI(ctx, display, pfdOx, pfdOy);
  drawInfoStrip(ctx, display, pfdOx, pfdOy);
  drawHSI(ctx, display, pfdOx, pfdOy);

  // MFD placeholder
  drawMFDPlaceholder();

  // EIS
  drawEIS(ctx, display);

  // Bottom bars
  drawBottomBars();

  requestAnimationFrame(render);
}

requestAnimationFrame(render);
