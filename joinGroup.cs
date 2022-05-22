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
using Microsoft.Azure.WebPubSub.Common;
using Newtonsoft.Json.Linq;
using chat.Repositories;

namespace Company.Function
{
    public static class joinGroup
    {
        [FunctionName("joinGroup")]
        public static async Task Run(
            [WebPubSubTrigger("simplechat", WebPubSubEventType.User, "joinGroup")] UserEventRequest request,
            BinaryData data, WebPubSubDataType dataType,
            [WebPubSub(Hub = "simplechat")] IAsyncCollector<WebPubSubAction> actions)
        {

            string json = data.ToString();

            //add error handling
            dynamic json2 = JObject.Parse(json);
            string username = json2.username;
            string group = json2.group;

            await actions.AddAsync(WebPubSubAction.CreateAddUserToGroupAction(username, group));

            UserRepository.AddGroupToUser(username, group, group);

        }
    }
}
