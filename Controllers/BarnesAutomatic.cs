using hotmailCheck.Utils;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hotmailCheck.Models;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.DevTools;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;
using System.Data;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace hotmailCheck.Controllers
{
    public class BarnesAutomatic
    {
        public UtilsController utils = new UtilsController();
        public ResRequest resRequest = new ResRequest();
        public string checkLogin(ChromeDriver driver, string cookie)
        {
            string text = string.Empty;
            try
            {
                WebDriverWait driverWait1 = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                driverWait1.Until(ExpectedConditions.ElementExists(By.Id("addPhoneCodeForm")));
                string code = "0";
                List<Mail> mailInput = resRequest.getMailInbox(cookie).Result;
                if (mailInput.Count != 0)
                {
                    foreach(Mail mail in mailInput) {
                        if (mail.predmet.Equals("Your secure OTP for transaction. Please do not share."))
                        {
                            HtmlDocument doc = new HtmlDocument();
                            doc.LoadHtml(mail.akce);

                            // Sử dụng XPath để lấy đường dẫn download-email
                            HtmlNode downloadLink = doc.DocumentNode.SelectSingleNode("//a[@title='Save this email']");
                            if (downloadLink != null)
                            {
                                string downloadUrl = downloadLink.GetAttributeValue("href", "");
                                code = utils.getCodeMail(cookie, "https://www.minuteinbox.com" + downloadUrl).Result;
                            }
                        }
                    }
                }
                while (code.Length != 6)
                {
                    mailInput = resRequest.getMailInbox(cookie).Result;
                    if (mailInput.Count != 0)
                    {
                        foreach (Mail mail in mailInput)
                        {
                            if (mail.predmet.Equals("Your secure OTP for transaction. Please do not share."))
                            {
                                HtmlDocument doc = new HtmlDocument();
                                doc.LoadHtml(mail.akce);

                                // Sử dụng XPath để lấy đường dẫn download-email
                                HtmlNode downloadLink = doc.DocumentNode.SelectSingleNode("//a[@title='Save this email']");
                                if (downloadLink != null)
                                {
                                    string downloadUrl = downloadLink.GetAttributeValue("href", "");
                                    code = utils.getCodeMail(cookie, "https://www.minuteinbox.com" + downloadUrl).Result;
                                }
                            }
                        }
                    }
                }
                IWebElement webElement = driver.FindElement(By.Id("addPhoneCodeForm"));
                Thread.Sleep(2000);
                List<WebElement> listInput = webElement.FindElements(By.XPath("//input[@name='/com/bn/reCaptcha/formHandler/ReCaptchaFormHandler.otpValue']")).Cast<WebElement>().ToList();
                Thread.Sleep(1000);
                foreach (WebElement element in listInput)
                {
                    try
                    {
                        element.SendKeys(code);
                    }
                    catch
                    {
                        Console.WriteLine("Error");
                    }
                }
                List<WebElement> listBtn = webElement.FindElements(By.XPath("//button[@class='btn verify-btn']")).Cast<WebElement>().ToList();
                Thread.Sleep(1000);
                foreach (WebElement element in listBtn)
                {
                    try
                    {
                        if (element.Text.Equals("Verify"))
                        {
                            element.Click();
                        }
                    }
                    catch (Exception err1) { }
                    {
                        Console.WriteLine("Error");
                    }
                    Thread.Sleep(1000);
                }
                text = "Đã login";
                return text;
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message.ToString());
            }
            try
            {
                driver.FindElement(By.Id("onetrust-close-btn-container")).Click();
                Thread.Sleep(2000);
            }
            catch { }
            driver.FindElement(By.XPath("//button[@class='navbar-toggler ']")).Click();
            Thread.Sleep(2000);
            try
            {
                driver.FindElement(By.Id("rhfCategoryFlyout_account")).Click();
                Thread.Sleep(2000);
            }
            catch { }

            
            try
            {
                text = driver.FindElement(By.XPath("//a[@class='rhf-modal-link rhf-atg-guest-user ']")).Text;
                Thread.Sleep(2000);
                if (!string.IsNullOrEmpty(text))
                {
                    text = "Chưa login";
                }
            }
            catch
            {
                try
                {
                    text = driver.FindElement(By.XPath("//span[@class='rhf-profile-text']")).Text;
                    Thread.Sleep(2000);
                    if (!string.IsNullOrEmpty(text))
                    {
                        text = "Đã login";
                    }
                }
                catch (Exception err)
                {
                    utils.WriteLogError(err.Message.ToString());
                }
            }
            return text;
        }

        public string loginBarnes(ChromeDriver driver,string cookie, string username, string password)
        {
            try
            {
                driver.Navigate().Refresh();
                _ = driver.Manage().Timeouts().ImplicitWait;
                Thread.Sleep(5000);
                try
                {
                    
                    driver.FindElement(By.Id("onetrust-close-btn-container")).Click();
                    Thread.Sleep(2000);
                }
                catch { }
                driver.FindElement(By.XPath("//button[@class='navbar-toggler ']")).Click();
                Thread.Sleep(2000);
                driver.FindElement(By.Id("rhfCategoryFlyout_account")).Click();
                Thread.Sleep(2000);
                driver.FindElement(By.XPath("//div[@class='New-modal-opener-link']")).Click();
                Thread.Sleep(2000);
                WebDriverWait driverWait1 = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                driverWait1.Until(ExpectedConditions.ElementExists(By.XPath("//iframe[@title='Sign in or Create an Account']")));
                IWebElement iframe = driver.FindElement(By.XPath("//iframe[@title='Sign in or Create an Account']"));
                driver.SwitchTo().Frame(iframe);
                Thread.Sleep(1000);
                driver.FindElement(By.Id("email")).SendKeys(username);
                Thread.Sleep(2000);
                driver.FindElement(By.Id("password")).SendKeys(password);
                Thread.Sleep(2000);
                driver.FindElement(By.XPath("//button[@class='btn btn--large']")).Click();
                Thread.Sleep(5000);
                _ = driver.Manage().Timeouts().ImplicitWait;
                driver.SwitchTo().DefaultContent();
                string checkLoginResult = checkLogin(driver,cookie);
                if (checkLoginResult != null)
                {
                    if (checkLoginResult.Equals("Đã login"))
                    {
                        return "Login thành công";
                    }
                    else
                    {
                        return "Login thất bại";
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception err)
            {
                utils.WriteLogError(err.Message.ToString());
                return null;
            }
        }

        public bool verifyCodeMail(ChromeDriver driver, string sessionID, string token)
        {
            string code = string.Empty;
            DropMailInput dropMailInput = resRequest.GetMailInput(sessionID, token).Result;
            if (dropMailInput != null)
            {
                SessionMailReceived sessionMail = dropMailInput.data.sessionMailReceived;
                if (sessionMail.downloadUrl != null)
                {
                    code = utils.getCodeMail(token,sessionMail.downloadUrl).Result;
                }
            }
            while(code.Length != 6)
            {
                dropMailInput = resRequest.GetMailInput(sessionID, token).Result;
                if (dropMailInput != null)
                {
                    SessionMailReceived sessionMail = dropMailInput.data.sessionMailReceived;
                    if (sessionMail.downloadUrl != null)
                    {
                        code = utils.getCodeMail(token, sessionMail.downloadUrl).Result;
                    }
                }
            }
            try
            {
                WebDriverWait driverWait1 = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                driverWait1.Until(ExpectedConditions.ElementExists(By.Id("addPhoneCodeForm")));
                IWebElement webElement = driver.FindElement(By.Id("addPhoneCodeForm"));
                Thread.Sleep(2000);
                List<WebElement> listInput = webElement.FindElements(By.XPath("//input[@name='/com/bn/reCaptcha/formHandler/ReCaptchaFormHandler.otpValue']")).Cast<WebElement>().ToList();
                Thread.Sleep(1000);
                foreach (WebElement element in listInput)
                {
                    try
                    {
                        element.SendKeys(code);
                    }
                    catch
                    {
                        Console.WriteLine("Error");
                    }
                }
                List<WebElement> listBtn = webElement.FindElements(By.XPath("//button[@class='btn verify-btn']")).Cast<WebElement>().ToList();
                Thread.Sleep(1000);
                foreach (WebElement element in listBtn)
                {
                    try
                    {
                        if (element.Text.Equals("Verify"))
                        {
                            element.Click();
                        }
                    }
                    catch (Exception err1) { }
                    {
                        Console.WriteLine("Error");
                    }
                    Thread.Sleep(1000);
                }
                try
                {
                    driverWait1.Until(ExpectedConditions.ElementExists(By.Id("successMsg")));
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message.ToString());
                return false;
               
            }
            
        }

        public void changePasswordFunc(ChromeDriver driver, string password, string newPassword)
        {
            try
            {
                driver.Navigate().GoToUrl("https://www.barnesandnoble.com/account/manage/settings/");
                _ = driver.Manage().Timeouts().ImplicitWait;
                Thread.Sleep(5000);
                WebDriverWait driverWait1 = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                driverWait1.Until(ExpectedConditions.ElementExists(By.Id("currPword"))).SendKeys(password);
                Thread.Sleep(1000);
                driver.FindElement(By.Id("newPword")).SendKeys(newPassword);
                Thread.Sleep(1000);
                driver.FindElement(By.Id("confNewPword")).SendKeys(newPassword);
                Thread.Sleep(1000);
                driver.FindElement(By.Id("btnSaveNewPassword")).Click();
            }
            catch
            {
            }
            
        }


        //signUpAccount
        public void signUpAccount(ChromeDriver driver, string email, string password, string firstName, string lastName)
        {
            try
            {
                driver.Navigate().Refresh();
                _ = driver.Manage().Timeouts().ImplicitWait;
                Thread.Sleep(5000);
                try
                {
                    driver.FindElement(By.Id("onetrust-close-btn-container")).Click();
                    Thread.Sleep(2000);
                }
                catch { }
                driver.FindElement(By.XPath("//button[@class='navbar-toggler ']")).Click();
                Thread.Sleep(2000);
                driver.FindElement(By.Id("rhfCategoryFlyout_account")).Click();
                Thread.Sleep(2000);
                driver.FindElement(By.XPath("//div[@class='New-modal-opener-link']")).Click();
                Thread.Sleep(2000);
                WebDriverWait driverWait1 = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                driverWait1.Until(ExpectedConditions.ElementExists(By.XPath("//iframe[@title='Sign in or Create an Account']")));
                IWebElement iframe = driver.FindElement(By.XPath("//iframe[@title='Sign in or Create an Account']"));
                driver.SwitchTo().Frame(iframe);
                Thread.Sleep(1000);
                driverWait1.Until(ExpectedConditions.ElementExists(By.Id("createAccountBtn"))).Click();
                Thread.Sleep(2000);
                driver.SwitchTo().DefaultContent();
                driverWait1.Until(ExpectedConditions.ElementExists(By.XPath("//iframe[@title='Create an Account']")));
                IWebElement iframe1 = driver.FindElement(By.XPath("//iframe[@title='Create an Account']"));
                driver.SwitchTo().Frame(iframe1);
                IWebElement element = driver.FindElement(By.Id("fName"));
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", element);
                element.SendKeys(firstName);
                Thread.Sleep(2000);
                element = driver.FindElement(By.Id("lName"));
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", element);
                element.SendKeys(lastName);
                Thread.Sleep(2000);
                element = driver.FindElement(By.Id("email"));
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", element);
                element.SendKeys(email);
                Thread.Sleep(2000);
                element = driver.FindElement(By.Id("password"));
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", element);
                element.SendKeys(password);
                Thread.Sleep(2000);
                element = driver.FindElement(By.Id("confPassword"));
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", element);
                element.SendKeys(password);
                Thread.Sleep(2000);
                element = driver.FindElement(By.XPath("//div[@class='select-menu select-menu--full-width']"));
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", element);
                element.Click();
                Thread.Sleep(1000);
                element = driver.FindElement(By.Id("securityQuestion-option-1"));
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", element);
                element.Click();
                Thread.Sleep(2000);
                element = driver.FindElement(By.Id("securityAnswer"));
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", element);
                element.SendKeys("New York");
                Thread.Sleep(2000);
                element = driver.FindElement(By.XPath("//span[@class='checkbox__box ']"));
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", element);
                element.Click();
                Thread.Sleep(2000);
                element = driver.FindElement(By.Id("btnCreateAccountDummy"));
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", element);
                element.Click();
                Thread.Sleep(10000);
                _ = driver.Manage().Timeouts().ImplicitWait;
                driver.SwitchTo().DefaultContent();
            }
            catch (Exception err)
            {
                utils.WriteLogError(err.Message.ToString());
            }
        }

        public bool ViewShoppingCart(ChromeDriver driver)
        {
            try
            {
                //https://www.barnesandnoble.com/?cart-redirect=true
                driver.Navigate().GoToUrl("https://www.barnesandnoble.com/?cart-redirect=true");
                _ = driver.Manage().Timeouts().ImplicitWait;
                WebDriverWait driverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                IWebElement element = null;
                try
                {
                    element = driver.FindElement(By.Name("/atg/commerce/order/purchase/CartModifierFormHandler.checkout"));
                }
                catch { }
                while (element == null)
                {
                    Thread.Sleep(5000);
                    try
                    {
                        element = driver.FindElement(By.Name("/atg/commerce/order/purchase/CartModifierFormHandler.checkout"));
                    }
                    catch { }

                }
                try
                {
                    IJavaScriptExecutor javascript = (IJavaScriptExecutor)driver;
                    javascript.ExecuteScript("arguments[0].click();", element);
                }
                catch
                {

                }
                return true;
            }
            catch (Exception err)
            {
                utils.WriteLogError(err.Message.ToString());
                return false;
            }
        }

        public bool buyBookAndCheckout(ChromeDriver driver, int index)
        {
            try
            {
                Thread.Sleep(2000);
                driver.Navigate().GoToUrl("https://www.barnesandnoble.com/b/books/_/N-1fZ29Z8q8");
                _ = driver.Manage().Timeouts().ImplicitWait;
                List<IWebElement> listBook = driver.FindElements(By.XPath("//input[@value='ADD TO CART']")).Cast<IWebElement>().ToList();
                Random random = new Random();
                Thread.Sleep(2000);
                listBook[random.Next(listBook.Count - 1)].Click();
                return true;
            }
            catch (Exception err)
            {
                utils.WriteLogError(err.Message.ToString());
                return false;
            }
        }

        public bool startAddVisaAndCheck(ChromeDriver driver, int index, string[] visa)
        {
            try
            {
                IWebElement element = driver.FindElement(By.Id("editRegCheckoutPayment"));
                IJavaScriptExecutor javascript = (IJavaScriptExecutor)driver;
                javascript.ExecuteScript("arguments[0].click();", element);
                Thread.Sleep(2000);
                try
                {
                    WebDriverWait driverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                    driverWait.Until(ExpectedConditions.ElementExists(By.XPath("//label[@id='addPaymentRadio']/span")));
                    element = driver.FindElement(By.XPath("//label[@id='addPaymentRadio']/span"));
                    javascript.ExecuteScript("arguments[0].click();", element);
                }
                catch { }
                driver.FindElement(By.Id("ccNumber")).Clear();
                Thread.Sleep(500);
                driver.FindElement(By.Id("ccNumber")).SendKeys(visa[0]);
                Thread.Sleep(1000);
                string firtName = Faker.Name.First();
                string lastName = Faker.Name.Last();
                driver.FindElement(By.Id("nameOnCard")).Clear();
                Thread.Sleep(500);
                driver.FindElement(By.Id("nameOnCard")).SendKeys(firtName + " " + lastName);
                Thread.Sleep(1000);
                IWebElement element1 = driver.FindElement(By.Id("ccMonth-replacement"));
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", element1);
                driver.FindElement(By.Id("ccMonth-replacement")).Click();
                List<IWebElement> mmElement = driver.FindElements(By.XPath("//ul[@id='ccMonth-option-list']/li/a")).Cast<IWebElement>().ToList();
                foreach (var mm in mmElement)
                {
                    if (mm.Text.Contains(visa[1]))
                    {
                        driver.ExecuteScript("arguments[0].scrollIntoView(true);", mm);
                        Thread.Sleep(500);
                        mm.Click();
                        break;
                    }
                }
                Thread.Sleep(1000);
                element1 = driver.FindElement(By.Id("ccYear-replacement"));
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", element1);
                Thread.Sleep(500);
                element1.Click();
                List<IWebElement> yyElement = driver.FindElements(By.XPath("//ul[@id='ccYear-option-list']/li/a")).Cast<IWebElement>().ToList();
                foreach (var yy in yyElement)
                {
                    if (yy.Text.Equals(visa[2]))
                    {
                        driver.ExecuteScript("arguments[0].scrollIntoView(true);", yy);
                        Thread.Sleep(500);
                        yy.Click();
                        break;
                    }
                }
                Thread.Sleep(1000);
                element1 = driver.FindElement(By.Id("csv"));
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", element1);
                Thread.Sleep(500);
                driver.FindElement(By.Id("csv")).SendKeys("111");
                Thread.Sleep(1000);
                element1 = driver.FindElement(By.XPath("//input[@class='btn btn--full-width order-summary__btn--checkout p-0']"));
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", element1);
                Thread.Sleep(1000);
                element1.Click();
                Thread.Sleep(1000);
                _ = driver.Manage().Timeouts().ImplicitWait;
                return true;
            }
            catch (Exception err)
            {
                utils.WriteLogError(err.Message.ToString());
                return false;
            }
        }

        public void addShippingAgain(ChromeDriver driver)
        {
            try
            {

                IWebElement element1 = null;
                Thread.Sleep(2000);
                element1 = driver.FindElement(By.XPath("//span[@data-trigger='Use the Address You Entered']"));
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", element1);
                Thread.Sleep(500);
                element1.Click();
                WebDriverWait driverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                try
                {
                    driverWait.Until(ExpectedConditions.ElementExists(By.XPath("//div[@class='modal__dialog focus']/div/div[3]/button")));
                    element1 = driver.FindElement(By.XPath("//div[@class='modal__dialog focus']/div/div[3]/button"));
                    driver.ExecuteScript("arguments[0].scrollIntoView(true);", element1);
                    Thread.Sleep(1000);
                    element1.Click();
                    Thread.Sleep(5000);
                }
                catch
                {

                }
                _ = driver.Manage().Timeouts().ImplicitWait; ;
            }
            catch { }
        }

        public void addShipping(ChromeDriver driver, Information infor, int index)
        {
            try
            {
                IWebElement element1 = null;
                Thread.Sleep(1000);
                element1 = driver.FindElement(By.Name("/atg/commerce/order/purchase/ShippingInfoFormHandler.shipContactInfo.address1"));
                Thread.Sleep(500);
                driver.FindElement(By.Name("/atg/commerce/order/purchase/ShippingInfoFormHandler.shipContactInfo.address1")).SendKeys(infor.address.street.Substring(2));
                Thread.Sleep(1000);
                element1 = driver.FindElement(By.Name("/atg/commerce/order/purchase/ShippingInfoFormHandler.shipContactInfo.city"));
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", element1);
                Thread.Sleep(500);
                driver.FindElement(By.Name("/atg/commerce/order/purchase/ShippingInfoFormHandler.shipContactInfo.city")).SendKeys(infor.address.city.Substring(2));
                Thread.Sleep(1000);
                element1 = driver.FindElement(By.XPath("//a[@title='State<sup>*<sup></sup></sup>']"));
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", element1);
                element1.Click();
                Thread.Sleep(500);
                List<IWebElement> stateElement = driver.FindElements(By.XPath("//ul[@id='state-option-list']/li/a")).Cast<IWebElement>().ToList();
                foreach (var state in stateElement)
                {
                    if (state.Text.ToLower().Equals(infor.address.state.ToLower().Substring(2)))
                    {
                        state.Click();
                        break;
                    }
                }
                Thread.Sleep(1000);
                element1 = driver.FindElement(By.Name("/atg/commerce/order/purchase/ShippingInfoFormHandler.shipContactInfo.postalCode"));
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", element1);
                Thread.Sleep(500);
                driver.FindElement(By.Name("/atg/commerce/order/purchase/ShippingInfoFormHandler.shipContactInfo.postalCode")).SendKeys(infor.address.Zipcode.Substring(2));
                Thread.Sleep(2000);
                element1 = driver.FindElement(By.Name("/atg/commerce/order/purchase/ShippingInfoFormHandler.shipContactInfo.phoneNumber"));
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", element1);
                Thread.Sleep(500);
                driver.FindElement(By.Name("/atg/commerce/order/purchase/ShippingInfoFormHandler.shipContactInfo.phoneNumber")).SendKeys(infor.address.Countrycallingcode.Substring(2) + infor.address.Phonenumber.Substring(2));
                Thread.Sleep(1000);
                element1 = driver.FindElement(By.XPath("//input[@class='btn btn--matches-done']"));
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", element1);
                Thread.Sleep(1000);
                element1.Click();
                _ = driver.Manage().Timeouts().ImplicitWait;
                WebDriverWait driverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                try
                {
                    driverWait.Until(ExpectedConditions.ElementExists(By.Id("asEnteredNoMatch")));
                    element1 = driver.FindElement(By.Id("asEnteredNoMatch"));
                    driver.ExecuteScript("arguments[0].scrollIntoView(true);", element1);
                    Thread.Sleep(1000);
                    element1.Click();
                    Thread.Sleep(5000);
                }
                catch
                {
                    //div/div/div[3]/button
                    driverWait.Until(ExpectedConditions.ElementExists(By.XPath("//div[@class='modal__dialog focus']/div/div[3]/button")));
                    element1 = driver.FindElement(By.XPath("//div[@class='modal__dialog focus']/div/div[3]/button"));
                    driver.ExecuteScript("arguments[0].scrollIntoView(true);", element1);
                    Thread.Sleep(1000);
                    element1.Click();
                    Thread.Sleep(5000);
                }
                _ = driver.Manage().Timeouts().ImplicitWait; ;
            }
            catch { }
        }

        public bool addShippingAddress(ChromeDriver driver, int index)
        {
            try
            {
                IWebElement element1 = driver.FindElement(By.XPath("//div[@id='savedAdressGU']/header/a"));
                driver.ExecuteScript("arguments[0].scrollIntoView(true);", element1);
                Thread.Sleep(2000);
                element1.Click();
                Thread.Sleep(1000);
                WebDriverWait driverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                driverWait.Until(ExpectedConditions.ElementExists(By.Name("/atg/commerce/order/purchase/ShippingInfoFormHandler.shipContactInfo.firstName"))).SendKeys(Faker.Name.First());
                Thread.Sleep(1000);
                driver.FindElement(By.Name("/atg/commerce/order/purchase/ShippingInfoFormHandler.shipContactInfo.lastName")).SendKeys(Faker.Name.Last());
                Thread.Sleep(1000);
                Information infor = resRequest.getInformation();
                int k = 0;
                while (infor == null)
                {
                    if (k > 5)
                    {
                        break;
                    }
                    infor = resRequest.getInformation();
                }
                if (infor != null)
                {
                    addShipping(driver, infor, index);
                    IWebElement errElement = null;
                    Thread.Sleep(1000);
                    try
                    {
                        errElement = driver.FindElement(By.XPath("//aside[@class='alert alert--error']"));
                    }
                    catch { }
                    while (errElement != null)
                    {

                        addShippingAgain(driver);
                        try
                        {
                            errElement = driver.FindElement(By.XPath("//aside[@class='alert alert--error']"));
                        }
                        catch
                        {
                            errElement = null;
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception err)
            {
                utils.WriteLogError(err.Message.ToString());
                return false;
            }
        }
    }
}
