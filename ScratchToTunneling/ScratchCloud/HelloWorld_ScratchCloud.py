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
from scratchcloud import CloudClient, CloudChange




def read_file(file_path):
    with open(file_path, 'r') as file:
        content = file.read()
    return content

file_path = 'C:\Key\ScratchPassword.txt'
password = read_file(file_path)
#print("Pass:"+password)


client = CloudClient(username='EloiStree', project_id='967799973')

@client.event
async def on_connect():
  print('Hello world!')

@client.event
async def on_disconnect():
  print('Goodbye world!')

@client.event
async def on_message(var: CloudChange):
  print(f'{var.name} changed to {var.value}   {var.id}   {var.sender}   {var .received_at}!')

client.run(password)


