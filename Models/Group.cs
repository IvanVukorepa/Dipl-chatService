using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chat.Models
{
    public class Group
    {
        public List<String> users { get; set; }
        public string name { get; set; }

        public Group()
        {

        }
    }
}
