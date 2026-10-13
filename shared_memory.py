"""
shared_memory.py — Windows Shared Memory IPC for GarminBridge

Provides a structured shared memory region for communication between:
  - Python middleware (garmin_bridge.py) ↔ GarminHook.dll (injected in Garmin Trainer)

Two shared memory regions:
  1. XP_TO_GARMIN: X-Plane data → Garmin Trainer (written by Python, read by DLL)
  2. GARMIN_TO_XP: Garmin Trainer data → X-Plane (written by DLL, read by Python)
"""

import ctypes
import ctypes.wintypes as wintypes
import struct
import time
import threading

# Windows API constants
FILE_MAP_ALL_ACCESS = 0xF001F
PAGE_READWRITE = 0x04
INVALID_HANDLE_VALUE = ctypes.c_void_p(-1).value

# Windows API functions
kernel32 = ctypes.WinDLL('kernel32', use_last_error=True)

CreateFileMappingW = kernel32.CreateFileMappingW
CreateFileMappingW.restype = wintypes.HANDLE
CreateFileMappingW.argtypes = [
    wintypes.HANDLE,   # hFile
    ctypes.c_void_p,   # lpAttributes
    wintypes.DWORD,    # flProtect
    wintypes.DWORD,    # dwMaximumSizeHigh
    wintypes.DWORD,    # dwMaximumSizeLow
    wintypes.LPCWSTR,  # lpName
]

OpenFileMappingW = kernel32.OpenFileMappingW
OpenFileMappingW.restype = wintypes.HANDLE
OpenFileMappingW.argtypes = [
    wintypes.DWORD,    # dwDesiredAccess
    wintypes.BOOL,     # bInheritHandle
    wintypes.LPCWSTR,  # lpName
]

MapViewOfFile = kernel32.MapViewOfFile
MapViewOfFile.restype = ctypes.c_void_p
MapViewOfFile.argtypes = [
    wintypes.HANDLE,   # hFileMappingObject
    wintypes.DWORD,    # dwDesiredAccess
    wintypes.DWORD,    # dwFileOffsetHigh
    wintypes.DWORD,    # dwFileOffsetLow
    ctypes.c_size_t,   # dwNumberOfBytesToMap
]

UnmapViewOfFile = kernel32.UnmapViewOfFile
UnmapViewOfFile.restype = wintypes.BOOL
UnmapViewOfFile.argtypes = [ctypes.c_void_p]

CloseHandle = kernel32.CloseHandle
CloseHandle.restype = wintypes.BOOL
CloseHandle.argtypes = [wintypes.HANDLE]


# ============================================================
# Shared Memory Structures — must match C++ struct in DLL
# ============================================================

