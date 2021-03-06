"""
FYP: Killing wilding pines with helicopter sprayed herbicide
Helicopter spray model
"""

# Import Libraries
import math as m
import numpy as np
from mpl_toolkits.mplot3d import Axes3D
import matplotlib.pyplot as plt
import time
import scipy.sparse as sps

# Constants
rho_a = 1                     # kg/m^3 - constant air density
k_constant = 7                # constant relating helicopter roll-up to fixed-wing roll-up
rho_liquid = 1000             # kg/m^3 - constant spray density
mew_a = 1.789e-5              # kg/m/s - constant air viscocity
g = np.array([-9.81, 0, 0])   # m/s^2 - gravity acceleration - [z, y, x]
V_min = 9e-7                  # m^3 per m^2 - minimum spray volume required

# Inputs
inp = {
    'R': 5,  # m - raidus of helicopter propeller
    'c': 0.25,  # m - blade chord length
    'U_inf': 10.0,  # m/s - helicopter speed
    'V_drop': -10.0,  # m/s - droplet initial speed
    'Omega': 405 * 2 * m.pi / 60,  # rad/s - rotor rotational speed
    'H': 30.0,  # m - height of helicopter
    'W': 1134 * 9.81,  # N - weight of helicopter and everything it is carrying, max downwash weight
    'q_squared': 0.1,  # square root of mean squared turbulence.
    'dry_bulb_temp': 12,  # degrees C
    'RH': 50 # Reletive humidity %
}

inp_weather = {  # Need to get this from somewhere.
    'Vx': 0,  # m/s
    'Vy': 0,  # m/s
    'Vz': 0  # m/s
}

