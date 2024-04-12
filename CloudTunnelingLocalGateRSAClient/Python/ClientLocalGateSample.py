import socket
import struct
import datetime
import random

import time;


index_count=0

date_1970=datetime.datetime(1970, 1, 1, 0, 0, 0, 0, datetime.timezone.utc)
while True:
    index_count+=1
    # Push byte array to port 5000
    byte_array = b'\x01\x02\x03\x04\x05\x06\x07\x08\x09\x0A\x0B\x0C'

    integer_value = random.randint(100000000, 999999999)

    byte_array = struct.pack('i', integer_value) + byte_array

    # Get current UTC time as milliseconds
    utc_time = datetime.datetime.now(datetime.timezone.utc)

    #milliseconds = int((utc_time - date_1970).total_seconds() * 1000)
	
    milliseconds= int(round(time.time() * 1000))
    
    byte_array = byte_array[:4] + struct.pack('Q', milliseconds) + byte_array[12:]

    udp_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    udp_socket.sendto(byte_array, ('localhost', 5010))



    # Push text to port 5001
    text = 'i|'+str(index_count)
    udp_socket.sendto(text.encode(), ('localhost', 5011))

    udp_socket.close()

    print('Tick')
    time.sleep(1)


