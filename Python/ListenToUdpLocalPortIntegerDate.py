import socket
import threading

def listen_udp(port, isUtf8):
    # Create a UDP socket
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    sock.bind(('localhost', port))

    print(f"Listening for UDP messages on port {port}...")

    while True:
        # Receive data from the socket
        data, addr = sock.recvfrom(600)

        # Decode the received data as text
        if(isUtf8):
            message = data.decode('utf-8')
            print(f"Received message: {message}")
        else:
            message = f"{len(data)} : {str(data)}" #''.join([str(x.hex()) for x in data])
            if len(data) == 12:
                value_integer = int.from_bytes(data[0:4], byteorder='little', signed=True)
                date_ulong = int.from_bytes(data[4:], byteorder='little', signed=False) 
                print(f"Received message: {message} : {value_integer} : {date_ulong}")




# Create a thread to run the listen_udp function
thread = threading.Thread(target=listen_udp, args=(3618,False))

# Start the thread
thread.start()
