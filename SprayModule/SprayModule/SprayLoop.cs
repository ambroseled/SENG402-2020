using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SprayModule.Communication.ApiServerIpc;
using SprayModule.Communication.BoomControllerIpc;
using SprayModule.Communication.GpsIpc;
using SprayModule.External.SprayModel;
using SprayModule.Model;
using SprayModule.Util.Reader;

namespace SprayModule
{
    /// <summary>
    /// Main spray loop. Handles communication with external processes and devices via sockets.
    /// </summary>
    public class SprayLoop
    {
        private const string NumNozzlesTag = "getnumbernozzles";
        private const string TestNozzlesTag = "testnozzles:";
        private const string SprayStateFlyTag = "updatestate:engagedeathmode:";
        private const string SprayStateLandTag = "updatestate:disengagedeathmode";

        private const int NumberOfNozzles = 5;

        private bool _onGround = true;
        private readonly IBoomControllerBridge _boomControllerBridge = new BoomControllerBridge();
        private readonly IApiServerBridge _apiServerBridge = new ApiServerBridge();
        private readonly IGpsBridge _gpsBridge = new GpsBridge();
        
        private readonly PythonRunner _runner = new PythonRunner();
        
        private string _msgToBoom;
        private GpsLocation _currentLocation;
        private double _currentBearing;
        private string _filePath;
        
        /// <summary>
        /// Run the spray module forever.
        /// </summary>
        /// <returns></returns>
        public async Task RunSprayModule()
        {
            var reader = new CsvFileReader();
            var treeLocations = new List<(double, double)>();
            while (true)
            {
                if (_apiServerBridge.HasUnreadMessage)
                {
                    var serverMsg = await _apiServerBridge.ReceiveMessageAsync();
                    await Console.Out.WriteLineAsync($"Received via socket: {serverMsg}");
                    _filePath = await ProcessServerMessage(serverMsg);
                    if (_filePath != null)
                    {
                        treeLocations = reader.GetDemoTreeLocations(_filePath);
                    }
                }
                
                if (_onGround) continue;
                if (treeLocations.Count == 0) continue; // TODO this may need to through an error

                if (_gpsBridge.HasUnreadData)
                {
                    (_currentLocation, _currentBearing) = await _gpsBridge.GetNextGpsLocation();
                }

                if (_currentLocation == null) continue;
                _msgToBoom = _runner.RunDemoSprayModel(treeLocations, _currentLocation.Latitude,
                    _currentLocation.Longitude, _currentBearing);
                await _boomControllerBridge.UpdateNozzleStatuses(_msgToBoom);
            }
        }

        /// <summary>
        /// Determine what the server wants, communicate with external components and respond to server.
        /// </summary>
        /// <param name="serverMsg"></param>
        /// <returns></returns>
        private async Task<string> ProcessServerMessage(string serverMsg)
        {
            if (serverMsg.Equals(NumNozzlesTag))
            {
                await _apiServerBridge.SendNumberOfAvailableNozzlesAsync(NumberOfNozzles);
            } else if (serverMsg.StartsWith(TestNozzlesTag))
            {
                var nozzles = serverMsg.Substring(TestNozzlesTag.Length);
                await _boomControllerBridge.UpdateNozzleStatuses(nozzles);
                await _apiServerBridge.SendConfirmationOfNozzlesTestAsync();
            } else if (serverMsg.StartsWith(SprayStateFlyTag))
            {
                _onGround = false;
                await _apiServerBridge.SendConfirmationOfChangingStateToFlyingAsync();
                var attributes = serverMsg.Split(":");
                var filePath = attributes[2]; // TODO maybe need validation??
                return filePath;
            } else if (serverMsg.Equals(SprayStateLandTag))
            {
                _onGround = true;
                await _apiServerBridge.SendConfirmationOfChangingStateToLandingAsync();
            }

            return null;
        }
    }
}