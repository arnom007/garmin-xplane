import json
with open(r'c:\Users\Arnom\Documents\projeto garminbridge\_memscan_state.json') as f:
    state = json.load(f)

print("Scan 1: target={}, addrs={}".format(state.get("target"), len(state.get("addrs", []))))
if 'scan2_results' in state:
    results = state['scan2_results']
    print("Scan 2: {} matches".format(len(results)))
    for addr, val in results:
        print("  0x{:08X}: {:.4f}".format(addr, val))
else:
    print("No scan2 results")
    print("Keys: {}".format(list(state.keys())))
