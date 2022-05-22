using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chat.Models
{
    public class MessageData
    {
        public string User { get; set; }
        public string Message { get; set; }
        public DateTime time { get; set; }

        public string guid { get; set; }

        public MessageData(string user, string message)
        {
            User = user;
            Message = message;
            time =  DateTime.Now;
            guid = Guid.NewGuid().ToString();
        }
    }
}
