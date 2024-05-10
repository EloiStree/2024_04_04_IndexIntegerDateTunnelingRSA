import time
import rtmidi
import random

midiout = rtmidi.MidiOut()
available_ports = midiout.get_ports()


print("Available ports:", available_ports)

port_name = "Microsoft GS Wavetable Synth 0"
port_name = "MPK mini play 2"
port_index = available_ports.index(port_name)
midiout.open_port(port_index)

exit_condition=False
with midiout:

    while True:

        for i in range(0, 127):
            random_note_127 = i
            note_on = [0x90, random_note_127, 65] # channel 1, middle C, velocity 112
            note_off = [0x80, random_note_127, 0]
            midiout.send_message(note_on)
            time.sleep(0.5)
            midiout.send_message(note_off)
            
        for i in range(0, 127):
            random_note_127 = random.randint(0, 127)
            random_velocity_127 = random.randint(0, 127)
            random_duration_seconds = random.random()*0.5
            note_on = [0x90, random_note_127, random_note_127] # channel 1, middle C, velocity 112
            note_off = [0x80, random_note_127, 0]
            midiout.send_message(note_on)
            time.sleep(random_duration_seconds)
            midiout.send_message(note_off)
del midiout