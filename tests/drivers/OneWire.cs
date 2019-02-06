using NUnit.Framework;
using rpi_dotnet;
using Moq;
using System.Collections.Generic;
namespace Tests
{
    public class OneWireTests
    {
        [Test]
        public void ShouldReturnNoDeviceWhenNogthingIsCOnnected()
        {
            var mock = new Mock<IFileWrapper>();
            mock.Setup(fileWrapper => fileWrapper.ListFiles("/sys/bus/w1/devices/")).Returns(new List<string> { });
            var bus = new OneWire(mock.Object);
            var devices = bus.getDevices();
            Assert.AreEqual(0, devices.Count);
            mock.Verify(fileWrapper => fileWrapper.ListFiles("/sys/bus/w1/devices/"), Times.Once());
        }

        [Test]
        public void ShouldReturnNoDeviceWhenOnlyBusIsConnected()
        {
            var mock = new Mock<IFileWrapper>();
            mock.Setup(fileWrapper => fileWrapper.ListFiles("/sys/bus/w1/devices/")).Returns(new List<string> { "w1_bus_master1" });
            var bus = new OneWire(mock.Object);
            var devices = bus.getDevices();
            Assert.AreEqual(0, devices.Count);
            mock.Verify(fileWrapper => fileWrapper.ListFiles("/sys/bus/w1/devices/"), Times.Once());
        }

        [Test]
        public void ShouldReturnDeviceWhenBusAndDeviceConnected()
        {
            var mock = new Mock<IFileWrapper>();
            mock.Setup(fileWrapper => fileWrapper.ListFiles("/sys/bus/w1/devices/"))
                .Returns(new List<string> { "w1_bus_master1", "28-000006cc02c9" });
            var bus = new OneWire(mock.Object);
            var devices = bus.getDevices();
            Assert.AreEqual(1, devices.Count);
            mock.Verify(fileWrapper => fileWrapper.ListFiles("/sys/bus/w1/devices/"), Times.Once());
        }

        [Test]
        public void ShouldReturnTwoDeviceWhenNoBusAndTwoDeviceConnected()
        {
            var mock = new Mock<IFileWrapper>();
            mock.Setup(fileWrapper => fileWrapper.ListFiles("/sys/bus/w1/devices/"))
                .Returns(new List<string> { "28-000006cc03c9", "28-000006cc02c9" });
            var bus = new OneWire(mock.Object);

            var devices = bus.getDevices();

            Assert.AreEqual(2, devices.Count);
            devices.ForEach((device) => {
                Assert.IsInstanceOf<DS18B20>(device);
            });
            mock.Verify(fileWrapper => fileWrapper.ListFiles("/sys/bus/w1/devices/"), Times.Once());
        }

        [Test]
        public void ShouldGetDeviceWhenNoBusAndTwoDeviceConnected()
        {
            var mock = new Mock<IFileWrapper>();
            mock.Setup(fileWrapper => fileWrapper.ListFiles("/sys/bus/w1/devices/"))
                .Returns(new List<string> { "28-000006cc03c9", "28-000006cc02c9" });
            var bus = new OneWire(mock.Object);

            var device = bus.getDevice("28-000006cc03c9");

            Assert.IsInstanceOf<DS18B20>(device);
            mock.Verify(fileWrapper => fileWrapper.ListFiles("/sys/bus/w1/devices/"), Times.Once());
        }

        [Test]
        public void ShouldNotReturnDeviceWithIncorrectName()
        {
            var mock = new Mock<IFileWrapper>();
            mock.Setup(fileWrapper => fileWrapper.ListFiles("/sys/bus/w1/devices/"))
                .Returns(new List<string> { "w28-000006cc03c9", "e28-000006cc02c9" });
            var bus = new OneWire(mock.Object);

            var devices = bus.getDevices();

            Assert.AreEqual(0, devices.Count);
            mock.Verify(fileWrapper => fileWrapper.ListFiles("/sys/bus/w1/devices/"), Times.Once());
        }
    }
}
