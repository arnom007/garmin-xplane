import struct, sys, collections, os

REC = struct.Struct("<IBBHHHII")  # t_ms, dir, api, lport, rport, rsv, sock, len

def read_cap(path):
    out = []
    with open(path, "rb") as f:
        data = f.read()
    off = 0
    while off + REC.size <= len(data):
        t, d, api, lp, rp, _, s, n = REC.unpack_from(data, off)
        off += REC.size
        if off + n > len(data):
            break
        out.append((t, d, api, lp, rp, s, data[off:off + n]))
        off += n
    return out

def hexdump(b, maxlen=96):
    b = b[:maxlen]
    lines = []
    for i in range(0, len(b), 16):
        chunk = b[i:i + 16]
        hx = " ".join(f"{c:02X}" for c in chunk)
        asc = "".join(chr(c) if 32 <= c < 127 else "." for c in chunk)
        lines.append(f"    {i:04X}: {hx:<48} {asc}")
    return "\n".join(lines)

if __name__ == "__main__":
    path = sys.argv[1]
    recs = read_cap(path)
    if not recs:
        print("no records"); sys.exit()
    dur = (recs[-1][0] - recs[0][0]) / 1000 or 1
    print(f"{os.path.basename(path)}: {len(recs)} records over {dur:.1f}s")
    flows = collections.defaultdict(list)
    for r in recs:
        flows[(r[1], r[2], r[3], r[4], r[5])].append(r)
    api_n = {0: "send", 1: "sendto", 2: "recv"}
    for k, rs in sorted(flows.items(), key=lambda kv: -len(kv[1])):
        d, api, lp, rp, s = k
        sizes = collections.Counter(len(r[6]) for r in rs)
        print(f"\n== {'TX' if d == 0 else 'RX'} {api_n[api]:6} sock={s} lport={lp} rport={rp}: {len(rs)} pkts ({len(rs)/dur:.1f}/s)")
        print("   top sizes:", sizes.most_common(8))
        # header byte stats: distinct values of first 8 bytes
        firsts = collections.Counter(r[6][:8].hex() for r in rs)
        print("   distinct first-8-bytes:", len(firsts), "top:", firsts.most_common(5))
        for sz, _ in sizes.most_common(3):
            ex = next(r for r in rs if len(r[6]) == sz)
            print(f"   sample len={sz}:")
            print(hexdump(ex[6]))
