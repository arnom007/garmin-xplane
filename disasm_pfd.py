import capstone
import pefile

dll_path = r'C:\ProgramData\Garmin\Trainers\GTI\Production\GTI__TXi__3_86\app-3.86.4\bin\cdp_pfd_b_sim.dll'
pe = pefile.PE(dll_path)

# Find .text section
text_sec = None
for s in pe.sections:
    if s.Name.startswith(b'.text'):
        text_sec = s
        break

rva_target = 0x18A0
offset_target = pe.get_offset_from_rva(rva_target)

with open(dll_path, 'rb') as f:
    f.seek(offset_target)
    code = f.read(512)

md = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_32)
print(f"=== Disassembly of cdp_pfd_b_sim.dll at RVA 0x{rva_target:X} ===")
for insn in md.disasm(code, rva_target):
    print(f"0x{insn.address:06X}:  {insn.mnemonic:8s} {insn.op_str}")
    if insn.mnemonic in ('ret', 'retf') and insn.address > rva_target + 0x40:
        break
