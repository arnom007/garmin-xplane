// AUTO-GENERATED — Shared memory structures for GarminBridge
// This header must match the Python struct layout in shared_memory.py exactly.

#pragma once
#pragma pack(push, 1)

#define SHM_NAME_XP_TO_GARMIN L"Local\\GarminBridge_XPtoGarmin"
#define SHM_NAME_GARMIN_TO_XP L"Local\\GarminBridge_GarminToXP"

#define MAGIC_XP_TO_GARMIN 0x47425250  // "GBRP"
#define MAGIC_GARMIN_TO_XP 0x47425247  // "GBRG"

// Data from X-Plane to Garmin (Python writes, DLL reads)
typedef struct {
    // Header
    unsigned int         magic;
    unsigned int         sequence;
    double               timestamp;

    // GPS Position
    double               latitude;          // degrees
    double               longitude;         // degrees
    float                altitude_msl_m;    // meters MSL
    float                altitude_agl_m;    // meters AGL
    float                groundspeed_ms;    // m/s

    // Attitude (AHRS)
    float                pitch_deg;         // nose up positive
    float                roll_deg;          // right wing down positive
    float                mag_heading_deg;   // degrees magnetic
    float                true_heading_deg;  // degrees true

    // Air Data
    float                ias_kts;           // indicated airspeed knots
    float                tas_kts;           // true airspeed knots
    float                vs_fpm;            // vertical speed ft/min
    float                baro_inhg;         // barometric pressure inHg
    float                indicated_alt_ft;  // indicated altitude ft
    float                oat_deg_c;         // outside air temp C

    // Slip/Turn
    float                slip_deg;
    float                turn_rate_deg_s;

    // Engine (PT6 single turboprop)
    float                n1_percent;
    float                n2_percent;
    float                torque_ftlb;
    float                torque_percent;
    float                itt_deg_c;
    float                oil_press_psi;
    float                oil_temp_deg_c;
    float                fuel_flow_pph;     // lbs/hr
    float                fuel_qty_kg_l;     // left tank kg
    float                fuel_qty_kg_r;     // right tank kg
    float                prop_rpm;
    unsigned int         eng_running;       // 0=off, 1=running

    // Electrical
    float                bus_volts;
    float                gen_amps;

    // Radio
    unsigned int         com1_active_khz;
    unsigned int         com1_standby_khz;
    unsigned int         com2_active_khz;
    unsigned int         com2_standby_khz;
    unsigned int         nav1_active_khz;
    unsigned int         nav1_standby_khz;
    unsigned int         nav2_active_khz;
    unsigned int         nav2_standby_khz;

    // Navigation
    float                nav1_hdef_dots;
    float                nav1_vdef_dots;
    float                gps_track_deg;

    // DateTime
    float                utc_hours;
    float                utc_minutes;
    float                utc_seconds;

    // Flags
    unsigned int         data_valid;        // bitmask: 1=GPS, 2=AHRS, 4=EIS, 8=RADIO
} XPToGarminData;

// Data from Garmin to X-Plane (DLL writes, Python reads)
typedef struct {
    // Header
    unsigned int         magic;
    unsigned int         sequence;
    double               timestamp;

    // Radio frequencies
    unsigned int         com1_active_khz;
    unsigned int         com1_standby_khz;
    unsigned int         com2_active_khz;
    unsigned int         com2_standby_khz;
    unsigned int         nav1_active_khz;
    unsigned int         nav1_standby_khz;
    unsigned int         nav2_active_khz;
    unsigned int         nav2_standby_khz;
    unsigned int         xpdr_code;
    unsigned int         xpdr_mode;

    // CDI/OBS
    float                obs_setting_deg;
    unsigned int         cdi_source;        // 0=GPS, 1=NAV1, 2=NAV2, 3=LOC

    // Autopilot
    float                ap_heading_deg;
    float                ap_altitude_ft;
    float                ap_vs_fpm;

    // Status
    unsigned int         data_valid;
    unsigned int         garmin_status;     // 0=disconnected, 1=running
} GarminToXPData;

#pragma pack(pop)
