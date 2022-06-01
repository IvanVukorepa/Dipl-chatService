using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using chat.Models;
using Azure.Messaging.WebPubSub;
using chat.Repositories;

namespace Company.Function
{
    public static class createGroup
    {
        [FunctionName("createGroup")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var content = await new StreamReader(req.Body).ReadToEndAsync();
            Console.WriteLine("test" + content);
            Group group = JsonConvert.DeserializeObject<Group>(content);
            WebPubSubServiceClient client =
                new WebPubSubServiceClient(Environment.GetEnvironmentVariable("WebPubSubConnectionString"), Environment.GetEnvironmentVariable("WebPubSubHub"));

            string groupId = Guid.NewGuid().ToString();

            foreach (string user in group.users)
            {
                client.AddUserToGroupAsync(groupId, user);
                UserRepository.AddGroupToUser(user, groupId, group.name);
            }

            return new OkObjectResult("success");
        }
    }
}
