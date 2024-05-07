from cryptography.hazmat.primitives.asymmetric import rsa
from cryptography.hazmat.primitives import serialization
from cryptography.hazmat.primitives.serialization import load_pem_private_key
from cryptography.hazmat.primitives.serialization import Encoding
from cryptography.hazmat.primitives.serialization import PrivateFormat
from cryptography.hazmat.primitives.serialization import NoEncryption
from cryptography.hazmat.primitives.serialization import PublicFormat
from cryptography.hazmat.primitives import hashes
from cryptography.hazmat.primitives.asymmetric import padding
import base64
import asyncio
import websockets
import time
import threading
import datetime
import random
import asyncio
import struct
import socket
import os
import ntplib
from datetime import datetime, timezone


udp_port_to_listen = 3614
ws_url = "ws://81.240.94.97:4501"

public_key =None
private_key =None

use_random_push=True
time_between_random_push=5
random_range_min=15000
random_range_max=45000


NTP_SERVERS = ['3.be.pool.ntp.org']
response = None
for server in NTP_SERVERS:
    client = ntplib.NTPClient()
    response = client.request(server, version=3)
    print(f"server: {server}")
    print(f"client time of request: {datetime.fromtimestamp(response.orig_time, timezone.utc)}")
    print(f"server responded with: {datetime.fromtimestamp(response.tx_time, timezone.utc)}")
    print(f"current time: {datetime.now(timezone.utc)}")
    print(f"offset: {response.offset}") 
    orig_timestamp = response.orig_time
    tx_timestamp = response.tx_time

    # Convert NTP timestamps to datetime objects
    orig_datetime = datetime.fromtimestamp(orig_timestamp,timezone.utc).timestamp()
    tx_datetime = datetime.fromtimestamp(tx_timestamp,timezone.utc).timestamp()

    print(f"{orig_datetime}\t\t:Client UTC")
    print(f"{tx_datetime} \t\t: Server UTC")


def get_current_time_with_offset():
    global response
    offset = response.offset
    current_datetime = datetime.now(timezone.utc)
    current_timestamp_utc = current_datetime.timestamp()
    current_timestamp_with_offset = current_timestamp_utc + offset
    return current_timestamp_with_offset

print(f"{get_current_time_with_offset()} \t\t: Client with offset")
time.sleep(3)
print(f"{get_current_time_with_offset()} \t\t: Client with offset 3s later")


private_key_path = os.path.abspath('RSA_PRIVATE_PEM.txt')

print("---- RSA_PRIVATE_PEM.txt Path |", private_key_path)s


# Check if 'private_key.pem' exists




if not os.path.exists('RSA_PRIVATE_PEM.txt'):
    print ("Generating new keys")
    # Generate a new RSA key pair
    private_key = rsa.generate_private_key(
        public_exponent=65537,
        key_size=1024
    )
    public_key = private_key.public_key()

    # Serialize the keys to PEM format
    private_pem = private_key.private_bytes(
        encoding=serialization.Encoding.PEM,
        format=serialization.PrivateFormat.PKCS8,
        encryption_algorithm=serialization.NoEncryption()
    )
    public_pem = public_key.public_bytes(
        encoding=serialization.Encoding.PEM,
        format=serialization.PublicFormat.PKCS1
    )         

    # Save the keys to files
    with open('RSA_PRIVATE_PEM.txt', 'wb') as f:
        f.write(private_pem)

    with open('RSA_PUBLIC_PEM.txt', 'wb') as f:
        f.write(public_pem)




# Load the private key from file
with open('RSA_PRIVATE_PEM.txt', 'rb') as f:
    private_key = serialization.load_pem_private_key(
        f.read(),
        password=None
    )
    
public_key = private_key.public_key()

# Serialize the keys to PEM format
private_pem = private_key.private_bytes(
    encoding=serialization.Encoding.PEM,
    format=serialization.PrivateFormat.PKCS8,
    encryption_algorithm=serialization.NoEncryption()
)
public_pem = public_key.public_bytes(
    encoding=serialization.Encoding.PEM,
    format=serialization.PublicFormat.PKCS1
)  

# Print the public and private keys
print("Public Key:\n", public_pem.decode('utf-8'))
print("Private Key:\n", private_pem.decode('utf-8'))




def sign_message(message):
    global private_key
    global public_key
    # Sign a message
    signature = private_key.sign(
        message,
        padding.PKCS1v15(),
        hashes.SHA256()
    )

    # Convert the signature to base64
    signature_b64 = base64.b64encode(signature)



    # Print the base64 encoded signature
    print("Signature (Base64):\n", signature_b64.decode('utf-8'))

    # Convert the base64 encoded signature back to bytes
    signature_bytes = base64.b64decode(signature_b64)

    # Verify the signature
    public_key = private_key.public_key()
    try:
        public_key.verify(
            signature_bytes,
            message,
            padding.PKCS1v15(),
            hashes.SHA256()
        )
        print("Signature is valid.")
    except Exception as e:
        print("Signature is invalid.")


    return signature_b64.decode('utf-8')



 

