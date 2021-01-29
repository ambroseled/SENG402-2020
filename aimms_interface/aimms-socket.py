import socket

def SendToNuc(stringMsg, socket, address):
    bytesToSend = str.encode(stringMsg)
    socket.sendto(bytesToSend, address)


if __name__ == '__main__':
    msgFromClient = "My Name a jeff"
    serverAddressPort = ("localhost", 30456)

    UDPClientSocket = socket.socket(family=socket.AF_INET, type=socket.SOCK_DGRAM)

    while True:
        SendToNuc(msgFromClient, UDPClientSocket, serverAddressPort)

