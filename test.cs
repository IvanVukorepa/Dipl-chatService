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
            [WebPubSubTrigger("simplechat", WebPubSubEventType.User, "testevent")] UserEventRequest request,
            BinaryData data, WebPubSubDataType dataType,
            [WebPubSub(Hub = "simplechat")] IAsyncCollector<WebPubSubAction> actions)
        {
            Console.WriteLine("testevent");
            String receivedData = request.Data.ToString();
            String[] receivedDataArr = receivedData.Split('[', ']');

            Console.WriteLine(receivedData);
            if (receivedDataArr.Length < 3)
            {
                //return some error
                return;
            }
            String group = receivedDataArr[1];
            String userId = request.ConnectionContext.UserId;

            String message = receivedDataArr[2];

            Console.WriteLine(group);
            MessageData messageData = new MessageData(userId, message);
            string messageDatajson = JsonConvert.SerializeObject(messageData);

            //await actions.AddAsync(WebPubSubAction.CreateSendToAllAction(request.Data, dataType));
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
