# Example Scratch https://scratch.mit.edu/projects/1018559011/editor/
import socket
import struct
import time
server_address = ('', 5649)




# Bind the socket to a specific IP address and port
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
sock.bind(server_address)


print("Hello IID WS RSA Debug integer")
# Listen for incoming data
while True:
    byte_received, address = sock.recvfrom(1024)  # Adjust the buffer size as needed
    #print(f'Received {len(byte_received)} bytes from {address}: {byte_received}')
    if byte_received is not None:
        if len(byte_received) == 16:
            index = struct.unpack('<i', byte_received[0:4])[0]
            value = struct.unpack('<i', byte_received[4:8])[0]
            ulong_milliseconds = struct.unpack('<q', byte_received[8:16])[0]
            print(f"Received Bytes {index} | {value} | { ulong_milliseconds}")
            
          
          
          