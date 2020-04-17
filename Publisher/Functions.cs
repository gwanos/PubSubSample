using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Publisher
{
    public class Functions
    {
        private readonly IPublisher _publisher;

        public Functions(IPublisher publisher)
        {
            _publisher = publisher;
        }

        [FunctionName("publish")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
        
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<PublishRequest>(requestBody);

            //
            var result = _publisher.Publish(data.Channel, data.Message);
            
            //
            string responseMessage = $"{result} clients received the message.";
            return new OkObjectResult(responseMessage);
        }
    }
}
