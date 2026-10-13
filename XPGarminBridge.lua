-- ============================================================
-- XPGarminBridge.lua — FlyWithLua Script for X-Plane 11
-- 
-- Sends flight data via UDP to the GarminBridge Python middleware.
-- Receives radio frequency updates back from Garmin.
--
-- Install: Copy to Resources/plugins/FlyWithLua/Scripts/
-- ============================================================

-- Configuration
local BRIDGE_HOST = "127.0.0.1"
local BRIDGE_PORT_SEND = 49002    -- Send flight data to bridge
local BRIDGE_PORT_RECV = 49003    -- Receive commands from bridge
local UPDATE_RATE = 0.05          -- 20 Hz (seconds between updates)

-- UDP Socket
local socket = require("socket")
local udp_send = nil
local udp_recv = nil
local last_update = 0
local connected = false
local frame_count = 0

-- ============================================================
-- DataRef References
-- ============================================================

-- GPS / Position
dataref("dr_latitude",       "sim/flightmodel/position/latitude", "readonly")
dataref("dr_longitude",      "sim/flightmodel/position/longitude", "readonly")
dataref("dr_elevation",      "sim/flightmodel/position/elevation", "readonly")
dataref("dr_agl",            "sim/flightmodel/position/y_agl", "readonly")
dataref("dr_groundspeed",    "sim/flightmodel/position/groundspeed", "readonly")

-- Attitude
dataref("dr_pitch",          "sim/flightmodel/position/true_theta", "readonly")
dataref("dr_roll",           "sim/flightmodel/position/true_phi", "readonly")
dataref("dr_mag_heading",    "sim/flightmodel/position/mag_psi", "readonly")
dataref("dr_true_heading",   "sim/flightmodel/position/true_psi", "readonly")

-- Air Data
dataref("dr_ias",            "sim/flightmodel/position/indicated_airspeed", "readonly")
dataref("dr_tas",            "sim/flightmodel/position/true_airspeed", "readonly")
dataref("dr_vs_fpm",         "sim/flightmodel/position/vh_ind_fpm", "readonly")
dataref("dr_baro",           "sim/weather/barometer_sealevel_inhg", "readonly")
dataref("dr_alt_indicated",  "sim/cockpit2/gauges/indicators/altitude_ft_pilot", "readonly")
dataref("dr_oat",            "sim/cockpit2/temperature/outside_air_temp_degc", "readonly")
dataref("dr_slip",           "sim/cockpit2/gauges/indicators/slip_deg", "readonly")
dataref("dr_turn_rate",      "sim/cockpit2/gauges/indicators/turn_rate_heading_deg_pilot", "readonly")

-- Engine (index 0 — single turboprop)
dataref("dr_n1",             "sim/cockpit2/engine/indicators/N1_percent", "readonly")
dataref("dr_n2",             "sim/cockpit2/engine/indicators/N2_percent", "readonly")
dataref("dr_torque",         "sim/cockpit2/engine/indicators/torque_n_mtr", "readonly")
dataref("dr_itt",            "sim/cockpit2/engine/indicators/ITT_deg_C", "readonly")
dataref("dr_oil_press",      "sim/cockpit2/engine/indicators/oil_pressure_psi", "readonly")
dataref("dr_oil_temp",       "sim/cockpit2/engine/indicators/oil_temperature_deg_C", "readonly")
dataref("dr_fuel_flow",      "sim/cockpit2/engine/indicators/fuel_flow_kg_sec", "readonly")
dataref("dr_prop_rpm",       "sim/cockpit2/engine/indicators/prop_speed_rpm", "readonly")
dataref("dr_eng_running",    "sim/flightmodel/engine/ENGN_running", "readonly")

-- Fuel
dataref("dr_fuel_qty",       "sim/cockpit2/fuel/fuel_quantity", "readonly")

-- Electrical
dataref("dr_bus_volts",      "sim/cockpit2/electrical/bus_volts", "readonly")
dataref("dr_gen_amps",       "sim/cockpit2/electrical/generator_amps", "readonly")

