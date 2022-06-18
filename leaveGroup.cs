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
using chat.Repositories;
using System.Collections.Generic;
using chat.Models;

namespace Company.Function
{
    public static class leaveGroup
    {
        [FunctionName("leaveGroup")]
        public static async Task Run(
            [WebPubSubTrigger("simplechat", WebPubSubEventType.User, "leaveGroup")] UserEventRequest request,
            BinaryData data, WebPubSubDataType dataType,
            [WebPubSub(Hub = "simplechat")] IAsyncCollector<WebPubSubAction> actions)
        {

            string group = data.ToString();
            string user = request.ConnectionContext.UserId;

            await actions.AddAsync(WebPubSubAction.CreateRemoveUserFromGroupAction(user, group));
            UserRepository.RemoveGroupFromUser(user, group);

            string message = "User " + user + " left the chat";
            MessageData messageData = new MessageData(user, message);
            string messageDatajson = JsonConvert.SerializeObject(messageData);

            await actions.AddAsync(WebPubSubAction.CreateSendToGroupAction(group, messageDatajson, WebPubSubDataType.Json));

            List<UserGroup> userGroups = await UserRepository.getAllUsersInGroup(group);

            foreach(UserGroup userGroup in userGroups)
            {
                if (userGroup.Username != user)
                {
                    UserRepository.saveMessage(userGroup, messageDatajson);
                }
            }

        }
    }
}
