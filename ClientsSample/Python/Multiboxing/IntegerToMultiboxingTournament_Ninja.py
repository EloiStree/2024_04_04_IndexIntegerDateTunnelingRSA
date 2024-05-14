import ctypes
import time
import socket
import pyperclip
import psutil
import pygetwindow as gw
import asyncio
import threading




debug_at_pression_send=True

player_index_to_window_index ={}

## use to broadcast on target window from source index id
player_index_to_window_index [2]= [0]
player_index_to_window_index [3]= [1]
player_index_to_window_index [4]= [2]
player_index_to_window_index [5]= [3]
player_index_to_window_index [6]= [0,1,2,3]
player_index_to_window_index [0]= [0,1,2,3,4,5,6,7,8,9,10,11]


integer_to_key_mapping = {}
integer_to_key_mapping[103] = ["Right",True]
integer_to_key_mapping[203] = ["Right",False]
integer_to_key_mapping[104] = ["Left",True]
integer_to_key_mapping[204] = ["Left",False]
integer_to_key_mapping[101] = ["Up",True]
integer_to_key_mapping[201] = ["Up",False]
integer_to_key_mapping[102] = ["Down",True]
integer_to_key_mapping[202] = ["Down",False]

#integer_to_key_mapping[116] = ["A" ,True]
#integer_to_key_mapping[216] = ["A" ,False]
#integer_to_key_mapping[112] = ["Z",True]
#integer_to_key_mapping[212] = ["Z",False]
#integer_to_key_mapping[113] = ["E",True]
#integer_to_key_mapping[213] = ["E",False]
#integer_to_key_mapping[114] = ["R",True]
#integer_to_key_mapping[214] = ["R",False]


integer_to_key_mapping[105] = ["F1" ,True]
integer_to_key_mapping[205] = ["F1" ,False]
integer_to_key_mapping[130] = ["F2",True]
integer_to_key_mapping[230] = ["F2",False]
integer_to_key_mapping[109] = ["F3",True]
integer_to_key_mapping[209] = ["F3",False]
integer_to_key_mapping[122] = ["F4",True]
integer_to_key_mapping[222] = ["F4",False]
integer_to_key_mapping[124] = ["Escape",True]
integer_to_key_mapping[224] = ["Escape",False]






user32 = ctypes.windll.user32

## ## ## ## ## ## ## ## 
##  PUBLIC
## ## ## ## ## ## ## ## 

## Port that app is listening to be used
lisent_udp_port_to_interact = 5648

## Do you want to us real input or send message that are send only to the app
use_real_input=False

## What window index should we use ?
target_window_index = 0


## What is the exact name to find in of the window we need to find.
#window_title = "bgb - SUPER MARIOLAND"
#window_title = "World of Warcraft"

window_title = "10 Second Ninja"
## window_title = "MORDHAU  "
## window_title = "Hollow Knight"
## window_title = "Chrome"


# Use real will simulate key, use false will send fake key

# Constants for SendMessage
WM_KEYDOWN = 0x0100
WM_KEYUP = 0x0101



# Find the window by its title
def find_window(title):
    return ctypes.windll.user32.FindWindowW(None, title)


def get_all_windows(title):
    list_window_found = [window for window in gw.getAllWindows() if title in window.title]
    return list_window_found



def find_in_all(title):
    global target_window_index
    list_window_found = [window for window in gw.getAllWindows() if title in window.title]
    if list_window_found:
        return list_window_found[0]
    else:
        return None
    
    
def find_in_all(title, index):
    global target_window_index
    list_window_found = [window for window in gw.getAllWindows() if title in window.title]
    if list_window_found and len(list_window_found) > index:
        return list_window_found[index]
    else:
        return None
def find_in_all_count(title):
    global target_window_index
    list_window_found = [window for window in gw.getAllWindows() if title in window.title]
    if list_window_found :
        return len(list_window_found)
    else:
        return 0





all_found_windows_at_start = get_all_windows(window_title)



first_window_foundhwnd = find_window(window_title)
found_window_count= find_in_all_count(window_title)
use_print_log=False

print (f"Window found:{first_window_foundhwnd} Count:{found_window_count}")

for windowt in all_found_windows_at_start:
    print("Window Title:", windowt.title)
    print("Window ID:", windowt._hWnd)


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

# def send_key(hwnd, key_code):
    
#         child_windows = enum_child_windows(hwnd)
#         # for child_hwnd in child_windows:
#         #     send_key_press(child_hwnd, key_code)
#         #     time.sleep(0.1)  # Optional delay between keydown and keyup
#         #     send_key_release(child_hwnd, key_code)

def send_key_press(hwnd, key_code):
   
        #print("A")
        ctypes.windll.user32.PostMessageW(hwnd, WM_KEYDOWN, key_code, 0)

        #print("B")
        # child_windows = enum_child_windows(hwnd)
        # for child_hwnd in child_windows:
        #     ctypes.windll.user32.SendMessageW(child_hwnd, WM_KEYDOWN, key_code, 0)

