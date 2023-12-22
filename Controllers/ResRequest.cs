using hotmailCheck.Models;
using Newtonsoft.Json;
using OpenQA.Selenium;
using RestSharp;
using SeleniumUndetectedChromeDriver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using ICredentials = System.Net.ICredentials;

namespace hotmailCheck.Controllers
{
    public class ResRequest
    {
        public Information getInformation()
        {
            var client = new RestClient("https://www.prepostseo.com/frontend/fakeAddressGenerator");
            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.Method = Method.Post;
            request.AddHeader("accept", "application/json, text/javascript, */*; q=0.01");
            request.AddHeader("accept-language", "vi,en-US;q=0.9,en;q=0.8");
            request.AddHeader("authority", "www.prepostseo.com");
            request.AddHeader("origin", "https://www.prepostseo.com");
            request.AddHeader("referer", "https://www.prepostseo.com/tool/fake-address-generator");
            request.AddHeader("sec-ch-ua", "\"Google Chrome\";v=\"119\", \"Chromium\";v=\"119\", \"Not?A_Brand\";v=\"24\"");
            request.AddHeader("x-requested-with", "XMLHttpRequest");
            request.AddParameter("application/json", "lang=en_US", ParameterType.RequestBody);
            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Information infor = JsonConvert.DeserializeObject<Information>(response.Content);
                return infor;
            }
            else
            {
                return null;
            }
        }