# Data from X-Plane → Garmin (Python writes, DLL reads)
# Total size: carefully packed for C interop
XP_TO_GARMIN_FORMAT = '<'  # little-endian
XP_TO_GARMIN_FIELDS = [
    # Header
    ('magic',           'I'),   # 0x47425250 = "GBRP" (Garmin Bridge Protocol)
    ('sequence',        'I'),   # incrementing sequence number
    ('timestamp',       'd'),   # time.time()

    # GPS Position
    ('latitude',        'd'),   # degrees
    ('longitude',       'd'),   # degrees
    ('altitude_msl_m',  'f'),   # meters MSL
    ('altitude_agl_m',  'f'),   # meters AGL
    ('groundspeed_ms',  'f'),   # m/s

    # Attitude (AHRS)
    ('pitch_deg',       'f'),   # degrees (nose up positive)
    ('roll_deg',        'f'),   # degrees (right wing down positive)
    ('mag_heading_deg', 'f'),   # degrees magnetic
    ('true_heading_deg','f'),   # degrees true

    # Air Data
    ('ias_kts',         'f'),   # indicated airspeed knots
    ('tas_kts',         'f'),   # true airspeed knots
    ('vs_fpm',          'f'),   # vertical speed ft/min
    ('baro_inhg',       'f'),   # barometric pressure inHg
    ('indicated_alt_ft','f'),   # indicated altitude ft
    ('oat_deg_c',       'f'),   # outside air temp °C

    # Slip/Turn
    ('slip_deg',        'f'),   # slip/skid degrees
    ('turn_rate_deg_s', 'f'),   # turn rate deg/s

    # Engine (PT6 turboprop - single engine)
    ('n1_percent',      'f'),   # N1 / Ng percent
    ('n2_percent',      'f'),   # N2 / Np percent (if available)
    ('torque_ftlb',     'f'),   # torque ft-lb
    ('torque_percent',  'f'),   # torque percent
    ('itt_deg_c',       'f'),   # ITT °C
    ('oil_press_psi',   'f'),   # oil pressure PSI
    ('oil_temp_deg_c',  'f'),   # oil temperature °C
    ('fuel_flow_pph',   'f'),   # fuel flow lbs/hr
    ('fuel_qty_kg_l',   'f'),   # fuel quantity left tank kg
    ('fuel_qty_kg_r',   'f'),   # fuel quantity right tank kg
    ('prop_rpm',        'f'),   # propeller RPM
    ('eng_running',     'I'),   # 0=off, 1=running

    # Electrical
    ('bus_volts',       'f'),   # main bus voltage
    ('gen_amps',        'f'),   # generator amps

    # Radio (from X-Plane current state)
    ('com1_active_khz', 'I'),   # COM1 active freq in kHz
    ('com1_standby_khz','I'),   # COM1 standby freq in kHz
    ('com2_active_khz', 'I'),   # COM2 active freq in kHz
    ('com2_standby_khz','I'),   # COM2 standby freq in kHz
    ('nav1_active_khz', 'I'),   # NAV1 active freq in kHz
    ('nav1_standby_khz','I'),   # NAV1 standby freq in kHz
    ('nav2_active_khz', 'I'),   # NAV2 active freq in kHz
    ('nav2_standby_khz','I'),   # NAV2 standby freq in kHz

    # Navigation
    ('nav1_hdef_dots',  'f'),   # CDI horizontal deflection dots
    ('nav1_vdef_dots',  'f'),   # GS vertical deflection dots
    ('gps_track_deg',   'f'),   # GPS ground track degrees

    # DateTime
    ('utc_hours',       'f'),   # UTC time hours
    ('utc_minutes',     'f'),   # UTC time minutes
    ('utc_seconds',     'f'),   # UTC time seconds

    # Flags
    ('data_valid',      'I'),   # bitmask: 1=GPS, 2=AHRS, 4=EIS, 8=RADIO

    # SIM Mode Control (sent to DLL)
    ('sim_mode_override', 'I'), # 0-4 = force mode, 0xFF = no override
]

# Data from Garmin → X-Plane (DLL writes, Python reads)
GARMIN_TO_XP_FORMAT = '<'
GARMIN_TO_XP_FIELDS = [
    # Header
    ('magic',           'I'),   # 0x47425247 = "GBRG" (Garmin Bridge Garmin)
    ('sequence',        'I'),
    ('timestamp',       'd'),

    # Radio frequencies set in Garmin
    ('com1_active_khz', 'I'),
    ('com1_standby_khz','I'),
    ('com2_active_khz', 'I'),
    ('com2_standby_khz','I'),
    ('nav1_active_khz', 'I'),
    ('nav1_standby_khz','I'),
    ('nav2_active_khz', 'I'),
    ('nav2_standby_khz','I'),
    ('xpdr_code',       'I'),   # transponder code (squawk)
    ('xpdr_mode',       'I'),   # 0=off, 1=stby, 2=on, 3=alt

    # CDI/OBS from Garmin
    ('obs_setting_deg', 'f'),
    ('cdi_source',      'I'),   # 0=GPS, 1=NAV1, 2=NAV2, 3=LOC

    # Autopilot commands from Garmin
    ('ap_heading_deg',  'f'),
    ('ap_altitude_ft',  'f'),
    ('ap_vs_fpm',       'f'),

    # Status flags
    ('data_valid',      'I'),   # bitmask
    ('garmin_status',   'I'),   # 0=disconnected, 1=running
]

MAGIC_XP_TO_GARMIN = 0x47425250  # "GBRP"
MAGIC_GARMIN_TO_XP = 0x47425247  # "GBRG"

SHM_NAME_XP_TO_GARMIN = "Local\\GarminBridge_XPtoGarmin"
SHM_NAME_GARMIN_TO_XP = "Local\\GarminBridge_GarminToXP"


def _build_format_string(fields):
    """Build a struct format string from field definitions."""
    return '<' + ''.join(fmt for _, fmt in fields)