-- Radio
dataref("dr_com1_act",       "sim/cockpit2/radios/actuators/com1_frequency_hz_833", "writable")
dataref("dr_com1_stby",      "sim/cockpit2/radios/actuators/com1_standby_frequency_hz_833", "writable")
dataref("dr_com2_act",       "sim/cockpit2/radios/actuators/com2_frequency_hz_833", "writable")
dataref("dr_com2_stby",      "sim/cockpit2/radios/actuators/com2_standby_frequency_hz_833", "writable")
dataref("dr_nav1_act",       "sim/cockpit2/radios/actuators/nav1_frequency_hz", "writable")
dataref("dr_nav1_stby",      "sim/cockpit2/radios/actuators/nav1_standby_frequency_hz", "writable")
dataref("dr_nav2_act",       "sim/cockpit2/radios/actuators/nav2_frequency_hz", "writable")
dataref("dr_nav2_stby",      "sim/cockpit2/radios/actuators/nav2_standby_frequency_hz", "writable")

-- Nav indicators
dataref("dr_nav1_hdef",      "sim/cockpit/radios/nav1_hdef_dot", "readonly")
dataref("dr_nav1_vdef",      "sim/cockpit/radios/nav1_vdef_dot", "readonly")
dataref("dr_gps_track",      "sim/cockpit2/gauges/indicators/ground_track_mag_pilot", "readonly")

-- Time
dataref("dr_zulu_hours",     "sim/cockpit2/clock_timer/zulu_time_hours", "readonly")
dataref("dr_zulu_minutes",   "sim/cockpit2/clock_timer/zulu_time_minutes", "readonly")
dataref("dr_zulu_seconds",   "sim/cockpit2/clock_timer/zulu_time_seconds", "readonly")


-- ============================================================
-- Binary Packing Helpers
-- ============================================================

-- Pack an unsigned 32-bit integer (little-endian)
local function pack_uint32(n)
    n = math.floor(n) % 4294967296
    local b0 = n % 256; n = math.floor(n / 256)
    local b1 = n % 256; n = math.floor(n / 256)
    local b2 = n % 256; n = math.floor(n / 256)
    local b3 = n % 256
    return string.char(b0, b1, b2, b3)
end

-- Pack a 32-bit float (little-endian) using string.pack if available (Lua 5.3+)
local has_string_pack = (string.pack ~= nil)

local function pack_float(f)
    if has_string_pack then
        return string.pack("<f", f)
    else
        -- Manual IEEE 754 float packing (fallback for Lua 5.1)
        if f == 0 then return "\0\0\0\0" end
        local sign = 0
        if f < 0 then sign = 1; f = -f end
        local mantissa, exponent = math.frexp(f)
        exponent = exponent + 126
        if exponent <= 0 then return "\0\0\0\0" end
        if exponent >= 255 then
            return string.char(0, 0, 128, sign * 128 + 127 + 128)
        end
        mantissa = (mantissa * 2 - 1) * 8388608 -- 2^23
        local m = math.floor(mantissa + 0.5)
        local b0 = m % 256; m = math.floor(m / 256)
        local b1 = m % 256; m = math.floor(m / 256)
        local b2 = m % 256 + exponent % 2 * 128
        local b3 = math.floor(exponent / 2) + sign * 128
        return string.char(b0, b1, b2, b3)
    end
end

-- Pack a 64-bit double (little-endian)
local function pack_double(d)
    if has_string_pack then
        return string.pack("<d", d)
    else
        -- Manual IEEE 754 double packing (fallback)
        if d == 0 then return "\0\0\0\0\0\0\0\0" end
        local sign = 0
        if d < 0 then sign = 1; d = -d end
        local mantissa, exponent = math.frexp(d)
        exponent = exponent + 1022
        if exponent <= 0 then return "\0\0\0\0\0\0\0\0" end
        mantissa = (mantissa * 2 - 1) * 4503599627370496 -- 2^52
        local bytes = {}
        local m = math.floor(mantissa + 0.5)
        for i = 1, 6 do
            bytes[i] = m % 256
            m = math.floor(m / 256)
        end
        bytes[7] = m % 16 + exponent % 16 * 16
        bytes[8] = math.floor(exponent / 16) + sign * 128
        return string.char(bytes[1], bytes[2], bytes[3], bytes[4],
                           bytes[5], bytes[6], bytes[7], bytes[8])
    end
end

-- Unpack uint32 from string at offset (1-based)
local function unpack_uint32(s, offset)
    local b0, b1, b2, b3 = s:byte(offset, offset + 3)
    return b0 + b1 * 256 + b2 * 65536 + b3 * 16777216
end

