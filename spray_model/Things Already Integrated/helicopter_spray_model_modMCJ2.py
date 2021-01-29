"""
FYP: Killing wilding pines with helicopter spraid pesticide
Helicopter spray model
"""

# Import Libraries
import math as m
import numpy as np
import matplotlib.pyplot as plt

# Constants
R = 5 # m - raidus of helicopter propeller
rho_a = 1 # kg/m^3 - constant air density
k = 7 # constant relating helicopter roll-up to fixed-wing roll-up
c = 0.25 # m - blade chord length
r_m = 5 # m - mean radius of rotor
n_b = 2 # number of blades

# Inputs
inp = {
    'U_inf': 10, # m/s - helicopter speed
    'Omega': 15, # rad/s - rotor rotational speed
    'H': 300, # m - heigh of helicopter
    'W': 600, # kg - weight of helicopter and everything it is carrying
    't': 1 # s - time
}

def calculate_values(inp):
    """ Calculate downwash values from constants and inputs """
    # Initialize
    vals = {} 

    # Helicopter forward advance
    vals['mew'] = inp['U_inf']/(inp['Omega']*R)

    # Helicopter Blade Solidity
    vals['sigma_s'] = c*n_b/(2*m.pi*r_m)

    # Circulation
    vals['Gamma0'] = inp['W']/(2*rho_a*R*inp['U_inf'])

    # Aerodynamic drag?
    vals['F'] = m.exp(-1*k*vals['mew']*vals['Gamma0']*inp['t']/
        (vals['sigma_s']*m.pi*(R**2)))
    
    # Downwash
    vals['w'] = m.sqrt(vals['F']*inp['W']/(2*m.pi*rho_a*(R**2)))

    return vals

def get_coordinates(n, inp):
    """ Make grid of size nxn for velocity field """
    r, z = np.meshgrid(np.linspace(0, R, n), np.linspace(0, inp['H'], n))
    return [r, z]

def get_downwash_speed_matrixes(inp, vals, r, z):
    """ Calculate velocities in grid """
    const = 0.1 # NOT SURE WHAT THIS IS SUPPOSED TO BE
    Vwr = [const*x for x in r]
    Vwz = [-vals['w']*x/inp['H'] for x in z]
    return [Vwr, Vwz]

##MCJ addition/
def get_dividing_streamline(n, inp, Vwr, Vwz, r, z):
    dividing_streamline_r=np.zeros(n) #MCJ this will contain coords of the dividing streamline, z coords=dividing_streamline[1,:] and r coords=dividing_streamline[2,:]
    dividing_streamline_z=np.zeros(n) #MCJ this will contain coords of the dividing streamline, z coords=dividing_streamline[1,:] and r coords=dividing_streamline[2,:]
    dividing_streamline_z=z[:,1] # [R/0.1*x for x in r] #MCJ
    dividing_streamline_r[n-1]=R
    #print(Vwr)
    for i in range (n-1, 0, -1):
        j=int(dividing_streamline_r[i]*n/R)
        local_Vwr_horiz_slice=Vwr[i]
        local_Vwr=local_Vwr_horiz_slice[j-1]
        local_Vwz_horiz_slice=Vwz[i]
        local_Vwz=local_Vwz_horiz_slice[j-1]
        print(local_Vwr, local_Vwz)
        dividing_streamline_r[i-1]=dividing_streamline_r[i]-0.01*local_Vwr/local_Vwz #MCJ
    return [dividing_streamline_r, dividing_streamline_z]
##/MCJ addition

def main():
    """ Main function """
    n = 10
    vals = calculate_values(inp)
    r, z = get_coordinates(n, inp)
    Vwr, Vwz = get_downwash_speed_matrixes(inp, vals, r, z)

    # Plot Vector Field
    plt.quiver(r, z, Vwr, Vwz)
    plt.xlabel('r (m)')
    plt.ylabel('z (m)')
    plt.title('Downwash Velocity Field')
##MCJ deletion/
#    plt.show()
##/MCJ deletion

##MCJ addition/
    dividing_streamline_r, dividing_streamline_z=get_dividing_streamline(n, inp, Vwr, Vwz, r, z)
    plt.plot(dividing_streamline_r,dividing_streamline_z)
    plt.show(block=False)
##/MCJ addition

main()
