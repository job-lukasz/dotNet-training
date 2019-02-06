using AdysTech.InfluxDB.Client.Net;
using System;
using System.Threading.Tasks;
namespace rpi_dotnet
{
    public class influxClient
    {
        private IInfluxDBClient influx;
        private string database;
        public influxClient(string url="home-server.local",string dbName = "home")
        {
            influx = new InfluxDBClient("home-server.local");
            database = dbName;
        }
        public async Task<bool> addMeasue(string measureName,string spaceID, double value){
            var point = new InfluxDatapoint<InfluxValueField>();
            point.UtcTimestamp = DateTime.Now;
            point.Tags.Add("spaceID", spaceID);
            point.Fields.Add("Doublefield", new InfluxValueField(value));
            point.MeasurementName = measureName;
            return await influx.PostPointAsync(database, point);
        }
    }
}