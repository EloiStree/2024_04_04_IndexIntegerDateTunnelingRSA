# import asyncio
# import websockets
# import rsa


# with open("RSA_PRIVATE_PEM.txt", "r") as file:
#     private_key = file.read()

# # Import the private RSA XML key
# private_key = rsa.PrivateKey.load_pkcs1(private_key.encode())
# # Create the public key from the private key
# public_key = private_key.publickey()

# # Convert the public key to XML string
# public_key_xml = public_key.save_pkcs1().decode()




# # Store the XML string in a file
# with open("RSA_PUBLIC_XML.txt", "w") as file:
#     file.write(public_key_xml)


# # Use the private key as needed

# async def connect_to_websocket():
#     uri = "ws://81.240.94.97:3615"
#     async with websockets.connect(uri) as websocket:
#         # Perform actions with the websocket connection
#         await websocket.send(f"Hello {public_key_xml}")
#         response = await websocket.recv()
#         print(f"Received message from server: {response}")
#         signature = rsa.sign(response.encode(), private_key, "SHA-256")
#         signature_str = signature.hex()
#         await websocket.send(f"SIGNED:{signature_str}")

# # Run the connection coroutine
# asyncio.get_event_loop().run_until_complete(connect_to_websocket())