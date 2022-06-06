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
using Azure.Storage.Blobs;
using chat.Repositories;
using Newtonsoft.Json.Linq;
using chat.Models;
using System.Collections.Generic;

namespace Company.Function
{
    public static class sendImage
    {
        [FunctionName("sendImage")]
        public static async Task<IActionResult> Run(
            [WebPubSubTrigger("simplechat", WebPubSubEventType.User, "sendImage")] UserEventRequest request,
            BinaryData data, WebPubSubDataType dataType,
            [WebPubSub(Hub = "simplechat")] IAsyncCollector<WebPubSubAction> actions)
        {
            Console.WriteLine("sendImage event");

            String datajson = data.ToString();
            dynamic json2 = JObject.Parse(datajson);
            Console.WriteLine(json2.group);
            String group = json2.group.ToString();
            String userId = request.ConnectionContext.UserId;
            //Console.WriteLine(json2.image);
            //Console.WriteLine(json2.image.getType());
            byte[] byteArr = Convert.FromBase64String(json2.image.ToString());
            
            //BinaryData bd = json2.image.ToObject<BinaryData>();
            string uri = await UserRepository.SaveImage(byteArr);
            Console.WriteLine(uri);

            MessageData messageData = new MessageData(userId, "test message", uri);
            string messageDatajson = JsonConvert.SerializeObject(messageData);
            await actions.AddAsync(WebPubSubAction.CreateSendToGroupAction(group, messageDatajson, WebPubSubDataType.Json));

            List<UserGroup> userGroups = await UserRepository.getAllUsersInGroup(group);

            foreach (UserGroup userGroup in userGroups)
            {
                if (userGroup.Username != userId)
                {
                    UserRepository.saveMessage(userGroup, messageDatajson);
                }
                else
                {
                    // remove messages?
                }
            }

            return new OkObjectResult(uri);
        }
    }
}
