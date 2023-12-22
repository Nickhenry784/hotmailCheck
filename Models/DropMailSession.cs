using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hotmailCheck.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class AddressSession
    {
        public string address { get; set; }
    }

    public class Data
    {
        public IntroduceSession introduceSession { get; set; }
    }

    public class IntroduceSession
    {
        public string id { get; set; }
        public DateTime expiresAt { get; set; }
        public List<AddressSession> addresses { get; set; }
    }

    public class DropMailSession
    {
        public Data data { get; set; }
    }
}
