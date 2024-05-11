import ctypes
import time
import socket
import pyperclip
import psutil
import pygetwindow as gw




user32 = ctypes.windll.user32

## ## ## ## ## ## ## ## 
##  PUBLIC
## ## ## ## ## ## ## ## 

## Port that app is listening to be used
lisent_udp_port_to_interact = 4556

## Do you want to us real input or send message that are send only to the app
use_real_input=False

## What window index should we use ?
target_window_index = 0


## What is the exact name to find in of the window we need to find.
window_title = "10 Second Ninja"

# Use real will simulate key, use false will send fake key

# Constants for SendMessage
WM_KEYDOWN = 0x0100
WM_KEYUP = 0x0101



# Find the window by its title
def find_window(title):
    return ctypes.windll.user32.FindWindowW(None, title)

def find_in_all(title):
    global target_window_index
    list_window_found = [window for window in gw.getAllWindows() if title in window.title]
    if list_window_found:
        return list_window_found[target_window_index]
    else:
        return None
    



## window_title = "10 Second Ninja"
## window_title = "MORDHAU  "
## window_title = "Hollow Knight"
## window_title = "Chrome"


hwnd = find_window(window_title)
use_print_log=False

keyboard_mappings = {
    "Backspace": 0x08,
    "Tab": 0x09,
    "Clear": 0x0C,
    "Enter": 0x0D,
    "Shift": 0x10,
    "Ctrl": 0x11,
    "Alt": 0x12,
    "Pause": 0x13,
    "CapsLock": 0x14,
    "Esc": 0x1B,
    "Space": 0x20,
    "PageUp": 0x21,
    "PageDown": 0x22,
    "End": 0x23,
    "Home": 0x24,
    "LeftArrow": 0x25,
    "Left": 0x25,
    "UpArrow": 0x26,
    "Up": 0x26,
    "RightArrow": 0x27,
    "Right": 0x27,
    "DownArrow": 0x28,
    "Down": 0x28,
    "Select": 0x29,
    "Print": 0x2A,
    "Execute": 0x2B,
    "PrintScreen": 0x2C,
    "Insert": 0x2D,
    "Delete": 0x2E,
    "0": 0x30,
    "1": 0x31,
    "2": 0x32,
    "3": 0x33,
    "4": 0x34,
    "5": 0x35,
    "6": 0x36,
    "7": 0x37,
    "8": 0x38,
    "9": 0x39,
    "A": 0x41,
    "B": 0x42,
    "C": 0x43,
    "D": 0x44,
    "E": 0x45,
    "F": 0x46,
    "G": 0x47,
    "H": 0x48,
    "I": 0x49,
    "J": 0x4A,
    "K": 0x4B,
    "L": 0x4C,
    "M": 0x4D,
    "N": 0x4E,
    "O": 0x4F,
    "P": 0x50,
    "Q": 0x51,
    "R": 0x52,
    "S": 0x53,
    "T": 0x54,
    "U": 0x55,
    "V": 0x56,
    "W": 0x57,
    "X": 0x58,
    "Y": 0x59,
    "Z": 0x5A,
    "LeftWindows": 0x5B,
    "RightWindows": 0x5C,
    "Applications": 0x5D,
    "Sleep": 0x5F,
    "Numpad0": 0x60,
    "Numpad1": 0x61,
    "Numpad2": 0x62,
    "Numpad3": 0x63,
    "Numpad4": 0x64,
    "Numpad5": 0x65,
    "Numpad6": 0x66,
    "Numpad7": 0x67,
    "Numpad8": 0x68,
    "Numpad9": 0x69,
    "Multiply": 0x6A,
    "NP0": 0x60,
    "NP1": 0x61,
    "NP2": 0x62,
    "NP3": 0x63,
    "NP4": 0x64,
    "NP5": 0x65,
    "NP6": 0x66,
    "NP7": 0x67,
    "NP8": 0x68,
    "NP9": 0x69,
    "Multiply": 0x6A,
    "Add": 0x6B,
    "Separator": 0x6C,
    "Subtract": 0x6D,
    "Decimal": 0x6E,
    "Divide": 0x6F,
    "F1": 0x70,
    "F2": 0x71,
    "F3": 0x72,
    "F4": 0x73,
    "F5": 0x74,
    "F6": 0x75,
    "F7": 0x76,
    "F8": 0x77,
    "F9": 0x78,
    "F10": 0x79,
    "F11": 0x7A,
    "F12": 0x7B,
    "F13": 0x7C,
    "F14": 0x7D,
    "F15": 0x7E,
    "F16": 0x7F,
    "F17": 0x80,
    "F18": 0x81,
    "F19": 0x82,
    "F20": 0x83,
    "F21": 0x84,
    "F22": 0x85,
    "F23": 0x86,
    "F24": 0x87,
    "NumLock": 0x90,
    "ScrollLock": 0x91,
    "LeftShift": 0xA0,
    "RightShift": 0xA1,
    "LeftControl": 0xA2,
    "RightControl": 0xA3,
    "LeftAlt": 0xA4,
    "RightAlt": 0xA5,
    "LeftMenu": 0xA4,
    "RightMenu": 0xA5,
    "BrowserBack": 0xA6,
    "BrowserForward": 0xA7,
    "BrowserRefresh": 0xA8,
    "BrowserStop": 0xA9,
    "BrowserSearch": 0xAA,
    "BrowserFavorites": 0xAB,
    "BrowserHome": 0xAC,
    "VolumeMute": 0xAD,
    "VolumeDown": 0xAE,
    "VolumeUp": 0xAF,
    "MediaNext Track": 0xB0,
    "MediaPrevious Track": 0xB1,
    "MediaStop": 0xB2,
    "MediaPlay": 0xB3,
    "LaunchMail": 0xB4,
    "LaunchMedia Select": 0xB5,
    "LaunchApp1": 0xB6,
    "LaunchApp2": 0xB7,
    "OEM1": 0xBA,
    "OEMPlus": 0xBB,
    "OEMComma": 0xBC,
    "OEMMinus": 0xBD,
    "OEMPeriod": 0xBE,
    "OEM2": 0xBF,
    "OEM3": 0xC0,
    "OEM4": 0xDB,
    "OEM5": 0xDC,
    "OEM6": 0xDD,
    "OEM7": 0xDE,
    "OEM8": 0xDF,
    "OEM102": 0xE2,
    "ProcessKey": 0xE5,
    "Packet": 0xE7,
    "Attn": 0xF6,
    "CrSel": 0xF7,
    "ExSel": 0xF8,
    "EraseEOF": 0xF9,
    "Play": 0xFA,
    "Zoom": 0xFB,
    "PA1": 0xFD,
    "0x08":"0x08",
    "0x09":"0x09",
    "0x0C":"0x0C",
    "0x0D":"0x0D",
    "0x10":"0x10",
    "0x11":"0x11",
    "0x12":"0x12",
    "0x13":"0x13",
    "0x14":"0x14",
    "0x1B":"0x1B",
    "0x20":"0x20",
    "0x21":"0x21",
    "0x22":"0x22",
    "0x23":"0x23",
    "0x24":"0x24",
    "0x25":"0x25",
    "0x26":"0x26",
    "0x27":"0x27",
    "0x28":"0x28",
    "0x29":"0x29",
    "0x2A":"0x2A",
    "0x2B":"0x2B",
    "0x2C":"0x2C",
    "0x2D":"0x2D",
    "0x2E":"0x2E",
    "0x30":"0x30",
    "0x31":"0x31",
    "0x32":"0x32",
    "0x33":"0x33",
    "0x34":"0x34",
    "0x35":"0x35",
    "0x36":"0x36",
    "0x37":"0x37",
    "0x38":"0x38",
    "0x39":"0x39",
    "0x41":"0x41",
    "0x42":"0x42",
    "0x43":"0x43",
    "0x44":"0x44",
    "0x45":"0x45",
    "0x46":"0x46",
    "0x47":"0x47",
    "0x48":"0x48",
    "0x49":"0x49",
    "0x4A":"0x4A",
    "0x4B":"0x4B",
    "0x4C":"0x4C",
    "0x4D":"0x4D",
    "0x4E":"0x4E",
    "0x4F":"0x4F",
    "0x50":"0x50",
    "0x51":"0x51",
    "0x52":"0x52",
    "0x53":"0x53",
    "0x54":"0x54",
    "0x55":"0x55",
    "0x56":"0x56",
    "0x57":"0x57",
    "0x58":"0x58",
    "0x59":"0x59",
    "0x5A":"0x5A",
    "0x5B":"0x5B",
    "0x5C":"0x5C",
    "0x5D":"0x5D",
    "0x5F":"0x5F",
    "0x60":"0x60",
    "0x61":"0x61",
    "0x62":"0x62",
    "0x63":"0x63",
    "0x64":"0x64",
    "0x65":"0x65",
    "0x66":"0x66",
    "0x67":"0x67",
    "0x68":"0x68",
    "0x69":"0x69",
    "0x6A":"0x6A",
    "0x6B":"0x6B",
    "0x6C":"0x6C",
    "0x6D":"0x6D",
    "0x6E":"0x6E",
    "0x6F":"0x6F",
    "0x70":"0x70",
    "0x71":"0x71",
    "0x72":"0x72",
    "0x73":"0x73",
    "0x74":"0x74",
    "0x75":"0x75",
    "0x76":"0x76",
    "0x77":"0x77",
    "0x78":"0x78",
    "0x79":"0x79",
    "0x7A":"0x7A",
    "0x7B":"0x7B",
    "0x7C":"0x7C",
    "0x7D":"0x7D",
    "0x7E":"0x7E",
    "0x7F":"0x7F",
    "0x80":"0x80",
    "0x81":"0x81",
    "0x82":"0x82",
    "0x83":"0x83",
    "0x84":"0x84",
    "0x85":"0x85",
    "0x86":"0x86",
    "0x87":"0x87",
    "0x90":"0x90",
    "0x91":"0x91",
    "0xA0":"0xA0",
    "0xA1":"0xA1",
    "0xA2":"0xA2",
    "0xA3":"0xA3",
    "0xA4":"0xA4",
    "0xA5":"0xA5",
    "0xA6":"0xA6",
    "0xA7":"0xA7",
    "0xA8":"0xA8",
    "0xA9":"0xA9",
    "0xAA":"0xAA",
    "0xAB":"0xAB",
    "0xAC":"0xAC",
    "0xAD":"0xAD",
    "0xAE":"0xAE",
    "0xAF":"0xAF",
    "0xB0":"0xB0",
    "0xB1":"0xB1",
    "0xB2":"0xB2",
    "0xB3":"0xB3",
    "0xB4":"0xB4",
    "0xB5":"0xB5",
    "0xB6":"0xB6",
    "0xB7":"0xB7",
    "0xBA":"0xBA",
    "0xBB":"0xBB",
    "0xBC":"0xBC",
    "0xBD":"0xBD",
    "0xBE":"0xBE",
    "0xBF":"0xBF",
    "0xC0":"0xC0",
    "0xDB":"0xDB",
    "0xDC":"0xDC",
    "0xDD":"0xDD",
    "0xDE":"0xDE",
    "0xDF":"0xDF",
    "0xE2":"0xE2",
    "0xE5":"0xE5",
    "0xE7":"0xE7",
    "0xF6":"0xF6",
    "0xF7":"0xF7",
    "0xF8":"0xF8",
    "0xF9":"0xF9",
    "0xFA":"0xFA",
    "0xFB":"0xFB",
    "0xFD":"0xFD"
}



