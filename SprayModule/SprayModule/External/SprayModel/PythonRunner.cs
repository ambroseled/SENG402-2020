using System;
using System.Collections.Generic;
using System.Diagnostics;
using NumSharp;
using SprayModule.Exception;

namespace SprayModule.External.SprayModel
{
    /// <summary>
    /// Class to act as a wrapper on the Python based physics model
    /// </summary>
    public class PythonRunner
    {
        /// <summary>
        /// Proof of concept method to check if running a python script from C# was feasible 
        /// </summary>
        [Obsolete]
        public void PocRunner() {
            var start = new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = "/home/cosc/student/ajl190/Desktop/program.py 1",
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            using var process = Process.Start(start);
            if (process == null) return;
            using var reader = process.StandardOutput;
            var result = reader.ReadToEnd();
            if (result.Equals("1\n"))
            {
                Console.WriteLine("Spray");
            } else if (result.Equals("0\n"))
            {
                Console.WriteLine("No Spray");
            } else
            {
                Console.WriteLine("Error");
            }
        }

        /// <summary>
        /// Method to run the python physics model with the current helicopter data to determine which nozzles to spray with 
        /// </summary>
        /// <param name="localSprayMatrix">NDArray of the relative spray locations to the helicopter position</param>
        /// <param name="helicopterIndex">Tuple of the index of the helicopter in the sprayMatrix</param>
        /// <param name="helicopterAngle">The bearing of the helicopter</param>
        /// <exception cref="PythonSprayModelException">Exception thrown when a null process object is found</exception>
        /// <returns>String of the output of the python script</returns>
        public string RunSprayModel(NDArray localSprayMatrix, (int, int) helicopterIndex, double helicopterAngle, 
            double windSpeed)
        {
            var start = new ProcessStartInfo
            {
                FileName = "python3",
                // Arguments = $"/Users/ambroseledbrook/Desktop/Repos/SENG402/wilding-pines-fyp/test_helicopter_spray_model.py " +
                //             $"\"{localSprayMatrix.ToString().Replace("\n", "")}\" {helicopterIndex.Item1} " +
                //             $"{helicopterIndex.Item2} {helicopterAngle} {windSpeed}",
                Arguments = $"/home/cosc/student/ajl190/Desktop/wilding-pines-fyp/test_helicopter_spray_model.py " +
                            $"\"{localSprayMatrix.ToString().Replace("\n", "")}\" {helicopterIndex.Item1} " +
                            $"{helicopterIndex.Item2} {helicopterAngle} {windSpeed}",
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            using var process = Process.Start(start);
            if (process == null) throw new PythonSprayModelException("Null process object");
            using var reader = process.StandardOutput;
            var result = reader.ReadToEnd();
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="treeLocations"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="yaw"></param>
        /// <returns></returns>
        public string RunDemoSprayModel(List<(double, double)> treeLocations, double latitude, double longitude, double yaw)
        {
            var start = new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"/home/doc/Desktop/wilding-pines-fyp/spray_model/demo_spray_model.py " +
                            $"\"{string.Join(",", treeLocations)}\" {latitude} " +
                            $"{longitude} {yaw}",
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            using var process = Process.Start(start);
            if (process == null) throw new PythonSprayModelException("Null process object");
            using var reader = process.StandardOutput;
            var result = reader.ReadToEnd();
            return result;
        }
    }
}
