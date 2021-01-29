import socket
import serial
import time
import struct


baud1 = 19200 # COM1 Baud
baud2 = 115200 # COM2 Baud

# Change the port relative to whatever the port name is on device code is run on
serialPort = serial.Serial(port = "/dev/serial/by-path/pci-0000:00:14.0-usb-0:3:1.0-port0", baudrate=baud2,  # TODO Need to check this port name
                           bytesize=8, timeout=1, stopbits=serial.STOPBITS_ONE,
                           parity=serial.PARITY_NONE)


def SendToNuc(stringMsg, socket, address):
    socket.sendto(str.encode(stringMsg), address)


def ParseInput(serialString):
    format_string = '< 7b h 2H 3h H b h 7b 2f 12h h'
    unpacked = struct.unpack(format_string, serialString)

    # GPS Coordinates (WGS84)
    latitude = unpacked[23]  # N
    longitude = unpacked[24]  # E
    yaw = unpacked[31] / 50  # Heading

    return latitude, longitude, yaw


def ReadAndSendData(socket, address):
    while True:
        bytes_available = serialPort.in_waiting

        # Wait until there is data waiting in the serial buffer
        if (bytes_available > 0):
            # Reading data out of the buffer
            serialString = serialPort.read(65)
            lat, lng, yaw = ParseInput(serialString)

            serialPort.reset_input_buffer()
            print("Latitude: ", lat)
            print("Longitude: ", lng)
            print("Yaw: ", yaw)
            SendToNuc("{0}:{1}:{2}".format(lat, lng, yaw), UDPClientSocket, serverAddressPort)


if __name__ == '__main__':
    serverAddressPort = ("localhost", 30456)
    UDPClientSocket = socket.socket(family=socket.AF_INET, type=socket.SOCK_DGRAM)
    ReadAndSendData(socket, UDPClientSocket)
    serialPort.close()
