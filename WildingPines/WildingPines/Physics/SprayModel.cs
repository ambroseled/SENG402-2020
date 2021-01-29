using System;
using System.Collections.Generic;
using NumSharp;

namespace WildingPines.Physics
{
    public class SprayModel
    {
        // Constants
        private const int AIR_DENSITY = 1;
        private const int SPRAY_DENSITY = 1000;
        private const double AIR_VISCOCITY = 1.789e-5;
        private NDArray gravity = np.array(-9.81, 0, 0);
        private const int NR = 3; // What is this?

        //Inputs
        private int heliRadius;
        private double heliSpeed;
        private double initialDropletSpeed;
        private double heliHeight; // Is this height above ground or tree?
        private double heliWeight;
        private double qSquared;


        private List<(double, double)> domain;
        private Dictionary<string, double> values =
            new Dictionary<string, double>();
        

        /// <summary>
        /// Constructer for spray model
        /// </summary>
        /// <param name="heliRadius">Radius of the helicopter rotors</param>
        /// <param name="heliSpeed">Current helicopter speed</param>
        /// <param name="initialDropletSpeed">Initial speed of a spray droplet</param>
        /// <param name="heliHeight">The height of the helicopter</param>
        /// <param name="heliWeight">Weight of the helicopter, including spray and fuel etc</param>
        /// <param name="qSquared">Square root of mean squared turbulence</param>
        public SprayModel(int heliRadius, double heliSpeed,
            double initialDropletSpeed, double heliHeight, double heliWeight,
            double qSquared)
        {
            this.heliRadius = heliRadius;
            this.heliSpeed = heliSpeed;
            this.initialDropletSpeed = initialDropletSpeed;
            this.heliHeight = heliHeight;
            this.heliWeight = heliWeight;
            this.qSquared = qSquared;
            domain = new List<(double, double)>
            {
                (-NR * heliRadius, NR * heliRadius),
                (1e-12, heliHeight)
            };
        }


        /// <summary>
        /// Method to make and return a grid of size n*n*n where n is gridSize
        /// </summary>
        /// <param name="gridSize">The size of the grid</param>
        /// <returns>A lsit of NDArrays for x, y, z</returns>
        private List<NDArray> getCoordinates(int gridSize)
        {
            (NDArray, NDArray) x_y = np.meshgrid(np.linspace(domain[0].Item1,
                domain[0].Item2, gridSize),
                np.linspace(domain[0].Item1, domain[0].Item2, gridSize)
              //  np.linspace(domain[1].Item1, domain[1].Item2, gridSize),
                );
            NDArray x = x_y.Item1;
            NDArray y = x_y.Item2;
            NDArray z = np.sin((Math.Pow(x, 2) + Math.Pow(y, 2)) /
                (Math.Pow(x, 2) + Math.Pow(y, 2)));
            // Based off of https://stackoverflow.com/questions/36013063/what-is-the-purpose-of-meshgrid-in-python-numpy
            // Need to check if this is correct / ok
            return new List<NDArray> { x, y, z };
        }


        /// <summary>
        /// Method to calculate the values for the helicopter down wash
        /// </summary>
        /// <returns>List of double values for the down wash</returns>
        private void calculateDownWashValues()
        {
            double w = Math.Sqrt(heliWeight / (2 * Math.PI * AIR_DENSITY *
                Math.Pow(heliRadius, 2)));
            values.Add("w", w);
        }

        /// <summary>
        /// Method to calculate the velocities of the downwash field
        /// </summary>
        /// <param name="downWashValues">Down wash values</param>
        /// <param name="x"></param> // Not sure what these actually are
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="gridSize">The size of the grid</param>
        /// <returns></returns>
        private List<double> getDownWashSpeedMatrixes(NDArray x, NDArray y,
            NDArray z, int gridSize)
        {
            double constant = values["w"] / (2 * heliHeight); // What is this constant / what could i name it to be descriptive?

            double xVelocity = constant * np.sqrt(Math.Pow(x, 2) +
                Math.Pow(y, 2)) * np.cos(np.arctan2(y, x));
            double yVelocity = constant * np.sqrt(Math.Pow(x, 2) +
                Math.Pow(y, 2)) * np.sin(np.arctan2(y, x));
            double zVelocity = (-values["w"]) * z / heliHeight;

            return new List<double> { xVelocity, yVelocity, zVelocity };
        }