# Define the necessary structures
PUL = ctypes.POINTER(ctypes.c_ulong)

class KeyBdInput(ctypes.Structure):
    _fields_ = [("wVk", ctypes.c_ushort),
                ("wScan", ctypes.c_ushort),
                ("dwFlags", ctypes.c_ulong),
                ("time", ctypes.c_ulong),
                ("dwExtraInfo", PUL)]

class HardwareInput(ctypes.Structure):
    _fields_ = [("uMsg", ctypes.c_ulong),
                ("wParamL", ctypes.c_short),
                ("wParamH", ctypes.c_ushort)]

class MouseInput(ctypes.Structure):
    _fields_ = [("dx", ctypes.c_long),
                ("dy", ctypes.c_long),
                ("mouseData", ctypes.c_ulong),
                ("dwFlags", ctypes.c_ulong),
                ("time",ctypes.c_ulong),
                ("dwExtraInfo", PUL)]

class Input_I(ctypes.Union):
    _fields_ = [("ki", KeyBdInput),
                ("mi", MouseInput),
                ("hi", HardwareInput)]

class Input(ctypes.Structure):
    _fields_ = [("type", ctypes.c_ulong),
                ("ii", Input_I)]


# Define the necessary functions
def press_key(hexKeyCode):
    extra = ctypes.c_ulong(0)
    ii_ = Input_I()
    ii_.ki = KeyBdInput(hexKeyCode, 0x48, 0, 0, ctypes.pointer(extra))
    x = Input(ctypes.c_ulong(1), ii_)
    ctypes.windll.user32.SendInput(1, ctypes.pointer(x), ctypes.sizeof(x))