-- Unpack float from string  
local function unpack_float(s, offset)
    if has_string_pack then
        return string.unpack("<f", s, offset)
    end
    return 0  -- fallback
end


-- ============================================================
-- Protocol: GarminBridge Binary Packet
-- ============================================================
-- 
-- Header: "GXPL" (4 bytes) + packet_length (uint32) + sequence (uint32)
-- Then all fields as defined, matching shared_memory.py layout exactly.
--

local sequence = 0

local function build_packet()
    sequence = sequence + 1
    
    local NM_TO_FTLB = 0.7375621
    local KGSEC_TO_PPH = 7936.641
    
    local torque_ftlb = dr_torque * NM_TO_FTLB
    local torque_pct = math.min(100, math.max(0, torque_ftlb / 16.0))
    local fuel_flow_pph = dr_fuel_flow * KGSEC_TO_PPH
    local eng_run = 0
    if dr_eng_running > 0.5 then eng_run = 1 end
    
    -- Build packet matching XP_TO_GARMIN struct in shared_memory.py
    local pkt = "GXPL"                              -- 4-byte header tag
    
    -- Header fields
    pkt = pkt .. pack_uint32(0x47425250)             -- magic
    pkt = pkt .. pack_uint32(sequence)               -- sequence
    pkt = pkt .. pack_double(os.clock())             -- timestamp
    
    -- GPS
    pkt = pkt .. pack_double(dr_latitude)            -- latitude (double)
    pkt = pkt .. pack_double(dr_longitude)           -- longitude (double)
    pkt = pkt .. pack_float(dr_elevation)            -- altitude_msl_m
    pkt = pkt .. pack_float(dr_agl)                  -- altitude_agl_m
    pkt = pkt .. pack_float(dr_groundspeed)          -- groundspeed_ms
    
    -- Attitude
    pkt = pkt .. pack_float(dr_pitch)                -- pitch_deg
    pkt = pkt .. pack_float(dr_roll)                 -- roll_deg
    pkt = pkt .. pack_float(dr_mag_heading)          -- mag_heading_deg
    pkt = pkt .. pack_float(dr_true_heading)         -- true_heading_deg
    
    -- Air Data
    pkt = pkt .. pack_float(dr_ias)                  -- ias_kts
    pkt = pkt .. pack_float(dr_tas)                  -- tas_kts
    pkt = pkt .. pack_float(dr_vs_fpm)               -- vs_fpm
    pkt = pkt .. pack_float(dr_baro)                 -- baro_inhg
    pkt = pkt .. pack_float(dr_alt_indicated)         -- indicated_alt_ft
    pkt = pkt .. pack_float(dr_oat)                  -- oat_deg_c
    
    -- Slip/Turn
    pkt = pkt .. pack_float(dr_slip)                 -- slip_deg
    pkt = pkt .. pack_float(dr_turn_rate)            -- turn_rate_deg_s
    
    -- Engine
    pkt = pkt .. pack_float(dr_n1)                   -- n1_percent
    pkt = pkt .. pack_float(dr_n2)                   -- n2_percent
    pkt = pkt .. pack_float(torque_ftlb)             -- torque_ftlb
    pkt = pkt .. pack_float(torque_pct)              -- torque_percent
    pkt = pkt .. pack_float(dr_itt)                  -- itt_deg_c
    pkt = pkt .. pack_float(dr_oil_press)            -- oil_press_psi
    pkt = pkt .. pack_float(dr_oil_temp)             -- oil_temp_deg_c
    pkt = pkt .. pack_float(fuel_flow_pph)           -- fuel_flow_pph
    pkt = pkt .. pack_float(dr_fuel_qty)             -- fuel_qty_kg_l (left)
    pkt = pkt .. pack_float(0)                       -- fuel_qty_kg_r (right, TODO)
    pkt = pkt .. pack_float(dr_prop_rpm)             -- prop_rpm
    pkt = pkt .. pack_uint32(eng_run)                -- eng_running
    
    -- Electrical
    pkt = pkt .. pack_float(dr_bus_volts)            -- bus_volts
    pkt = pkt .. pack_float(dr_gen_amps)             -- gen_amps
    
    -- Radio
    pkt = pkt .. pack_uint32(dr_com1_act)            -- com1_active_khz
    pkt = pkt .. pack_uint32(dr_com1_stby)           -- com1_standby_khz
    pkt = pkt .. pack_uint32(dr_com2_act)            -- com2_active_khz
    pkt = pkt .. pack_uint32(dr_com2_stby)           -- com2_standby_khz
    pkt = pkt .. pack_uint32(dr_nav1_act)            -- nav1_active_khz
    pkt = pkt .. pack_uint32(dr_nav1_stby)           -- nav1_standby_khz
    pkt = pkt .. pack_uint32(dr_nav2_act)            -- nav2_active_khz
    pkt = pkt .. pack_uint32(dr_nav2_stby)           -- nav2_standby_khz
    
    -- Navigation
    pkt = pkt .. pack_float(dr_nav1_hdef)            -- nav1_hdef_dots
    pkt = pkt .. pack_float(dr_nav1_vdef)            -- nav1_vdef_dots
    pkt = pkt .. pack_float(dr_gps_track)            -- gps_track_deg
    
    -- Time
    pkt = pkt .. pack_float(dr_zulu_hours)           -- utc_hours
    pkt = pkt .. pack_float(dr_zulu_minutes)         -- utc_minutes
    pkt = pkt .. pack_float(dr_zulu_seconds)         -- utc_seconds
    
    -- Flags
    pkt = pkt .. pack_uint32(0x0F)                   -- data_valid (all valid)
    
    return pkt