def _calc_size(fields):
    """Calculate total size of the struct."""
    return struct.calcsize(_build_format_string(fields))


def _field_names(fields):
    """Get ordered list of field names."""
    return [name for name, _ in fields]


class SharedMemoryRegion:
    """Manages a named shared memory region on Windows."""

    def __init__(self, name, fields, create=True):
        self.name = name
        self.fields = fields
        self.fmt = _build_format_string(fields)
        self.size = struct.calcsize(self.fmt)
        self.field_names = _field_names(fields)
        self.handle = None
        self.address = None
        self._lock = threading.Lock()

        if create:
            self._create()
        else:
            self._open()

    def _create(self):
        """Create a new shared memory region."""
        self.handle = CreateFileMappingW(
            INVALID_HANDLE_VALUE,
            None,
            PAGE_READWRITE,
            0,
            self.size,
            self.name
        )
        if not self.handle:
            raise OSError(f"CreateFileMapping failed: {ctypes.get_last_error()}")

        self.address = MapViewOfFile(
            self.handle,
            FILE_MAP_ALL_ACCESS,
            0, 0,
            self.size
        )
        if not self.address:
            CloseHandle(self.handle)
            raise OSError(f"MapViewOfFile failed: {ctypes.get_last_error()}")

        # Zero-initialize
        ctypes.memset(self.address, 0, self.size)

    def _open(self):
        """Open an existing shared memory region."""
        self.handle = OpenFileMappingW(
            FILE_MAP_ALL_ACCESS,
            False,
            self.name
        )
        if not self.handle:
            raise OSError(f"OpenFileMapping failed for '{self.name}': {ctypes.get_last_error()}")

        self.address = MapViewOfFile(
            self.handle,
            FILE_MAP_ALL_ACCESS,
            0, 0,
            self.size
        )
        if not self.address:
            CloseHandle(self.handle)
            raise OSError(f"MapViewOfFile failed: {ctypes.get_last_error()}")

    def write(self, **kwargs):
        """Write values to shared memory. Only specified fields are updated."""
        with self._lock:
            # Read current data
            raw = ctypes.string_at(self.address, self.size)
            values = list(struct.unpack(self.fmt, raw))

            # Update specified fields
            for name, value in kwargs.items():
                idx = self.field_names.index(name)
                values[idx] = value

            # Write back
            packed = struct.pack(self.fmt, *values)
            ctypes.memmove(self.address, packed, self.size)

    def write_all(self, values_dict):
        """Write all fields at once from a dictionary."""
        with self._lock:
            values = []
            for name, fmt in self.fields:
                val = values_dict.get(name, 0)
                values.append(val)
            packed = struct.pack(self.fmt, *values)
            ctypes.memmove(self.address, packed, self.size)

    def read(self):
        """Read all values from shared memory. Returns a dict."""
        with self._lock:
            raw = ctypes.string_at(self.address, self.size)
            values = struct.unpack(self.fmt, raw)
            return dict(zip(self.field_names, values))

    def read_field(self, field_name):
        """Read a single field."""
        data = self.read()
        return data[field_name]

    def close(self):
        """Cleanup shared memory handles."""
        if self.address:
            UnmapViewOfFile(self.address)
            self.address = None
        if self.handle:
            CloseHandle(self.handle)
            self.handle = None

    def __del__(self):
        self.close()

    def __enter__(self):
        return self

    def __exit__(self, *args):
        self.close()


