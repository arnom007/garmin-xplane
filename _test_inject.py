"""Quick test of mem_injector scan."""
import sys
sys.stdout.reconfigure(encoding='utf-8', errors='replace')
from mem_injector import GDUMemoryInjector
import time

print("Testing mem_injector...")
inj = GDUMemoryInjector()

if not inj.connect():
    print("GDU not running! Start the Garmin trainer first.")
    sys.exit(1)

print("Connected to GDU PID={}".format(inj.pid))
print("Scanning (this takes ~30-120 sec)...")

t0 = time.time()
result = inj.scan()
dt = time.time() - t0

print("Scan result: {} in {:.1f}s".format(result, dt))
status = inj.get_status()
print("Found {}/{} addresses".format(status['addrs_found'], status['addrs_total']))
for name, addr_str in status['addresses'].items():
    if addr_str:
        val = inj._read_float(inj.addrs[name])
        print("  {}: {} = {:.4f}".format(name, addr_str, val if val else 0))

inj.disconnect()