# Tree inputs (20x20 grid. 1m per entry):
tree_data = [[0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
             [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
             [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
             [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0],
             [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0],
             [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
             [0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0],
             [0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0],
             [0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
             [0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
             [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0],
             [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0],
             [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
             [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
             [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
             [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
             [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
             [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
             [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
             [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]]
heli_position = (19, 15)  # (columns from left, rows from top) coordinate position in tree matrix
heli_direction = 45  # Angle (degrees), anticlockwise from ^ direction
tree_spacing = 1  # (m)

# Boom Inputs:
boom_length = 0.8 * 2 * inp['R']  # (m)
n_nozzles = 10
boom_height = 2  # (m) - height distance from boom to rotor blades
boom_offset = 0.5  # (m) - distance of boom from helicopter blade centre in positive x direction

# For each Accu-Flo 0.28 nozzle
number_drops = 500
droplet_min = 800e-6    # 800 microns
droplet_max = 1000e-6   # 1000 microns
# http://www.bishopequipment.com/technicalinformation.html
# Manufacturer specifies droplet sizes of 800-1000 microns
# Log normal will incorporate outliers
droplet_avg = 1110e-6       # (VMD) (updated Friday 08th with Volume Based Estimates
sigma = 0.15                # Standard deviation (updated Friday 08th)
cone_angle = 20         # Accu-Flo 0.28 has very narrow cone range (20 deg est)
nozzle_flowrate = 2e-5  # m^3/s

# Sparse Domain:
nr = 3  # Can change the multiplier on the radius to choose domain size
s_domain = [(-nr * inp['R'], nr * inp['R']), (1e-12, inp['H'])]

# Numerical Values:
inp_num = {
    'n_base': 5,  # Dividing streamline sections and number of vortices
    'nt': 1000  # Max time iterations
}

# To make velocity field work
vortex_factor = 0.1

# Complete Domain:
def calc_required_domain(n, dx, nz):
    """Calculates required domain size based on helicopter and wind speeds."""

    # Calculate time estimate:
    SF = 3
    t = SF * (-inp_weather['Vz'] - np.sqrt(inp_weather['Vz']**2 - 2 * g[0] * (s_domain[1][1] - s_domain[1][0]))) / g[0]

    # Calculate extra distance
    Lx = int((inp_weather['Vx'] - inp['U_inf']) * t / dx) * dx  # Need to make sure Ls are multiples of ds
    Ly = int(inp_weather['Vy'] * t / dx) * dx

    # Calculate extra nodes required
    nx = int(abs(Lx / dx))
    ny = int(abs(Ly / dx))

    # Construct new domain
    domain = [[s_domain[0][0], s_domain[0][1]],  # [start_x], [stop_x]
              [s_domain[0][0], s_domain[0][1]]]  # [start_y], [stop_y]
    if Lx > 0:
        domain[0][1] = domain[0][1] + Lx
    else:
        domain[0][0] = domain[0][0] + Lx

    if Ly > 0:
        domain[1][1] = domain[1][1] + Ly
    else:
        domain[1][0] = domain[1][0] + Ly

    # Construct new coordinates
    x, y, z = np.meshgrid(np.linspace(domain[0][0], domain[0][1], n + nx),
                          np.linspace(domain[1][0], domain[1][1], n + ny),
                          np.linspace(s_domain[1][0], s_domain[1][1], nz))

    return x, y, z, domain, nx, ny

# Field Positions
def get_coordinates(n):
    """ Make grid of size nxnxnz for velocity field """
    
    # Get x and y direction vector and dx
    x = np.linspace(s_domain[0][0], s_domain[0][1], n)
    dx = abs(x[1] - x[0])
    
    # Calculate appropriate nz, to give same/similar spacing
    nz = int(np.ceil((s_domain[1][1] - s_domain[1][0]) / dx + 1))

    # Get smaller domain grid for calculating vortices
    x, y, z = np.meshgrid(x, x, np.linspace(s_domain[1][0], s_domain[1][1], nz))

    return [x, y, z, dx, nz]

# Velocity Field Functions
def get_downwash_speed_matrixes(w, x, y, z, n):
    """ Calculate velocities in grid """
    # Calculating Constant:
    const = w / (2 * inp['H'])

    # Calculating Velocities
    Vx = const * np.sqrt(x**2 + y**2) * np.cos(np.arctan2(y, x))
    Vy = const * np.sqrt(x**2 + y**2) * np.sin(np.arctan2(y, x))
    Vz = -w * z / inp['H']

    return [Vx, Vy, Vz]

# Vortex Funcitons
def cart2pol(X, Y, Z, r_ds, current_z):
    # horizontal angle in 3d spherical coords
    φ = np.arctan2(Y, X)
    # coordinate of vortex ring line in same plane as point
    xr = r_ds * np.cos(φ)
    yr = r_ds * np.sin(φ)
    zr = current_z
    # coordinate of point from corresponding point on ring
    x = X - xr
    y = Y - yr
    z = Z - zr
    # radial location from corresponding point on ring
    ρ = np.sqrt((x)**2 + (y)**2 + (z)**2)
    # vertical angle from corresponding point on ring
    θ = np.arctan2((z), np.sqrt((x)**2 + (y)**2))
    return(ρ, θ, φ, xr, yr, zr)

def get_vortex_speed_matrixes(Γ, ρc, X, Y, Z, xr, yr, zr, ρ, θ, φ):
    'Calculate velocities in grid'
    # velocity of air in polar coordinates (Lamb-Oseen Vortex model)
    Vr = 0
    Vθ = -Γ / (2 * m.pi *ρ) * (1 - np.exp(-ρ**2 / ρc**2))
    Vφ = 0
    # convert velocities of air in to cartesian coordinates
    Vx = Vθ* np.sin(θ) * np.cos(φ)
    Vy = Vθ* np.sin(θ) * np.sin(φ)
    Vz = Vθ* np.cos(θ)
    # #loop thought points with i j k coordinates
    for k in range(len(Z)):
        for j in range(len(Y)):
            for i in range(len(X)):
                if (X[i][j][k]**2 + Y[i][j][k]**2) > (xr[i][j][k]**2 + yr[i][j][k]**2):
                    Vz[i][j][k] = -1 * Vz[i][j][k]
    return [Vx, Vy, Vz]

# Streamline function:
def get_dividing_streamline(n, Vx, Vy, Vz):
    """ Get dividing streamline coordinates. """
    n = int(n / nr)

    # Initialize z
    z = [0.] * n

    # X, Y 2D coordinate matrixes
    r = np.linspace(inp['R'], s_domain[0][1], n)  # Need this to work out z
    Vr = np.sqrt(Vx[1, :, 1]**2 + Vy[:, 1, 1]**2)
    R, P = np.meshgrid(r, np.linspace(0, 2 * np.pi, n))
    dividing_streamline_x, dividing_streamline_y = R * np.cos(P), R * np.sin(P)

    # Top:
    z[0] = inp['H']

    # Iterate:
    for i in range(0, n - 1):
        j = int((nr - 1) * n + i)
        z[i + 1] = z[i] + ((s_domain[0][1] - inp['R']) / (n - 1)) * Vz[1, 1, -i - 1] / Vr[j]
        if z[i + 1] < 0:
            z[i + 1] = 0

    # Make z matrix
    dividing_streamline_z = np.outer(np.ones(n), z)

    return [dividing_streamline_x, dividing_streamline_y, dividing_streamline_z]

# Plotting Functions:
def plot_velocity_field_and_dividing_streamline(x, y, z, Vx, Vy, Vz, ds_x, ds_y, ds_z):
    """Plot the vector field of the air with the dividing streamline."""
    fig = plt.figure()
    ax = fig.gca(projection='3d')
    ax.quiver(x, y, z, Vx, Vy, Vz, length=0.5, normalize='True', arrow_length_ratio=0.1)
    ax.plot_wireframe(ds_x, ds_y, ds_z, cmap=plt.cm.YlGnBu_r)
    ax.set_xlabel('x (m)')
    ax.set_ylabel('y (m)')
    ax.set_zlabel('z (m)')
    plt.title('Velocity Field')

def plot_air_velocity_slice(plotH, n, x, y, z, Vx, Vy, Vz):
    """Plot an air velocity contour and vector slice at a certain height."""
    # Plotting a Contour at a certain height (horizontal velocities only)
    plt.figure()
    ploti = int(plotH * n / inp['H'])
    plt.streamplot(x[:, :, ploti], y[:, :, ploti], Vx[:, :, ploti], Vy[:, :, ploti])

    # Plotting velocity vectors only at that height
    fig2 = plt.figure()
    ax2 = fig2.gca(projection='3d')
    ax2.quiver(x[:, :, ploti], y[:, :, ploti], z[:, :, ploti], Vx[:, :, ploti], Vy[:, :, ploti], Vz[:, :, ploti])

# Air Velocities Function:
def get_velocity_field(n):
    """Takes in downwash, vortex and weather velocities, as well as dividing 
    streamline poitions and the coordinates grid. Returns air velocities."""

    # Get field coordinates:
    x, y, z, dx, nz = get_coordinates(n)

    # Get Steady State Downwash Velocities:
    ss_w = m.sqrt(inp['W'] / (2 * m.pi * rho_a * (inp['R']**2)))
    Vwx, Vwy, Vwz = get_downwash_speed_matrixes(ss_w, x, y, z, n)

    # Get Dividing Streamling Coordinates:
    ds_x, ds_y, ds_z = get_dividing_streamline(n, Vwx, Vwy, Vwz)

    # Calculate constants:
    # mew = inp['U_inf'] / (inp['Omega'] * inp['R'])  # Helicopter forward advance
    #sigma_s = 2 * inp['c'] / (m.pi * inp['R'])  # Helicopter Blade Solidity - Using equation Illia gave
    Gamma0 = inp['W'] / (2 * rho_a * inp['R'] * inp['U_inf'])  # Circulation constant

    # Initialise air velocities for small domain
    Vx_s = x * 0
    Vy_s = y * 0
    Vz_s = z * 0

    # Put together velocity field with dividing streamline:
    F = 1
    dz = z[0, 0, 1]
    for ds_i in range(0, int(n / nr) - 1):
        """Since the node number in the dividing streamline are 1/nr the nodes in
        the regular velocities, each dividing streamline node is used for 
        multiple velocity nodes in the z direction."""

        # Work out indicies for dz:
        start_i = (np.abs(z[0, 0, :] - ds_z[0, ds_i])).argmin()
        stop_i = (np.abs(z[0, 0, :] - ds_z[0, ds_i + 1])).argmin()
        r_ds = ds_x[0, ds_i + 1]

        for k in range(start_i, stop_i, -1):
            k = max(k, 0)

            # Calculate F for height:
            F = np.exp(-(inp['H'] - z[0, 0, k]) / inp['R'])

            # Calculate new vortex velocity:
            r_ds = ds_x[0, ds_i] + (z[0, 0, k] - ds_z[0, ds_i]) * (ds_x[0, ds_i + 1] - ds_x[0, ds_i]) / (ds_z[0, ds_i + 1] - ds_z[0, ds_i])  # Using linear interpolation
            ρc = r_ds - inp['R']
            ρ, θ, φ, xr, yr, zr = cart2pol(x, y, z, r_ds, z[0, 0, k])  # local polar coordinated of each vortex
            Gamma = Gamma0 * F * vortex_factor
            Vvx, Vvy, Vvz = get_vortex_speed_matrixes(Gamma, ρc, x, y, z, xr, yr, zr, ρ, θ, φ)
            Vx_s = Vx_s + Vvx
            Vy_s = Vy_s + Vvy
            Vz_s = Vz_s + Vvz
            
            for i in range(0, n):
                for j in range(0, n):
                    r_coord = np.sqrt(x[i, j, k]**2 + y[i, j, k]**2)
                    if r_coord < r_ds:
                        Vx_s[i, j, k] = Vx_s[i, j, k] + np.sqrt(F) * Vwx[i, j, k]
                        Vy_s[i, j, k] = Vy_s[i, j, k] + np.sqrt(F) * Vwy[i, j, k]
                        Vz_s[i, j, k] = Vz_s[i, j, k] + np.sqrt(F) * Vwz[i, j, k]




    # Plot Velocity Field and Dividing Streamline
    # plot_velocity_field_and_dividing_streamline(x, y, z, Vx_s, Vy_s, Vz_s, ds_x, ds_y, ds_z)

    # Plot Velocity Slice at Certain Height:
    plotH=10  # m
    # plot_air_velocity_slice(plotH, n, x, y, z, Vx_s, Vy_s, Vz_s)

    # Construct Larger Domain:
    X, Y, Z, domain, nx, ny=calc_required_domain(n, dx, nz)

    # Initialise air velocities for large domain and add Weather
    Vx=X * 0 + inp_weather['Vx']
    Vy=Y * 0 + inp_weather['Vy']
    Vz=Z * 0 + inp_weather['Vz']

    # With the helicopter flying in the positive x-direction:
    Vx=Vx - inp['U_inf']

    # Place small velocity field in the large one:
    xi=0
    yi=0
    if domain[0][0] < s_domain[0][0]:
        yi=nx
    if domain[1][0] < s_domain[0][0]:
        xi=ny
    Vx[xi:xi + n, yi:yi + n, :] += Vx_s
    Vy[xi:xi + n, yi:yi + n, :] += Vy_s
    Vz[xi:xi + n, yi:yi + n, :] += Vz_s

    # Plot Total Velocity Field:
    # plot_air_velocity_slice(plotH, n, X, Y, Z, Vx, Vy, Vz)
    return Vx, Vy, Vz, domain, (X, Y, Z)


# Droplet Tracking Functions:
def calc_reynolds(V_slip, D_drop):
    """Calculate Droplet Reynold's Number in 3D"""

    Re = (rho_a * D_drop * V_slip) / mew_a
    return Re

def calc_drag(Re):
    """Calculate Droplet Drag Coefficient in 3D"""
    # Langmuir and Blodgett 1964
    Cd = (24 / Re) * (1 + (0.197 * Re**0.63) + (2.6e-4 * Re**1.38))

    # Clift, Grace and Weber 1978
    #Cd = (24 / Re) * (1 + (0.15 * Re**0.687)) + 0.42 / (1 + 4.25e-4 * (Re**-1.16))
    return Cd

def calc_evaporation_model(Dia, Re, time, dt):
    # Function that returns the new diameter of droplet
    # Dia is the diameter of the droplet in metres
    # Re is the Reynolds number of the droplet

    # Calculate the wet bulb temperature
    a = 611.21  # Pa
    b = 18.678
    c = 257.14  # deg C
    d = 234.5  # deg C
    psm = a * np.exp((b - inp['dry_bulb_temp'] / d) * (inp['dry_bulb_temp'] / (c + inp['dry_bulb_temp'])))
    gammam = np.log((inp['RH'] / 100) * np.exp((b - inp['dry_bulb_temp'] / d) * (inp['dry_bulb_temp'] / (c + inp['dry_bulb_temp']))))
    dewPointTemp = c * gammam / (b - gammam)
    wetBulbTemp = (2 / 3) * inp['dry_bulb_temp'] + (1 / 3) * dewPointTemp
    
    # bulbTemp is the wet bulb temperature depression in degrees Celsius
    bulbTemp = inp['dry_bulb_temp'] - wetBulbTemp

    # Calculate Beta:
    beta = 84.76 * (1 + 0.27 * np.sqrt(Re)) * 1e-12

    # Calculate Droplet Life:
    tl = Dia**2 / (beta * bulbTemp)

    # Calculate New Diameter
    should_break = False
    if time < tl:
        Dia = Dia * (np.sqrt(1 - dt / tl))
    else:
        should_break = True

    return Dia, should_break

def calc_relaxation_time(D, Cd, V_slip):
    """Calculates droplet relaxation time. Teske 1989 Eq 8."""
    Tp = (4 * D * rho_liquid) / (3 * Cd * rho_a * abs(V_slip))
    return Tp

def calc_travel_time(V_slip):
    """Calculates droplet time of travel. Teske 1989 Eq 16."""
    Tt = inp['R'] / (abs(V_slip) + (3 / 8) * np.sqrt(inp['q_squared']))
    return Tt

def calc_K(T_ratio):
    """Calculate K function. T_ratio is Tp/Tt. Teske 1989 Eq 17."""
    K = 0.5 * ((3 - T_ratio**2) * (1 - T_ratio) + (T_ratio)**2 - 1) / (1 - T_ratio**2)**2
    return K

def calc_variances(D, V_slip, variances, dt):
    """Calculates the variances in one direction with previous variances 
    inputted as a tuple (xx0, xv0, vv0).Returns new tuple (xx, xv, vv)."""
    # Accounting for zero slip velocities:
    Re = np.array([0., 0., 0.])
    Cd = np.array([0., 0., 0.])
    Tp = np.array([0., 0., 0.])
    for di in range(3):
        if V_slip[di] != 0:
            # Calculating new drag and Reynolds
            Re[di] = calc_reynolds(V_slip[di], D)
            Cd[di] = calc_drag(Re[di])  # Possible that V_slip could be zero and result in infinite drag, causing Tp to be zero

            # Calculate relacation time, travel time and K
            Tp[di] = calc_relaxation_time(D, Cd[di], V_slip[di])

    Tt = calc_travel_time(V_slip)
    K = calc_K(Tp / Tt)

    # Initialize variances
    xx0, xv0, vv0 = variances

    # Calculate new variances
    xu = inp['q_squared'] * (-Tp * K + Tt / 2) / 3               # Teske 1989 Eq 14
    uv = inp['q_squared'] * K / 3                                # Teske 1989 Eq 15
    vv = (2 * dt * uv + Tp * vv0) / (Tp + 2 * dt)         # Discretisation of Teske 1989 Eq 13
    xv = (dt * xu + Tp * xv0 + Tp * dt * vv) / (Tp + dt)  # Discretisation of Teske 1989 Eq 12
    xx = 2 * dt * xv + xx0                                # Discretisation of Teske 1989 Eq 11

    return (xx, xv, vv)

def binormal_distribution(x_mean, y_mean, x_var, y_var, x_array, y_array):
    """Calculates the binormal distribution for domain."""
    fx = (1 / np.sqrt(x_var * 2 * np.pi)) * np.exp(-(x_array - x_mean)**2 / (2 * x_var))
    fy = (1 / np.sqrt(y_var * 2 * np.pi)) * np.exp(-(y_array - y_mean)**2 / (2 * y_var))
    f = fx * fy
    return f

def update_droplet(V_drop, L_drop, D_drop, variances, i, Va, current_time, domain):
    """update droplet with next time step."""
    # Initialising next row:
    V_drop = np.vstack((V_drop, np.array([0., 0., 0.])))
    L_drop = np.vstack((L_drop, np.array([0., 0., 0.])))
    D_drop = D_drop + [0.]

    Vax = Va[0]
    
    # Calculate timestep:
    inv_z_transit_time = np.abs((V_drop[i - 1, 0] * Vax.shape[2]) / (s_domain[1][1] - s_domain[1][0]))  # 1/characteristic time to transit 1 cell in the z direction
    inv_y_transit_time = np.abs((V_drop[i - 1, 1] * Vax.shape[0]) / (domain[1][1] - domain[1][0]))  # 1/characteristic time to transit 1 cell in the y direction
    inv_x_transit_time = np.abs((V_drop[i - 1, 2] * Vax.shape[1]) / (domain[0][1] - domain[0][0]))  # 1/characteristic time to transit 1 cell in the x direction
    inv_cell_transit_time = np.amax([inv_z_transit_time, inv_y_transit_time, inv_x_transit_time])
    inverse_times = [inv_cell_transit_time]
    dt = 1 / np.amax(inverse_times)

    # Finding Current Position Index:
    iy = int((L_drop[i - 1, 2] - domain[0][0]) * (Vax.shape[1] - 1) / (domain[0][1] - domain[0][0]))
    ix = int((L_drop[i - 1, 1] - domain[1][0]) * (Vax.shape[0] - 1) / (domain[1][1] - domain[1][0]))
    iz = int(L_drop[i - 1, 0] * (Vax.shape[2] - 1) / (s_domain[1][1] - s_domain[1][0]))

    # Reynolds Calculation in 3D coords
    Vxa, Vya, Vza = Va
    V_air = np.array([Vza[ix, iy, iz], Vya[ix, iy, iz], Vxa[ix, iy, iz]])
    # V_air = [-5, 0, 0.1]  # Constant air testing _________________________________ ENABLED
    V_slip = abs(V_drop[i - 1, :] - V_air)
    Re = calc_reynolds(V_slip, D_drop[i - 1])

    # If statements to catch zero slip velocity components:
    directions = np.array([0., 0., 0.])
    Cd = np.array([0., 0., 0.])
    Fd = np.array([0., 0., 0.])
    mass = np.array([0., 0., 0.])
    Ad = np.array([0., 0., 0.])
    for di in range(3):
        if V_slip[di] != 0:
            # Direction of air slip
            directions[di] = -V_slip[di] / (V_drop[i - 1, di] - V_air[di])

            # Drag Calculation in 3D coords
            Cd[di] = calc_drag(Re[di])

            # Drag Force Calculations
            Fd[di] = Cd[di] * (np.pi * (D_drop[i - 1] / 2)**2) * (rho_a / 2) * V_slip[di]**2
            mass[di] = (1 / 6) * (np.pi * (D_drop[i - 1]**3)) * rho_liquid
            Ad[di] = (Fd[di] / mass[di]) * directions[di]

    # Calculate change in diameter with time:
    overall_Re = np.sqrt(Re[0]**2 + Re[1]**2 + Re[2]**2)
    D_drop[i], should_break = calc_evaporation_model(D_drop[i - 1], overall_Re, current_time, dt)

    # Update droplet velocity and position and diameter
    V_drop[i, :] = V_drop[i - 1, :] + g * dt + Ad * dt
    L_drop[i, :] = L_drop[i - 1, :] + V_drop[i - 1, :] * dt

    # Update Variances
    new_V_slip = abs(V_drop[i, :] - V_air)
    variances = calc_variances(D_drop[i], new_V_slip,
                               variances, dt)

    # Update Time
    current_time = current_time + dt

    return V_drop, L_drop, D_drop, variances, should_break, current_time

def testing_droplet_outcome(L_drop, D_drop, xx, x_array, y_array, volume_matrix, should_break_initial):
    """Test if droplet has been deposited, evaporated, or exited domain."""
    should_break = False

    # Testing if droplet has been deposited
    if L_drop[0] <= 0:
        V_deposition = (1 / 6) * (np.pi * D_drop ** 3) * 1e6  # Volume depositied (ml)

        #x_index = int((L_drop[i, 2]) * n / (6 * inp['R']) + n / 2)
        # y_index = int((L_drop[i, 1]) * n / (6 * inp['R']) + n / 2)  # n/2 to shift to middle?
        #volume_matrix[x_index, y_index] += V_deposition
        V_array = V_deposition * binormal_distribution(L_drop[2], L_drop[1], xx[2], xx[1], x_array, y_array)
        volume_matrix += V_array
        should_break = True

    # Testing if droplet has escaped
    has_escaped = (L_drop[2] < x_array[0, 0]) or (L_drop[2] > x_array[0, -1])
    has_escaped = has_escaped or (L_drop[1] < y_array[0, 0]) or (L_drop[1] > y_array[-1, 0])
    if has_escaped:      # This just testing if escaped domain not dividing streamline
        print(f"Escaped at %s" % L_drop)
        should_break = True

    # Testing if droplet has completely evaporated
    if D_drop <= 0 or should_break_initial:
        print(f"Droplet evaporated at %s: " % L_drop)
        should_break = True

    return volume_matrix, should_break

def droplet_plots(number_drops, droplet_paths, x, y, volume_matrix):
    """Plot droplet tracking plots"""
    fig = plt.figure()
    ax = plt.axes(projection='3d')
    # ax = fig.add_subplot(111, projection='3d')
    # Axes3D.plot(L_drop[:,2], L_drop[:,1], L_drop[:,0])
    ax.set_xlim([x[0], x[-1]])
    ax.set_ylim([y[0], y[-1]])
    ax.set_zlim([0, inp['H']])
    ax.set_xlabel('X')
    ax.set_ylabel('Y')
    ax.set_zlabel('Z')
    ax.set_title('Trajectory of droplets')
    for j in range(0, number_drops):
        max_index = np.max(np.where(droplet_paths[j, :, :])) + 1
        ax.plot3D(droplet_paths[j, 0:max_index, 2], droplet_paths[j, 0:max_index, 1], droplet_paths[j, 0:max_index, 0])
        plt.show(block=False)

    fig = plt.figure()  # Plot mass deposited on ground
    plt.contourf(x, y, volume_matrix)
    plt.colorbar()
    plt.xlabel('X')
    plt.ylabel('Y')
    plt.show(block=False)

def droplet_tracking(nt, domain, positions, Va, nozzle_position):
    """
    Single droplet tracker (takes in info on nozzle position, 
    droplet size and initial velocity, air velocity matrixes drag coeff and evaporation 
    and tracks droplet trajectory)
    
    Should sample droplet size & velocity from distribution
    Compute Re and Cd then advance droplet along trajectory
    Compute evaporation then update mass
    
    Advance time until deposition or escape
    
    Grab new sample until enough spray has been sampled
    """

    # Initial Value Inputs:
    droplet_paths = np.zeros((number_drops, nt, 3))
    max_i = np.zeros(number_drops)

    # Accu-Flo 0.28 Parameters
    # http://www.bishopequipment.com/technicalinformation.html
    # Manufacturer specifies droplet sizes of 800-1000 microns
    # Log normal will incorporate outliers

    # Uniform Distribution with these values (testing)
    # droplet_size = np.random.uniform(droplet_min, droplet_max)

    droplet_size = np.random.lognormal(np.log(droplet_avg), sigma)

    x_array = positions[0][:, :, 0]
    y_array = positions[1][:, :, 0]
    volume_matrix = x_array * 0.0

    # Will need to get this info from somewhere
    inp_droplet = {
        'V_init': np.array([inp['V_drop'], 0., 0.]),        # m/s - droplet initial speed
        'L_init': np.array([nozzle_position[2], nozzle_position[1], nozzle_position[0]]),    # m - nozzle location
        'D_init': droplet_size,                    # m - droplet diameter
    }

    for j in range(0, number_drops):  # Loop over all droplets

        # Velocity
        theta = (np.pi / 180) * np.random.uniform(0, cone_angle / 2)  # Determine droplet direction: sample an angular range within the nozzle cone angle
        phi = (np.pi / 180) * np.random.uniform(0, 360)  # Determine droplet direction: sample a 360 degree range
        x_comp = inp['V_drop'] * np.sin(theta) * np.sin(phi)  # x cpt of droplet velocity
        y_comp = inp['V_drop'] * np.sin(theta) * np.cos(phi)  # y cpt of droplet velocity
        inp_droplet['V_init'] = np.array([inp['V_drop'], y_comp, x_comp])

        # Initialising
        V_drop = inp_droplet['V_init']  # Initial Droplet Velocity (z, y, x) - m/s
        L_drop = inp_droplet['L_init']  # Initial Droplet start position (z, y, x) - m
        # D_drop = [inp_droplet['D_init']]  # Initial Droplet Diameter - m
        D_drop = [np.random.lognormal(np.log(droplet_avg), sigma)]  # Initial Droplet Diameter - m
        xx = np.array([0., 0., 0.])  # Initialising Position Variance (z, y, x) - m
        xv = np.array([0., 0., 0.])  # Initial Variance
        vv = np.array([0., 0., 0.])  # Initial Variance
        variances = (xx, xv, vv)

        # Iterate with time:
        i = 1
        current_t = 0
        while i < nt:
            # Update Droplet
            V_drop, L_drop, D_drop, variances, should_break, current_t = update_droplet(V_drop, L_drop, D_drop, variances, i, Va, current_t, domain)

            # Paths
            droplet_paths[j, i - 1, :] = L_drop[i - 1, :]

            # Update position variance
            xx = np.vstack((xx, np.array([0., 0., 0.])))
            xx[i, :] = variances[0]

            # Testing droplet outcome
            volume_matrix, should_break = testing_droplet_outcome(L_drop[i, :],
                                                                  D_drop[i],
                                                                  xx[i, :],
                                                                  x_array,
                                                                  y_array,
                                                                  volume_matrix,
                                                                  should_break)
            if should_break:
                break

            i = i + 1

    # Did it break
    if i == nt:
        print("Didn't break. Ran out of time steps.")

    # Plotting
    x = positions[0][0, :, 0]
    y = positions[1][:, 0, 0]
    #droplet_plots(number_drops, droplet_paths, x, y, volume_matrix)

    # Mean Droplet Path:
    tot_drop = np.sum(droplet_paths, 0)
    num_zeros = np.count_nonzero(droplet_paths, 0)
    mean_path = tot_drop / num_zeros

    return x, y, volume_matrix, mean_path

def process_tree_data():
    """
    - Calculate tree vector distances
    - Maybe: Construct new tree position matrix, with tree positions changed to 
      velocity reference frame (needs to be rotated by angle)?
      - Might not need this so hold off
    - Output:
       - Maybe: New tree matrix in velocity field reference frame?
       - A list/vector of tupples with x and y distances from helicopter in velocity field reference frame
          - e.g. [(2, 3),(5, 2),(1, 1),(6, 2)]
          - So that in each tupple: 
            (distance tree is to left/right of helicopter, distance until helicopter in line with tree)
    """

    # Create coordinate arrays for only trees
    S = sps.find(tree_data)     # create sparse matrix containing only tree locations
    xt = S[1][:]
    yt = S[0][:]

    tree_distance = tree_spacing * np.sqrt((xt - heli_position[0])**2 + (heli_position[1] - yt)**2)      # find absolude displacement from helicopter to trees
    old_tree_angle = np.arctan2(((heli_position[1] - yt)), ((xt - heli_position[0])))                # find angle between tree and ground domain reference frame
    new_tree_angle = old_tree_angle - heli_direction                                             # find angle between
    a = tree_distance * np.cos(new_tree_angle)                                                     # horizontal distance to tree in helicopter reference frame
    b = tree_distance * np.sin(new_tree_angle)                                                     # vertical distance to tree in helicopter reference frame
	
    # map two lists into a single list of tuples
    tree_coord = list(zip(a, b))

    return tree_coord

def plot_spray(x, y, spray):
    """Plot spray deposited in domain.""" 
    fig = plt.figure()  # Plot mass deposited on ground
    plt.contourf(x, y, spray)
    plt.colorbar()
    plt.xlabel('X')
    plt.ylabel('Y')
    plt.show(block=False) 

def spray_distances(dx, volume_matrix, domain):
    """ Calculates the distance from the helicopter and width of spray. """

    # Find positions and locations where volume above 1e-7:
    x_indices, y_indices = np.where(volume_matrix > 1e-7)
    x_locations = y_indices * dx + domain[0][0]
    y_locations = x_indices * dx + domain[1][0]
    
    # Find Distances:
    x_span = (min(x_locations), max(x_locations))
    y_span = (min(y_locations), max(y_locations))

    return x_span, y_span

def spray_outcome(time, on_off_times, dt, volume_matrix, trees):
    """
    Simulate the true volume from nozzle and where it 
    all landed. Compares to tree matrix.
    """
    volume_matrix = np.hstack((volume_matrix, volume_matrix * 0.))  # Resize matrix to add more space at front
    spray_matrix = volume_matrix * 0.
    count = 0
    spray_time = 0
    for i in range(volume_matrix.shape[1]):
        if (time > on_off_times[count][0]) and (time < on_off_times[count][1]):
            spray_matrix[:, i:] += volume_matrix[:, :volume_matrix.shape[1] - i]
        elif (time > on_off_times[count][1]):
            spray_time += on_off_times[count][1] - on_off_times[count][0]
            count += 1
        if count == len(on_off_times):
            break
        time += dt
    # Multiply to correct volume based on nozzle flowrate:
    n = (nozzle_flowrate * spray_time) / np.sum(spray_matrix)
    spray_matrix = n * spray_matrix
    
    return spray_matrix

def nozzle_simulation(nt, positions, domain, V_field, trees, nozzle, start_time):
    """
    - Inputs:
      - Droplet tracking inputs
      - Nozzle position (m) as tupple (x, y, z) in velocity field reference frame
      - Tree positions (m) as list of tupples [(distance to side of heli, distance until reach tree), (x, y)]
    - Run droplet tracking once
    - Calculate spray distances and spray span in both x and y directions using mean and variance info or volume matrix
    - Calculate start stop times - using helicopter forward flight and current time
    - Decide if nozzle should turn on - using info on how far to side tree is and minimum volume required
    - Outputs:
      - List of ordered start/stop nozzle times as tuples: 
      - e.g. [[start time 1, stop time 1], [start time 2, stop time 2], [start time 3, stop time 3]]
    """
    
    # Run Droplet Tracking:
    x, y, volume_matrix, mean_path = droplet_tracking(nt, domain, positions, V_field, nozzle)

    # Get spray location:
    dx = abs(x[0] - x[1])
    x_span, y_span = spray_distances(dx, volume_matrix, domain)

    # Calculate spray times for each tree:
    times = []
    for i in range(len(trees)):
        d = np.array([trees[i][1] - x_span[1], trees[i][1] - x_span[0]])  # [start_d, stop_d]

        # Will spray reach tree?
        if (trees[i][0] > y_span[0]) and (trees[i][0] < y_span[1]):
            times += [start_time + d / inp['U_inf']]
    times = np.sort(np.array(times), 0)

    # Streamline spray times so they do not overlap:
    try:
        on_off_times = [times[0]]
        count = 0
        for i in range(len(times)):
            if times[i, 0] > on_off_times[count][1]:
                count += 1
                on_off_times += [times[i]]
            elif times[i, 1] > on_off_times[count][1]:
                on_off_times[count][1] = times[i, 1]
        on_off_times = np.array(on_off_times)
    except IndexError:
        on_off_times = [[False]]

    # Find spray on trees:
    dt = dx / inp['U_inf']
    spray = np.hstack((volume_matrix * 0, volume_matrix * 0))
    if on_off_times[0][0] is not False:
        spray = spray_outcome(start_time, on_off_times, dt, volume_matrix, trees)

    return on_off_times, mean_path, volume_matrix, spray

def get_nozzle_positions():
    """Use boom inputs to work out a list of nozzle positions as (x, y, z) tupples."""
    nozzle_positions = [(0.0)] * n_nozzles
    d_between = boom_length / (n_nozzles - 1)
    for i in range(n_nozzles):
        nozzle_positions[i] = (boom_offset, i * d_between - boom_length / 2, inp['H'] - boom_height)
    return nozzle_positions

def plot_mean_droplet_paths(Vy, Vz, domain, positions, n, droplet_paths):
    """ Plots the air velocity field and the 
    droplet paths in 2D to match AGDISP."""
    plt.figure()

    # Plot Velocity Field
    plt.quiver(positions[1][:, -int(n / 2), :], positions[2][:, -int(n / 2), :], Vy[:, -int(n / 2), :], Vz[:, -int(n / 2), :], headwidth=2, headlength=3)
    plt.xlabel('y (m)')
    plt.ylabel('z (m)')
    plt.title('Droplet Path and Velocity Field')
    plt.xlim(domain[1][0], domain[1][1])
    plt.ylim(s_domain[1][0], s_domain[1][1])

    # Plot mean droplet path for each nozzle:
    for droplet in droplet_paths:
        max_n = np.where(droplet[:, 0] > 0)[0][-1]
        plt.plot(droplet[:max_n, 1], droplet[:max_n, 0])
    
    plt.show(block=False)           

def main():
    """ Main function """
    # Get time of code begining:
    start_time = time.time()

    # Model Step Sizes:
    n_base = inp_num['n_base']  # Dividing streamline sections and number of vortices
    n = 2 * nr * n_base  # Number of nodes in space - must be even to avoid dividing by zero, also a multiple of nr
    #         Same in every direction.
    nt = inp_num['nt']                    # unitless - number of timesteps

    # Air Velocity Field
    Vx, Vy, Vz, domain, positions = get_velocity_field(n)
    
    # Get Nozzle Times:
    trees = process_tree_data()
    nozzle_positions = get_nozzle_positions()  # (x, y, z) in metres
    nozzle_times = [0] * n_nozzles
    droplet_paths = [0] * n_nozzles
    count = 0
    volume_matrix = positions[0][:, :, 0] * 0.0
    spray_matrix = positions[0][:, :, 0] * 0.0
    spray_matrix = np.hstack((spray_matrix, spray_matrix))
    for nozzle in nozzle_positions:
        nozzle_times[count], droplet_paths[count], spray, spraid = nozzle_simulation(nt, positions, domain, (Vx, Vy, Vz), trees, nozzle, start_time)
        volume_matrix += spray
        spray_matrix += spraid
        print('Nozzle ' + str(count + 1))
        print(nozzle_times[count])
        count += 1

    # Plot mean droplet paths with velocity field in 2D:
    plot_mean_droplet_paths(Vy, Vz, domain, positions, n, droplet_paths)
    x = positions[0][0, :, 0]
    y = positions[1][:, 0, 0]
    #plot_spray(x, y, volume_matrix)
    x = np.hstack((x, x + (x[-1] - x[0]) + (x[1] - x[0])))
    plot_spray(x, y, spray_matrix)
    for tree in trees:
        xi = abs(y - tree[0]).argmin()
        yi = abs(x - tree[1]).argmin()
        if spray_matrix[xi, yi] > V_min * tree_spacing:
            # Foliage recieved enough herbicide
            plt.plot(tree[1], tree[0], 'g.')
        else:
            # Foliage did not recieved enough herbicide
            plt.plot(tree[1], tree[0], 'rx')
    plt.show()


if __name__ == '__main__':
    main()