        public DropMailSession GetSession(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest();
            request.Method = Method.Get;
            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                DropMailSession dropMailSession = JsonConvert.DeserializeObject<DropMailSession>(response.Content);
                return dropMailSession;
            }
            else
            {
                return null;
            }
        }

        public async Task<DropMailInput> GetMailInput(string sessionId, string token)
        {
            try
            {
                string url = string.Format("https://dropmail.me/api/graphql/{0}?query=subscription%20(%24id%3A%20ID!)%20%7BsessionMailReceived(id%3A%24id)%20%7B%20rawSize%2C%20fromAddr%2C%20toAddr%2C%20downloadUrl%2C%20text%2C%20headerSubject%7D%20%7D&variables=%7B%22id%22%3A%22{1}%22%7D", token, sessionId);
                var client = new RestClient(url);
                var request = new RestRequest();
                request.Method = Method.Get;
                var response = await client.ExecuteAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    DropMailInput dropMailSession = JsonConvert.DeserializeObject<DropMailInput>(response.Content.ToString());
                    return dropMailSession;
                }
                else
                {
                    return null;
                }
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return null;
            }
            
        }

        public EmailAddress getEmail(string cookie, string proxy)
        {
            var options = new RestClientOptions("https://www.minuteinbox.com/index/index");
            if (!string.IsNullOrEmpty(proxy))
            {
                options = new RestClientOptions("https://www.minuteinbox.com/index/index")
                {
                    Proxy = GetWebProxy(proxy),
                    ThrowOnAnyError = true,
                    MaxTimeout = 10000
                };
            }
            var client = new RestClient(options);
            var request = new RestRequest();
            request.AddHeader("X-Requested-With", "XMLHttpRequest");
            request.AddHeader("accept", "application/json, text/javascript, */*; q=0.01");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("accept-language", "vi,en-US;q=0.9,en;q=0.8");
            request.AddHeader("Sec-Fetch-Mode", "cors");
            request.AddHeader("Cookie", cookie);
            request.AddHeader("Sec-Fetch-Site", "same-origin");
            request.AddHeader("Sec-Fetch-Dest", "empty");
            request.AddHeader("referer", "https://www.minuteinbox.com/");
            request.AddHeader("sec-ch-ua-mobile", "?0");
            request.AddHeader("sec-ch-ua-platform", "\"Windows\"");
            request.AddHeader("sec-ch-ua", "\"Not_A Brand\";v=\"8\", \"Chromium\";v=\"119\", \"Google Chrome\";v=\"119\"");
            request.Method = Method.Get;
            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                EmailAddress email = JsonConvert.DeserializeObject<EmailAddress>(response.Content);
                return email;
            }
            else
            {
                return null;
            }
        }

        public async Task<string> returnEmail(string cookie, string addressMail, string proxy)
        {
            var options = new RestClientOptions("https://www.minuteinbox.com/index/new-email/");
            if (!string.IsNullOrEmpty(proxy))
            {
                options = new RestClientOptions("https://www.minuteinbox.com/index/new-email/")
                {
                    Proxy = GetWebProxy(proxy),
                    ThrowOnAnyError = true,
                    MaxTimeout = 10000
                };
            }
            var client = new RestClient(options);
            var request = new RestRequest();
            request.AddHeader("X-Requested-With", "XMLHttpRequest");
            request.AddHeader("accept", "application/json, text/javascript, */*; q=0.01");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.AddHeader("accept-language", "vi,en-US;q=0.9,en;q=0.8");
            request.AddHeader("Sec-Fetch-Mode", "cors");
            request.AddHeader("Cookie", cookie);
            request.AddHeader("Sec-Fetch-Site", "same-origin");
            request.AddHeader("Sec-Fetch-Dest", "empty");
            request.AddHeader("referer", "https://www.minuteinbox.com/");
            request.AddHeader("Origin", "https://www.minuteinbox.com/");
            request.AddHeader("sec-ch-ua-mobile", "?0");
            request.AddHeader("sec-ch-ua-platform", "\"Windows\"");
            request.AddHeader("sec-ch-ua", "\"Not_A Brand\";v=\"8\", \"Chromium\";v=\"119\", \"Google Chrome\";v=\"119\"");
            request.Method = Method.Post;
            string json = string.Format("emailInput={0}&format=json", addressMail.Split('@')[0]);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return "ok";
            }
            else
            {
                string[] slipMi = cookie.Split("MI=");
                string[] duoiSlipMi = slipMi[1].Split("%40milkcreeks.com");
                string newCookie = slipMi[0] + "MI=" + Uri.EscapeUriString(addressMail) + duoiSlipMi[1];
                deleteEmail(newCookie,proxy);
                return newCookie;
            }
        }

        public async Task<string> readMail(string cookie, string url, string proxy)
        {
            var options = new RestClientOptions(url);
            if (!string.IsNullOrEmpty(proxy))
            {
                options = new RestClientOptions(url)
                {
                    Proxy = GetWebProxy(proxy),
                    ThrowOnAnyError = true,
                    MaxTimeout = 10000
                };
            }
            var client = new RestClient(options);
            var request = new RestRequest();
            request.AddHeader("X-Requested-With", "XMLHttpRequest");
            request.AddHeader("accept", "application/json, text/javascript, */*; q=0.01");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.AddHeader("accept-language", "vi,en-US;q=0.9,en;q=0.8");
            request.AddHeader("Sec-Fetch-Mode", "cors");
            request.AddHeader("Cookie", cookie);
            request.AddHeader("Sec-Fetch-Site", "same-origin");
            request.AddHeader("Sec-Fetch-Dest", "empty");
            request.AddHeader("referer", "https://www.minuteinbox.com/");
            request.AddHeader("Origin", "https://www.minuteinbox.com/");
            request.AddHeader("sec-ch-ua-mobile", "?0");
            request.AddHeader("sec-ch-ua-platform", "\"Windows\"");
            request.AddHeader("sec-ch-ua", "\"Not_A Brand\";v=\"8\", \"Chromium\";v=\"119\", \"Google Chrome\";v=\"119\"");
            request.Method = Method.Get;
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Content.ToString() ;
            }
            else
            {
                return null;
            }
        }

        public bool deleteEmail(string cookie, string proxy)
        {
            if (string.IsNullOrEmpty(cookie))
            {
                return false;
            }
            var options = new RestClientOptions("https://www.minuteinbox.com/delete");
            if (!string.IsNullOrEmpty(proxy))
            {
                options = new RestClientOptions("https://www.minuteinbox.com/delete")
                {
                    Proxy = GetWebProxy(proxy),
                    ThrowOnAnyError = true,
                    MaxTimeout = 10000
                };
            }
            var client = new RestClient(options);
            var request = new RestRequest();
            request.AddHeader("X-Requested-With", "XMLHttpRequest");
            request.AddHeader("accept", "application/json, text/javascript, */*; q=0.01");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("accept-language", "vi,en-US;q=0.9,en;q=0.8");
            request.AddHeader("Sec-Fetch-Mode", "cors");
            request.AddHeader("Cookie", cookie);
            request.AddHeader("Sec-Fetch-Site", "same-origin");
            request.AddHeader("Sec-Fetch-Dest", "empty");
            request.AddHeader("referer", "https://www.minuteinbox.com/");
            request.AddHeader("sec-ch-ua-mobile", "?0");
            request.AddHeader("sec-ch-ua-platform", "\"Windows\"");
            request.AddHeader("sec-ch-ua", "\"Not_A Brand\";v=\"8\", \"Chromium\";v=\"119\", \"Google Chrome\";v=\"119\"");
            request.Method = Method.Get;
            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<Mail>> getMailInbox (string cookie, string proxy)
        {
            var options = new RestClientOptions("https://www.minuteinbox.com/index/refresh");
            if (!string.IsNullOrEmpty(proxy))
            {
                options = new RestClientOptions("https://www.minuteinbox.com/index/refresh")
                {
                    Proxy = GetWebProxy(proxy),
                    ThrowOnAnyError = true,
                    MaxTimeout = 10000
                };
            }
            var client = new RestClient(options);
            var request = new RestRequest();
            request.AddHeader("X-Requested-With", "XMLHttpRequest");
            request.AddHeader("accept", "application/json, text/javascript, */*; q=0.01");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("accept-language", "vi,en-US;q=0.9,en;q=0.8");
            request.AddHeader("Sec-Fetch-Mode", "cors");
            request.AddHeader("Cookie", cookie);
            request.AddHeader("Sec-Fetch-Site", "same-origin");
            request.AddHeader("Sec-Fetch-Dest", "empty");
            request.AddHeader("referer", "https://www.minuteinbox.com/");
            request.AddHeader("sec-ch-ua-mobile", "?0");
            request.AddHeader("sec-ch-ua-platform", "\"Windows\"");
            request.AddHeader("sec-ch-ua", "\"Not_A Brand\";v=\"8\", \"Chromium\";v=\"119\", \"Google Chrome\";v=\"119\"");
            request.Method = Method.Get;
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                List<Mail> mail = JsonConvert.DeserializeObject<List<Mail>>(response.Content);
                return mail;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<Mail>> getMailInboxWithWebClient(string cookie, string proxy)
        {
            var options = new RestClientOptions("https://www.minuteinbox.com/index/refresh");
            if (!string.IsNullOrEmpty(proxy))
            {
                options = new RestClientOptions("https://www.minuteinbox.com/index/refresh")
                {
                    Proxy = GetWebProxy(proxy),
                    ThrowOnAnyError = true,
                    MaxTimeout = 10000
                };
            }
            var client = new RestClient(options);
            var request = new RestRequest();
            request.AddHeader("X-Requested-With", "XMLHttpRequest");
            request.AddHeader("accept", "application/json, text/javascript, */*; q=0.01");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("accept-language", "vi,en-US;q=0.9,en;q=0.8");
            request.AddHeader("Sec-Fetch-Mode", "cors");
            request.AddHeader("Cookie", cookie);
            request.AddHeader("Sec-Fetch-Site", "same-origin");
            request.AddHeader("Sec-Fetch-Dest", "empty");
            request.AddHeader("referer", "https://www.minuteinbox.com/");
            request.AddHeader("sec-ch-ua-mobile", "?0");
            request.AddHeader("sec-ch-ua-platform", "\"Windows\"");
            request.AddHeader("sec-ch-ua", "\"Not_A Brand\";v=\"8\", \"Chromium\";v=\"119\", \"Google Chrome\";v=\"119\"");
            request.Method = Method.Get;
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                List<Mail> mail = JsonConvert.DeserializeObject<List<Mail>>(response.Content);
                return mail;
            }
            else
            {
                return null;
            }
        }

        public IWebProxy GetWebProxy(string proxyAcc)
        {
            var proxy = new WebProxy();
            if (!string.IsNullOrEmpty(proxyAcc))
            {
                if(proxyAcc.Split(':').Length > 2)
                {
                    var proxyURI = new Uri(string.Format("http://{0}:{1}", proxyAcc.Split(':')[0], proxyAcc.Split(':')[1]));
                    ICredentials credentials = new NetworkCredential(proxyAcc.Split(':')[2], proxyAcc.Split(':')[3]);
                    proxy = new WebProxy(proxyURI, true, null, credentials);

                }
                else {
                    var proxyURI = new Uri(string.Format("{0}:{1}", proxyAcc.Split(':')[0], proxyAcc.Split(':')[1]));
                    //Set credentials
                    ICredentials credentials = CredentialCache.DefaultNetworkCredentials;
                    proxy = new WebProxy(proxyURI, true, null, credentials);
                }
            }
            return proxy;
        }
    }
}
