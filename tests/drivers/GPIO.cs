using NUnit.Framework;
using Moq;
using rpi_dotnet;
using System;

namespace Tests
{
    public class GPIOTests
    {
        [Test]
        public void ShouldReturnFalsewhenExportsFail()
        {
            var mock = new Mock<IFileWrapper>();
            mock.Setup(fileWrapper => fileWrapper.Write("./sys/class/gpio/export", "20")).Throws(new Exception());

            var device = new GPIO("20", mock.Object);

            var value = device.getValue();
            Assert.AreEqual(false, value);
            mock.Verify(fileWrapper => fileWrapper.Read("./sys/class/gpio/gpio20/value"), Times.Never());
            mock.Verify(fileWrapper => fileWrapper.Write("./sys/class/gpio/gpio20/direction", "IN"), Times.Never());
            mock.Verify(fileWrapper => fileWrapper.Write("./sys/class/gpio/export", "20"), Times.Once());
        }

        [Test]
        public void ShouldReturnFalsewhenSetDirectionFail()
        {
            var mock = new Mock<IFileWrapper>();
            mock.Setup(fileWrapper => fileWrapper.Read("./sys/class/gpio/gpio20/value")).Returns("1");
            mock.Setup(fileWrapper => fileWrapper.Write("./sys/class/gpio/gpio20/direction", "IN")).Throws(new Exception());

            var device = new GPIO("20", mock.Object);

            var value = device.getValue();
            Assert.AreEqual(false, value);
            mock.Verify(fileWrapper => fileWrapper.Read("./sys/class/gpio/gpio20/value"), Times.Never());
            mock.Verify(fileWrapper => fileWrapper.Write("./sys/class/gpio/gpio20/direction", "IN"), Times.Once());
            mock.Verify(fileWrapper => fileWrapper.Write("./sys/class/gpio/export", "20"), Times.Once());
        }

        [Test]
        public void ShouldReturnTrueValue()
        {
            var mock = new Mock<IFileWrapper>();
            mock.Setup(fileWrapper => fileWrapper.Read("./sys/class/gpio/gpio20/value")).Returns("1");
            mock.Setup(fileWrapper => fileWrapper.Write("./sys/class/gpio/gpio20/direction", "IN"));
            mock.Setup(fileWrapper => fileWrapper.Write("./sys/class/gpio/export", "20"));
            var device = new GPIO("20", mock.Object);

            var value = device.getValue();

            Assert.AreEqual(true, value);
            mock.Verify(fileWrapper => fileWrapper.Read("./sys/class/gpio/gpio20/value"), Times.Once());
            mock.Verify(fileWrapper => fileWrapper.Write("./sys/class/gpio/gpio20/direction", "IN"), Times.Once());
            mock.Verify(fileWrapper => fileWrapper.Write("./sys/class/gpio/export", "20"), Times.Once());
        }

        [Test]
        public void ShouldReturnFalseValue()
        {
            var mock = new Mock<IFileWrapper>();
            mock.Setup(fileWrapper => fileWrapper.Read("./sys/class/gpio/gpio20/value")).Returns("0");
            mock.Setup(fileWrapper => fileWrapper.Write("./sys/class/gpio/gpio20/direction", "IN"));
            mock.Setup(fileWrapper => fileWrapper.Write("./sys/class/gpio/export", "20"));
            var device = new GPIO("20", mock.Object);

            var value = device.getValue();

            Assert.AreEqual(false, value);
            mock.Verify(fileWrapper => fileWrapper.Read("./sys/class/gpio/gpio20/value"), Times.Once());
            mock.Verify(fileWrapper => fileWrapper.Write("./sys/class/gpio/gpio20/direction", "IN"), Times.Once());
            mock.Verify(fileWrapper => fileWrapper.Write("./sys/class/gpio/export", "20"), Times.Once());
        }

    }
}
