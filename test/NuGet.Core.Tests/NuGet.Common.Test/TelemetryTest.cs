using System;
using System.Collections.Generic;
using System.Threading;
using Moq;
using Xunit;

namespace NuGet.Common.Test
{
    public class TelemetryTest
    {
        [Fact]
        public void TelemetryTest_TelemetryActivity()
        {
            // Arrange
            var telemetryService = new Mock<INuGetTelemetryService>();
            TelemetryEvent telemetryEvent = null;
            telemetryService.Setup(x => x.EmitTelemetryEvent(It.IsAny<TelemetryEvent>()))
                .Callback<TelemetryEvent>(x => telemetryEvent = x);
            var guid = Guid.NewGuid();
            TelemetryActivity.NuGetTelemetryService = telemetryService.Object;

            // Act
            using (var telemetry = new TelemetryActivity(new KeyValuePair<string, Guid>("operationId", guid)))
            {
                // Wait for 5 seconds
                Thread.Sleep(5000);
                telemetry.TelemetryEvent = new TelemetryEvent("testEvent", new Dictionary<string, object>());
            }

            // Assert
            Assert.NotNull(telemetryEvent["StartTime"]);
            Assert.NotNull(telemetryEvent["EndTime"]);
            Assert.Equal(5, Convert.ToInt32(telemetryEvent["Duration"]));
        }
    }
}