def send_key_release(hwnd, key_code):
   
        ctypes.windll.user32.PostMessageW(hwnd, WM_KEYUP, key_code, 0)
        # child_windows = enum_child_windows(hwnd)
        # for child_hwnd in child_windows:
        #     ctypes.windll.user32.SendMessageW(child_hwnd, WM_KEYUP, key_code, 0)



def check_and_copy(message):
    if message.startswith("c "):
        content = message[2:]  # Extract the content after "c "
        pyperclip.copy(content)
        if use_print_log:
            print("Content copied to clipboard:", content)
        return True
    else:
        return False


for key in integer_to_key_mapping:
    integer_to_key_mapping[key][0] =integer_to_key_mapping[key][0].strip().lower()

    

def push_to_all_integer(int_value):
    print("Un coded yet")

def push_test(window, press, key_id):
    global debug_at_pression_send
    #print(f"Push {press} {key_id} to {window.title}")
    if window:
        if press==True:
            if debug_at_pression_send:
                print(f"Press {key_id} to {window.title}")
            send_key_press(window._hWnd, key_id)
        else:
            if debug_at_pression_send:
                print(f"Release {key_id} to {window.title}")
            send_key_release(window._hWnd, key_id)
            


def push_to_index_integer(int_index, int_value):
    global keyboard_mappings
    #print("start")
    #print(f"R | Index {int_index}| Value {int_value}")
    key_name_last_found=""
    press_last_found=False
    one_found=False
    ## Is player index existing in register
    if( int_index in player_index_to_window_index):
        ## Get the list of window index for this player to broadcast
        window_index_list = player_index_to_window_index[int_index]
        ## For each window index to broadcast
        for window_index in window_index_list:
            ## If the window index in range of existing one at start
            if window_index < len(all_found_windows_at_start):
                    ## If the value is existing in the mapping allows to player
                    if(int_value in integer_to_key_mapping):
                        ## Get the action to do and the pression or release wanted.
                        store_pression_info= integer_to_key_mapping[int_value]
                        key_name =store_pression_info[0]
                        press= store_pression_info[1]
                        ## If the key name is existing in the range of possible input
                        
                        if key_name in keyboard_mappings:
                            key_id = keyboard_mappings[key_name]
                            #print(f"Id {key_id} Found {one_found}")
                             ## Get the window pointer to broadcast
                            window = all_found_windows_at_start[window_index]
                            ## If the window is existing
                            push_test(window, press, key_id)

        #if(one_found):
        #    print(f"Index {int_index} | Value {int_value} | Key {key_name_last_found} | Press {press_last_found}")
  
   # print("Stop")






async def async_task():
        
        print("Async task started")
        await asyncio.sleep(2)
        print("Async task ready")
        
        # Launch the async task

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
                #print("received message:", data)  
                #print(f"R| {len(data)} | {data}")
                if(len(data)==4):
                    int_value = int.from_bytes(data, byteorder='little')

                    push_to_all_integer(int_value)
                elif  len(data)==12:

                    int_value= int.from_bytes(data[0:4], byteorder='little')
                    long_data_2= int.from_bytes(data[4:12], byteorder='little')
                    push_to_all_integer(int_value)

                elif  len(data)==16:

                    int_index= int.from_bytes(data[0:4], byteorder='little')
                    int_value= int.from_bytes(data[4:8], byteorder='little')
                    long_data_2= int.from_bytes(data[8:16], byteorder='little')
                    print("Index",int_index,"Value",int_value)
                    push_to_index_integer(int_index, int_value)
                    # thread = threading.Thread(target=push_to_index_integer, args=(int_index, int_value))
                    # thread.start()



           
                
                    
        except KeyboardInterrupt:
            print("Server stopped.")
        


    

if __name__ == "__main__":
   
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
    "Escape": 0x1B,
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
    '0': 0x30,
    '1': 0x31,
    '2': 0x32,
    '3': 0x33,
    '4': 0x34,
    '5': 0x35,
    '6': 0x36,
    '7': 0x37,
    '8': 0x38,
    '9': 0x39,
    'A': 0x41,
    'B': 0x42,
    'C': 0x43,
    'D': 0x44,
    'E': 0x45,
    'F': 0x46,
    'G': 0x47,
    'H': 0x48,
    'I': 0x49,
    'J': 0x4A,
    'K': 0x4B,
    'L': 0x4C,
    'M': 0x4D,
    'N': 0x4E,
    'O': 0x4F,
    'P': 0x50,
    'Q': 0x51,
    'R': 0x52,
    'S': 0x53,
    'T': 0x54,
    'U': 0x55,
    'V': 0x56,
    'W': 0x57,
    'X': 0x58,
    'Y': 0x59,
    'Z': 0x5A,
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

    asyncio.run(async_task())