using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hotmailCheck.Models
{
    public class ActiveKey
    {
        public string key {  get; set; }

        public string creationDate { get; set; }

        public string expireDate { get; set; }

        public int dayLeft { get; set; }

        public bool activeForm { get; set; }
    }
}
