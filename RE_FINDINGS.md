# Garmin Trainer Reverse Engineering — Findings (handoff)

## Objetivo
Alimentar o Garmin Trainer (GDU 1060 TXi 3.50 e GTN 650Xi) com dados em tempo real do X-Plane
(AHRS, ADC, GPS). O trainer deve tratar os dados como vindos de equipamentos reais (GRS 56 / GAD 29B / GPS),
sem delay e sem flags de integridade.

## Ambiente
- Trainer GDU: `C:\ProgramData\Garmin\Trainers\GTI\Production\GTI__TXi__3_50\app-3.50.20\bin\`
  - Processo: `gdu_7xx_simulator.exe` (32-bit, WoW64)
- Trainer GTN: `C:\ProgramData\Garmin\Trainers\GTI\Production\GTI__GTN_Xi__21_03\app-21.3.7\bin\` (`gtn_simulator.exe`)
- Em runtime os módulos têm sufixo `_sim`: `krnl_sim.dll`, `iop_a_sim.dll` ... `iop_d_sim.dll`, `iop_m_sim.dll`,
  `iop_sim_sim.dll`, `prx_sim.dll`, `dat_sim.dll`, `mon_sim.dll`, `eis_sim.dll`, `cdp_*_sim.dll`.
  No disco do TXi existem `krnl.dll`, `IOP_SIM.dll` etc. (sem sufixo). `GetModuleHandle("krnl.dll")` FALHA; usar `krnl_sim.dll`.
- Injeção: EasyHook (`RhInjectLibrary`), DLL 32-bit. Build: `GarminHook_v2\build.bat`. O DLL fica travado enquanto o trainer
  estiver aberto, então é preciso fechar o trainer antes de recompilar.
- Registro do Windows: `HKCU\SOFTWARE\Garmin\GDU7xx Sim\Settings\GDU1` tem `ETHR1..4`, `UART1..18` = NONE
  (possíveis pontos de configuração de transporte externo — não explorado).
- Shared memory do bridge: ver `garmin_bridge_shm.h`.

## Descobertas confirmadas
1. `krnl_sim.dll` carrega em ~0x53CB0000 e exporta: `reg_read`, `reg_write`, `REG_pvg_lookup_tag_by_name`,
   `HWM_pvg_hsdb_set_sim_lrus_online`, `HWM_pvg_hsdb_get_lrus_online`, `TSK_pvg_send_msg_ex`,
   `HWM_pvg_hsdb_put`, `HWM_pvg_a429_put`, `SIM_roar_set_endpoint_name` (esses últimos vistos em análise de exports).
   Todos resolveram com GetProcAddress.
2. Existe um roteador chamado **ROAR** em `krnl.dll`, com strings: "Router GSIM UDP Input Port",
   "Socket (UDP or TCP)", "HSDB(2)", "Serial", "GARINC", "TokenPasser", "Growl", "GrowlRouter",
   "ROAR: Endpoint parameters: name=%s, client port=%d, master port=%d", "ROAR: Endpoint bind failure on port %d",
   formatos "HSDB format used by ROAR", "Raw HSDB frames"; tipos ARINC 429/453/717, RS-232/485, CAN.
3. Strings de registro IOP encontradas (ex.: `IOP_AHRS_PITCH`, `IOP_AHRS_ROLL`, `IOP_AHRS_M_HDG`, `IOP_ADC_IAS`,
   `IOP_ADC_PRESS_ALT`, `IOP_AHRS_LBL_324/325`, `IOP_GPS_PVT_MSG`), além de tags `reg_tag_*` em minúsculas.
4. Repo de referência: https://github.com/philhug/gsim (GTN/G500 trainer, ARINC429 sobre UDP, porta 4469, usa `gsim.pmf`).
   Pacote descrito: header 12 bytes LE (lru id, canal, n words) + words ARINC429 de 32 bits.
   ATENÇÃO: o resumo do código veio de um subagente; o `server.js` real, buscado diretamente, estava truncado.
   Confirmar o protocolo lendo o código-fonte original antes de confiar.

## O que FALHOU (não repetir sem mudar algo)
- Hook de `TSK_pvg_send_msg_ex` com injeção de mensagens parciais -> flags LOI/integridade, avião "caranguejando"
  (GPS sem heading) e AHRS/ADC fail. O trainer faz validação de redundância/voter.
- Hook de `TSK_pvg_send_msg_ex` dentro do V8.1 -> **crash da GDU** (suspeita: bypass/original mal inicializado).
- `REG_pvg_lookup_tag_by_name("IOP_AHRS_PITCH", &id)` -> retorna **32006** para os 57 nomes testados (nomes como estão no binário).
  Assinatura do protótipo ainda NÃO verificada (possivelmente argumentos/convenção errados, ou nomes exigem sufixo/instância,
  ou o registro ainda não existia no momento da chamada).
- Teste UDP ARINC para 127.0.0.1 portas 4469-4500 (`test_arinc_udp.py`) -> nenhum efeito. Provavelmente falta registrar um endpoint
  no ROAR (porta/nome/protocolo), e a porta real é desconhecida.
- `hsdb_get_lrus_online` retornou ret=218318864 e máscara 0 (provável assinatura errada).
- V8.1 probe thread (5s x 30 tentativas + varredura de IDs 1..200) nunca chegou a rodar até o fim no último teste (crash antes).

## Próximos passos sugeridos
1. Desmontar (IDA/Ghidra/x32dbg) `krnl.dll` (disco, sem sufixo) para obter as assinaturas reais de
   `REG_pvg_lookup_tag_by_name`, `reg_read`, `reg_write`, `HWM_pvg_hsdb_*`, `HWM_pvg_a429_put`, `SIM_roar_*`.
2. Descobrir como o ROAR é configurado (xrefs das strings "Router GSIM UDP Input Port" / "ROAR: Endpoint parameters")
   e de onde vem a porta (arquivo .pmf? registro? argumento de linha de comando do trainer?).
3. Descobrir quem produz AHRS/ADC no modo demo (`IOP_SIM.dll` / `iop_sim_sim.dll`) e interceptar/substituir NA FONTE
   (ou chamar a mesma função de put com LRU simulado), em vez de injetar mensagens de baixo nível.
4. Evitar hooks inline em `krnl` até entender a convenção de chamada; preferir chamar funções exportadas.
5. Procurar `*.pmf` / configs do simulador (não encontrados em `C:\ProgramData\Garmin`).

## Arquivos do projeto relevantes
- `GarminHook_v2\dllmain.cpp` — V8.1 (probe-only, sem hooks), `DllInjector.cpp`, `build.bat`
- `test_arinc_udp.py` — teste UDP ARINC 429
- `garmin_bridge_shm.h` — estruturas de memória compartilhada X-Plane -> Garmin
- Logs: `%APPDATA%\GarminBridge\GarminHook_V8_<pid>.log`
