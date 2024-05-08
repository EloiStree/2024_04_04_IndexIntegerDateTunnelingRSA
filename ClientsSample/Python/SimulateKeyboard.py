import socket
import struct
import keyboard
import time

server_address = ('', 5648)



dico_previous_state = {}


dico_to_int_up = {'up': 101, 'down': 102, 'left': 103, 'right': 104,'space': 105,
                'a': 111, 'z': 112, 'e': 113, 'r': 114,'t': 115,
                'q': 116, 's': 117, 'd': 118, 'f': 119,
                }
dico_to_int_down = {'up': 201, 'down': 202, 'left': 203, 'right': 204,'space': 205,
                    'a': 211, 'z': 212, 'e': 213, 'r': 214,'t': 215,
                    'q': 216, 's': 217, 'd': 218, 'f': 219
                    }
int_up_to_dico = {v: k for k, v in dico_to_int_up.items()}
int_down_to_dico = {v: k for k, v in dico_to_int_down.items()}


def integer_to_keyboard_input(int_value):
    if int_up_to_dico.get(int_value) is not None:
        keyboard.press(int_up_to_dico[int_value])
    if int_down_to_dico.get(int_value) is not None:
        keyboard.release(int_down_to_dico[int_value])



    


# Create a UDP socket

# Bind the socket to a specific IP address and port
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
sock.bind(server_address)



# Listen for incoming data
while True:
    byte_received, address = sock.recvfrom(1024)  # Adjust the buffer size as needed
    print(f'Received {len(byte_received)} bytes from {address}: {byte_received}')
    if byte_received is not None:
        if len(byte_received) == 16:
            index = struct.unpack('<i', byte_received[0:4])[0]
            value = struct.unpack('<i', byte_received[4:8])[0]
            ulong_milliseconds = struct.unpack('<q', byte_received[8:16])[0]
            print(f"Received Bytes {index} | {value} | { ulong_milliseconds}")
            integer_to_keyboard_input(value)

          