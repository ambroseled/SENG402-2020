"""
FYP: Killing wilding pines with helicopter sprayed herbicide
Droplet tracking
"""

# Import Libraries
import math as m
import numpy as np
from matplotlib import pyplot as plt
from mpl_toolkits.mplot3d import Axes3D


# Constants
rho_liquid = 1000           # kg/m^3 - constant spray density
rho_a = 1                   # kg/m^3 - constant air density
mew_a = 1.789e-5            # kg/m/s - constant air viscocity
dt=0.1                      # s - time interval
nt=100                      # unitless
g = np.array([-9.81,0,0])   # m/s^2 - gravity acceleration

# Initialising 3D space
nx=41
ny=41
nz=41

zmax=10
ymax=10
xmax=10

z=np.linspace(0,zmax,nz)
y=np.linspace(0,ymax,ny)
x=np.linspace(0,xmax,nx)

Z,Y,X=np.meshgrid(z,y,x)    


# Not currently used - in future this will be downwash model
airspeedz=np.ones((nz,ny,nx))*-10
airspeedy=np.zeros((nz,ny,nx))
airspeedx=np.zeros((nz,ny,nx))

# Input parameters
inp = {
    'V_init': np.array([-10,1,2]),              # m/s - droplet initial speed
    'L_init': np.array([zmax, ymax/2, xmax/2]), # m - nozzle location
    'D_init': 100e-6,                           # m - droplet diameter [100 microns]
}

# Functions
def calc_reynolds(V_slip, D_drop):
    """Calculate Droplet Reynold's Number in 3D"""
    
    Re = (rho_a*D_drop*V_slip)/mew_a
    return Re
    
    
def calc_drag(Re):
    """Calculate Droplet Drag Coefficient in 3D"""
    # Langmuir and Blodgett 1964
    Cd = (24/Re)*(1+(0.197*Re**0.63)+(2.6e-4*Re**1.38))
    
    # Clift, Grace and Weber 1978
    # Cd = (24/Re)*(1+(0.15*Re**0.687))+0.42/(1+4.25e-4*(Re**-1.16)
    return Cd
    
    
def calc_evaporation_model(Dia, Re, bulbTemp, time):
    # Function that returns the evaporation model of a droplet
    # Dia is the diameter of the droplet in metres
    # Re is the Reynolds number of the droplet
    # bulbTemp is the wet bulb temperature depression in degrees Celsius

    Te = (Dia ** 2) / (84.76 * bulbTemp * (1 + (0.27 * (Re ** 0.5))))
    evapModel = -3 / ((2 * Te) * (1 - (t / Te)))
    return evapModel


def droplet_tracking(inp):
    """
    Single droplet tracker (takes in info on nozzle position, 
    droplet size and initial velocity, drag coeff and evaporation 
    and tracks droplet trajectory)
    
    Should sample droplet size & velocity from distribution
    Compute Re and Cd then advance droplet along trajectory
    Compute evaporation then update mass
    
    Advance time until deposition or escape
    
    Grab new sample until enough spray has been sampled
    """
    
    V_drop=inp['V_init']
    L_drop=inp['L_init']
    
    D_drop = inp['D_init']
 
    for i in range (1, nt):
        V_drop = np.vstack ((V_drop, np.array([0, 0, 0]))) 
        L_drop = np.vstack ((L_drop, np.array([0, 0, 0]))) 
        
        # Reynolds Calculation in 3D coords
        
        V_slip = abs(V_drop[i-1,:]-np.array([-15,0,0]))
        Re = calc_reynolds(V_slip, D_drop)
        
        # Drag Calculation in 3D coords
        Cd = calc_drag(Re)          
     
        # Calculation of drag force (droplet relaxation time?)
        # Evaporation calculation?
        # Update droplet diameter after evap   
        
        V_drop[i,:]=V_drop[i-1,:]+g*dt     
        L_drop[i,:]=L_drop[i-1,:]+V_drop[i-1,:]*dt   
        
        # Testing if droplet has been deposited (ONLY DEPOSITION NOT ESCAPE)
        if L_drop[i,0] <= 0:                 
            V_deposition = (1/6) * (np.pi * D_drop ** 3) * 1e6 # Volume depositied (ml)
            print(f"Deposited at %s: " % L_drop[i,:])
            print(f"Volume: %f ml" % V_deposition)
            break
        
    
    
    # Plotting
    
    fig = plt.figure()
    ax=plt.axes(projection='3d')
    #ax = fig.add_subplot(111, projection='3d')
    #Axes3D.plot(L_drop[:,2], L_drop[:,1], L_drop[:,0])
    ax.plot3D(L_drop[:,2], L_drop[:,1], L_drop[:,0])
    ax.set_xlim([0,xmax])
    ax.set_ylim([0,ymax])
    ax.set_zlim([0,zmax])
    ax.set_xlabel('X')
    ax.set_ylabel('Y');
    ax.set_zlabel('Z');
    ax.set_title('Trajectory of droplet')
    plt.show(block=False)
    
    
    
def main():
    droplet_tracking(inp)
    
main()