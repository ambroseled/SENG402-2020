"""
FYP: Killing wilding pines with helicopter sprayed pesticide
Vortex model
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
  vals['Γ'] = 1 #inp['U_inf']/(inp['Omega']*R)
  # replace with proper equation

  return vals

def get_coordinates(n, inp, domain):
    'Make grid of size nxn for velocity field'
    X, Z = np.meshgrid(np.linspace(domain[0], domain[1], n), np.linspace(domain[2], domain[3], n))

    return [X, Z]

def cart2pol(X, Z):
    ρ = np.sqrt(X**2 + Z**2)
    θ = np.arctan2(Z, X)
    return(ρ, θ)

def get_vortex_speed_matrixes(inp, vals, ρ, θ):
  'Calculate velocities in grid'
  Vr = 0
  Vθ = vals['Γ']/(2*m.pi)
  Vx = -Vθ*np.sin(θ)*(np.max(ρ)-ρ)
  Vz = Vθ*np.cos(θ)*(np.max(ρ)-ρ)
  return [Vx, Vz, Vr, Vθ]

def main():
  """ Main function """
  n = 50
  domain = [-20, 20, 0, 20]
  linspace = (domain[1] -domain[0])/n
  vals = calculate_w_values(inp)
  vals = calculate_vortex_values(inp, vals)
  X, Z = get_coordinates(n, inp, domain)
  'local polar coordinated of each vortex'
  ρ1, θ1 = cart2pol(X-linspace*np.ceil(R/linspace), Z-inp['H'])
  ρ2, θ2 = cart2pol(X+linspace*np.ceil(R/linspace), Z-inp['H'])
  'velocities produced by individual vortices'
  Vx1, Vz1, Vρ, Vθ = get_vortex_speed_matrixes(inp, vals, ρ1, θ1)
  Vx2, Vz2, Vρ, Vθ = get_vortex_speed_matrixes(inp, vals, ρ2, θ2)
  'superposing velocity vectors'
  Vx = Vx1 - Vx2
  Vz = Vz1 - Vz2

  'Plot Vector Field'
  plt.figure()
  plt.quiver(X, Z, Vx, Vz, headwidth=0, headlength=0)
  plt.xlabel('x (m)')
  plt.ylabel('z (m)')
  plt.title('Vortex Velocity Field')
  plt.xlim(-20, 20)
  plt.ylim(0, 20)
  plt.show(block = False)

  plt.figure()
  plt.streamplot(X, Z, Vx, Vz, 2)
  plt.xlabel('x (m)')
  plt.ylabel('z (m)')
  plt.title('Vortex Streamlines')
  plt.xlim(-20, 20)
  plt.ylim(0, 20)
  plt.show(block = False)
  
  plt.figure()
  plt.contourf(X, Z, (np.sqrt(Vx**2 + Vz**2)), 100, cmap = 'jet') 
  #   plt.contour(X, Z, (np.sqrt(Vx**2 + Vz**2)), 100)
  plt.colorbar() 
  plt.xlim(-20, 20)
  plt.ylim(0, 20)
  plt.show() 
main()