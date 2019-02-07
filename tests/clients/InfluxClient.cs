using NUnit.Framework;
using rpi_dotnet;
using Moq;
using AdysTech.InfluxDB.Client.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Tests
{
    public class InfluxClientTests
    {
        [Test]
        public async Task ShouldAddMeasureWhenDBExists()
        {
            var mock = new Mock<IInfluxDBClient>();
            var callArgs = new Dictionary<string, object>();
            mock.Setup(client => client.GetInfluxDBNamesAsync()).Returns(Task.FromResult(new List<string> { "database" }));
            mock.Setup(client => client.PostPointAsync("database", It.IsAny<InfluxDatapoint<InfluxValueField>>()))
                .Returns(Task.FromResult(true))
                .Callback<string, InfluxDatapoint<InfluxValueField>>((database, datapoint) =>
                {
                    callArgs.Add("measureName", datapoint.MeasurementName);
                    callArgs.Add("tags", datapoint.Tags);
                    callArgs.Add("fields", datapoint.Fields);
                });

            var influxClient = new InfluxClient(mock.Object, "database");
            var value = await influxClient.addMeasure("temperature", "salon", 27.122);

            mock.Verify(client => client.GetInfluxDBNamesAsync(), Times.Once());
            mock.Verify(client => client.PostPointAsync("database", It.IsAny<InfluxDatapoint<InfluxValueField>>()), Times.Once());
            Assert.AreEqual(true, value);
            Assert.AreEqual("temperature", callArgs["measureName"]);
            Assert.AreEqual("salon", (callArgs["tags"] as Dictionary<string, string>)["spaceID"]);
            Assert.AreEqual(27.122, (callArgs["fields"] as Dictionary<string, InfluxValueField>)["Doublefield"].Value);
        }

        [Test]
        public async Task ShouldCreateDBWhenNotExists()
        {
            var mock = new Mock<IInfluxDBClient>();
            var callArgs = new Dictionary<string, object>();
            mock.Setup(client => client.GetInfluxDBNamesAsync())
                .Returns(Task.FromResult(new List<string> { "database1" }));
            mock.Setup(client => client.CreateDatabaseAsync("database"))
                .Returns(Task.FromResult(true));

            var influxClient = new InfluxClient(mock.Object, "database");
            await influxClient.addMeasure("", "", 0.0);

            mock.Verify(client => client.GetInfluxDBNamesAsync(), Times.Once());
            mock.Verify(client => client.CreateDatabaseAsync("database"), Times.Once());
        }
    }
}
