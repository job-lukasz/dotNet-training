using NUnit.Framework;
using rpi_dotnet;
using Moq;

namespace Tests
{
    public class DS18B20Tests
    {
        [Test]
        public void GetDeviceName()
        {
            var device = new DS18B20("deviceID");
            var deviceID = device.deviceID;
            Assert.AreEqual("deviceID", deviceID);
        }

        [Test]
        public void MeasureDeviceShouldReturnCorrectValue()
        {
            var deviceID = "28-000006cc02c9";
            var mock = new Mock<IFileWrapper>();
            mock.Setup(fileWrapper => fileWrapper.Read($"/sys/bus/w1/devices/{deviceID}/w1_slave"))
                .Returns("56 01 4b 46 7f ff 0a 10 d1 : crc=d1 YES\n56 01 4b 46 7f ff 0a 10 d1 t=21375\n");
            var device = new DS18B20(deviceID, mock.Object);
            var measureValue = device.Measure();
            Assert.AreEqual(21.375f, measureValue);
        }

        [Test]
        public void MeasureDeviceThrowErrorWhenCRCIncorrect()
        {
            var deviceID = "28-000006cc02c9";
            var mock = new Mock<IFileWrapper>();
            mock.Setup(fileWrapper => fileWrapper.Read($"/sys/bus/w1/devices/{deviceID}/w1_slave"))
                .Returns("56 01 4b 46 7f ff 0a 10 d1 : crc=d2 NO\n56 01 4b 46 7f ff 0a 10 d1 t=01200\n");
            var device = new DS18B20(deviceID, mock.Object);

            Assert.Throws<System.Exception>(() => device.Measure());
        }
    }
}
