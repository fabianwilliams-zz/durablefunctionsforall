using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FireOne
{
    public static class FireOne
    {
        [FunctionName("FireOne")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<string>("FireOne_Alpha", "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>("FireOne_Bravo", "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>("FireOne_Charlie", "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName("FireOne_Alpha")]
        public static string SayHelloAlpha([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Logger: Alpha says hello to {name}.");
            return $"Alpha says hello to {name}!";
        }

        [FunctionName("FireOne_Bravo")]
        public static string SayHelloBravo([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Logger: Bravo says hello to {name}.");
            return $"Bravo says hello to {name}!";
        }

        [FunctionName("FireOne_Charlie")]
        public static string SayHelloCharlie([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Logger: Charlie says hello to {name}.");
            return $"Charlie says hello to {name}!";
        }

        [FunctionName("FireOne_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("FireOne", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}