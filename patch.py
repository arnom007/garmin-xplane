import os

filepath = r'c:\Users\Arnom\Documents\projeto garminbridge\memscan_interactive.py'
with open(filepath, 'r', encoding='utf-8') as f:
    content = f.read()

content = content.replace('print(', 'L(')

header = """
import builtins
import datetime
import os

log_dir = os.path.join(os.path.expandvars(r'%APPDATA%'), 'GarminBridge')
os.makedirs(log_dir, exist_ok=True)
log_path = os.path.join(log_dir, 'memscan_interactive.log')
_log_f = open(log_path, 'a', encoding='utf-8')
_log_f.write(f'\\n\\n=== NEW SESSION: {datetime.datetime.now().isoformat()} ===\\n')

def L(*args, **kwargs):
    builtins.print(*args, **kwargs)
    end = kwargs.get('end', '\\n')
    msg = " ".join(str(a) for a in args) + end
    _log_f.write(msg)
    _log_f.flush()

"""

idx = content.find('import ctypes')
final_content = content[:idx] + header + content[idx:]

with open(filepath, 'w', encoding='utf-8') as f:
    f.write(final_content)
print('Done!')
