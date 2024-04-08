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
import websocket
import threading
import datetime
import random

import struct
import socket
import os


udp_port_to_listen = 3614
ws_url = "ws://81.240.94.97:3615"

public_key =None
private_key =None

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
    except InvalidSignature:
        print("Signature is invalid.")


    return signature_b64.decode('utf-8')



 

is_connected_to_server = False
websocket_linked=None



def action():
    # Code for the action to be performed every 5 seconds
    print("Performing action...")
    if(is_connected_to_server):
        print("Connected to server")
        if websocket_linked is not None:
            print("Sending message to server")
            random_int = random.randint(1, 10000000)
            
            ulong_milliseconds = int(datetime.datetime.now(datetime.timezone.utc).timestamp() * 1000)          
            data = bytearray(12)
            
            random_bytes = random_int.to_bytes(4, byteorder='little')
            milliseconds_bytes = ulong_milliseconds.to_bytes(8, byteorder='little')
            data = random_bytes + milliseconds_bytes

            datab64 =f"b|{base64.b64encode(data).decode('utf-8')}" 
            print("Debug: ", data)
            print("Size: ", len(data))
            print(f"Random: {random_int} Milliseconds: {ulong_milliseconds}")
            print("B64: ", datab64)
            try:
                #SEND DATA CREATE A ERROR BY BINARY SEND. PROTOCAL ERROR I SUPPOSE? SO I DO B64 in waiting to understand.
                websocket_linked.send(datab64)
            except:
                print("Error sending data")


def perform_action():
    while True:
        action()
        time.sleep(5)

# Create a thread for performing the action
action_thread = threading.Thread(target=perform_action)

# Start the thread
action_thread.start()



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



def on_message(ws, message):
    global is_connected_to_server
    print(f"Received message: {message}")
    if message.startswith("SIGNIN:"):
        # Extract the signed message from the response
        signed_message = message[7:].strip()
        signature_b64s = sign_message(signed_message.encode('utf-8'))
        print(f"SIGNED:{signature_b64s}")
        to_send = f"SIGNED:{signature_b64s}"
        # Send the signature
        ws.send(to_send)
    
    if message.startswith("RSA:Verified"):
        print(f"RSA Verified :) ")
        is_connected_to_server = True
 
def on_error(ws, error):
    print(f"Error: {error}")
    global is_connected_to_server
    is_connected_to_server = False

def on_close(ws):
    global websocket_linked
    print("WebSocket connection closed")
    websocket_linked=None

def on_open(ws):
    global websocket_linked
    print("WebSocket connection opened")
    websocket_linked=ws
    
    message = "Hello "+public_pem.decode('utf-8')
    ws.send(message)

if __name__ == "__main__":
    
    # Construct the WebSocket URL
    

    # Create a WebSocket connection
    ws = websocket.WebSocketApp(ws_url,
                                on_message=on_message,
                                on_error=on_error,
                                on_close=on_close)

    ws.on_open = on_open

    # Run the WebSocket connection
    ws.run_forever()
    
