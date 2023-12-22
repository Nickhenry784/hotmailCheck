using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hotmailCheck.Models
{
    public class EmailAddress
    {
        public string email { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
    public class Mail
    {
        public string predmetZkraceny { get; set; }
        public string predmet { get; set; }
        public string od { get; set; }
        public int id { get; set; }
        public string kdy { get; set; }
        public string akce { get; set; }
        public string precteno { get; set; }
    }


}
