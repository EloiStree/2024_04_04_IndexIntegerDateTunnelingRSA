import socket
import random
import time

port = 3614  # UDP port number
time_wait = 1  # Time to wait before closing the socket
def push_bytes_on_udp_port( port):
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    quit_bool=False
    while not quit_bool:
        int_value = random.randint(1, 8)
        data = int_value.to_bytes(4, 'little')
        print(f"Pushing {int_value} to port {port}")
        sock.sendto(data, ('localhost', port))
        time.sleep(time_wait)

    sock.close()

# Example usage
push_bytes_on_udp_port( port)