import numpy as np
from matplotlib import pyplot as plt
from mpl_toolkits.mplot3d import Axes3D

# Droplet trasnport model; Mark Jermy
# To do:
# check logic of addition of delta V_drop_rel
# Compute over distribution of nozzle speed and droplet size
# Trajectories of small droplets are curtailed: could be incorrect handling of relaxation time?
# While statement: (while droplet is within domain): should used limits of floor +1?


# Set dimensions of domain
zmax=10 # metres
ymax=10 # metres
xmax=10 # metres

# Set coordinates of nozzle
L_nozzle=[zmax, ymax/2, xmax/2]  # metres

# Set size of drop
d_drop_microns=500 #microns
d_drop=d_drop_microns/1e6 # m
number_drops=100 # number to be simulated

# Set drop velocity on exiting nozzle
speed_nozzle=-10 # m/s
cone_angle=80 # included angle, degrees

# Set densities
density_liquid=1000 # liquid kg/m**3
density_air=1.2 # air kg/m**3
viscosity_air=1.5e-5 # kinematic m**2/s

mass_drop=density_liquid*np.pi*(d_drop**3)/6 # kg

# Set acceleration due to gravity as a vector
g=np.array([-9.81,0,0]) # z y x in m/s**2

# Set number of cells in each direction
nz=41
ny=41
nx=41

# Create axes
z=np.linspace(0,zmax,nz)
y=np.linspace(0,ymax,ny)
x=np.linspace(0,xmax,nx)

# Create mesh
Z,Y,X=np.meshgrid(z,y,x)

# Set max number of timesteps
nt=100

# Set air velocity
airspeedz=np.ones((nz,ny,nx))*0
airspeedy=np.ones((nz,ny,nx))*0
airspeedx=np.ones((nz,ny,nx))*0.5

# Initialize deposited mass matrices
deposited_mass_zmin=np.zeros((ny,nx))
deposited_mass_zmax=np.zeros((ny,nx))
deposited_mass_ymin=np.zeros((nz,nx))
deposited_mass_ymax=np.zeros((nz,nx))
deposited_mass_xmin=np.zeros((nz,ny))
deposited_mass_xmax=np.zeros((nz,ny))

# Initialize droplet trajectory
L_drop=np.zeros((number_drops,nt,3)) # Location vector z y x metres
V_drop=np.zeros((number_drops,nt,3)) # Velocity vector w v u m/s

def cdlb(re): # Drag coefficient function 
    cdlb=(24/re)*(1+0.197*(re**0.63)+0.00026*(re**1.38)) #Langmuir and Blodgett 1964
    return cdlb

def cdcgw(re): # Drag coefficient function 
    cdcgw=(24/re)*(1+0.15*(re**0.687))+0.42/(1+42500*(re**(-1.16))) #Clift, Grace and Weber 1978. Valid at Re<3e5. Verified.
    return cdcgw

#Plot a graph og Cd vs Re for the two Cd functions defined
##logre=np.linspace(1, 80, 80)
##re=10**(logre/10)
##cd_arraylb=cdlb(re)
##cd_arraycgw=cdcgw(re)
##
##lb=plt.plot(re,cd_arraylb, label='Langmuir & Blodgett 1964')
##cgw=plt.plot(re,cd_arraycgw, label='Clift, Grace & Weber 1978')
##plt.xscale("log")
##plt.yscale("linear")
##plt.legend()
##plt.xlabel('Re')
##plt.ylabel('Cd')
##plt.show(block=False)

max_i=np.zeros(number_drops) # this records the last timestep for which droplet position and velocity data is recorded (the last timestep before it leaves the domain)

