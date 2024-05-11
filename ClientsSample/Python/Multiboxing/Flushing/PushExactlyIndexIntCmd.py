import random
import time
import socket
import struct


index= 54654 
command= 16546

# What is the computer you want to redirect index int cmd to
host_int_cmd = '127.0.0.1'
# What is the port (the app) that need to received those int cmd information
port_int_cmd = 8089

def send_intCmd_value(intIndex, intValue):
    # Convert text inputs to integers
    intIndex = int(intIndex)
    intValue = int(intValue)
    
    # Create a bytes array with intIndex and intValue as integers
    data = struct.pack('<ii', intIndex, intValue)
    
    print(f"Sent {host_int_cmd}:{port_int_cmd}: {intIndex} {intValue}")
    
    # Create UDP socket
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    
    try:
        # Send data via UDP
        sock.sendto(data, (host_int_cmd, port_int_cmd))
        print("Data sent successfully.")
    except Exception as e:
        print("Error occurred while sending data:", e)
    finally:
        # Close the socket
        sock.close()

while True:
    print(f"Push:{index}{command}", )
    send_intCmd_value(index,command)
    time.sleep(random.uniform(0.1,0.3))
    print(f"Push:{index}{0}", )
    send_intCmd_value(index,0)
    time.sleep(random.uniform(0.1,0.3))
