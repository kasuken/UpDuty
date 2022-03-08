using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace UpDuty.Worker
{
    public class Function2
    {
        private readonly ILogger _logger;

        public Function2(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function2>();
        }

        [Function("Function2")]
        public async Task Run([QueueTrigger("mystoragequeue", Connection = "AzureWebJobsStorage")] string myQueueItem)
        {
            QueueClient queue = new QueueClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"), "mystoragequeue");

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes("my new message!");
            var base64 = System.Convert.ToBase64String(plainTextBytes);

            await queue.SendMessageAsync(base64, TimeSpan.FromMinutes(3), null);

            _logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
