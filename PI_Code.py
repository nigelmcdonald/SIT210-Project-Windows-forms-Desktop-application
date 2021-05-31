import smbus
import time
from random import randint
import codecs # for decoding hex
import RPi.GPIO as GPIO

#variables
LED_PIN = 17
PIR_PIN = 23
RELAY_PIN = 27
channel = 1
address = 0x08 #listen on adress 0x10
sensorOn = True; # turn sensor on or off, True = on
waterOn = True; # turn water on or off, True = on
spray = False; # short spray action
allOff = False; # shut down all systems

#setup
GPIO.setwarnings(False)
GPIO.setmode(GPIO.BCM) # BCM VS BOARD is pin mode type, BCM is GPIO PINS
GPIO.setup(PIR_PIN, GPIO.IN) #Define pins as an output pin
GPIO.setup(RELAY_PIN, GPIO.OUT) 
GPIO.setup(LED_PIN, GPIO.OUT) 
bus = smbus.SMBus(channel)
cooldown = 0# cool down for motion sprayer to fire againt

#Read a single byte of data
def ReadByte(addr):
    reading = bus.read_byte(addr)
    return reading

#send a single byte to slave
def SendByte(addr, value):
    bus.write_byte(addr, value)

# MAIN LOOP
try:
    cooldown = time.time()  # time counter
    print("System Activated, waiting for argon boot...")
    time.sleep(20) # wait for argon to boot
    SendByte(address, 55) # Send Boot Notification
    while True:
        try:# catch loss of connection to argon
            if (sensorOn and not allOff and time.time() - cooldown > 10): #motion on/off not spray possible if inside cooldown
                if GPIO.input(PIR_PIN): # check if PIR is active
                    cooldown = time.time() # reset timer
                    GPIO.output(LED_PIN, GPIO.HIGH) # activate LED
                    print("Movement detected!")
                    SendByte(address, 11) # send notification of motion and spray activation
                    if (waterOn and not allOff): # water on/off
                        GPIO.output(RELAY_PIN, GPIO.HIGH) # activate sprayer
                    time.sleep(0.2)
                    GPIO.output(LED_PIN, GPIO.LOW) # deactivate LED
            if (spray and not allOff and waterOn): #manual spray
                cooldown = time.time() # reset timer
                GPIO.output(RELAY_PIN, GPIO.HIGH)
                spray = False;
                print("Spraying..")
                time.sleep(0.2)
            data = ReadByte(address)
            time.sleep(0.1)#
            if data != 0: # if there is data then look for command values                    
                print("Recieved: " + str(data))
                if data == 99:
                    spray = True                    
                elif data == 100:
                    sensorOn = True
                    allOff = False
                    print("Sensing On!")
                elif data == 101:
                    sensorOn = False
                    print("Sensing Off!")
                elif data == 102:
                    waterOn = True
                    allOff = False
                    print("Water On!")
                elif data == 103:
                    waterOn = False
                    print("Water Off!")
                elif data == 104:
                    allOff = True
                    spray, waterOn, sensorOn = False
                    print("All Off!")
        except: # throw error message rather than crash when bus I/O error when connection is lost
            print("Bus I/O connection error!")
        time.sleep(1) # loop logic after 1 second
        GPIO.output(RELAY_PIN, GPIO.LOW) # deactivate sprayer        
except KeyboardInterrupt:
    bus.close()
    print("End")
    

