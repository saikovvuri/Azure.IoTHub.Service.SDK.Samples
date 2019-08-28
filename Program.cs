using System;
using Microsoft.Azure.Devices.Shared;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace ApplyConfigurationSingleDevice
{
    class Program
    {
        static string connectionString = Environment.GetEnvironmentVariable("IOTHUB_CONNECTIONSTRING");
        static string deviceId = Environment.GetEnvironmentVariable("DEVICE");

        static void Main(string[] args)
        {
            //ApplyConfigurationOnDevice().Wait();
            QueryConfigurationOnDevice().Wait();
        }

        private static async Task ApplyConfigurationOnDevice()
        {
            // load local sample deployment manifest
            dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("deployment.json"));


            var cc = new ConfigurationContent() { ModulesContent = new Dictionary<string, IDictionary<string, object>>() };
            foreach (var module in config.modulesContent)
            {
                cc.ModulesContent.Add(module.Name, new Dictionary<string, object> { ["properties.desired"] = module.Value["properties.desired"] });   
                
            }

            var rm = RegistryManager.CreateFromConnectionString(connectionString);
            await rm.ApplyConfigurationContentOnDeviceAsync(deviceId, cc);
        }

        private static async Task QueryConfigurationOnDevice()
        {
            // success in applying deployment manifest
            string edgeReportedSuccessQuery = "SELECT deviceId, properties.reported.lastDesiredVersion FROM devices.modules where moduleId='$edgeAgent'";

            // failure in applying deployment manifest
            string edgeReportedFailedQuery = "SELECT deviceId FROM devices.modules where moduleId='$edgeAgent' AND properties.desired.$version = properties.reported.lastDesiredVersion AND properties.reported.lastDesiredStatus.code != 200";


            var rm = RegistryManager.CreateFromConnectionString(connectionString);

            var query = rm.CreateQuery(edgeReportedSuccessQuery);

            // results = each record in the resultset is of type IEnumerable for KeyValuePairs - column name and value
            //           only columns with non-null values will be added as KeyValuePairs
            var results = (await query.GetNextAsJsonAsync()).Select(r => JsonConvert.DeserializeObject<Dictionary<string, Object>>(r));
            
            foreach(var record in results)
            {
                foreach(var kv in record)
                {
                    Console.WriteLine("Column Name: {0}  Value: {1}", kv.Key, kv.Value);                    
                }
                Console.WriteLine("...");
            }

        }

    }
}
