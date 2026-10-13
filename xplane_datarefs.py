"""
xplane_datarefs.py — X-Plane DataRef reader for GarminBridge

Reads flight data from X-Plane 11 using the XPlaneConnect library (xpc.py)
and provides it in a structured format for the shared memory bridge.
"""

import time
import threading
from xpc import XPlaneConnect


# All datarefs we need from X-Plane, grouped by category
DATAREFS = {
    # GPS Position
    'latitude':         'sim/flightmodel/position/latitude',
    'longitude':        'sim/flightmodel/position/longitude',
    'altitude_msl_m':   'sim/flightmodel/position/elevation',
    'altitude_agl_m':   'sim/flightmodel/position/y_agl',
    'groundspeed_ms':   'sim/flightmodel/position/groundspeed',

    # Attitude
    'pitch_deg':        'sim/flightmodel/position/true_theta',
    'roll_deg':         'sim/flightmodel/position/true_phi',
    'mag_heading_deg':  'sim/flightmodel/position/mag_psi',
    'true_heading_deg': 'sim/flightmodel/position/true_psi',

    # Air Data
    'ias_kts':          'sim/flightmodel/position/indicated_airspeed',
    'tas_kts':          'sim/flightmodel/position/true_airspeed',
    'vs_fpm':           'sim/flightmodel/position/vh_ind_fpm',
    'baro_inhg':        'sim/weather/barometer_sealevel_inhg',
    'indicated_alt_ft': 'sim/cockpit2/gauges/indicators/altitude_ft_pilot',
    'oat_deg_c':        'sim/cockpit2/temperature/outside_air_temp_degc',

    # Slip/Turn
    'slip_deg':         'sim/cockpit2/gauges/indicators/slip_deg',
    'turn_rate_deg_s':  'sim/cockpit2/gauges/indicators/turn_rate_heading_deg_pilot',

    # Engine (single turboprop, index [0])
    'n1_percent':       'sim/cockpit2/engine/indicators/N1_percent',
    'n2_percent':       'sim/cockpit2/engine/indicators/N2_percent',
    'torque_ftlb':      'sim/cockpit2/engine/indicators/torque_n_mtr',
    'itt_deg_c':        'sim/cockpit2/engine/indicators/ITT_deg_C',
    'oil_press_psi':    'sim/cockpit2/engine/indicators/oil_pressure_psi',
    'oil_temp_deg_c':   'sim/cockpit2/engine/indicators/oil_temperature_deg_C',
    'fuel_flow_pph':    'sim/cockpit2/engine/indicators/fuel_flow_kg_sec',
    'prop_rpm':         'sim/cockpit2/engine/indicators/prop_speed_rpm',

    # Fuel
    'fuel_qty_kg_l':    'sim/cockpit2/fuel/fuel_quantity',

    # Engine running
    'eng_running':      'sim/flightmodel/engine/ENGN_running',

    # Electrical
    'bus_volts':        'sim/cockpit2/electrical/bus_volts',
    'gen_amps':         'sim/cockpit2/electrical/generator_amps',

    # Radios
    'com1_active_khz':  'sim/cockpit2/radios/actuators/com1_frequency_hz_833',
    'com1_standby_khz': 'sim/cockpit2/radios/actuators/com1_standby_frequency_hz_833',
    'com2_active_khz':  'sim/cockpit2/radios/actuators/com2_frequency_hz_833',
    'com2_standby_khz': 'sim/cockpit2/radios/actuators/com2_standby_frequency_hz_833',
    'nav1_active_khz':  'sim/cockpit2/radios/actuators/nav1_frequency_hz',
    'nav1_standby_khz': 'sim/cockpit2/radios/actuators/nav1_standby_frequency_hz',
    'nav2_active_khz':  'sim/cockpit2/radios/actuators/nav2_frequency_hz',
    'nav2_standby_khz': 'sim/cockpit2/radios/actuators/nav2_standby_frequency_hz',

    # Nav indicators
    'nav1_hdef_dots':   'sim/cockpit/radios/nav1_hdef_dot',
    'nav1_vdef_dots':   'sim/cockpit/radios/nav1_vdef_dot',
    'gps_track_deg':    'sim/cockpit2/gauges/indicators/ground_track_mag_pilot',

    # Time
    'utc_hours':        'sim/cockpit2/clock_timer/zulu_time_hours',
    'utc_minutes':      'sim/cockpit2/clock_timer/zulu_time_minutes',
    'utc_seconds':      'sim/cockpit2/clock_timer/zulu_time_seconds',
}

# Torque conversion: N·m to ft·lb
NM_TO_FTLB = 0.7375621

# Fuel flow conversion: kg/sec to lbs/hr
KGSEC_TO_PPH = 7936.641


