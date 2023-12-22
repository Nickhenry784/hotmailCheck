using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hotmailCheck.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class DataInput
    {
        public SessionMailReceived sessionMailReceived { get; set; }
    }

    public class DropMailInput
    {
        public DataInput data { get; set; }
    }

    public class SessionMailReceived
    {
        public string toAddr { get; set; }
        public string text { get; set; }
        public int rawSize { get; set; }
        public string headerSubject { get; set; }
        public string fromAddr { get; set; }
        public string downloadUrl { get; set; }
    }
}
