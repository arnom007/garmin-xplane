// ─── GDU TXi 1060 Configuration ───────────────────────────────────────────
// Colors and layout constants derived from actual Garmin TXi trainer screenshots

// ─── COLORS ───────────────────────────────────────────────────────────────
export const C = {
  BG:             '#000000',

  // Attitude indicator
  SKY_TOP:        '#0A2E8A',
  SKY_HORIZON:    '#4488CC',
  GND_HORIZON:    '#8A6B20',
  GND_BOTTOM:     '#5A3A08',
  HORIZON_LINE:   '#FFFFFF',

  // Aircraft reference symbol
  AIRCRAFT_SYM:   '#C8B000',

  // Tapes
  TAPE_BG:        'rgba(8,8,14,0.92)',
  TAPE_TEXT:       '#FFFFFF',
  TAPE_TICK:       '#FFFFFF',

  // Speed color arcs
  SPD_WHITE:      '#FFFFFF',
  SPD_GREEN:      '#00AA00',
  SPD_YELLOW:     '#CCCC00',
  SPD_RED:        '#FF0000',

  // Pointer boxes
  PTR_BG:         '#000000',
  PTR_BORDER:     '#888888',

  // Bugs & trends
  BUG_CYAN:       '#00CCCC',
  TREND_MAGENTA:  '#FF00FF',

  // HSI
  HSI_BG:         '#0A0A0E',
  HSI_RING:       '#FFFFFF',
  HSI_TICK:       '#FFFFFF',

  // Labels & indicators
  GPS_GREEN:      '#00DD00',
  BARO_CYAN:      '#00CCDD',
  HDG_CYAN:       '#00CCCC',
  CRS_GREEN:      '#00CC00',
  VS_WHITE:       '#FFFFFF',
  MAGENTA:        '#FF00FF',

  // EIS gauge arcs
  EIS_ARC_GREEN:  '#00BB00',
  EIS_ARC_YELLOW: '#CCCC00',
  EIS_ARC_RED:    '#CC0000',
  EIS_ARC_BG:     '#333333',
  EIS_LABEL:      '#AAAAAA',
  EIS_VALUE_GREEN:'#00FF00',
  EIS_VALUE_YELLOW:'#FFFF00',
  EIS_VALUE_RED:  '#FF2222',

  // General
  WHITE:          '#FFFFFF',
  GREEN:          '#00CC00',
  CYAN:           '#00CCCC',
  YELLOW:         '#CCCC00',
  AMBER:          '#FF8800',
  RED:            '#FF0000',
  DIM:            '#666666',
  PANEL_BORDER:   '#333333',
};

// ─── FONTS ────────────────────────────────────────────────────────────────
export const FONT = {
  GARMIN_TITLE:   '500 15px "Roboto Condensed", sans-serif',
  TAPE_NUM:       '500 13px "Roboto Condensed", sans-serif',
  TAPE_PTR:       '700 17px "Roboto Condensed", sans-serif',
  TAPE_PTR_SM:    '700 13px "Roboto Condensed", sans-serif',
  PITCH_LABEL:    '400 10px "Roboto Condensed", sans-serif',
  HSI_NUM:        '500 13px "Roboto Condensed", sans-serif',
  HSI_HDG:        '700 22px "Roboto Condensed", sans-serif',
  INFO_LABEL:     '400 11px "Roboto Condensed", sans-serif',
  INFO_VALUE:     '500 12px "Roboto Condensed", sans-serif',
  INFO_BOX:       '700 14px "Roboto Condensed", sans-serif',
  EIS_LABEL:      '400 10px "Roboto Condensed", sans-serif',
  EIS_VALUE:      '700 22px "Roboto Condensed", sans-serif',
  EIS_UNIT:       '400 9px "Roboto Condensed", sans-serif',
  EIS_SMALL:      '500 11px "Roboto Condensed", sans-serif',
  BTN_LABEL:      '400 11px "Roboto Condensed", sans-serif',
  STATUS_LABEL:   '500 10px "Roboto Condensed", sans-serif',
};

// ─── LAYOUT ───────────────────────────────────────────────────────────────
// GDU TXi 1060: 1280×768 pixels
export const W = 1280;
export const H = 768;

export const HEADER_H = 24;
export const BTN_BAR_H = 28;
export const STATUS_H = 18;

export const CONTENT_Y = HEADER_H;
export const CONTENT_H = H - HEADER_H - BTN_BAR_H - STATUS_H; // 698

// Three-column layout: PFD | MFD | EIS
export const PFD_X = 0;
export const PFD_W = 444;

export const MFD_X = 446;
export const MFD_W = 470;

export const EIS_X = 918;
export const EIS_W = 362;

// ─── PFD INTERNAL LAYOUT ─────────────────────────────────────────────────
export const PFD = {
  // Airspeed tape (left strip)
  IAS_X: 0,
  IAS_W: 54,

  // Attitude indicator
  ATT_X: 54,
  ATT_W: 290,

  // Altimeter tape (right of attitude)
  ALT_X: 344,
  ALT_W: 68,

  // VSI (far right)
  VSI_X: 412,
  VSI_W: 32,

  // Vertical split
  ATT_H: 350,         // Attitude indicator height
  INFO_H: 58,         // Info strip (TAS, GPS, baro, HDG, CRS, VS)
  // HSI_H = CONTENT_H - ATT_H - INFO_H = 290

  get HSI_H() { return CONTENT_H - this.ATT_H - this.INFO_H; },
  get ATT_Y() { return CONTENT_Y; },
  get INFO_Y() { return CONTENT_Y + this.ATT_H; },
  get HSI_Y() { return CONTENT_Y + this.ATT_H + this.INFO_H; },
};

// ─── T-27 AIRCRAFT DATA ──────────────────────────────────────────────────
export const T27 = {
  // Speed bands (kts)
  VS0:  55,   // Stall speed clean
  VS1:  48,   // Stall speed landing config
  VFE:  130,  // Max flap extended
  VNO:  210,  // Max structural cruising
  VNE:  280,  // Never exceed

  // EIS limits
  TRQ_MAX: 133,       // % scale
  TRQ_GREEN: [0, 100],
  TRQ_YELLOW: [100, 115],
  TRQ_RED: [115, 133],

  NH_MAX: 106,        // % propeller
  NH_GREEN: [60, 100],
  NH_YELLOW: [100, 104],
  NH_RED: [104, 106],

  NG_MAX: 114,        // % gas generator
  NG_GREEN: [55, 101.5],
  NG_YELLOW: [101.5, 104],
  NG_RED: [104, 114],

  T5_MIN: 200,        // °C
  T5_MAX: 1190,       // °C scale
  T5_GREEN: [200, 770],
  T5_YELLOW: [770, 810],
  T5_RED: [810, 1190],
};