class XPlaneDataReader:
    """
    Reads datarefs from X-Plane and returns them as a dictionary
    compatible with the shared memory structure.
    """

    def __init__(self, xplane_host='127.0.0.1', xplane_port=49009):
        self.host = xplane_host
        self.port = xplane_port
        self.client = None
        self.connected = False
        self._dref_list = list(DATAREFS.values())
        self._dref_names = list(DATAREFS.keys())
        self._lock = threading.Lock()
        self._last_data = {}
        self._read_count = 0
        self._error_count = 0

    def connect(self):
        """Establish connection to X-Plane."""
        try:
            self.client = XPlaneConnect(self.host, self.port)
            # Test connection
            self.client.getDREF('sim/test/test_float')
            self.connected = True
            self._error_count = 0
            print(f"[XPlane] Conectado a X-Plane em {self.host}:{self.port}")
            return True
        except Exception as e:
            self.connected = False
            print(f"[XPlane] Erro ao conectar: {e}")
            return False

    def disconnect(self):
        """Close connection to X-Plane."""
        if self.client:
            try:
                self.client.close()
            except:
                pass
            self.client = None
            self.connected = False

    def read_all(self):
        """
        Read all datarefs from X-Plane.
        Returns a dict compatible with shared memory write_xplane_data().
        Returns None if read fails.
        """
        if not self.connected:
            if not self.connect():
                return None

        try:
            result = {}

            # Read datarefs in batches to avoid overloading
            # XPlaneConnect can only handle a limited message size
            batch_size = 10
            raw_values = {}

            for i in range(0, len(self._dref_list), batch_size):
                batch = self._dref_list[i:i+batch_size]
                batch_names = self._dref_names[i:i+batch_size]

                values = self.client.getDREFs(batch)

                for name, val in zip(batch_names, values):
                    # getDREFs returns arrays, take first element
                    if val and len(val) > 0:
                        raw_values[name] = val[0]
                    else:
                        raw_values[name] = 0.0

            # Post-process values
            result = dict(raw_values)

            # Convert torque N·m to ft·lb
            if 'torque_ftlb' in result:
                result['torque_ftlb'] = result['torque_ftlb'] * NM_TO_FTLB

            # Compute torque percent (approximate for PT6, max ~1600 ft-lb)
            result['torque_percent'] = min(100.0, max(0.0,
                result.get('torque_ftlb', 0) / 16.0))  # rough approximation

            # Convert fuel flow kg/sec to lbs/hr
            if 'fuel_flow_pph' in result:
                result['fuel_flow_pph'] = result['fuel_flow_pph'] * KGSEC_TO_PPH

            # Engine running as integer
            result['eng_running'] = int(result.get('eng_running', 0))

            # Radio frequencies — ensure integer
            for key in ['com1_active_khz', 'com1_standby_khz',
                        'com2_active_khz', 'com2_standby_khz',
                        'nav1_active_khz', 'nav1_standby_khz',
                        'nav2_active_khz', 'nav2_standby_khz']:
                result[key] = int(result.get(key, 0))

            # Data valid flags
            data_valid = 0x01  # GPS always valid if we got data
            if abs(result.get('pitch_deg', 0)) < 180:
                data_valid |= 0x02  # AHRS valid
            if result.get('eng_running', 0) or result.get('n1_percent', 0) > 0:
                data_valid |= 0x04  # EIS valid
            data_valid |= 0x08  # Radio always valid
            result['data_valid'] = data_valid

            with self._lock:
                self._last_data = result
                self._read_count += 1
                self._error_count = 0

            return result

        except Exception as e:
            self._error_count += 1
            if self._error_count > 5:
                print(f"[XPlane] Muitos erros consecutivos, reconectando...")
                self.disconnect()
            return self._last_data if self._last_data else None

    def get_last_data(self):
        """Get the last successfully read data."""
        with self._lock:
            return dict(self._last_data)

    def get_stats(self):
        """Get reader statistics."""
        return {
            'connected': self.connected,
            'read_count': self._read_count,
            'error_count': self._error_count,
        }

    def write_radio_to_xplane(self, garmin_data):
        """
        Write radio frequency changes from Garmin back to X-Plane.
        Called when the pilot changes frequencies in the Garmin unit.
        """
        if not self.connected or not garmin_data:
            return

        try:
            # Only write if Garmin has valid data
            if garmin_data.get('data_valid', 0) == 0:
                return

            # COM1
            if garmin_data.get('com1_active_khz', 0) > 0:
                self.client.sendDREF(
                    'sim/cockpit2/radios/actuators/com1_frequency_hz_833',
                    garmin_data['com1_active_khz']
                )
            if garmin_data.get('com1_standby_khz', 0) > 0:
                self.client.sendDREF(
                    'sim/cockpit2/radios/actuators/com1_standby_frequency_hz_833',
                    garmin_data['com1_standby_khz']
                )

            # NAV1
            if garmin_data.get('nav1_active_khz', 0) > 0:
                self.client.sendDREF(
                    'sim/cockpit2/radios/actuators/nav1_frequency_hz',
                    garmin_data['nav1_active_khz']
                )
            if garmin_data.get('nav1_standby_khz', 0) > 0:
                self.client.sendDREF(
                    'sim/cockpit2/radios/actuators/nav1_standby_frequency_hz',
                    garmin_data['nav1_standby_khz']
                )

        except Exception as e:
            print(f"[XPlane] Erro ao escrever rádio: {e}")


if __name__ == '__main__':
    reader = XPlaneDataReader()
    if reader.connect():
        print("\nLendo datarefs do X-Plane...\n")
        for i in range(10):
            data = reader.read_all()
            if data:
                print(f"[{i}] lat={data.get('latitude', 0):.4f} "
                      f"lon={data.get('longitude', 0):.4f} "
                      f"alt={data.get('altitude_msl_m', 0):.0f}m "
                      f"hdg={data.get('mag_heading_deg', 0):.0f}° "
                      f"ias={data.get('ias_kts', 0):.0f}kt "
                      f"n1={data.get('n1_percent', 0):.1f}%")
            time.sleep(0.5)
        reader.disconnect()
    else:
        print("Não foi possível conectar ao X-Plane.")
