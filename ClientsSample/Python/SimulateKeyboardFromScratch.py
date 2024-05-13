# Example Scratch https://scratch.mit.edu/projects/1018559011/editor/
import socket
import struct
import time
import pyautogui
server_address = ('', 5648)



dico_previous_state = {}


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
int_up_to_dico = {v: k for k, v in dico_to_int_up.items()}
int_down_to_dico = {v: k for k, v in dico_to_int_down.items()}


def integer_to_keyboard_input(int_value):
    if int_up_to_dico.get(int_value) is not None:
        pyautogui.keyDown(int_up_to_dico[int_value],_pause=False)
    if int_down_to_dico.get(int_value) is not None:
        pyautogui.keyUp(int_down_to_dico[int_value],_pause=False)



    


# Create a UDP socket

# Bind the socket to a specific IP address and port
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
sock.bind(server_address)



# Listen for incoming data
while True:
    byte_received, address = sock.recvfrom(1024)  # Adjust the buffer size as needed
    #print(f'Received {len(byte_received)} bytes from {address}: {byte_received}')
    if byte_received is not None:
        if len(byte_received) == 16:
            index = struct.unpack('<i', byte_received[0:4])[0]
            value = struct.unpack('<i', byte_received[4:8])[0]
            ulong_milliseconds = struct.unpack('<q', byte_received[8:16])[0]
            #print(f"Received Bytes {index} | {value} | { ulong_milliseconds}")
            integer_to_keyboard_input(value)

          
          
          
          
'''

Alphanumeric Keys:
------------------
- Letters (A-Z)
- Numbers (0-9)

Special Characters:
-------------------
- !, @, #, $, %, ^, &, *, (, ), etc.

Function Keys:
--------------
- F1, F2, F3, ..., F12

Arrow Keys:
-----------
- Up, Down, Left, Right

Modifier Keys:
--------------
- Shift, Ctrl, Alt, Super/Windows key

Navigation Keys:
----------------
- Home, End, Page Up, Page Down

Other Function Keys:
---------------------
- Enter/Return
- Backspace/Delete
- Tab
- Escape (Esc)
- Spacebar
- Caps Lock
- Num Lock
- Scroll Lock
- Print Screen
- Pause/Break
- Insert
- Delete
'''