for j in range (0, number_drops): # Loop over all droplets
    L_drop[j,0,:]=L_nozzle    # Start droplet trajectory at nozzle
    theta=(np.pi/180)*np.random.uniform(0,cone_angle/2) # Determine droplet direction: sample an angular range within the nozzle cone angle
    phi=(np.pi/180)*np.random.uniform(0,360) # Determine droplet direction: sample a 360 degree range
    V_drop[j,0,2]=speed_nozzle*np.sin(theta)*np.sin(phi) # x cpt of droplet velocity 
    V_drop[j,0,1]=speed_nozzle*np.sin(theta)*np.cos(phi) # y cpt of droplet velocity
    V_drop[j,0,0]=np.sign(speed_nozzle)*np.sqrt(speed_nozzle**2-V_drop[j,0,1]**2-V_drop[j,0,2]**2) # z cpt of droplet velocity; this line ensures droplet speed is the same from all drops
    i=0
    while L_drop[j,i,0]>0 and L_drop[j,i,0]<(zmax*(nz+1)/nz) and L_drop[j,i,1]>0 and L_drop[j,i,1]<(ymax*(ny+1)/ny) and L_drop[j,i,2]>0 and L_drop[j,i,2]<(xmax*(nx+1)/nx) and i<nt-1: # while droplet is in domain
        # Find z y x indices of cell in which droplet is located
        z_cell=int(np.floor(L_drop[j,i,0]*nz/zmax))-1
        y_cell=int(np.floor(L_drop[j,i,1]*ny/ymax))-1
        x_cell=int(np.floor(L_drop[j,i,2]*nx/xmax))-1
        # find local air velocity in this cell
        local_air_velocity=[airspeedz[z_cell,y_cell,x_cell],airspeedy[z_cell,y_cell,x_cell],airspeedx[z_cell,y_cell,x_cell]]
        #print(z_cell, y_cell, x_cell,local_air_velocity, '\n')
        #print(L_drop[j,i,0], L_drop[j,i,1], L_drop[j,i,2], L_drop[j,i+1,0], L_drop[j,i+1,1], L_drop[j,i+1,2], '\n')
        V_drop_relative=V_drop[j,i,:]-local_air_velocity # Velocity of droplet relative to air
        V_drop_relative_mag=np.linalg.norm(V_drop_relative) # Magnitude of droplet-air relative velocity
        Re_drop=d_drop*V_drop_relative_mag/viscosity_air # Droplet Reynolds number
        Cd_drop=cdcgw(Re_drop) # Droplet drag coefficient
        drag_relax_time=np.abs(4*d_drop*density_liquid/(3*Cd_drop*density_air*V_drop_relative_mag)) # Droplet drag relaxation time (drag characteristic time)
        grav_relax_time=np.abs(np.dot(g,V_drop[j,i,:])/(np.linalg.norm(g))**2) # Characteristic time of gravitational acceleration
        inv_z_transit_time=np.abs((V_drop[j,i,0]*nz)/zmax) # 1/characteristic time to transit 1 cell in the z direction
        inv_y_transit_time=np.abs((V_drop[j,i,1]*ny)/ymax) # 1/characteristic time to transit 1 cell in the y direction
        inv_x_transit_time=np.abs((V_drop[j,i,2]*nx)/xmax) # 1/characteristic time to transit 1 cell in the x direction
        inv_cell_transit_time=np.amax([inv_z_transit_time,inv_y_transit_time,inv_x_transit_time])
        #if drag_relax_time<0.01:
        #inverse_times=[1/drag_relax_time,1/grav_relax_time,inv_cell_transit_time]
        #else:
        #    inverse_times=[1/grav_relax_time,inv_cell_transit_time]
        inverse_times=[inv_cell_transit_time]
        #need drag relax time and grav relax time?
        dt=1/np.amax(inverse_times) # Choose as the timestep the shortest of the characteristic times calculated above. Currently the drag time and grav time are excluded. Inverses are used to avoid divide by zeros.
        #print(dt, drag_relax_time, grav_relax_time)
        #print(drag_relax_time,grav_relax_time,1/inv_z_transit_time,1/inv_y_transit_time,1/inv_x_transit_time, dt, '\n')
        #V_drop_rel(t+dt)=V_drop_rel(t)*exp(-dt/tau)
        #V_drop[j,i+1,:]=V_drop[j,i,:]+V_drop_relative*(np.exp(-dt/drag_relax_time)-1)+g*dt
        V_drop[j,i+1,:]=V_drop_relative*(np.exp(-dt/drag_relax_time))+local_air_velocity+g*dt # Update droplet velocity, using relaxation to local air velocity (the exponential is a solution to the droplet aerodynamic equation of motion) and acceleration due to gravity
        L_drop[j,i+1,:]=L_drop[j,i,:]+V_drop[j,i,:]*dt # Update droplet position
        #print(V_drop[i,:],V_drop_mag,Cd_drop, accel_drag_drop,'\n')
        i+=1

    max_i[j]=np.max(np.where(L_drop[j,:,:])) # Find index of last nonzero droplet location. This is the last location generated before the droplet exited the domain.
    max_index=int(max_i[j])
    if L_drop[j,max_index,0]<zmax/nz: # If droplet strikes the ground plane
            y_hit=int(L_drop[j,max_index,1]*ny/ymax)-1 # Find coordinates at which droplet strikes the ground
            x_hit=int(L_drop[j,max_index,2]*nx/xmax)-1
            deposited_mass_zmin[y_hit,x_hit]=+density_liquid*np.pi*(d_drop**3)/6 # Add droplet mass to the total mass deposited at this location
            #print(y_hit,x_hit)
  
#print(max_i[0])
fig = plt.figure() # Plot droplet trajectories
ax=plt.axes(projection='3d')
ax.set_xlim([0,xmax])
ax.set_ylim([0,ymax])
ax.set_zlim([0,zmax])
ax.set_xlabel('X')
ax.set_ylabel('Y');
ax.set_zlabel('Z');
ax.set_title('Trajectories of droplets')
#ax = fig.add_subplot(111, projection='3d')
#Axes3D.plot(L_drop[:,2], L_drop[:,1], L_drop[:,0])
for j in range (0, number_drops):
    max_index=int(max_i[j])
    #print(max_index)
    ax.plot3D(L_drop[j,0:max_index,2], L_drop[j,0:max_index,1], L_drop[j,0:max_index,0])
plt.show(block=False)

fig = plt.figure() # Plot mass deposited on ground
plt.contourf(x, y, deposited_mass_zmin)
plt.colorbar()
plt.xlabel('X')
plt.ylabel('Y');
plt.show(block=False)
