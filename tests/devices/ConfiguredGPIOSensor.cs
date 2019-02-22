using NUnit.Framework;
using rpi_dotnet;
using Moq;
using System;
using System.Collections.Generic;

namespace Tests
{
    public class ConfiguredGPIOSensorTest
    {
        [Test]
        public void ShouldInitDevice()
        {
            var mock = new Mock<IGPIODevice>();
            mock.Setup(gpio => gpio.setDirection(Direction.IN)).Returns(true);
            var device = new ConfiguredGPIOSensor(mock.Object, "testSpace");
            var measureValue = device.FindDevice();
            Assert.AreEqual(true, measureValue);
        }

        [Test]
        public void ShouldNotInitDevice()
        {
            var mock = new Mock<IGPIODevice>();
            mock.Setup(gpio => gpio.setDirection(Direction.IN)).Throws(new Exception("Tested Exception"));
            var device = new ConfiguredGPIOSensor(mock.Object, "testSpace");
            var measureValue = device.FindDevice();
            Assert.AreEqual(false, measureValue);
        }

        [Test]
        public void ShouldNotMesureValueWhenSensorNotInit()
        {
            var mock = new Mock<IGPIODevice>();
            mock.Setup(gpio => gpio.setDirection(Direction.IN)).Throws(new Exception("Tested Exception"));
            var device = new ConfiguredGPIOSensor(mock.Object, "testSpace");

            device.FindDevice();
            var measureValue = device.Act();

            Assert.AreEqual(false, measureValue);
            mock.Verify(gpio => gpio.getValue(), Times.Never());
        }

        [Test]
        public void ShouldMesureValue()
        {
            var mock = new Mock<IGPIODevice>();
            mock.Setup(gpio => gpio.setDirection(Direction.IN)).Returns(true);
            mock.Setup(gpio => gpio.getValue()).Returns(false);
            var device = new ConfiguredGPIOSensor(mock.Object, "testSpace");
            device.FindDevice();
            var measureValue = device.Act();

            Assert.AreEqual(true, measureValue);
            mock.Verify(gpio => gpio.getValue(), Times.Once());
        }

        [Test]
        public void ShouldCallActionHandler()
        {
            var mock = new Mock<IGPIODevice>();
            mock.Setup(gpio => gpio.setDirection(Direction.IN)).Returns(true);
            mock.Setup(gpio => gpio.getValue()).Returns(false);
            var callArgs = new Dictionary<string, object>();
            var eventListenerMock = new Mock<IEventListener>();
            eventListenerMock.Setup(e => e.onEvent(It.IsAny<object>(), It.IsAny<MeasuredValueChange>()))
                .Callback<object, MeasuredValueChange>((sender, mesuredValue) =>
                {
                    callArgs.Add("deviceID", mesuredValue.DeviceID);
                    callArgs.Add("measureName", mesuredValue.MeasureName);
                    callArgs.Add("spaceID", mesuredValue.SpaceID);
                    callArgs.Add("value", mesuredValue.NewValue);
                });
            var device = new ConfiguredGPIOSensor(mock.Object, "testSpace");
            device.ValueChanged += eventListenerMock.Object.onEvent;

            device.FindDevice();
            var measureValue = device.Act();

            Assert.AreEqual(true, measureValue);
            Assert.AreEqual(null, callArgs["deviceID"]);
            Assert.AreEqual("indoorSensor", (string)callArgs["measureName"]);
            Assert.AreEqual("testSpace", (string)callArgs["spaceID"]);
            Assert.AreEqual(false, (bool)callArgs["value"]);
            mock.Verify(gpio => gpio.getValue(), Times.Once());
            eventListenerMock.Verify(e => e.onEvent(It.IsAny<object>(), It.IsAny<MeasuredValueChange>()), Times.Once());
        }
        [Test]
        public void ShouldNotCallActionHandlerWhenValueNotChange()
        {
            var mock = new Mock<IGPIODevice>();
            mock.Setup(gpio => gpio.setDirection(Direction.IN)).Returns(true);
            mock.Setup(gpio => gpio.getValue()).Returns(false);
            mock.SetupGet(gpio => gpio.lastValue).Returns(false);
            var device = new ConfiguredGPIOSensor(mock.Object, "testSpace");
            var eventListenerMock = new Mock<IEventListener>();
            eventListenerMock.Setup(e => e.onEvent(It.IsAny<object>(), It.IsAny<MeasuredValueChange>()));
            device.ValueChanged += eventListenerMock.Object.onEvent;

            device.FindDevice();
            var measureValue = device.Act();

            Assert.AreEqual(false, measureValue);
            mock.Verify(gpio => gpio.getValue(), Times.Once());
            eventListenerMock.Verify(e => e.onEvent(It.IsAny<object>(), It.IsAny<MeasuredValueChange>()), Times.Never());
        }
        [Test]
        public void DoesNotCallAnythingWhenUnalbeToGetValue()
        {
            var mock = new Mock<IGPIODevice>();
            mock.Setup(gpio => gpio.setDirection(Direction.IN)).Returns(true);
            mock.Setup(gpio => gpio.getValue()).Throws(new Exception("Test exception"));
            var eventListenerMock = new Mock<IEventListener>();
            eventListenerMock.Setup(e => e.onEvent(It.IsAny<object>(), It.IsAny<MeasuredValueChange>()));

            var device = new ConfiguredGPIOSensor(mock.Object, "testSpace");
            device.ValueChanged += eventListenerMock.Object.onEvent;

            device.FindDevice();
            var measureValue = device.Act();

            Assert.AreEqual(false, measureValue);
            mock.Verify(gpio => gpio.getValue(), Times.Once());
            eventListenerMock.Verify(e => e.onEvent(It.IsAny<object>(), It.IsAny<MeasuredValueChange>()), Times.Never());
        }
    }
}
