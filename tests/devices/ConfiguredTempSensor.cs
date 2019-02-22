using NUnit.Framework;
using rpi_dotnet;
using Moq;
using System;
using System.Collections.Generic;

namespace Tests
{
    public class ConfiguredTempSensorTest
    {
        [Test]
        public void ShouldInitDevice()
        {
            var mock = new Mock<IOneWire>();
            mock.Setup(w1 => w1.getDevice("TestUUID")).Returns(new DS18B20("TestUUID"));

            var device = new ConfiguredTempSensor("TestUUID", "testSpace", mock.Object);
            var measureValue = device.FindDevice();

            Assert.AreEqual(true, measureValue);
        }
        [Test]
        public void ShouldNotFoundDevice()
        {
            var mock = new Mock<IOneWire>();
            mock.Setup(w1 => w1.getDevice("TestUUID")).Returns<IOneWireDevice>(null);

            var device = new ConfiguredTempSensor("TestUUID", "testSpace", mock.Object);
            var measureValue = device.FindDevice();
            
            Assert.AreEqual(false, measureValue);
        }
    }
}
