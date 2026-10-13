import os

filepath = r'c:\Users\Arnom\Documents\projeto garminbridge\memscan_interactive.py'
with open(filepath, 'r', encoding='utf-8') as f:
    content = f.read()

# I want to inject a custom input function
input_func = """
_orig_input = builtins.input
def I(prompt=''):
    builtins.print(prompt, end='', flush=True)
    _log_f.write(prompt)
    _log_f.flush()
    val = _orig_input()
    _log_f.write(val + '\\n')
    _log_f.flush()
    return val

"""

idx = content.find('def L(*args, **kwargs):')
if idx != -1:
    idx2 = content.find('\n\n', idx)
    content = content[:idx2+2] + input_func + content[idx2+2:]

content = content.replace('input(', 'I(')

with open(filepath, 'w', encoding='utf-8') as f:
    f.write(content)
print('Done!')
