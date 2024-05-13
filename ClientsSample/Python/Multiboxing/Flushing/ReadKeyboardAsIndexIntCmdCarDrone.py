import keyboard
import threading
import time
from keyboard import KEY_DOWN, KEY_UP
import random
import socket
import struct


def generate_random_number():
    return random.randint(0, 999999999)

intCmdValue =1000000000
intCmdValuePrevious =1000000000

# Random player id
intIndex_player =generate_random_number()
# Fixed player id
#intIndex_player =78645


# What is the computer you want to redirect index int cmd to
host_int_cmd = '127.0.0.1'
# What is the port (the app) that need to received those int cmd information
port_int_cmd = 12346

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

def on_value_changed():
        global intCmdValuePrevious
        value = 1000000000  # Initialize value to a large number
        if key_state.get('a') == 1 or  key_state.get('num7') == 1:  
            value += 100000000  
        if key_state.get('e') == 1 or  key_state.get('num9') == 1:  
            value += 10000000  
        if key_state.get('q') == 1 or  key_state.get('num4') == 1:  
            value += 1000000  
        if key_state.get('d') == 1 or  key_state.get('num6') == 1:  
            value += 100000  
        if key_state.get('h') == 1:  
            value += 90000  
        elif key_state.get('f') == 1:  
            value += 20000  
        if key_state.get('t') == 1:  
            value += 9000  
        elif key_state.get('g') == 1:  
            value += 2000

            
        if key_state.get('l') == 1:  
            value += 900  
        elif key_state.get('j') == 1:  
            value += 200

            
        if key_state.get('i') == 1:  
            value += 90  
        elif key_state.get('k') == 1:  
            value += 20
            
        if key_state.get('space') == 1 or  key_state.get('num8') == 1:  
            value += 1  
        if(value!= intCmdValuePrevious):
            intCmdValuePrevious= value
            send_intCmd_value(intIndex_player, value)
            print(f"Input: {value}")  # Print the value


# Dictionary to store the state of keys
key_state = {}

# Function to handle key press events
def on_press(event):
    if event.name in {'q','a','e', 'd', 's', 'z', 'f', 'h', 't', 'g',
                      'j', 'l', 'i', 'k', 'space',
                      'num1', 'num2', 'num3', 'num4', 'num5', 'num6', 'num7', 'num8', 'num9', 'num0', 'num*','num/','num+', 'num-'}:     
        key_state[event.name] = 1
        print(f"{event.name} Key Pressed")
        on_value_changed()

# Function to handle key release events
def on_release(event):
    if event.name in {'q','a','e', 'd', 's', 'z', 'f', 'h', 't', 'g', 'j', 'l', 'i', 'k', 'space','num1', 'num2', 'num3', 'num4', 'num5', 'num6', 'num7', 'num8', 'num9', 'num0', 'num*','num/','num+', 'num-'}:
        key_state[event.name] = 0
        print(f"{event.name} Key Released")
        on_value_changed()

# Function to handle both key press and release events
def on_action(event):
    if event.event_type == KEY_DOWN:
        on_press(event)
    elif event.event_type == KEY_UP:
        on_release(event)

# Hook the action handler to keyboard events
keyboard.hook(lambda e: on_action(e))



while True:
    time.sleep(0.1)



"""
Alphanumeric keys:

'a' to 'z'
'A' to 'Z'
'0' to '9'
Special keys:

'esc'
'space'
'tab'
'enter'
'shift'
'ctrl'
'alt'
'backspace'
'caps lock'
'num lock'
'scroll lock'
'insert'
'delete'
'home'
'end'
'page up'
'page down'
'up'
'down'
'left'
'right'
'f1' to 'f24'
'print screen'
'pause'
Numpad keys:

'num1' to 'num9'
'num0' (zero)
'num*' (multiplication)
'num/' (division)
'num+' (addition)
'num-' (subtraction)
'num lock' (Num Lock key)
Additional keys (may vary depending on keyboard layout and system):

'browser back'
'browser forward'
'browser refresh'
'browser stop'
'browser search'
'browser favorites'
'browser start and home'
'volume mute'
'volume down'
'volume up'
'media next track'
'media previous track'
'media stop'
'media play/pause'
'launch mail'
'launch media select'
'launch app 1'
'launch app 2'
"""