is_connected_to_server = False
websocket_linked=None



async def action():
    usePrintDebug= False
    useUTF8= False
    # Code for the action to be performed every 5 seconds
    if(usePrintDebug): 
        print("Performing action...")
    if(is_connected_to_server):
        if(usePrintDebug): 
            print("Connected to server")
        if websocket_linked is not None:
            if(usePrintDebug): 
                print("Sending message to server")
            random_int = random.randint(random_range_min, random_range_max)
            
            ulong_milliseconds=int(get_current_time_with_offset() * 1000)

            #ulong_milliseconds = int(datetime.datetime.now(datetime.timezone.utc).timestamp() * 1000)          
            data = bytearray(12)
            
            random_bytes = random_int.to_bytes(4, byteorder='little')
            milliseconds_bytes = ulong_milliseconds.to_bytes(8, byteorder='little')
            data = random_bytes + milliseconds_bytes

            if(usePrintDebug): 
                print("Debug: ", data)
                print("Size: ", len(data))
                print("B64: ", datab64)
            try:
                print(f"Random Push: {random_int} Milliseconds: {ulong_milliseconds}")
                if(useUTF8):
                    datab64 =f"b|{base64.b64encode(data).decode('utf-8')}" 
                    await websocket_linked.send(datab64)
                    if(usePrintDebug): 
                        print("Data sent:", datab64)
                else :
                    await websocket_linked.send(data)
            except:
                print("Error sending data")


def perform_action():
    while True:
        asyncio.run(action())
        time.sleep(time_between_random_push)


if(use_random_push):
    udp_thread = threading.Thread(target=perform_action)
    udp_thread.start()




def listen_udp():
    global udp_port_to_listen
    given_data_previous = ""
    udp_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    udp_socket.bind(('0.0.0.0', udp_port_to_listen))
    while True:
        # Receive data from the socket
        data, address = udp_socket.recvfrom(64)

        # Process the received data
        print(f"Received UDP message from {address}: {data.decode('utf-8')}")
        if(websocket_linked is not None and  is_connected_to_server):
            try:
                given_data = data.decode('utf-8')
                if given_data_previous != given_data:
                    websocket_linked.send(given_data)
                else:
                    print("Data already sent")
                given_data_previous = given_data
            except Exception as e:
                print("Error sending data:", str(e))

# Create a thread for listening to UDP
udp_thread = threading.Thread(target=listen_udp)

# Start the thread
udp_thread.start()


async def on_int_as_bytes(ws, byte_received):

    if byte_received is not None:
        if len(byte_received) == 16:
            index = struct.unpack('<i', byte_received[0:4])[0]
            value = struct.unpack('<i', byte_received[4:8])[0]
            ulong_milliseconds = struct.unpack('<q', byte_received[8:16])[0]
        

            print(f"Received Bytes {index} | {value} | { ulong_milliseconds}")


async def on_message(ws, message):
    global is_connected_to_server
    print(f"Received message: {message}")
    if message.startswith("SIGNIN:"):
        # Extract the signed message from the response
        signed_message = message[7:].strip()
        signature_b64s = sign_message(signed_message.encode('utf-8'))
        print(f"SIGNED:{signature_b64s}")
        to_send = f"SIGNED:{signature_b64s}"
        # Send the signature
        await ws.send(to_send)
    
    if message.startswith("RSA:Verified"):
        print(f"RSA Verified :) ")
        is_connected_to_server = True
 
async def on_error(ws, error):
    print(f"Error: {error}")
    global is_connected_to_server
    is_connected_to_server = False

async def on_close(ws):
    global websocket_linked
    print("WebSocket connection closed")
    websocket_linked=None

async def on_open(ws):
    global websocket_linked
    print("WebSocket connection opened")
    websocket_linked=ws
    
    message = "Hello "+public_pem.decode('utf-8')
    await ws.send(message)


async def websocket_listener(uri):
    
    while True:
        async with websockets.connect(uri) as websocket:

            await on_open(websocket)
            while websocket.open:
                try:
                    async for message in websocket:
                        if isinstance(message, bytes):
                            
                            #print("Received binary message:", message)
                            await on_int_as_bytes(websocket, message)
                        elif isinstance(message, str):
                            #print("Received text message:", message)
                            await on_message(websocket, message)
                        
                            
                except Exception as e:
                    print("Error receiving data:", str(e))
                    await on_error(websocket, str(e))
                    
            await on_close(websocket)
        await asyncio.sleep(5)
        print("Reconnecting to server...")
        
            

if __name__ == "__main__":

    asyncio.run(websocket_listener(ws_url))
    