        /// <summary>
        /// Method to calculate the values of the vortex field
        /// </summary>
        /// <returns>tuple of rotor tip vorticity and core radius vortex</returns>
        private void calculateVortexValues()
        {
            // TODO this need the actual equations in it
            values.Add("R", 10);
            values.Add("pc", 1);
        }

        /// <summary>
        /// Converts points from cartesian to polar coordinates
        /// </summary>
        /// <param name="x">NDArray for x coordinate</param>
        /// <param name="y">NDArray for y coordinate</param>
        /// <param name="z">NDArray for z coordinate</param>
        /// <returns>List of NDArrays of order {rho, theta, phi, x, y, z}</returns>
        private List<NDArray> cartesianToPolar(NDArray x, NDArray y, NDArray z) // What is a better name for this?
        {
            // Horizontal angle in 3d spherical coordinates
            NDArray phi = np.arctan2(y, x);

            // Coordinates of vortex ring line in same plane as point
            NDArray xR = heliRadius * np.cos(phi);
            NDArray yR = heliRadius * np.sin(phi);
            NDArray zR = heliRadius;

            // Coordinate of point form corresponding point on ring
            NDArray xCoord = x - xR;
            NDArray yCoord = y - yR;
            NDArray zCoord = z - zR;

            // Radial location from corresponding point on ring
            NDArray rho = np.sqrt(Math.Pow(xCoord, 2) + Math.Pow(yCoord, 2)
                + Math.Pow(zCoord, 2));
            // Vertical angle from corresponding point on ring
            NDArray theta = np.arctan2(zCoord, np.sqrt(Math.Pow(xCoord, 2)
                + Math.Pow(yCoord, 2)));

            return new List<NDArray>
            {
                rho, theta, phi, xCoord, yCoord, zCoord
            };
        }

        /// <summary>
        /// Method to get the vortex speed matrixes
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="z">Z coordinate</param>
        /// <param name="xRing">X ring coordinate</param>
        /// <param name="yRing">Y ring coordinate</param>
        /// <param name="zRing">Z ring coordinate</param>
        /// <param name="rho">rho value</param>
        /// <param name="theta">theta value</param>
        /// <param name="phi">phi value</param>
        /// <returns></returns>
        private List<NDArray> getVortexSpeedMatrixes(NDArray x, NDArray y,
            NDArray z, NDArray xRing, NDArray yRing, NDArray zRing, NDArray rho,
            NDArray theta, NDArray phi)
        {
            // Velocity of air in polar coordinates
            int vR = 0; // This is unused in Python
            double vTheta = (-heliRadius) / (2 * Math.PI * rho) * (1 -
                np.exp(Math.Pow(-rho, 2) / Math.Pow(values["pc"], 2)));
            double vPhi = 0; // This is also unused

            // Convert velocities to carrtesian
            NDArray vX = vTheta * np.sin(theta) * np.cos(phi);
            NDArray vY = vTheta * np.sin(theta) * np.sin(phi);
            NDArray vZ = vTheta * np.cos(theta);

            // TODO: this can probably be done more effeciently
            for (int k = 0; k < z.size; k++)
            {
                for (int j = 0; j < y.size; j++)
                {
                    for (int i = 0; i < x.size; i++)
                    {
                        if ((Math.Pow(x[k][j][i], 2) + Math.Pow(y[k][j][i], 2)) >
                            (Math.Pow(xRing[k][j][i], 2) +
                            Math.Pow(yRing[k][j][i], 2)))
                        {
                            vZ[k][j][i] = -1 * vZ[k][j][i];
                        }
                    }
                }
            }
            return new List<NDArray>
            {
                vX, vY, vZ
            };
        }

        /// <summary>
        /// Method to calculate the dividing streamline of the model
        /// </summary>
        /// <param name="n"></param> I don't know what this is
        /// <param name="vX">X velocity</param>
        /// <param name="vY">Y velocity</param>
        /// <param name="vZ">Z velocity</param>
        /// <returns></returns>
        private List<NDArray> getDividingStreamline(int n, NDArray vX,
            NDArray vY, NDArray vZ)
        {
            n = n / NR;

            List<double> z = new List<double>();

            NDArray radius = np.linspace(heliRadius, domain[0].Item2, n);
            // Im not sure what the next lines are doing or how to convert
            return new List<NDArray>();
        }
    }
}
