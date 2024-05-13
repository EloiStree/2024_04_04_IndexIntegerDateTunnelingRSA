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

dico_to_int_up = { 
     'up': 101,
    'down': 102,
      'right': 103,
        'left': 104,
        'a': 105,
        'b': 106,
        'c': 107,
        'd': 108,
        'e': 109,
        'f': 110,
        'g': 111,
        'h': 112,
        'i': 113,
        'j': 114,
        'k': 115,
        'l': 116,
        'm': 117,
        'n': 118,
        'o': 119,
        'p': 120,
        'q': 121,
        'r': 122,
        's': 123,
        't': 124,
        'u': 125,
        'v': 126,
        'w': 127,
        'x': 128,
        'y': 129,
        'z': 130,
        '0': 131,
        '1': 132,
        '2': 133,
        '3': 134,
        '4': 135,
        '5': 136,
        '6': 137,
        '7': 138,
        '8': 139,
        '9': 140,
                    }
dico_to_int_down = { 
     'up': 201,
    'down': 202,
      'right': 203,
        'left': 204,
        'a': 205,
        'b': 206,
        'c': 207,
        'd': 208,
        'e': 209,
        'f': 210,
        'g': 211,
        'h': 212,
        'i': 213,
        'j': 214,
        'k': 215,
        'l': 216,
        'm': 217,
        'n': 218,
        'o': 219,
        'p': 220,
        'q': 221,
        'r': 222,
        's': 223,
        't': 224,
        'u': 225,
        'v': 226,
        'w': 227,
        'x': 228,
        'y': 229,
        'z': 230,
        '0': 231,
        '1': 232,
        '2': 233,
        '3': 234,
        '4': 235,
        '5': 236,
        '6': 237,
        '7': 238,
        '8': 239,
        '9': 240,
                    }

def push_bytes_on_udp_port( int_value):
        data = int_value.to_bytes(4, 'little')
        sock.sendto(data, ('localhost', port))


def on_arrow_press(string_key):    
        if  string_key not in dico_previous_state:
            dico_previous_state[string_key] = 0
        if   dico_previous_state[string_key]==0:
            dico_previous_state[string_key] = 1
            print(f"Arrow {string_key} key pressed {dico_to_int_up[string_key]}")
            push_bytes_on_udp_port(dico_to_int_up[string_key])

def on_arrow_release(string_key):
    
        if  string_key not in dico_previous_state:
            dico_previous_state[string_key] = 0
        
        if dico_previous_state[string_key]==1:
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



