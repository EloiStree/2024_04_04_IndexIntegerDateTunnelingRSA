import socket
import random
import time
import keyboard
port = 3614  # UDP port number
time_wait = 1  # Time to wait before closing the socket
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)


dico_previous_state = {}

#use_debug_info = True
use_debug_info = False

dico_to_int_up={}
dico_to_int_down={}
if use_debug_info:
    dico_to_int_up = { 'up': 301, 'down': 302, 'left': 303, 'right': 304,'space': 305,
                    'a': 311, 'z': 312, 'e': 313, 'r': 314,'t': 315,
                    'q': 316, 's': 317, 'd': 318, 'f': 319,
                    }
    dico_to_int_down = { 'up': 401, 'down': 402, 'left': 403, 'right': 404,'space': 405,
                        'a': 411, 'z': 412, 'e': 413, 'r': 414,'t': 415,
                        'q': 416, 's': 417, 'd': 418, 'f': 419
                        }
else:
    dico_to_int_up = { 'up': 101, 'down': 102, 'left': 103, 'right': 104,'space': 105,
                    'a': 111, 'z': 112, 'e': 113, 'r': 114,'t': 115,
                    'q': 116, 's': 117, 'd': 118, 'f': 119,
                    }
    dico_to_int_down = { 'up': 201, 'down': 202, 'left': 203, 'right': 204,'space': 205,
                        'a': 211, 'z': 212, 'e': 213, 'r': 214,'t': 215,
                        'q': 216, 's': 217, 'd': 218, 'f': 219
                        }

def push_bytes_on_udp_port( int_value):
        data = int_value.to_bytes(4, 'little')
        sock.sendto(data, ('localhost', port))


def on_arrow_press(string_key):    
    
        if  string_key not in dico_previous_state or dico_previous_state[string_key]==0:
            dico_previous_state[string_key] = 1
            print(f"Arrow {string_key} key pressed{dico_to_int_up[string_key]}")
            push_bytes_on_udp_port(dico_to_int_up[string_key])

def on_arrow_release(string_key):
    
        if string_key not in dico_previous_state or dico_previous_state[string_key]==1:
            dico_previous_state[string_key] = 0
            print(f"Arrow {string_key} key released {dico_to_int_down[string_key]}")
            push_bytes_on_udp_port(dico_to_int_down[string_key])





while True:

    

    for key, value in dico_to_int_up.items():
        if keyboard.is_pressed(key):
            on_arrow_press(key)
    for key, value in dico_to_int_down.items():
        if not keyboard.is_pressed(key):
            on_arrow_release(key)
    time.sleep(0.0001)



