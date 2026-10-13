"""
bridge_gui.py — GUI de Controle do GarminBridge

Interface gráfica com tkinter para monitorar e controlar a integração
Garmin Trainer ↔ X-Plane 11.
"""

import tkinter as tk
from tkinter import ttk, scrolledtext, filedialog, messagebox
import threading
import time
import os
import json

from garmin_bridge import GarminBridge, load_config, save_config, find_process


# ============================================================
# Color Palette
# ============================================================

COLORS = {
    'bg_dark':      '#1a1a2e',
    'bg_panel':     '#16213e',
    'bg_card':      '#0f3460',
    'accent':       '#e94560',
    'accent_green': '#00d4aa',
    'accent_blue':  '#4da6ff',
    'accent_yellow':'#ffd93d',
    'text_primary': '#ffffff',
    'text_secondary':'#a0a0b0',
    'text_dim':     '#606080',
    'border':       '#2a2a4a',
    'success':      '#00d4aa',
    'error':        '#e94560',
    'warning':      '#ffd93d',
}


class StatusIndicator(tk.Canvas):
    """A small LED-style status indicator."""

    def __init__(self, parent, size=12, **kwargs):
        super().__init__(parent, width=size, height=size,
                         bg=COLORS['bg_panel'], highlightthickness=0, **kwargs)
        self.size = size
        self._oval = self.create_oval(2, 2, size-2, size-2,
                                       fill=COLORS['text_dim'], outline='')

    def set_status(self, active):
        color = COLORS['success'] if active else COLORS['text_dim']
        self.itemconfigure(self._oval, fill=color)