def release_key(hexKeyCode):
    extra = ctypes.c_ulong(0)
    ii_ = Input_I()
    ii_.ki = KeyBdInput(hexKeyCode, 0x48, 0x0002, 0, ctypes.pointer(extra))
    x = Input(ctypes.c_ulong(1), ii_)
    ctypes.windll.user32.SendInput(1, ctypes.pointer(x), ctypes.sizeof(x))























def def_try_to_execute(key_name):

    

    if key_name is None or len(key_name)<=0:
        return
    key_name= key_name.lower();
    text_array = key_name.strip().split(' ')
    if text_array is None or len(text_array)<=0:
        return
    
    is_true = text_array[0]
    if len(text_array)<2:
        return
    command = text_array[1]


    if len(text_array)== 2:
        if use_print_log:
            print(f"_{is_true}_{command}_")
        if command in keyboard_mappings:

            if use_print_log:
                print(f"_{is_true}_{command}_InDico_{keyboard_mappings[command]}")
            if is_true== "1":
                send_key_press(hwnd, keyboard_mappings[command])
            else:
                send_key_release(hwnd, keyboard_mappings[command])
    else:
        print(f"_{is_true}_{command}_")
    
        

    

timebetweenaction=0.1
timepress=0.1




def enum_child_windows(parent_hwnd):
    child_windows = []

    def enum_child_proc(hwnd, lParam):
        nonlocal child_windows
        child_windows.append(hwnd)
        return True  # Continue enumeration

    # Convert the callback function to a C function pointer
    EnumChildProc = ctypes.WINFUNCTYPE(ctypes.c_bool, ctypes.POINTER(ctypes.c_int), ctypes.POINTER(ctypes.c_int))

    # Call EnumChildWindows with the parent window handle and the callback function
    ctypes.windll.user32.EnumChildWindows(parent_hwnd, EnumChildProc(enum_child_proc), 0)

    return child_windows