class GarminBridgeSharedMemory:
    """
    High-level interface for the GarminBridge shared memory regions.
    
    Usage (Python middleware):
        shm = GarminBridgeSharedMemory()
        
        # Write X-Plane data for Garmin to read
        shm.write_xplane_data(latitude=..., longitude=..., ...)
        
        # Read Garmin data (frequencies, transponder, etc.)
        garmin_data = shm.read_garmin_data()
    """

    def __init__(self):
        self.xp_to_garmin = SharedMemoryRegion(
            SHM_NAME_XP_TO_GARMIN,
            XP_TO_GARMIN_FIELDS,
            create=True
        )
        self.garmin_to_xp = SharedMemoryRegion(
            SHM_NAME_GARMIN_TO_XP,
            GARMIN_TO_XP_FIELDS,
            create=True
        )
        self._sequence_xp = 0
        print(f"[SharedMemory] Created regions:")
        print(f"  XP->Garmin: {SHM_NAME_XP_TO_GARMIN} ({self.xp_to_garmin.size} bytes)")
        print(f"  Garmin->XP: {SHM_NAME_GARMIN_TO_XP} ({self.garmin_to_xp.size} bytes)")

    def write_xplane_data(self, **data):
        """Write X-Plane flight data to shared memory for Garmin to read."""
        self._sequence_xp += 1
        data['magic'] = MAGIC_XP_TO_GARMIN
        data['sequence'] = self._sequence_xp
        data['timestamp'] = time.time()
        # Include SIM mode override (set by GUI, 0xFF = no override)
        if 'sim_mode_override' not in data:
            data['sim_mode_override'] = getattr(self, '_sim_mode_override', 0xFF)
        self.xp_to_garmin.write_all(data)

    def read_garmin_data(self):
        """Read data that Garmin has written (frequencies, OBS, etc.)."""
        data = self.garmin_to_xp.read()
        if data.get('magic') != MAGIC_GARMIN_TO_XP:
            return None  # No valid data yet
        return data

    def close(self):
        self.xp_to_garmin.close()
        self.garmin_to_xp.close()

    def __enter__(self):
        return self

    def __exit__(self, *args):
        self.close()


# ============================================================
# C Header Generation — for the DLL to include
# ============================================================

def generate_c_header():
    """Generate a C header file that matches the shared memory layout."""
    header = """// AUTO-GENERATED by shared_memory.py — DO NOT EDIT MANUALLY
// This header must match the Python struct layout exactly.

#pragma once
#pragma pack(push, 1)

#define SHM_NAME_XP_TO_GARMIN L"Global\\\\GarminBridge_XPtoGarmin"
#define SHM_NAME_GARMIN_TO_XP L"Global\\\\GarminBridge_GarminToXP"

#define MAGIC_XP_TO_GARMIN 0x47425250  // "GBRP"
#define MAGIC_GARMIN_TO_XP 0x47425247  // "GBRG"

// Data from X-Plane → Garmin (Python writes, DLL reads)
typedef struct {
"""
    c_type_map = {
        'I': 'unsigned int',
        'i': 'int',
        'f': 'float',
        'd': 'double',
        'H': 'unsigned short',
        'h': 'short',
        'B': 'unsigned char',
        'b': 'char',
    }

    for name, fmt in XP_TO_GARMIN_FIELDS:
        ctype = c_type_map.get(fmt, f'/* unknown: {fmt} */')
        header += f"    {ctype:20s} {name};\n"

    header += """} XPToGarminData;

// Data from Garmin → X-Plane (DLL writes, Python reads)
typedef struct {
"""
    for name, fmt in GARMIN_TO_XP_FIELDS:
        ctype = c_type_map.get(fmt, f'/* unknown: {fmt} */')
        header += f"    {ctype:20s} {name};\n"

    header += """} GarminToXPData;

#pragma pack(pop)

// Sizes for validation
"""
    header += f"#define SIZEOF_XP_TO_GARMIN {_calc_size(XP_TO_GARMIN_FIELDS)}\n"
    header += f"#define SIZEOF_GARMIN_TO_XP {_calc_size(GARMIN_TO_XP_FIELDS)}\n"

    return header


if __name__ == '__main__':
    # Generate C header
    header = generate_c_header()
    with open('garmin_bridge_shm.h', 'w') as f:
        f.write(header)
    print(f"Generated garmin_bridge_shm.h")
    print(f"XP→Garmin struct size: {_calc_size(XP_TO_GARMIN_FIELDS)} bytes")
    print(f"Garmin→XP struct size: {_calc_size(GARMIN_TO_XP_FIELDS)} bytes")

    # Quick test
    print("\nTesting shared memory...")
    with GarminBridgeSharedMemory() as shm:
        shm.write_xplane_data(
            latitude=-15.8697,
            longitude=-47.9172,
            altitude_msl_m=1000.0,
            mag_heading_deg=180.0,
            ias_kts=120.0,
            data_valid=0x0F
        )
        data = shm.xp_to_garmin.read()
        print(f"  Written lat={data['latitude']:.4f}, lon={data['longitude']:.4f}")
        print(f"  alt={data['altitude_msl_m']:.0f}m, hdg={data['mag_heading_deg']:.0f}°")
        print("  OK!")
