import serial
import time
import struct

baud1 = 19200 # COM1 Baud
baud2 = 115200 # COM2 Baud

# Change the port relative to whatever the portname is on device code is run on
serialPort = serial.Serial(port = "/dev/tty.usbserial-FT8WZO71", baudrate=baud2,
                           bytesize=8, timeout=0.5, stopbits=serial.STOPBITS_ONE,
                           parity=serial.PARITY_NONE)


serialString = ""                           # Used to hold data coming over UART
count = 0

while(count<100): # 100s testing window ( remove / change to call once )
    bytes_available = serialPort.in_waiting
    
    # Wait until there is data waiting in the serial buffer
    if(bytes_available > 0): 
        
        # Reading data out of the buffer
        serialString = serialPort.read(65)
        # serialString = serialPort.readline() # Works for COM1
        # print(serialString)
        
        if count > 0:
            format_string = '< 7b h 2H 3h H b h 7b 2f 12h h'
            unpacked = struct.unpack(format_string, serialString)
            
            # UTC Timestamps
            time_hour = unpacked[4]
            time_min = unpacked[5]
            time_sec = unpacked[6]
            
            # Temperature (Celsius)
            temperature = unpacked[7] / 100
            
            # Relative Humidity (fraction from zero to one)
            rh = unpacked[8] / 1000
            
            # Barometric pressure (Pa)
            pressure = unpacked[9] * 2
            
            # Wind vectors, speed, direction (m/s)
            wind_n = unpacked[10] / 100
            wind_e = unpacked[11] / 100
            wind_speed = unpacked[12] / 100
            wind_dir = unpacked[13] / 100 # Deg from true
            
            # GPS Status
            gps_stat = unpacked[14] # 4 seems to represent good - manual indicates 3
            
            # GPS Coordinates (WGS84)
            latitude = unpacked[23]   # N
            longitude = unpacked[24]  # E
            
            # Altitude above Geoid (m)
            altitude = unpacked[25]
            
            # Velocities (m/s)
            vel_north = unpacked[26] / 100
            vel_east = unpacked[27] / 100
            vel_down = unpacked[28] / 100
            
            # Position info (deg)
            roll = unpacked[29] / 100   
            pitch = unpacked[30] / 100
            yaw = unpacked[31] / 50     # Heading
            
            # True airspeed 
            airspeed = unpacked[32] / 100
            
            # Vertical wind
            vert_wind = unpacked[33] / 100
            
            # Dictionary creation
            met_dict = {'time_hour': time_hour, 
                        'time_min': time_min, 
                        'time_sec': time_sec, 
                        'relative_humidity': rh,
                        'pressure': pressure,
                        'wind_north': wind_n,
                        'wind_east': wind_e,
                        'wind_speed': wind_speed,
                        'wind_direction': wind_dir,
                        'gps_status': gps_stat,
                        'latitude': latitude,
                        'longitude': longitude,
                        'altitude': altitude,
                        'velocity_north': vel_north,
                        'velocity_east': vel_east,
                        'velocity_down': vel_down,
                        'roll': roll,
                        'pitch': pitch,
                        'yaw': yaw,
                        'airspeed': airspeed,
                        'vertical_wind': vert_wind} 
            
            #print(met_dict)
            
            # Displaying data (testing of module only)
            
            if time_hour < 10:
                time_hour = '0'+str(time_hour)            
            
            if time_sec < 10:
                time_sec = '0'+str(time_sec)
            
            if time_min < 10:
                time_min = '0'+str(time_min)     
            
            print('Time (UTC): {}:{}:{}'.format(time_hour, time_min, time_sec))
            print('Latitude: {:.5f} N\nLongitude: {:.5f} E\n'.format(latitude, longitude))
        
        # Print the contents of the serial data
        # print(serialString.decode('Ascii'))   # ASCII DECODING FOR COM1

        serialPort.reset_input_buffer()  # Clearing buffer
        count += 1
        
serialPort.close()

    
    

