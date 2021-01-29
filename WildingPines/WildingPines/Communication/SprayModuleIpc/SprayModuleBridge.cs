using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WildingPines.Communication.Sockets;
using WildingPines.Models;

namespace WildingPines.Communication.SprayModuleIpc
{
    /// <inheritdoc cref="ISprayModuleBridge"/>
    public class SprayModuleBridge : ISprayModuleBridge
    {
        private readonly ILogger<SprayModuleBridge> _logger;
        private readonly IUdpSocket _udpSocket = new UdpSocket(60606, 54321, "localhost");

        private bool _hasInitialisedAvailableNozzles = false;
        private List<Nozzle> _nozzles = new List<Nozzle>();

        private const string GetNozzlesTag = "getnumbernozzles";
        private const string NumberOfNozzlesTag = "numberofnozzles:";
        private const string UpdateNozzlesTag = "testnozzles:";
        private const string SprayStateFlyTag = "updatestate:engagedeathmode";
        private const string SprayStateLandTag = "updatestate:disengagedeathmode";
        private const string SprayStateFlyReply = "updatestate:deathmodeengaged";
        private const string SprayStateLandReply = "updatestate:deathmodedisengaged";

        public SprayModuleBridge(ILogger<SprayModuleBridge> logger)
        {
            _logger = logger;
        }
        
        public async Task<List<Nozzle>> GetAvailableNozzlesAsync()
        {
            if (!_hasInitialisedAvailableNozzles)
            {
                // NOTE: remove this when using sockets.
                // _nozzles = CreateNozzles(3);
                // _hasInitialisedAvailableNozzles = true;
                // return _nozzles;
                await _udpSocket.SendAsync(GetNozzlesTag);
                var response = await _udpSocket.ReceiveAsync();
                var numberOfNozzles = GetNumberOfNozzlesFromResponse(response);
                _logger.Log(LogLevel.Information, $"Answer from SprayModule socket: {response}");
                _logger.Log(LogLevel.Information, $"Number of nozzles from spray module: {numberOfNozzles}");
                _nozzles = CreateNozzles(numberOfNozzles);
                _hasInitialisedAvailableNozzles = true;
            }

            return _nozzles;
        }
        
        public async Task UpdateNozzleStatusesAsync(List<Nozzle> patchedNozzles)
        {
            // NOTE: remove this when using sockets.
            // _nozzles = patchedNozzles;
            // return;
            var encodedNozzleStatuses = EncodeNozzlesUpdate(patchedNozzles);
            await _udpSocket.SendAsync($"{UpdateNozzlesTag}{encodedNozzleStatuses}");
            var sprayModuleResponse = await _udpSocket.ReceiveAsync();
            Console.Out.WriteLine($"Answer from SprayModule socket: {sprayModuleResponse}");
            _nozzles = patchedNozzles;
        }
        
        public async Task<bool> SetSprayModuleToFlyingState(string sprayPlanFilePath)
        {
            var message = $"{SprayStateFlyTag}:{sprayPlanFilePath}";
            _logger.Log(LogLevel.Information,$"Sending message to spray module: {message}");
            await _udpSocket.SendAsync(message);
            var sprayModuleResponse = await _udpSocket.ReceiveAsync();
            return sprayModuleResponse.Equals(SprayStateFlyReply);
        }

        public async Task<bool> SetSprayModuleToLandedState()
        {
            await _udpSocket.SendAsync(SprayStateLandTag);
            var sprayModuleResponse = await _udpSocket.ReceiveAsync();
            return sprayModuleResponse.Equals(SprayStateLandReply);
        }

        /// <summary>
        /// Attempts to parse a response to return the number of nozzles available.
        /// </summary>
        /// <param name="response"></param>
        /// <returns>The number of nozzles available in the spray module, or 0 if could not parse the response.</returns>
        private int GetNumberOfNozzlesFromResponse(string response)
        {
            var trimmed = response.Trim();
            if (!trimmed.StartsWith(NumberOfNozzlesTag)) return 0;
            var numberOfNozzles = trimmed.Split(NumberOfNozzlesTag)[1];
            return int.TryParse(numberOfNozzles, out var parsedNumberOfNozzles) ? parsedNumberOfNozzles : 0;
        }

        /// <summary>
        /// Creates nozzles to return to the API client based on the number of nozzles reported by the spray module.
        /// </summary>
        /// <param name="numberOfNozzles">the number of nozzles reported by the spray module</param>
        /// <returns></returns>
        private List<Nozzle> CreateNozzles(int numberOfNozzles)
        {
            return Enumerable.Range(1, numberOfNozzles)
                .Select(i => new Nozzle(i, false, $"Nozzle {i}"))
                .ToList();
        }

        /// <summary>
        /// Given a list of updated nozzles from the API client, construct a string encoding the new status for the
        /// nozzles.
        /// </summary>
        /// <param name="nozzles">the nozzles with the new status that we wish to send to the spray module.</param>
        /// <returns>
        /// The update string that will be sent to the spray module, e.g. "000100" would indicate to turn all
        /// off but the third one, which should be turned on. 
        /// </returns>
        private string EncodeNozzlesUpdate(List<Nozzle> nozzles)
        {
            nozzles.Sort((nozzleA, nozzleB) => nozzleA.Id.CompareTo(nozzleB.Id));
            return String.Join("", nozzles.Select(nozzle => nozzle.ShouldBeSpraying ? 1 : 0));
        }
    }
}