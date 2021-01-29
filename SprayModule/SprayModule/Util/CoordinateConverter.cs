using System;

namespace SprayModule.Util
{
    /// <summary>
    /// Class to facilitate the conversion of the different possible input coordinate systems
    /// </summary>
    public class CoordinateConverter
    {
        /// <summary>
        /// Function to convert NZTM coordiantes to latitude/longitude coordinates (WGS84)
        /// Based off: https://www.linz.govt.nz/data/geodetic-services/coordinate-conversion/projection-conversions/transverse-mercator-transformation-formulae
        /// </summary>
        /// <param name="E">The east NZTM coordinate</param>
        /// <param name="N">The north NZTM cooridnate</param>
        /// <returns>A tuple of (latitude, longitude) both as doubles</returns>
        public (double, double) NZTMtoLatLng(double E, double N) 
        {
            // TODO check for / catch division by zero 
            //Common variables for NZTM2000
            double a = 6378137;
            var f = 1/298.257222101;
            double phizero = 0;
            double lambdazero = 173;
            double Nzero = 10000000;
            double Ezero = 1600000;
            var kzero = 0.9996;            

            //input Northing(Y); Easting(X) variables
            // double N       = 5427502.0;
            // double E       = 1749165.0;

            //Calculation: From NZTM to lat/Long
            var b = a * (1 - f);
            var esq = 2 * f - Math.Pow(f, 2);
            var Z0 = 1 - esq / 4 - 3 * Math.Pow(esq, 2) / 64 - 5 * Math.Pow(esq, 3) / 256;
            var A2 = 0.375 * (esq + Math.Pow(esq, 2) / 4 + 15 * Math.Pow(esq, 3) / 128);
            var A4 = 15 * (Math.Pow(esq, 2) + 3 * Math.Pow(esq, 3) / 4) / 256;
            var A6 = 35 * Math.Pow(esq, 3) / 3072;

            var Nprime = N - Nzero;
            var mprime = Nprime / kzero;
            var smn = (a - b) / (a + b);
            var G = a * (1 - smn) * (1 - Math.Pow(smn, 2)) * (1 + 9 * Math.Pow(smn, 2) / 4 + 225 * Math.Pow(smn, 4) / 64) * Math.PI/ 180.0;
            var sigma = mprime * Math.PI / (180 * G);
            var phiprime = sigma + (3 * smn / 2 - 27 * Math.Pow(smn, 3) / 32) * Math.Sin(2 * sigma) + (21 * Math.Pow(smn, 2) / 16 - 55 * Math.Pow(smn, 4) / 32) * Math.Sin(4 * sigma) + (151 * Math.Pow(smn, 3) / 96) * Math.Sin(6 * sigma) + (1097 * Math.Pow(smn, 4) / 512) *Math.Sin(8 * sigma);
            var rhoprime = a * (1 - esq) / Math.Pow((1 - esq * Math.Pow((Math.Sin(phiprime)), 2)), 1.5);
            var upsilonprime = a / Math.Sqrt(1 - esq * Math.Pow((Math.Sin(phiprime)), 2));

            var psiprime = upsilonprime / rhoprime;
            var tprime = Math.Tan(phiprime);
            var Eprime = E - Ezero;
            var chi = Eprime / (kzero * upsilonprime);
            var term_1 = tprime * Eprime * chi / (kzero * rhoprime * 2);
            var term_2 = term_1 * Math.Pow(chi, 2) / 12 * (-4 * Math.Pow(psiprime, 2) + 9 * psiprime * (1 - Math.Pow(tprime, 2)) + 12 * Math.Pow(tprime, 2));
            var term_3 = tprime * Eprime * Math.Pow(chi, 5) / (kzero * rhoprime * 720) * (8 * Math.Pow(psiprime, 4) * (11 - 24 * Math.Pow(tprime, 2)) - 12 * Math.Pow(psiprime, 3) * (21 - 71 * Math.Pow(tprime, 2)) + 15 * Math.Pow(psiprime, 2) * (15 - 98 * Math.Pow(tprime, 2) + 15 * Math.Pow(tprime, 4)) + 180 * psiprime * (5 * Math.Pow(tprime, 2) - 3 * Math.Pow(tprime, 4)) + 360 * Math.Pow(tprime, 4));
            var term_4 = tprime * Eprime * Math.Pow(chi ,7) / (kzero * rhoprime * 40320) * (1385 + 3633 * Math.Pow(tprime, 2) + 4095 * Math.Pow(tprime, 4) + 1575 * Math.Pow(tprime, 6));
            var term1 = chi * (1 / Math.Cos(phiprime));
            var term2 = Math.Pow(chi, 3) * (1 / Math.Cos(phiprime)) / 6 * (psiprime + 2 * Math.Pow(tprime, 2));
            var term3 = Math.Pow(chi, 5) * (1 / Math.Cos(phiprime)) / 120 * (-4 * Math.Pow(psiprime, 3) * (1 - 6 * Math.Pow(tprime, 2)) + Math.Pow(psiprime, 2) * (9 - 68 * Math.Pow(tprime, 2)) + 72 * psiprime * Math.Pow(tprime, 2) + 24 * Math.Pow(tprime, 4));
            var term4 = Math.Pow(chi, 7) * (1 / Math.Cos(phiprime)) / 5040 * (61 + 662 * Math.Pow(tprime, 2) + 1320 * Math.Pow(tprime, 4) + 720 * Math.Pow(tprime, 6));

            var latitude = (phiprime - term_1 + term_2 - term_3 + term_4) * 180 / Math.PI;
            var longitude = lambdazero + 180 / Math.PI * (term1 - term2 + term3 - term4);

            return (latitude, longitude);
        }
    }
}