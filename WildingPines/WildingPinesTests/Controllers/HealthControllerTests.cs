using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using WildingPines.Controllers;
using Xunit;

namespace WildingPinesTests.Controllers
{
    public class HealthControllerTests : IDisposable
    {
        private readonly HealthController _healthController;
        
        public HealthControllerTests()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();
            var factory = serviceProvider.GetService<ILoggerFactory>();
            var logger = factory.CreateLogger<HealthController>();

            _healthController = new HealthController(logger);
        }
        
        public void Dispose()
        {
            _healthController.Dispose();
        }

        [Fact]
        public void ResponseIsADatetimeTimestamp()
        {
            Assert.IsType<DateTime>(_healthController.GetHealthCheck().Timestamp);
        }

        [Fact]
        public void ResponseTimestampIsRecent()
        {
            var responseTimestamp = _healthController.GetHealthCheck().Timestamp;
            var now = DateTime.Now;
            var difference = now.Subtract(responseTimestamp);
            Assert.InRange(difference.TotalSeconds, 0, 3);
        }
    }
}