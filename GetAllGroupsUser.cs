using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using chat.Models;
using chat.Repositories;

namespace Company.Function
{
    public static class GetAllGroupsUser
    {
        [FunctionName("GetAllGroupsUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            List<UserGroup> userGroups = new List<UserGroup>();
            userGroups = await UserRepository.GetAllUserGroupsForUser(username: req.Query["username"]);
            return new OkObjectResult(userGroups);
        }
    }
}
