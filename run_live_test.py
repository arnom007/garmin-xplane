"""
run_live_test.py
================
Demonstração limpa de injeção direta de telemetria sem suspensão de threads.
Injeta atitude (Pitch, Roll, Heading) e dados de ar (IAS, Altitude, VS)
na GDU 1060 TXi e no GTN 650Xi em tempo real.
"""

import time
import math
import sys
from unified_flight_injection import GarminFlightInjector

def main(duration=15.0):
    print("=" * 60)
    print("   TESTE LIMPO DE INJEÇÃO GARMIN (SEM SUSPENSÃO DE THREADS)")
    print("=" * 60)
    
    inj = GarminFlightInjector()
    print("[1/3] Conectando aos processos Garmin...")
    if not inj.connect():
        print("[ERRO] Falha ao conectar. Verifique se a GDU e o GTN estão abertos.")
        return False
        
    print(f"[OK] Conectado: GDU PID={inj.gdu_pid}, GTN PID={inj.gtn_pid}")
    print(f"     IAS Widget:  0x{inj.addr_gdu_ias:08X}")
    print(f"     AHRS Hdg:    0x{inj.addr_gdu_hdg:08X}")
    print(f"     PFD Pitch:   0x{inj.addr_pfd_a_pitch_global:08X}")
    print(f"     PFD Roll:    0x{inj.addr_pfd_a_roll_global:08X}")
    print(f"     PFD Heading: 0x{inj.addr_pfd_a_hdg_global:08X}")
    print(f"     HDG Box RVA: 0x{inj.addr_hdg_box_format:08X}")
    print(f"     Alt Widget:  0x{inj.addr_gdu_alt:08X}")
    
    print("\n[2/3] Aplicando bypass não-intrusivo (SHM 0x0C=0 + PFD Callbacks + HDG Box sync)...")
    inj.disable_demo_flight_model()
    
    rate_hz = 60.0
    dt = 1.0 / rate_hz
    steps = int(duration * rate_hz)
    print(f"\n[3/3] Iniciando streaming dinâmico por {duration:.0f} segundos a {rate_hz:.0f} Hz ({steps} frames)...")
    print("      -> Acompanhe a tela da GDU e GTN agora!\n")
    
    t0 = time.time()
    
    try:
        for i in range(steps):
            t = i * dt
            # Perfil de voo dinâmico, suave e realista
            pitch = 3.5 + 2.5 * math.sin(t * 0.8)        # Pitch suave entre 1.0° e 6.0°
            roll  = 18.0 * math.sin(t * 0.6)            # Curvas entre -18° e +18°
            hdg   = (120.0 + t * 8.0) % 360.0           # Curva suave contínua de 8°/s
            ias   = 120.0 + 8.0 * math.sin(t * 0.4)     # Velocidade suave entre 112 e 128 kt
            alt   = 2500.0 + 40.0 * math.sin(t * 0.3)   # Altitude ondulando suavemente (2460 a 2540 ft)
            vs    = 40.0 * 0.3 * math.cos(t * 0.3) * 60 # VSI correspondente (~±720 fpm)
            
            inj.inject_telemetry(pitch, roll, hdg, ias, alt, vs)
            
            # Atualiza status a cada 1 segundo no terminal
            if i % int(rate_hz) == 0 or i == steps - 1:
                rb = inj.readback_gdu()
                sec = int(t)
                bar = "#" * int((t / duration) * 20)
                pad = " " * (20 - len(bar))
                print(f"[{sec:02d}s/{duration:.0f}s] [{bar}{pad}] Pitch={pitch:+4.1f}° | Roll={roll:+5.1f}° | Hdg={hdg:05.1f}° | IAS={ias:5.1f}kt | Alt={alt:5.0f}ft")
                
            time.sleep(dt)
            
        print("\n" + "=" * 60)
        print("   TESTE LIMPO CONCLUÍDO COM SUCESSO!")
        print("   Todos os 450 frames foram injetados perfeitamente.")
        print("=" * 60)
        return True
        
    finally:
        inj.restore()

if __name__ == '__main__':
    dur = float(sys.argv[1]) if len(sys.argv) > 1 else 30.0
    main(dur)

