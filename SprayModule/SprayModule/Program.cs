using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SprayModule.External.SprayModel;
using SprayModule.Util.Reader;

namespace SprayModule
{
    /// <summary>
    /// Entrypoint to the program.
    /// </summary>
    internal static class Program
    {
        static async Task Main(string[] args)
        {
            var sprayLoop = new SprayLoop();
            await sprayLoop.RunSprayModule();
        }
    }
}