class DataGauge(tk.Frame):
    """A small data display widget with label and value."""

    def __init__(self, parent, label, unit='', width=100):
        super().__init__(parent, bg=COLORS['bg_card'])
        self.label_text = label

        self.label = tk.Label(self, text=label, font=('Consolas', 8),
                              fg=COLORS['text_secondary'], bg=COLORS['bg_card'])
        self.label.pack(side=tk.TOP, anchor=tk.W, padx=4, pady=(2, 0))

        self.value_var = tk.StringVar(value='---')
        self.value = tk.Label(self, textvariable=self.value_var,
                              font=('Consolas', 14, 'bold'),
                              fg=COLORS['accent_blue'], bg=COLORS['bg_card'],
                              width=width // 10)
        self.value.pack(side=tk.TOP, anchor=tk.W, padx=4)

        if unit:
            self.unit = tk.Label(self, text=unit, font=('Consolas', 8),
                                 fg=COLORS['text_dim'], bg=COLORS['bg_card'])
            self.unit.pack(side=tk.TOP, anchor=tk.W, padx=4, pady=(0, 2))

    def set_value(self, value, fmt='{:.1f}'):
        try:
            self.value_var.set(fmt.format(value))
        except:
            self.value_var.set(str(value))


class BridgeGUI:
    """Main GUI window for GarminBridge."""

    def __init__(self, config):
        self.config = config
        self.bridge = GarminBridge(config)
        self.bridge.on_status_update(self._on_status_update)
        self.bridge.on_log(self._on_log)

        self.root = tk.Tk()
        self.root.title('GarminBridge — T27 EMB-312')
        self.root.geometry('900x700')
        self.root.configure(bg=COLORS['bg_dark'])
        self.root.minsize(800, 600)

        # Try to set icon
        try:
            self.root.iconbitmap(default='')
        except:
            pass

        self._build_ui()
        self._setup_periodic_update()

    def _build_ui(self):
        """Build the complete UI."""
        # Main container
        main = tk.Frame(self.root, bg=COLORS['bg_dark'])
        main.pack(fill=tk.BOTH, expand=True, padx=10, pady=10)

        # ---- Header ----
        header = tk.Frame(main, bg=COLORS['bg_dark'])
        header.pack(fill=tk.X, pady=(0, 10))

        tk.Label(header, text='✈ GarminBridge', font=('Segoe UI', 20, 'bold'),
                 fg=COLORS['accent'], bg=COLORS['bg_dark']).pack(side=tk.LEFT)

        tk.Label(header, text='T27 EMB-312 / GDU 1060 + GTN 650Xi',
                 font=('Segoe UI', 10),
                 fg=COLORS['text_secondary'], bg=COLORS['bg_dark']).pack(side=tk.LEFT, padx=20)

        # ---- Status Bar ----
        status_frame = tk.Frame(main, bg=COLORS['bg_panel'], relief=tk.FLAT)
        status_frame.pack(fill=tk.X, pady=(0, 10))

        self.indicators = {}
        for name, label in [('xplane', 'X-Plane 11'),
                            ('shm', 'Shared Memory'),
                            ('txi', 'GDU TXi'),
                            ('gtn', 'GTN Xi')]:
            f = tk.Frame(status_frame, bg=COLORS['bg_panel'])
            f.pack(side=tk.LEFT, padx=15, pady=8)
            ind = StatusIndicator(f)
            ind.pack(side=tk.LEFT, padx=(0, 5))
            tk.Label(f, text=label, font=('Segoe UI', 9),
                     fg=COLORS['text_primary'], bg=COLORS['bg_panel']).pack(side=tk.LEFT)
            self.indicators[name] = ind

        self.fps_label = tk.Label(status_frame, text='0 Hz',
                                   font=('Consolas', 10, 'bold'),
                                   fg=COLORS['accent_green'],
                                   bg=COLORS['bg_panel'])
        self.fps_label.pack(side=tk.RIGHT, padx=15)
        tk.Label(status_frame, text='Rate:', font=('Segoe UI', 9),
                 fg=COLORS['text_secondary'],
                 bg=COLORS['bg_panel']).pack(side=tk.RIGHT)

        # ---- Content area (2 columns) ----
        content = tk.Frame(main, bg=COLORS['bg_dark'])
        content.pack(fill=tk.BOTH, expand=True)
        content.columnconfigure(0, weight=3)
        content.columnconfigure(1, weight=2)

        # ---- Left: Data Gauges ----
        left = tk.Frame(content, bg=COLORS['bg_dark'])
        left.grid(row=0, column=0, sticky='nsew', padx=(0, 5))

        # GPS Card
        gps_card = tk.LabelFrame(left, text=' 📍 GPS / Posição ',
                                  font=('Segoe UI', 10, 'bold'),
                                  fg=COLORS['accent_blue'], bg=COLORS['bg_card'],
                                  relief=tk.FLAT, bd=2)
        gps_card.pack(fill=tk.X, pady=(0, 8))

        gps_inner = tk.Frame(gps_card, bg=COLORS['bg_card'])
        gps_inner.pack(fill=tk.X, padx=5, pady=5)

        self.gauge_lat = DataGauge(gps_inner, 'LAT', '°')
        self.gauge_lat.pack(side=tk.LEFT, padx=5, expand=True, fill=tk.X)
        self.gauge_lon = DataGauge(gps_inner, 'LON', '°')
        self.gauge_lon.pack(side=tk.LEFT, padx=5, expand=True, fill=tk.X)
        self.gauge_alt = DataGauge(gps_inner, 'ALT', 'm MSL')
        self.gauge_alt.pack(side=tk.LEFT, padx=5, expand=True, fill=tk.X)
        self.gauge_gs = DataGauge(gps_inner, 'GS', 'kt')
        self.gauge_gs.pack(side=tk.LEFT, padx=5, expand=True, fill=tk.X)

        # Attitude Card
        att_card = tk.LabelFrame(left, text=' 🧭 Attitude / Air Data ',
                                  font=('Segoe UI', 10, 'bold'),
                                  fg=COLORS['accent_blue'], bg=COLORS['bg_card'],
                                  relief=tk.FLAT, bd=2)
        att_card.pack(fill=tk.X, pady=(0, 8))

        att_inner = tk.Frame(att_card, bg=COLORS['bg_card'])
        att_inner.pack(fill=tk.X, padx=5, pady=5)

        self.gauge_hdg = DataGauge(att_inner, 'HDG', '°')
        self.gauge_hdg.pack(side=tk.LEFT, padx=5, expand=True, fill=tk.X)
        self.gauge_ias = DataGauge(att_inner, 'IAS', 'kt')
        self.gauge_ias.pack(side=tk.LEFT, padx=5, expand=True, fill=tk.X)
        self.gauge_vs = DataGauge(att_inner, 'VS', 'fpm')
        self.gauge_vs.pack(side=tk.LEFT, padx=5, expand=True, fill=tk.X)

        # Engine Card
        eng_card = tk.LabelFrame(left, text=' ⚙ Motor PT6 (EIS) ',
                                  font=('Segoe UI', 10, 'bold'),
                                  fg=COLORS['accent_yellow'], bg=COLORS['bg_card'],
                                  relief=tk.FLAT, bd=2)
        eng_card.pack(fill=tk.X, pady=(0, 8))

        eng_inner = tk.Frame(eng_card, bg=COLORS['bg_card'])
        eng_inner.pack(fill=tk.X, padx=5, pady=5)

        self.gauge_n1 = DataGauge(eng_inner, 'N1', '%')
        self.gauge_n1.pack(side=tk.LEFT, padx=5, expand=True, fill=tk.X)
        self.gauge_trq = DataGauge(eng_inner, 'TRQ', '%')
        self.gauge_trq.pack(side=tk.LEFT, padx=5, expand=True, fill=tk.X)
        self.gauge_itt = DataGauge(eng_inner, 'ITT', '°C')
        self.gauge_itt.pack(side=tk.LEFT, padx=5, expand=True, fill=tk.X)
        self.gauge_rpm = DataGauge(eng_inner, 'RPM', '')
        self.gauge_rpm.pack(side=tk.LEFT, padx=5, expand=True, fill=tk.X)

        # ---- Right: Controls + Log ----
        right = tk.Frame(content, bg=COLORS['bg_dark'])
        right.grid(row=0, column=1, sticky='nsew', padx=(5, 0))

        # Controls Card
        ctrl_card = tk.LabelFrame(right, text=' 🎮 Controles ',
                                   font=('Segoe UI', 10, 'bold'),
                                   fg=COLORS['accent'], bg=COLORS['bg_panel'],
                                   relief=tk.FLAT, bd=2)
        ctrl_card.pack(fill=tk.X, pady=(0, 8))

        btn_frame = tk.Frame(ctrl_card, bg=COLORS['bg_panel'])
        btn_frame.pack(fill=tk.X, padx=10, pady=10)

        style = ttk.Style()
        style.configure('Action.TButton', font=('Segoe UI', 10))

        self.btn_start = ttk.Button(btn_frame, text='▶ Iniciar Bridge',
                                     command=self._on_start, style='Action.TButton')
        self.btn_start.pack(fill=tk.X, pady=3)

        self.btn_stop = ttk.Button(btn_frame, text='■ Parar Bridge',
                                    command=self._on_stop, style='Action.TButton',
                                    state=tk.DISABLED)
        self.btn_stop.pack(fill=tk.X, pady=3)

        ttk.Separator(btn_frame, orient=tk.HORIZONTAL).pack(fill=tk.X, pady=8)

        self.btn_launch = ttk.Button(btn_frame, text='🚀 Abrir Garmin Trainer',
                                      command=self._on_launch, style='Action.TButton')
        self.btn_launch.pack(fill=tk.X, pady=3)

        self.btn_inject = ttk.Button(btn_frame, text='💉 Injetar DLLs',
                                      command=self._on_inject, style='Action.TButton',
                                      state=tk.DISABLED)
        self.btn_inject.pack(fill=tk.X, pady=3)

        # Direct launch button (green, distinctive)
        style.configure('Direct.TButton', font=('Segoe UI', 10, 'bold'),
                         foreground='#00a86b')
        self.btn_direct = ttk.Button(btn_frame, text='⚡ Power On Direto',
                                      command=self._on_direct_launch,
                                      style='Direct.TButton')
        self.btn_direct.pack(fill=tk.X, pady=3)

        ttk.Separator(btn_frame, orient=tk.HORIZONTAL).pack(fill=tk.X, pady=8)

        self.btn_config = ttk.Button(btn_frame, text='⚙ Configurações',
                                      command=self._on_config, style='Action.TButton')
        self.btn_config.pack(fill=tk.X, pady=3)

        # SIM Mode Selector
        mode_card = tk.LabelFrame(right, text=' 🎛 SIM Mode Override ',
                                   font=('Segoe UI', 10, 'bold'),
                                   fg=COLORS['text_secondary'], bg=COLORS['bg_panel'],
                                   relief=tk.FLAT, bd=2)
        mode_card.pack(fill=tk.X, pady=(8, 0))

        mode_inner = tk.Frame(mode_card, bg=COLORS['bg_panel'])
        mode_inner.pack(fill=tk.X, padx=5, pady=5)

        self.sim_mode_var = tk.IntVar(value=0xFF)  # Default: no override

        mode_options = [
            (0xFF, "Auto (No Override)"),
            (0, "Mode 0 (Normal?)"),
            (1, "Mode 1"),
            (2, "Mode 2"),
            (3, "Mode 3"),
            (4, "Mode 4 (Demo)"),
        ]

        for val, text in mode_options:
            rb = tk.Radiobutton(mode_inner, text=text, variable=self.sim_mode_var,
                                value=val, command=self._on_sim_mode_change,
                                font=('Consolas', 8), bg=COLORS['bg_panel'],
                                fg=COLORS['text_primary'], selectcolor=COLORS['bg_dark'],
                                activebackground=COLORS['bg_panel'],
                                activeforeground=COLORS['accent'])
            rb.pack(anchor=tk.W)

        self.sim_mode_label = tk.Label(mode_inner, text='DLL: aguardando...',
                                        font=('Consolas', 7), bg=COLORS['bg_panel'],
                                        fg=COLORS['text_secondary'])
        self.sim_mode_label.pack(anchor=tk.W, pady=(4, 0))

        # Log Card
        log_card = tk.LabelFrame(right, text=' 📋 Log ',
                                  font=('Segoe UI', 10, 'bold'),
                                  fg=COLORS['text_secondary'], bg=COLORS['bg_panel'],
                                  relief=tk.FLAT, bd=2)
        log_card.pack(fill=tk.BOTH, expand=True, pady=(0, 0))

        self.log_text = scrolledtext.ScrolledText(
            log_card, font=('Consolas', 8),
            bg=COLORS['bg_dark'], fg=COLORS['text_secondary'],
            insertbackground=COLORS['text_primary'],
            relief=tk.FLAT, wrap=tk.WORD, height=12
        )
        self.log_text.pack(fill=tk.BOTH, expand=True, padx=5, pady=5)
        self.log_text.configure(state=tk.DISABLED)

        self._append_log("[GarminBridge] Pronto. Clique 'Iniciar Bridge' para começar.")

    def _setup_periodic_update(self):
        """Setup periodic GUI updates from the bridge thread."""
        self._pending_logs = []
        self._pending_status = None
        self._check_updates()

    def _check_updates(self):
        """Check for pending updates from the bridge thread."""
        # Process pending logs
        if self._pending_logs:
            logs = self._pending_logs[:]
            self._pending_logs.clear()
            for msg in logs:
                self._append_log(msg)

        # Process pending status
        if self._pending_status:
            self._update_gui_status(self._pending_status)
            self._pending_status = None

        self.root.after(100, self._check_updates)  # 10Hz GUI update

    def _on_status_update(self, status):
        """Called from bridge thread — queue for GUI thread."""
        self._pending_status = status

    def _on_log(self, msg):
        """Called from bridge thread — queue for GUI thread."""
        self._pending_logs.append(msg)

    def _update_gui_status(self, status):
        """Update GUI with new status (runs in GUI thread)."""
        self.indicators['xplane'].set_status(status.get('xplane_connected', False))
        self.indicators['shm'].set_status(status.get('shm_active', False))
        self.indicators['txi'].set_status(status.get('txi_running', False))
        self.indicators['gtn'].set_status(status.get('gtn_running', False))

        rate = status.get('updates_per_sec', 0)
        self.fps_label.config(text=f'{rate:.0f} Hz')

        data = status.get('last_data', {})
        if data:
            self.gauge_lat.set_value(data.get('lat', 0), '{:.4f}')
            self.gauge_lon.set_value(data.get('lon', 0), '{:.4f}')
            self.gauge_alt.set_value(data.get('alt', 0), '{:.0f}')
            self.gauge_hdg.set_value(data.get('hdg', 0), '{:.0f}')
            self.gauge_ias.set_value(data.get('ias', 0), '{:.0f}')
            self.gauge_vs.set_value(data.get('vs', 0), '{:.0f}')
            self.gauge_n1.set_value(data.get('n1', 0), '{:.1f}')

    def _append_log(self, msg):
        """Append a message to the log text widget."""
        self.log_text.configure(state=tk.NORMAL)
        self.log_text.insert(tk.END, msg + '\n')
        self.log_text.see(tk.END)
        self.log_text.configure(state=tk.DISABLED)

    # ---- Button Handlers ----

    def _on_start(self):
        self.btn_start.config(state=tk.DISABLED)
        self.btn_stop.config(state=tk.NORMAL)
        self.btn_inject.config(state=tk.NORMAL)
        self.bridge.start()

    def _on_stop(self):
        self.btn_start.config(state=tk.NORMAL)
        self.btn_stop.config(state=tk.DISABLED)
        self.btn_inject.config(state=tk.DISABLED)
        self.bridge.stop()

    def _on_launch(self):
        self.bridge.launch_trainer()

    def _on_inject(self):
        threading.Thread(target=self.bridge.inject_garmin, daemon=True).start()

    def _on_direct_launch(self):
        """Launch simulators directly, bypassing Garmin Trainer."""
        self.btn_direct.config(state=tk.DISABLED)
        self._append_log("[GUI] Iniciando lancamento direto...")

        def run():
            try:
                self.bridge.launch_direct()
            except Exception as e:
                self._pending_logs.append(f"[GUI] ERRO no lancamento direto: {e}")
            finally:
                self.root.after(0, lambda: self.btn_direct.config(state=tk.NORMAL))

        threading.Thread(target=run, daemon=True).start()

    def _on_sim_mode_change(self):
        """Handle SIM mode radio button change."""
        mode = self.sim_mode_var.get()
        mode_names = {0xFF: "Auto", 0: "Mode 0", 1: "Mode 1", 2: "Mode 2", 3: "Mode 3", 4: "Mode 4 (Demo)"}
        mode_name = mode_names.get(mode, f"Mode {mode}")
        self._append_log(f"[SIM Mode] Alterando para: {mode_name} (valor={mode})")

        # Set the mode on the shared memory object so it's written on every cycle
        if hasattr(self.bridge, 'shm') and self.bridge.shm:
            self.bridge.shm._sim_mode_override = mode
            self.sim_mode_label.config(text=f'Enviando: {mode_name}')
        else:
            self.sim_mode_label.config(text='SHM não conectada')

    def _on_config(self):
        """Open configuration dialog."""
        dialog = tk.Toplevel(self.root)
        dialog.title('Configurações — GarminBridge')
        dialog.geometry('600x500')
        dialog.configure(bg=COLORS['bg_dark'])
        dialog.transient(self.root)
        dialog.grab_set()

        # Config entries
        entries = {}
        row = 0

        config_fields = [
            ('xplane_host', 'X-Plane Host', 'str'),
            ('xplane_port', 'X-Plane Port', 'int'),
            ('update_rate_hz', 'Taxa de Atualização (Hz)', 'int'),
            ('garmin_trainer_path', 'Caminho Garmin Trainer', 'path'),
            ('dll_injector_path', 'Caminho DllInjector.exe', 'path'),
            ('garmin_hook_dll_path', 'Caminho GarminHook.dll', 'path'),
            ('txi_process_name', 'Processo GDU TXi', 'str'),
            ('gtn_process_name', 'Processo GTN Xi', 'str'),
        ]

        frame = tk.Frame(dialog, bg=COLORS['bg_dark'])
        frame.pack(fill=tk.BOTH, expand=True, padx=20, pady=20)

        for key, label, ftype in config_fields:
            lbl = tk.Label(frame, text=label, font=('Segoe UI', 9),
                           fg=COLORS['text_secondary'], bg=COLORS['bg_dark'],
                           anchor=tk.W)
            lbl.grid(row=row, column=0, sticky=tk.W, pady=3)

            val = self.config.get(key, '')
            entry = tk.Entry(frame, font=('Consolas', 9),
                             bg=COLORS['bg_panel'], fg=COLORS['text_primary'],
                             insertbackground=COLORS['text_primary'],
                             relief=tk.FLAT)
            entry.insert(0, str(val))
            entry.grid(row=row, column=1, sticky=tk.EW, padx=(10, 0), pady=3)

            if ftype == 'path':
                btn = ttk.Button(frame, text='...', width=3,
                                  command=lambda e=entry: self._browse_file(e))
                btn.grid(row=row, column=2, padx=(5, 0), pady=3)

            entries[key] = (entry, ftype)
            row += 1

        frame.columnconfigure(1, weight=1)

        # Save button
        def save():
            for key, (entry, ftype) in entries.items():
                val = entry.get()
                if ftype == 'int':
                    self.config[key] = int(val)
                else:
                    self.config[key] = val
            save_config(self.config)
            self.bridge.config = self.config
            self._append_log("[Config] Configurações salvas!")
            dialog.destroy()

        btn_frame = tk.Frame(dialog, bg=COLORS['bg_dark'])
        btn_frame.pack(fill=tk.X, padx=20, pady=(0, 20))

        ttk.Button(btn_frame, text='Salvar', command=save).pack(side=tk.RIGHT, padx=5)
        ttk.Button(btn_frame, text='Cancelar',
                    command=dialog.destroy).pack(side=tk.RIGHT, padx=5)

    def _browse_file(self, entry):
        path = filedialog.askopenfilename()
        if path:
            entry.delete(0, tk.END)
            entry.insert(0, path)

    def run(self):
        """Start the GUI main loop."""
        self.root.protocol('WM_DELETE_CLOSE', self._on_close)
        self.root.mainloop()

    def _on_close(self):
        """Handle window close."""
        if self.bridge.running:
            self.bridge.stop()
        self.root.destroy()


def run_gui(config):
    """Entry point for GUI mode."""
    gui = BridgeGUI(config)
    gui.run()


if __name__ == '__main__':
    config = load_config()
    run_gui(config)
