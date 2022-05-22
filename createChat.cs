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
    public static class createChat
    {
        [FunctionName("createChat")]
        public static async Task<IActionResult> Run(
            [WebPubSubTrigger("simplechat", WebPubSubEventType.User, "createChat")] UserEventRequest request,
            BinaryData data, WebPubSubDataType dataType,
            [WebPubSub(Hub = "simplechat")] IAsyncCollector<WebPubSubAction> actions)
        {

            string json = data.ToString();

            Console.WriteLine(json);

            //add error handling
            dynamic json2 = JObject.Parse(json);
            string username1 = json2.username1;
            string username2 = json2.username2;

            string group = getChatName(username1, username2);

            await actions.AddAsync(WebPubSubAction.CreateAddUserToGroupAction(username1, group));
            await actions.AddAsync(WebPubSubAction.CreateAddUserToGroupAction(username2, group));

            UserRepository.AddGroupToUser(username1, group, username2);
            UserRepository.AddGroupToUser(username2, group, username1);

            return new OkObjectResult(group);

        }

        public static string getChatName(string username1, string username2){
            string name = "";
            if (string.Compare(username1, username2) < 0){
                name = username1 + "_" + username2;
            } else{
                name = username2 + "_" + username1;
            }

            return name;
        }
    }
}
