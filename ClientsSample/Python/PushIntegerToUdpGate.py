import socket
import random
import time
import keyboard
port = 3614  # UDP port number
time_wait = 1  # Time to wait before closing the socket
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

def push_bytes_on_udp_port( int_value):
        data = int_value.to_bytes(4, 'little')
        sock.sendto(data, ('localhost', port))




while True:
        try:
                int_value = int(input("Enter an integer: "))
                print("Integer:", int_value)
                push_bytes_on_udp_port(int_value)
        except ValueError:
                print("Invalid input. Please enter an integer.")