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
using System.Text.RegularExpressions;
using System.Collections.Generic;
using chat.Models;
using Newtonsoft.Json.Linq;
using Azure.Messaging.WebPubSub;
using chat.Repositories;

namespace Company.Function
{
    public static class test
    {
        [FunctionName("test")]
        public static async Task Run(
            [WebPubSubTrigger("simplechat", WebPubSubEventType.User, "message")] UserEventRequest request,
            BinaryData data, WebPubSubDataType dataType,
            [WebPubSub(Hub = "simplechat")] IAsyncCollector<WebPubSubAction> actions)
        {
            Console.WriteLine("testevent");

            String datajson = data.ToString();
            dynamic json2 = JObject.Parse(datajson);
            String userId = request.ConnectionContext.UserId;
            String group = json2.group;
            String message = json2.message == null ? "" : json2.message;
            String image = json2.image == null ? "" : json2.image;
            string uri = "";
            Console.WriteLine(group);

            if (image != null && image != "" )
            {
                Console.WriteLine("image found");
                byte[] byteArr = Convert.FromBase64String(json2.image.ToString());

                uri = await UserRepository.SaveImage(byteArr);
                Console.WriteLine(uri);
            }

            MessageData messageData = new MessageData(userId, message, uri);
            string messageDatajson = JsonConvert.SerializeObject(messageData);

            await actions.AddAsync(WebPubSubAction.CreateSendToGroupAction(group, messageDatajson, WebPubSubDataType.Json));

            WebPubSubServiceClient client =
                new WebPubSubServiceClient(Environment.GetEnvironmentVariable("WebPubSubConnectionString"), Environment.GetEnvironmentVariable("WebPubSubHub"));


            List<UserGroup> userGroups = await UserRepository.getAllUsersInGroup(group);

            foreach(UserGroup userGroup in userGroups)
            {
                if (userGroup.Username != userId)
                {
                    UserRepository.saveMessage(userGroup, messageDatajson);
                }
                else{
                    // remove messages?
                }
            }
        }
    }
}
