using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using chat.Models;
using System.Collections.Generic;
using chat.Repositories;

namespace Company.Function
{
    public static class rejoinGroups
    {
        [FunctionName("rejoinGroups")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            [WebPubSub(Hub = "simplechat")] IAsyncCollector<WebPubSubAction> actions)
        {

            List<UserGroup> userGroups = new List<UserGroup>();
            userGroups = await UserRepository.GetAllUserGroupsForUser(username: req.Query["username"]);

            foreach (UserGroup ug in userGroups)
            {
                //TODO: make these calls concurrent
                await actions.AddAsync(WebPubSubAction.CreateAddUserToGroupAction(ug.Username, ug.Group));
            }

            return new OkObjectResult("success");
        }
    }
}
