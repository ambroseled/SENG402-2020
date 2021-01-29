"""
FYP: Trailer test. outputs nozzle spray condition
"""

# Import Libraries
import math as m
import numpy as np

# Constants
Lb = 2                   # m - length of boom
b = 0,-2                 # m - location of boom centre letive to GPS
C = 111320               # m - multiplication factor (1 degree = 111320 m)
R = 2                    # m - radius of tree swath

# tree coordinates from google maps in global coordinates in degrees(Y, X) = (lat, long) (WGS 84)
T = ((-43.525243, 172.586752),(-43.525024, 172.586327),(-43.525552, 172.586497))
T = np.transpose(T)

# Inputs
inp = {
    'longitude': 172.586752, # degrees (WGS40 = WGS 84?)
    'latitude': -43.525243,  # degrees (WGS40 = WGS 84?)
    'yaw': -73,              # degrees (bearing)
}

def test_model(inp, Lb, b, C, R, T):
    """
    Determines whether a nozzle should spray based on distance from a tree centre

    Inputs: length of boom, Lb
            location of boom centre letive to GPS, b
            multiplication factor (1 degree = 111320 m), C
            radius of tree swath, R

    Outputs: Spray conditions for each nozzle
    """
    # tree location in global coordinates in meters
    t = T*C

    GPS = inp['longitude']*C,inp['latitude']*C
    yaw = inp['yaw']

    # location of the center of the boom in global coordinates
    yaw = yaw/180*m.pi
    bcx = GPS[0] + (b[0]*np.cos(yaw) + b[1]*np.sin(yaw))
    bcy = GPS[1] + (b[1]*np.cos(yaw) - b[0]*np.sin(yaw))

    # horizontal location of individual nozzles reletive to center of boom
    n1 = -Lb/2
    n2 = -Lb/4
    n3 = 0
    n4 = Lb/4
    n5 = Lb/2

    # location of individual nozzles in global coordinates
    n1x = bcx + n1*np.cos(yaw)
    n1y = bcy - n1*np.sin(yaw)

    n2x = bcx + n2*np.cos(yaw)
    n2y = bcy - n2*np.sin(yaw)

    n3x = bcx
    n3y = bcy

    n4x = bcx + n4*np.cos(yaw)
    n4y = bcy - n4*np.sin(yaw)

    n5x = bcx + n5*np.cos(yaw)
    n5y = bcy - n5*np.sin(yaw)

    # distance between each tree and each individual nozzles
    r1 = np.sqrt((t[0]-n1y)**2+(t[1]-n1x)**2)
    r2 = np.sqrt((t[0]-n2y)**2+(t[1]-n2x)**2)
    r3 = np.sqrt((t[0]-n3y)**2+(t[1]-n3x)**2)
    r4 = np.sqrt((t[0]-n4y)**2+(t[1]-n4x)**2)
    r5 = np.sqrt((t[0]-n5y)**2+(t[1]-n5x)**2)

    # individual nozzle spray condition with respect to nozzle distance to closest tree
    if min(r1)<R:
        Nozzle_1 = 1
    else:
        Nozzle_1 = 0

    if min(r2)<R:
        Nozzle_2 = 1
    else:
        Nozzle_2 = 0

    if min(r3)<R:
        Nozzle_3 = 1
    else:
        Nozzle_3 = 0

    if min(r4)<R:
        Nozzle_4 = 1
    else:
        Nozzle_4 = 0

    if min(r5)<R:
        Nozzle_5 = 1
    else:
        Nozzle_5 = 0

    return Nozzle_1, Nozzle_2, Nozzle_3, Nozzle_4, Nozzle_5

Nozzle_1, Nozzle_2, Nozzle_3, Nozzle_4, Nozzle_5 = test_model(inp, Lb, b, C, R, T)
print(Nozzle_1, Nozzle_2, Nozzle_3, Nozzle_4, Nozzle_5)