end


-- ============================================================
-- Process received commands from bridge
-- ============================================================

local function process_received()
    if not udp_recv then return end
    
    local data, err = udp_recv:receive()
    if data and #data >= 8 then
        local header = data:sub(1, 4)
        if header == "GBRX" then  -- Garmin Bridge → X-Plane command
            local cmd_type = unpack_uint32(data, 5)
            
            -- cmd_type 1 = Set radio frequencies
            if cmd_type == 1 and #data >= 40 then
                local com1_act = unpack_uint32(data, 9)
                local com1_stby = unpack_uint32(data, 13)
                local nav1_act = unpack_uint32(data, 17)
                local nav1_stby = unpack_uint32(data, 21)
                
                if com1_act > 0 then dr_com1_act = com1_act end
                if com1_stby > 0 then dr_com1_stby = com1_stby end
                if nav1_act > 0 then dr_nav1_act = nav1_act end
                if nav1_stby > 0 then dr_nav1_stby = nav1_stby end
            end
        end
    end
end


-- ============================================================
-- Initialization
-- ============================================================

function garmin_bridge_init()
    logMsg("GarminBridge: Initializing UDP connection...")
    
    -- Create send socket
    udp_send = socket.udp()
    udp_send:settimeout(0)
    
    -- Create receive socket (non-blocking)
    udp_recv = socket.udp()
    udp_recv:settimeout(0)
    local ok, err = udp_recv:setsockname(BRIDGE_HOST, BRIDGE_PORT_RECV)
    if not ok then
        logMsg("GarminBridge: WARNING - Could not bind receive port " .. BRIDGE_PORT_RECV .. ": " .. tostring(err))
        udp_recv = nil
    end
    
    connected = true
    logMsg("GarminBridge: Ready! Sending to " .. BRIDGE_HOST .. ":" .. BRIDGE_PORT_SEND)
end


-- ============================================================
-- Main Update Loop (called by FlyWithLua)
-- ============================================================

function garmin_bridge_update()
    if not connected then return end
    
    local now = os.clock()
    if now - last_update < UPDATE_RATE then return end
    last_update = now
    
    -- Build and send data packet
    local packet = build_packet()
    udp_send:sendto(packet, BRIDGE_HOST, BRIDGE_PORT_SEND)
    
    -- Check for incoming commands
    process_received()
    
    frame_count = frame_count + 1
end


-- ============================================================
-- Cleanup
-- ============================================================

function garmin_bridge_shutdown()
    logMsg("GarminBridge: Shutting down...")
    connected = false
    if udp_send then udp_send:close() end
    if udp_recv then udp_recv:close() end
end


-- ============================================================
-- Register with FlyWithLua
-- ============================================================

garmin_bridge_init()
do_every_draw("garmin_bridge_update()")
do_on_exit("garmin_bridge_shutdown()")

logMsg("GarminBridge: FlyWithLua script loaded successfully!")
logMsg("GarminBridge: T27 EMB-312 / GDU 1060 + GTN 650Xi integration active")