def is_window_focused(hwnd):
    return user32.GetForegroundWindow() == hwnd

def send_key(hwnd, key_code):
    if use_print_log:
        print(f"Click:{key_code}")



    if use_real_input and is_window_focused(hwnd):
        press_key(key_code)  # Press DOWN key
        time.sleep(1)  # Wait for 1 second
        release_key(key_code)  # Release DOWN key
    else:
        child_windows = enum_child_windows(hwnd)
        for child_hwnd in child_windows:
            send_key_press(child_hwnd, key_code)
            time.sleep(0.1)  # Optional delay between keydown and keyup
            send_key_release(child_hwnd, key_code)

def send_key_press(hwnd, key_code):
    if use_print_log:
        print(f"Press:{key_code}")
        
    if use_real_input and is_window_focused(hwnd):
         press_key(key_code) 
    else:
        ctypes.windll.user32.SendMessageW(hwnd, WM_KEYDOWN, key_code, 0)
        child_windows = enum_child_windows(hwnd)
        for child_hwnd in child_windows:
            ctypes.windll.user32.SendMessageW(child_hwnd, WM_KEYDOWN, key_code, 0)

def send_key_release(hwnd, key_code):
    if use_print_log:
        print(f"Release:{key_code}")
    if use_real_input and is_window_focused(hwnd):
         release_key(key_code) 
    else:
        ctypes.windll.user32.SendMessageW(hwnd, WM_KEYUP, key_code, 0)
        child_windows = enum_child_windows(hwnd)
        for child_hwnd in child_windows:
            ctypes.windll.user32.SendMessageW(child_hwnd, WM_KEYUP, key_code, 0)



def check_and_copy(message):
    if message.startswith("c "):
        content = message[2:]  # Extract the content after "c "
        pyperclip.copy(content)
        if use_print_log:
            print("Content copied to clipboard:", content)
        return True
    else:
        return False


def main():

    for key in list(keyboard_mappings.keys()):
        keyboard_mappings[key.lower().replace(" ", "")] = keyboard_mappings.pop(key)

    # Define the UDP IP address and port to listen on
    UDP_IP = "0.0.0.0" 
    

    # Create a UDP socket
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    # Bind the socket to the port
    sock.bind((UDP_IP, lisent_udp_port_to_interact))


    print("UDP server listening on port", lisent_udp_port_to_interact)

    try:
        while True:
            data, addr = sock.recvfrom(1024)  
            message = data.decode("utf-8")

            if use_print_log:
                print("Received message from {}: {}".format(addr, message))
            
            if(not check_and_copy(message)):
                def_try_to_execute(message)
                
    except KeyboardInterrupt:
        print("Server stopped.")

if __name__ == "__main__":
    # Replace "Your Window Title" with the title of the window you want to send keys to

  
    while not hwnd:
        print(f"Window with title '{window_title}' not found.")
        window_title = input("Put app name here:")  # Python 3
        hwnd = find_window(window_title)
        if hwnd is None:
            hwnd = find_in_all(window_title)


    print(f"Window with title '{window_title}' found.")
    main()



