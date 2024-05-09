import pyxinput
import time

#pyxinput.test_virtual()


 # AxisLx          , Left Stick X-Axis
 # AxisLy          , Left Stick Y-Axis
 # AxisRx          , Right Stick X-Axis
 # AxisRy          , Right Stick Y-Axis
 # BtnBack         , Menu/Back Button
 # BtnStart        , Start Button
 # BtnA            , A Button
 # BtnB            , B Button
 # BtnX            , X Button
 # BtnY            , Y Button
 # BtnThumbL       , Left Thumbstick Click
 # BtnThumbR       , Right Thumbstick Click
 # BtnShoulderL    , Left Shoulder Button
 # BtnShoulderR    , Right Shoulder Button
 # Dpad            , Set Dpad Value (0 = Off, Use DPAD_### Constants) 1 up 2 down 3 left 4 right
 # TriggerL        , Left Trigger
 # TriggerR        , Right Trigger


int_offset=450000
int_player_offset=100

max_input=3

pv={}
pr={}


def set_value_for_all(value_key,value):
    for key in pv:
        pv[key].set_value(value_key, value)


for i in range(1,max_input+1):
    pv[str(i)] = pyxinput.vController()
    pr[str(i)] = pyxinput.rController(i)



    print_dico = {
        1: lambda x: print(x),
        2: lambda x: print(x),
        3: lambda x: print(x),
    }
    


