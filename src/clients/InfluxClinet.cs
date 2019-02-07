using AdysTech.InfluxDB.Client.Net;
using System;
using System.Threading.Tasks;
namespace rpi_dotnet
{
    public class InfluxClient
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IInfluxDBClient influx;
        private string database;
        private bool? dbExists = null;
        public InfluxClient(string url = "home-server.local", string dbName = "home")
        {
            influx = new InfluxDBClient(url);
            database = dbName;
        }
        public InfluxClient(IInfluxDBClient client, string dbName = "home")
        {
            influx = client;
            database = dbName;
        }
        private async Task<bool> checkIfDBExists()
        {
            var names = await influx.GetInfluxDBNamesAsync();
            return names.Contains(database);
        }
        private async Task<bool> CreateDB()
        {
            log.Info($"Create database {database}");
            return await influx.CreateDatabaseAsync(database);
        }
        public async Task<bool> addMeasue(string measureName, string spaceID, double value)
        {
            if (dbExists == null)
            {
                if ((bool)(dbExists = await checkIfDBExists()) == false)
                {
                    dbExists = await CreateDB();
                }
            }
            var point = new InfluxDatapoint<InfluxValueField>();
            point.UtcTimestamp = DateTime.Now;
            point.Tags.Add("spaceID", spaceID);
            point.Fields.Add("Doublefield", new InfluxValueField(value));
            point.MeasurementName = measureName;
            log.Info($"Add measure point: {{MeasureName: {measureName}, spaceID: {spaceID}, value: {value} }} to {database}");
            return await influx.PostPointAsync(database, point);
        }
    }
}