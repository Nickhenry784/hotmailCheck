using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hotmailCheck.Models
{
    public class Address
    {
        [JsonProperty("Country calling code")]
        public string Countrycallingcode { get; set; }

        [JsonProperty("Phone number")]
        public string Phonenumber { get; set; }

        [JsonProperty("Zip code")]
        public string Zipcode { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string street { get; set; }
    }

    public class Credit
    {
        public string type { get; set; }
        public string number { get; set; }
        public string name { get; set; }
        public string expirationDate { get; set; }
    }

    public class RandomInfor
    {
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string postcode { get; set; }
        public string streetAddress { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string company { get; set; }
        public string gender { get; set; }
        public Credit credit { get; set; }
        public string accountNo { get; set; }
        public string username { get; set; }
        public string passw { get; set; }
        public string ipv4 { get; set; }
        public string ipv6 { get; set; }
        public string macad { get; set; }
        public string semail { get; set; }
        public string uagent { get; set; }
        public string jobtitle { get; set; }
        public string comemail { get; set; }
        public string salary { get; set; }
        public int iban { get; set; }
        public string dob { get; set; }
        public int age { get; set; }
        public string height { get; set; }
        public string weight { get; set; }
        public string hair { get; set; }
        public string eye { get; set; }
        public string bank { get; set; }
        public string bcode { get; set; }
    }

    public class Information
    {
        public List<Random> random { get; set; }
        public Address address { get; set; }
    }
}
