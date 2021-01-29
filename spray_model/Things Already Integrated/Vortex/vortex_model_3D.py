"""
FYP: Killing wilding pines with helicopter sprayed pesticide
Vortex model
"""

# Import Libraries
import math as m
import numpy as np
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D

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
  'H': 10, # m - heigh of helicopter
  'W': 600, # kg - weight of helicopter and everything it is carrying
  't': 1 # s - time
}

def calculate_w_values(inp):
  """ Calculate downwash values from constants and inputs """
  # Initialize
  vals = {} 

  # Helicopter forward advance
  vals['mew'] = inp['U_inf']/(inp['Omega']*R)

  # Helicopter Blade Solidity
  vals['sigma_s'] = c*n_b/(2*m.pi*r_m)

  # Circulation
  vals['Gamma0'] = inp['W']/(2*rho_a*R*inp['U_inf'])

  # Decay Model
  vals['F'] = m.exp(-1*k*vals['mew']*vals['Gamma0']*inp['t']/
  (vals['sigma_s']*m.pi*(R**2)))
  """May not be correct decay model?? Or need to work out 
  time properly??"""
  # Replace this part with correct decay model?
    
  # Downwash
  vals['w'] = m.sqrt(vals['F']*inp['W']/(2*m.pi*rho_a*(R**2)))

  return vals

# Vortex Funcitons
def calculate_vortex_values(inp, vals):
  """Calculate vortex values, adding them to 
  the vals dictionary"""

  # Rotor tip vorticity
  vals['Γ'] = 10 #inp['U_inf']/(inp['Omega']*R)
  # replace with proper equation

  # core radius of vortex
  vals['ρc'] = 1

  return vals

def get_coordinates(n, inp, domain):
  'Make grid of size nxn for velocity field'
  X, Y, Z = np.meshgrid(np.linspace(domain[0], domain[1], n), np.linspace(domain[0], domain[1], n), np.linspace(domain[2], domain[3], n))
  return [X, Y, Z]

def cart2pol(X, Y, Z):
  # horizontal angle in 3d spherical coords
  φ = np.arctan2(Y, X)
  # coordinate of vortex ring line in same plane as point
  xr = R*np.cos(φ)
  yr = R*np.sin(φ)
  zr = inp['H']
  # coordinate of point from corresponding point on ring
  x = X-xr
  y = Y-yr
  z = Z-zr
  # radial location from corresponding point on ring
  ρ = np.sqrt((x)**2 + (y)**2 + (z)**2)
  # vertical angle from corresponding point on ring
  θ = np.arctan2((z), np.sqrt((x)**2 + (y)**2))
  return(ρ, θ, φ, xr, yr, zr)

def get_vortex_speed_matrixes(inp, vals, X, Y, Z, xr, yr, zr, ρ, θ, φ):
  'Calculate velocities in grid'
  # velocity of air in polar coordinates (Lamb-Oseen Vortex model)
  Vr = 0
  Vθ = -vals['Γ']/(2*m.pi*ρ)*(1-np.exp(-ρ**2/vals['ρc']**2))
  Vφ = 0
  # convert velocities of air in to cartesian coordinates
  Vx = Vθ*np.sin(θ)*np.cos(φ)
  Vy = Vθ*np.sin(θ)*np.sin(φ)
  Vz = Vθ*np.cos(θ)
  # #loop thought points with i j k coordinates
  for k in range(len(Z)):
      for j in range(len(Y)):
          for i in range(len(X)):
            if (X[k][j][i]**2+Y[k][j][i]**2)>(xr[k][j][i]**2+yr[k][j][i]**2):
              Vz[k][j][i] = -1*Vz[k][j][i]
  return [Vx, Vy , Vz]

def main():
  """ Main function """
  n = 30
  domain = [-10, 10, 0, 20]
  linspace = (domain[1] -domain[0])/n
  vals = calculate_w_values(inp)
  vals = calculate_vortex_values(inp, vals)
  X, Y, Z = get_coordinates(n, inp, domain)
  'local polar coordinated of each vortex'
  ρ, θ, φ, xr, yr, zr = cart2pol(X, Y, Z)
  'velocities produced by individual vortices'
  Vx, Vy, Vz = get_vortex_speed_matrixes(inp, vals, X, Y, Z, xr, yr, zr, ρ, θ, φ)

  'Plot Vector Field'
  fig = plt.figure()
  ax = fig.gca(projection='3d')
  # ax.scatter(X, Y, Z, 'o')
  # ax.scatter(xr, yr, zr, 'o')
  ax.quiver(X, Y, Z, Vx, Vy, Vz, arrow_length_ratio = .3)
  ax.set_xlabel('X Label')
  ax.set_ylabel('Y Label')
  ax.set_zlabel('Z Label')
  plt.title('Vortex Velocity Field')
  ax.set_xlim(-10, 10)
  ax.set_ylim(-10, 10)
  ax.set_zlim(0, 20)
  plt.show(block = False)

  plt.figure()
  plt.quiver(X[15][:][:], Z[15][:][:], Vx[15][:][:], Vz[15][:][:], headwidth=2, headlength=3)
  plt.xlabel('x (m)')
  plt.ylabel('z (m)')
  plt.title('Vortex Velocity Field')
  plt.xlim(-10, 10)
  plt.ylim(0, 20)
  plt.show(block = False)
  
  plt.figure()
  plt.contourf(X[15][:][:], Z[15][:][:], (np.sqrt(Vx[15][:][:]**2 + Vz[15][:][:]**2)), 100, cmap = 'jet') 
  # plt.contour(X[15][:][:], Z[15][:][:], (np.sqrt(Vx[15][:][:]**2 + Vz[15][:][:]**2)), 100, cmap = 'jet') 
  plt.colorbar() 
  plt.xlim(-10, 10)
  plt.ylim(0, 20)
  plt.show() 
main()