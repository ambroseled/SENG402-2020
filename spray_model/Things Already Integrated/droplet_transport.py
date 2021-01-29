import numpy as np
from matplotlib import pyplot as plt
from mpl_toolkits.mplot3d import Axes3D


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

L_nozzle=[zmax, ymax/2, xmax/2]

d=100 #microns
#speed=10 #initial speed
V_nozzle=np.array([-10,1,2]) # z y x

dt=0.1 #s
nt=100

rho_liquid=1000 #kg/m**3
g=np.array([-9.81,0,0])

airspeedz=np.zeros((nz,ny,nx))
airspeedy=np.zeros((nz,ny,nx))
airspeedx=np.ones((nz,ny,nx))*10

L_drop=np.zeros((nt,3))
V_drop=np.zeros((nt,3))

#check both cd functions
#normalize direction?

def cdlb(re):
    cdlb=(24/re)*(1+0.197*(re**0.63)+0.00026*(re**1.38)) #Langmuir and Blodgett 1964)
    return cdlb

def cdcgw(re):
    cdcgw=(24/re)*(1+0.15*(re**0.687))+0.42/(1+0.000425*(re**(-1.16))) #Clift, Grace and Weber 1978
    return cdcgw

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

V_drop[0,:]=V_nozzle
L_drop[0,:]=L_nozzle

for i in range (1, nt):
    V_drop[i,:]=V_drop[i-1,:]+g*dt
    L_drop[i,:]=L_drop[i-1,:]+V_drop[i-1,:]*dt
   # print(V_drop)

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