button_dico_all_down = {
    #ABXY
    1001: lambda: set_value_for_all('BtnA', 1),
    2001: lambda: set_value_for_all('BtnA', 0),
    1002: lambda: set_value_for_all('BtnB', 1),
    2002: lambda: set_value_for_all('BtnB', 0),
    1003: lambda: set_value_for_all('BtnX', 1),
    2003: lambda: set_value_for_all('BtnX', 0),
    1004: lambda: set_value_for_all('BtnY', 1),
    2004: lambda: set_value_for_all('BtnY', 0),
    #DPAD
    1005: lambda: set_value_for_all('Dpad', 1),# 1 up 2 down 3 left 4 right
    2005: lambda: set_value_for_all('Dpad', 0),
    1006: lambda: set_value_for_all('Dpad', 2),# 1 up 2 down 3 left 4 right
    2006: lambda: set_value_for_all('Dpad', 0),
    1007: lambda: set_value_for_all('Dpad', 3),# 1 up 2 down 3 left 4 right
    2007: lambda: set_value_for_all('Dpad', 0),
    1008: lambda: set_value_for_all('Dpad', 4),# 1 up 2 down 3 left 4 right
    2008: lambda: set_value_for_all('Dpad', 0),
    
    #THUMB 
    1009: lambda: set_value_for_all('BtnThumbL', 1),
    2009: lambda: set_value_for_all('BtnThumbL', 0),
    1010: lambda: set_value_for_all('BtnThumbR', 1),
    2010: lambda: set_value_for_all('BtnThumbR', 0),

    #SHOULDER    
    1011: lambda: set_value_for_all('BtnShoulderL', 1),
    2011: lambda: set_value_for_all('BtnShoulderL', 0),
    1012: lambda: set_value_for_all('BtnShoulderR', 1),
    2012: lambda: set_value_for_all('BtnShoulderR', 0),
    
    #START BACK
    1013: lambda: set_value_for_all('BtnStart', 1),
    2013: lambda: set_value_for_all('BtnStart', 0),
    1014: lambda: set_value_for_all('BtnBack', 1),
    2014: lambda: set_value_for_all('BtnBack', 0),
    
    #TRIGGER
    2020: lambda: set_value_for_all('TriggerL', 0),
    2021: lambda: set_value_for_all('TriggerL', 0.1),
    2022: lambda: set_value_for_all('TriggerL', 0.2),
    2023: lambda: set_value_for_all('TriggerL', 0.3),
    2024: lambda: set_value_for_all('TriggerL', 0.4),
    2025: lambda: set_value_for_all('TriggerL', 0.5),
    2026: lambda: set_value_for_all('TriggerL', 0.6),
    2027: lambda: set_value_for_all('TriggerL', 0.7),
    2028: lambda: set_value_for_all('TriggerL', 0.8),
    2029: lambda: set_value_for_all('TriggerL', 0.9),
    1030: lambda: set_value_for_all('TriggerL', 1),
    
    #TRIGGER
    2040: lambda: set_value_for_all('TriggerR', 0),
    2041: lambda: set_value_for_all('TriggerR', 0.1),
    2042: lambda: set_value_for_all('TriggerR', 0.2),
    2043: lambda: set_value_for_all('TriggerR', 0.3),
    2044: lambda: set_value_for_all('TriggerR', 0.4),
    2045: lambda: set_value_for_all('TriggerR', 0.5),
    2046: lambda: set_value_for_all('TriggerR', 0.6),
    2047: lambda: set_value_for_all('TriggerR', 0.7),
    2048: lambda: set_value_for_all('TriggerR', 0.8),
    2049: lambda: set_value_for_all('TriggerR', 0.9),
    1050: lambda: set_value_for_all('TriggerR', 1),
    
    3100: lambda: set_value_for_all('AxisLx', -1),
    3101: lambda: set_value_for_all('AxisLx', -0.90),
    3102: lambda: set_value_for_all('AxisLx', -0.75),
    3103: lambda: set_value_for_all('AxisLx', -0.5),
    3104: lambda: set_value_for_all('AxisLx', -0.25),
    3105: lambda: set_value_for_all('AxisLx', 0),
    3106: lambda: set_value_for_all('AxisLx', 0.25),
    3107: lambda: set_value_for_all('AxisLx', 0.5),
    3108: lambda: set_value_for_all('AxisLx', 0.75),
    3109: lambda: set_value_for_all('AxisLx', 0.90),
    3110: lambda: set_value_for_all('AxisLx', 1),
    
    3200: lambda: set_value_for_all('AxisLy', -1),
    3201: lambda: set_value_for_all('AxisLy', -0.90),
    3202: lambda: set_value_for_all('AxisLy', -0.75),
    3203: lambda: set_value_for_all('AxisLy', -0.5),
    3204: lambda: set_value_for_all('AxisLy', -0.25),
    3205: lambda: set_value_for_all('AxisLy', 0),
    3206: lambda: set_value_for_all('AxisLy', 0.25),
    3207: lambda: set_value_for_all('AxisLy', 0.5),
    3208: lambda: set_value_for_all('AxisLy', 0.75),
    3209: lambda: set_value_for_all('AxisLy', 0.90),
    3210: lambda: set_value_for_all('AxisLy', 1),
    
    3300: lambda: set_value_for_all('AxisRx', -1),
    3301: lambda: set_value_for_all('AxisRx', -0.90),
    3302: lambda: set_value_for_all('AxisRx', -0.75),
    3303: lambda: set_value_for_all('AxisRx', -0.5),
    3304: lambda: set_value_for_all('AxisRx', -0.25),
    3305: lambda: set_value_for_all('AxisRx', 0),
    3306: lambda: set_value_for_all('AxisRx', 0.25),
    3307: lambda: set_value_for_all('AxisRx', 0.5),
    3308: lambda: set_value_for_all('AxisRx', 0.75),
    3309: lambda: set_value_for_all('AxisRx', 0.90),
    3310: lambda: set_value_for_all('AxisRx', 1),
    
    3400: lambda: set_value_for_all('AxisRy', -1),
    3401: lambda: set_value_for_all('AxisRy', -0.90),
    3402: lambda: set_value_for_all('AxisRy', -0.75),
    3403: lambda: set_value_for_all('AxisRy', -0.5),
    3404: lambda: set_value_for_all('AxisRy', -0.25),
    3405: lambda: set_value_for_all('AxisRy', 0),
    3406: lambda: set_value_for_all('AxisRy', 0.25),
    3407: lambda: set_value_for_all('AxisRy', 0.5),
    3408: lambda: set_value_for_all('AxisRy', 0.75),
    3409: lambda: set_value_for_all('AxisRy', 0.90),
    3410: lambda: set_value_for_all('AxisRy', 1),
    
    
    
    }



while True: 
        
    for key in pv:
        pv[key].set_value('BtnUp', 1)
        print(f"{key} BtnA: {1}")
        time.sleep(0.01)
    time.sleep(0.1)
    for key in pv:
        pv[key].set_value('BtnA', 0)
        time.sleep(0.01)

    time.sleep(1)
    for key in pv:
        pv[key].set_value('AxisLx', -1)
        time.sleep(0.01)

    time.sleep(1)
    for key in pv:
        pv[key].set_value('AxisLx', 1) 
        time.sleep(0.01)
        
        
    time.sleep(1)
    for key in pv:
        pv[key].set_value('AxisLy', -1)
        time.sleep(0.01)

    time.sleep(1)
    for key in pv:
        pv[key].set_value('AxisLy', 1) 
        time.sleep(0.01)
        
    time.sleep(1)
    
    for key in pv:
        pv[key].set_value('AxisLx', 0)
        time.sleep(0.01)

    time.sleep(1)
    for key in pv:
        pv[key].set_value('BtnA', 0)
        time.sleep(0.01)

    time.sleep(1)
        