"""Decode ROAR/HSDB datagrams captured by sniffer.dll.

Working hypothesis (to be validated by this script):
  ROAR header (16 bytes):
    u32 fmt        (0x42)
    u16 roar_len   (= total - 8)
    u16 port       (master port of the peer)
    u32 a, u32 b   (channel / routing info)
  HSDB frame:
    u16 msg_id, u8 ?, u8 lru, u16 seq, u16 len
    then either FE FF block header + tag records, or raw data.
"""
import struct, sys, collections
from analyze_cap import read_cap, hexdump

def parse_roar(b):
    if len(b) < 16:
        return None
    fmt, rlen, port, a, c = struct.unpack_from("<IHHII", b, 0)
    return dict(fmt=fmt, rlen=rlen, port=port, a=a, c=c, body=b[16:])

def parse_hsdb(body):
    if len(body) < 8:
        return None
    mid, z, lru, seq, ln = struct.unpack_from("<HBBHH", body, 0)
    return dict(mid=mid, z=z, lru=lru, seq=seq, ln=ln, rest=body[8:])

def parse_tags(rest):
    """Try to parse FE FF block + [id u16][len u16][4 bytes][data] records."""
    if len(rest) < 12 or rest[0:2] != b"\xFE\xFF":
        return None
    hdr = rest[:12]
    off = 12
    recs = []
    while off + 8 <= len(rest):
        tid, tlen = struct.unpack_from("<HH", rest, off)
        meta = rest[off + 4:off + 8]
        if off + 8 + tlen > len(rest) or tlen > 2048:
            break
        recs.append((tid, tlen, meta, rest[off + 8:off + 8 + tlen]))
        off += 8 + tlen
    return hdr, recs, len(rest) - off

def fmtval(d):
    s = d.hex()
    if len(d) == 4:
        f = struct.unpack("<f", d)[0]
        i = struct.unpack("<i", d)[0]
        s += f"  f={f:.6g} i={i}"
    elif len(d) == 8:
        s += f"  d={struct.unpack('<d', d)[0]:.9g}"
    elif len(d) in (1, 2):
        s += f"  u={int.from_bytes(d, 'little')}"
    return s

if __name__ == "__main__":
    recs = read_cap(sys.argv[1])
    want_dir = int(sys.argv[2]) if len(sys.argv) > 2 else None
    stats = collections.Counter()
    tagsamples = {}
    parsed_ok = 0
    mids = collections.Counter()
    for t, d, api, lp, rp, s, b in recs:
        if want_dir is not None and d != want_dir:
            continue
        r = parse_roar(b)
        if not r:
            continue
        stats[(d, r["fmt"], r["port"], r["a"], r["c"])] += 1
        if r["rlen"] != len(b) - 8:
            stats[("BADLEN", d)] += 1
        h = parse_hsdb(r["body"])
        if not h:
            continue
        mids[(d, h["mid"], h["lru"])] += 1
        pt = parse_tags(h["rest"])
        if pt:
            parsed_ok += 1
            hdr, trs, left = pt
            for tid, tlen, meta, data in trs:
                key = (d, h["mid"], tid)
                tagsamples.setdefault(key, []).append((t, meta, data))
    print("ROAR header combos:")
    for k, v in stats.most_common(20):
        print("  ", k, v)
    print("\nHSDB (dir, msg_id, lru) counts:")
    for k, v in mids.most_common(40):
        print(f"   dir={k[0]} mid=0x{k[1]:04X} lru={k[2]}: {v}")
    print(f"\nframes with FE FF tag block: {parsed_ok}")
    print(f"distinct (dir,mid,tag): {len(tagsamples)}")
    for key in sorted(tagsamples)[:400]:
        smp = tagsamples[key]
        vals = collections.Counter(x[2] for x in smp)
        first, last = smp[0], smp[-1]
        print(f"  dir={key[0]} mid=0x{key[1]:04X} tag=0x{key[2]:04X} n={len(smp)} distinct={len(vals)} meta={first[1].hex()} "
              f"first=[{fmtval(first[2])}] last=[{fmtval(last[2])}]")
