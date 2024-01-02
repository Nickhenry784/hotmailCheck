using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hotmailCheck.Models;
using Keys = OpenQA.Selenium.Keys;
using System.Windows.Forms;

namespace hotmailCheck.Controllers
{
    public class HotmailAutomatic
    {
        public ResRequest resRequest = new ResRequest();
        SaveData saveData = new SaveData();

        public void addLogToDataGridView1(DataGridView dataGridView1, int indexRow, int indexCell, string log)
        {
            dataGridView1.Invoke(new Action(() =>
            {
                dataGridView1.Rows[indexRow].Cells[indexCell].Value = log;
            }));
        }

        public bool fakeAddress(ChromeDriver driver, int index, DataGridView dataGridView1)
        {
            addLogToDataGridView1(dataGridView1,index, 6, "Send Name");
            driver.FindElement(By.Id("accountHolderName")).SendKeys(Faker.Name.First() + " " + Faker.Name.Last());
            Thread.Sleep(2000);
            addLogToDataGridView1(dataGridView1,index, 6, "Get Address Us");
            Information infor = resRequest.getInformation();
            if (infor == null)
            {
                return false;
            }
            try
            {
                addLogToDataGridView1(dataGridView1,index, 6, "Send Address");
                driver.FindElement(By.Id("address_line1")).Clear();
                driver.FindElement(By.Id("address_line1")).SendKeys(infor.address.street.Substring(2));
                Thread.Sleep(1000);
                addLogToDataGridView1(dataGridView1,index, 6, "Send City");
                driver.FindElement(By.Id("city")).Clear();
                driver.FindElement(By.Id("city")).SendKeys(infor.address.city.Substring(2));
                Thread.Sleep(1000);
                addLogToDataGridView1(dataGridView1,index, 6, "Click State");
                driver.FindElement(By.Id("input_region-option")).Click();
                Thread.Sleep(5000);
                List<IWebElement> webElements = driver.FindElements(By.XPath("//span[@data-automationid='splitbuttonprimary']")).Cast<IWebElement>().ToList();
                int k = 0;
                for (int i = 0; i < webElements.Count; i++)
                {
                    IWebElement webElement = webElements[i];
                    string text = webElement.Text.ToLower();
                    string state = infor.address.state.ToLower().Substring(2);
                    if (text.Contains(state))
                    {
                        webElement.Click();
                        k = i;
                        break;
                    }
                }
                if (k == 0)
                {
                    addLogToDataGridView1(dataGridView1,index, 6, "Có lỗi thoát Thread!");
                    webElements[0].Click();
                    return false;
                }
                else
                {
                    Thread.Sleep(1000);
                    addLogToDataGridView1(dataGridView1,index, 6, "Send ZipCode");
                    driver.FindElement(By.Id("postal_code")).Clear();
                    driver.FindElement(By.Id("postal_code")).SendKeys(infor.address.Zipcode.Substring(2));
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public void checkUrlAfterLogin(string url, ChromeDriver driver, int index, DataGridView dataGridView1)
        {
            addLogToDataGridView1(dataGridView1,index, 6, "Đang kiểm tra....");
            switch (url)
            {
                case string a when a.Contains("https://privacynotice.account.microsoft.com/notice?ru=https://login.live.com/"):
                    try
                    {
                        addLogToDataGridView1(dataGridView1,index, 6, "Click Continue");
                        driver.FindElement(By.XPath("//span[@data-automationid='splitbuttonprimary']")).Click();
                        Thread.Sleep(1000);
                    }
                    catch { }
                    break;
                case string b when b.Contains("https://login.live.com/login.srf?id="):
                    try
                    {
                        addLogToDataGridView1(dataGridView1,index, 6, "Click Yes/No");
                        try
                        {
                            new Actions(driver).SendKeys(Keys.Enter).Perform();
                            // driver.FindElement(By.Id("idBtn_Back")).Click();
                        }
                        catch
                        {
                            addLogToDataGridView1(dataGridView1,index, 6, "Không Click được");

                        }
                        Thread.Sleep(1000);
                    }
                    catch { }
                    break;
                case string b when b.Contains("https://login.live.com/login.srf?wa="):
                    try
                    {
                        try
                        {
                            IWebElement errorPassword = driver.FindElement(By.Id("passwordError"));
                            if (errorPassword != null)
                            {
                                addLogToDataGridView1(dataGridView1,index, 6, "Sai Pass");
                                break;
                            }
                        }
                        catch { }
                        addLogToDataGridView1(dataGridView1,index, 6, "Click Yes/No");
                        try
                        {
                            new Actions(driver).SendKeys(Keys.Enter).Perform();
                            // driver.FindElement(By.Id("idBtn_Back")).Click();
                        }
                        catch
                        {
                            addLogToDataGridView1(dataGridView1,index, 6, "Không Click được");

                        }

                        Thread.Sleep(1000);
                    }
                    catch { }

                    break;
                case string c when c.Contains("https://login.live.com/ppsecure/post.srf"):
                    try
                    {
                        IWebElement errorLogin = driver.FindElement(By.Id("idTD_Error"));
                        if (errorLogin.Text.Equals("Sign-in is blocked"))
                        {
                            addLogToDataGridView1(dataGridView1,index, 6, "Sign-in is blocked");
                            break;
                        }
                    }
                    catch
                    {
                        try
                        {
                            IWebElement errorLogin = driver.FindElement(By.Id("main-frame-error"));
                            if (errorLogin != null)
                            {
                                driver.Navigate().Refresh();
                            }
                        }
                        catch { }

                    }
                    try
                    {
                        IWebElement errorPassword = driver.FindElement(By.Id("passwordError"));
                        if (errorPassword != null)
                        {
                            addLogToDataGridView1(dataGridView1,index, 6, "Sai Pass");
                            break;
                        }
                    }
                    catch { }
                    try
                    {
                        driver.FindElement(By.Id("idBtn_Back")).Click();

                    }
                    catch { }
                    try
                    {
                        new Actions(driver).SendKeys(Keys.Enter).Perform();
                    }
                    catch { }
                    Thread.Sleep(1000);
                    break;
                case string d when d.Contains("https://account.live.com/recover?mkt=EN-US&uiflavor=web&cobrandid="):
                    addLogToDataGridView1(dataGridView1,index, 6, "Verify");
                    break;
                case string e when e.Contains("https://account.live.com/identity/confirm?mkt=EN-US&uiflavor=web"):
                    try
                    {
                        string errorCode = driver.FindElement(By.Id("iTimeoutTitle")).Text;
                        if (errorCode.Contains("We didn't hear from you"))
                        {
                            addLogToDataGridView1(dataGridView1,index, 6, "Verify");
                            break;
                        }
                    }
                    catch { }
                    try
                    {
                        driver.SwitchTo().NewWindow(WindowType.Tab);
                        driver.Navigate().GoToUrl("https://support.microsoft.com/en-US");
                        Thread.Sleep(5000);
                        IWebElement picture = driver.FindElement(By.Id("mectrl_main_body"));
                        if (picture == null)
                        {
                            driver.Navigate().Refresh();
                            Thread.Sleep(5000);
                            picture = driver.FindElement(By.Id("mectrl_main_body"));
                        }
                        driver.Navigate().GoToUrl("https://account.microsoft.com/");
                        Thread.Sleep(8000);
                    }
                    catch { }
                    break;
                case string g when g.Contains("https://account.live.com/tou/accrue?mkt=EN-US&uiflavor=web&cobrandid="):
                    try
                    {

                        driver.FindElement(By.Id("iNext")).Click();
                        addLogToDataGridView1(dataGridView1,index, 6, "Click Next");
                        Thread.Sleep(1000);
                    }
                    catch { }
                    break;
                case string h when h.Contains("https://account.live.com/proofs/remind?mkt=EN-US&uiflavor=web&cobrandid="):
                    try
                    {
                        driver.FindElement(By.Id("iLooksGood")).Click();
                    }
                    catch { }
                    Thread.Sleep(1000);
                    break;
                case string h when h.Contains("https://account.live.com/ar/cancel?mkt=EN-US&uiflavor=web&cobrandid="):
                    try
                    {
                        driver.FindElement(By.Id("iLandingViewAction")).Click();
                    }
                    catch
                    {
                        Console.WriteLine("Oke");
                    }
                    break;
                case string h when h.Equals("https://login.live.com/"):
                    try
                    {
                        string text = driver.FindElement(By.Id("passwordError")).Text;
                        if (text.Equals("Please enter the password for your Microsoft account."))
                        {
                            addLogToDataGridView1(dataGridView1,index, 6, "Nhập lại password!");
                            try
                            {
                                WebDriverWait driverWait1 = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                                Thread.Sleep(2000);
                                driverWait1.Until(ExpectedConditions.ElementExists(By.Name("passwd"))).SendKeys(dataGridView1.Rows[index].Cells[2].Value.ToString());
                            }
                            catch
                            {
                                addLogToDataGridView1(dataGridView1,index, 6, "Không thể nhập Password");
                                try
                                {
                                    saveData.updateStatusByID(dataGridView1.Rows[index].Cells[1].Value.ToString(), dataGridView1.Rows[index].Cells[6].Value.ToString());
                                }
                                catch { }
                                try
                                {
                                    driver.Close();
                                    driver.Quit();
                                    driver.Dispose();
                                }
                                catch { }
                                dataGridView1.Invoke(new Action(() =>
                                {
                                    dataGridView1.Rows[index].DefaultCellStyle.BackColor = Color.White;
                                }));
                            }
                            Thread.Sleep(2000);
                            addLogToDataGridView1(dataGridView1,index, 6, "Click Login");
                            try
                            {
                                WebDriverWait driverWait1 = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                                Thread.Sleep(2000);
                                driverWait1.Until(ExpectedConditions.ElementExists(By.Id("idSIButton9"))).Click();
                            }
                            catch
                            {
                                addLogToDataGridView1(dataGridView1,index, 6, "không thể click login");
                                Thread.Sleep(5000);
                                WebDriverWait driverWait1 = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                                driverWait1.Until(ExpectedConditions.ElementExists(By.Id("idSIButton9"))).Click();
                            }
                        }

                    }
                    catch
                    {
                        Console.WriteLine("No findelement");
                    }
                    try
                    {
                        //usernameError
                        string text = driver.FindElement(By.Id("usernameError")).Text;
                        if (text.Equals("Enter a valid email address, phone number, or Skype name."))
                        {
                            addLogToDataGridView1(dataGridView1,index, 6, "Nhập lại Username!");
                            driver.FindElement(By.Name("loginfmt")).SendKeys(dataGridView1.Rows[index].Cells[1].Value.ToString());
                            Thread.Sleep(2000);
                            addLogToDataGridView1(dataGridView1,index, 6, "Click Next");
                            driver.FindElement(By.Id("idSIButton9")).Click();
                            Thread.Sleep(5000);
                            try
                            {
                                WebDriverWait driverWait1 = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                                Thread.Sleep(2000);
                                driverWait1.Until(ExpectedConditions.ElementExists(By.Name("passwd"))).SendKeys(dataGridView1.Rows[index].Cells[2].Value.ToString());
                            }
                            catch
                            {

                            }
                            Thread.Sleep(2000);
                            addLogToDataGridView1(dataGridView1,index, 6, "Click Login");
                            try
                            {
                                WebDriverWait driverWait1 = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                                Thread.Sleep(2000);
                                driverWait1.Until(ExpectedConditions.ElementExists(By.Id("idSIButton9"))).Click();
                            }
                            catch
                            {

                            }
                        }
                    }
                    catch { }
                    break;
                case string h when h.Contains("https://account.live.com/apps/upsell?app=Authenticator&mkt=EN-US&uiflavor=web&cobrandid="):
                    try
                    {
                        driver.FindElement(By.Id("iCancel")).Click();
                    }
                    catch
                    {
                        Console.WriteLine("Oke");
                    }
                    Thread.Sleep(1000);
                    break;
                case string h when h.Contains("https://account.live.com/Abuse?mkt=EN-US&uiflavor=web&cobrandid="):
                    addLogToDataGridView1(dataGridView1,index, 6, "Sign-in is blocked");
                    break;
                case string h when h.Contains("https://account.live.com/proofs/Verify?mkt=EN-US&uiflavor=web"):
                    addLogToDataGridView1(dataGridView1,index, 6, "Verify");
                    break;
                case string h when h.Contains("https://account.live.com/proofs/Add?mkt=EN-US&uiflavor=web"):
                    addLogToDataGridView1(dataGridView1,index, 6, "Verify");
                    ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight - 100)");
                    Thread.Sleep(1000);
                    try
                    {
                        IWebElement Signinbutton = driver.FindElement(By.Id("iShowSkip"));
                        IJavaScriptExecutor javascriptExecutor = (IJavaScriptExecutor)driver;
                        javascriptExecutor.ExecuteScript("arguments[0].click();", Signinbutton);
                    }
                    catch (Exception er)
                    {
                        Console.WriteLine($"{er.Message}");
                    }
                    Thread.Sleep(1000);
                    break;
            }
        }
    }
}
