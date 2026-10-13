# Garmin Trainer (GDU 1060 TXi & GTN 650Xi) ↔ X-Plane 11 Integration Bridge

## 1. Executive Summary & Objective
This project integrates real-time telemetry from **X-Plane 11.5** into official **Garmin Aviation Trainer PC simulators**:
- **Garmin GDU 1060 TXi (Primary Flight Display / MFD)** (`gdu_7xx_simulator.exe`)
- **Garmin GTN 650Xi / GTN 750Xi (Touchscreen GPS Navigator)** (`gtn_simulator.exe`)

The primary challenge was eliminating the trainer's internal demo physics model (`supernav.exe` / internal GDU demo flight model), which continuously overwrites aircraft telemetry, causes display freezes, trips watchdog monitors (`HDG FAIL`, red X's), and produces visual jitter/flicker.

Through extensive reverse engineering and dynamic memory analysis, this project achieves **instantaneous, 60 Hz, clean telemetry injection** for AHRS (Pitch, Roll, Heading) and ADC (Airspeed, Altitude, Vertical Speed) with **zero thread suspension** and **100% readback stability**.

---

## 2. Reverse Engineering Findings & Architecture

### A. GTN 650Xi / Navigation Subsystem (`gtn_simulator.exe`)
- **Inter-Process Communication:** GTN Xi connects to external navigation and flight controls via Win32 Named Shared Memory: `Local\trainerShrSuperObj` (148 bytes).
- **Demo Control Flag:** Byte offset `0x0C` is the demo flight model control flag. Setting byte `0x0C = 0` disables `supernav.exe` from overwriting navigation parameters.
- **Flight Data Struct:** Starts at offset `0x3C` (`22` x `float32` values, 88 bytes):
  - `floats[5]` & `floats[18]`: Altitude MSL (meters)
  - `floats[7]` & `floats[19]`: Indicated Airspeed (knots)
  - `floats[12]` & `floats[21]`: Heading (radians)
  - `floats[14]`: Roll angle (radians)
  - `floats[20]`: Vertical Speed (meters/second)

---

### B. GDU 1060 TXi PFD Subsystem (`gdu_7xx_simulator.exe`)

#### 1. Artificial Horizon & Pitch/Roll Ladder (`cdp_pfd_a_sim.dll`)
- **OpenGL Projection Matrix & Sphere:** Handled inside `cdp_pfd_a_sim.dll` at struct `cdp_pfd_a_sim.dll + 0xBD7348`.
- **Global Variables:**
  - Pitch: `cdp_pfd_a_sim.dll + 0x1E6A0C` (float degrees)
  - Roll: `cdp_pfd_a_sim.dll + 0x1E6A34` (float degrees)
  - Heading: `cdp_pfd_a_sim.dll + 0x1E6A60` (float degrees)
- **Attitude Updater Callback:** RVA `0x2950` in `cdp_pfd_a_sim.dll` is patched with `RET (0xC3)` to prevent internal demo physics from overwriting pitch and roll.
- **Sensor Validity:** Flags at RVAs `0x1E6A08`, `0x1E6A5C`, and `0x1E6A70` are set to `1` (Valid) to prevent AHRS warning banners.

#### 2. Heading, Compass Rose & Digital Heading Box (`cdp_pfd_b_sim.dll`)
- **Compass Rose (HSI):** Fixed RVA at `cdp_pfd_b_sim.dll + 0x3CB9F0` (float degrees) and `+ 0x3CB9EC` (radians).
- **Heading Callback:** RVA `0x18A0` in `cdp_pfd_b_sim.dll` is patched with `RET (0xC3)`.
- **Digital Heading Box (`selected_heading_display`):**
  - RVA `0x000C18BB` in `cdp_pfd_b_sim.dll` formats the float value into the display box.
  - Patched with `movss xmm0, dword ptr [ahrs_hdg]` + NOPs (`\xF3\x0F\x10\x05<addr>\x90...`) to synchronize the digital readout with aircraft heading in real time.

#### 3. Air Data Computer (ADC - Airspeed, Altitude, Vertical Speed)
- **IAS Widget:** Identified dynamically via signature `[float IAS] [20.0f] [999.0f]`. Injected directly at 60 Hz to match GDU display refresh rate.
- **Altitude Tape:** Located at `adc_hdg + 0x1B800`.
- **Vertical Speed Tape:** Located at `addr_gdu_alt + 0x90`.

---

## 3. Key Files Reference

| File | Purpose |
| :--- | :--- |
| `unified_flight_injection.py` | Complete dual-trainer injection engine with automated memory scanning, non-intrusive patches, live streaming, readback verification, and cleanup. |
| `mem_injector.py` | Production memory injection service used by `garmin_bridge.py` for live X-Plane integration. |
| `garmin_bridge.py` | Central middleware integrating X-Plane UDP datarefs with the memory injection engine and GUI. |
| `run_live_test.py` | Standalone 60 Hz live flight profile verification script. |
| `shared_memory.py` | Named shared memory interface for GTN Xi IPC. |
| `xplane_datarefs.py` | UDP dataref reader communicating with X-Plane 11. |
| `GarminHook_v2/` | C++ DLL injection source code for function hooking experiments. |
| `garmin_binaries/` | Curated set of official Garmin simulator executables and DLLs for reverse engineering analysis. |
| `re/decomp/` | Decompiled C# source code of the Aviation Trainer Launcher. |
| `RE_FINDINGS.md` | In-depth reverse engineering notes, struct layouts, and disassembly findings. |

---

## 4. How to Run the Verification Test

Make sure **Garmin Aviation Trainer** is open with **GDU 1060 TXi** and **GTN 650Xi** running, then execute:

```bash
python run_live_test.py 30
```

This runs a 30-second live test at 60 Hz with dynamic pitch, roll, heading sweep, airspeed, and altitude, demonstrating smooth, rock-solid instrumentation without freezes or fighting.
