using hotmailCheck.Controllers;
using hotmailCheck.Utils;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome.ChromeDriverExtensions;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using hotmailCheck.Models;
using hotmailCheck.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Security.Cryptography;
using FoxLearn.License;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Net;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.DevTools;
using Cookie = OpenQA.Selenium.Cookie;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace hotmailCheck
{
    public partial class Dashboard : Form
    {
        UtilsController utils = new UtilsController();
        SaveData saveData = new SaveData();
        HotmailAutomatic hotmailAutomatic = new HotmailAutomatic();
        ResRequest resRequest = new ResRequest();
        Queue<int> listCC;
        Queue<int> listRun;
        Queue<string> listProxy;
        List<string> listUserAgent;
        bool isRun = false;
        string testCokkies = string.Empty;

        public Dashboard()
        {
            InitializeComponent();
        }

        #region

        public void addLogToDataGridView1(int indexRow, int indexCell, string log)
        {
            dataGridView1.Invoke(new Action(() =>
            {
                dataGridView1.Rows[indexRow].Cells[indexCell].Value = log;
            }));
        }

        public void addLogToDataGridView4(int indexRow, int indexCell, string log)
        {
            dataGridView4.Invoke(new Action(() =>
            {
                dataGridView4.Rows[indexRow].Cells[indexCell].Value = log;
            }));
        }

        public void addLogToDataGridView5(int indexRow, int indexCell, string log)
        {
            dataGridView5.Invoke(new Action(() =>
            {
                dataGridView5.Rows[indexRow].Cells[indexCell].Value = log;
            }));
        }


        public void addLogToDataGridView2(int indexRow, string log)
        {
            dataGridView2.Invoke(new Action(() =>
            {
                dataGridView2.Rows[indexRow].Cells[1].Value = log;
            }));
        }

        public void randomTime()
        {
            Random random = new Random();
            int rdn = random.Next((int)numericUpDown1.Value, (int)numericUpDown2.Value);
            Thread.Sleep(rdn * 1000);
        }

        public void loginHotmailAndAddCC(int thread, int posX, int posY)
        {
            while (listRun.Count > 0)
            {
                if (!isRun)
                {
                    return;
                }
                int index = -1;
                try
                {
                    Thread.Sleep(500 * thread);
                    index = listRun.Dequeue();
                }
                catch
                {
                }
                if (index < 0)
                {
                    return;
                }
                dataGridView1.Invoke(new Action(() =>
                {
                    dataGridView1.Rows[index].DefaultCellStyle.BackColor = Color.Yellow;
                }));
                string username = dataGridView1.Rows[index].Cells[1].Value.ToString();
                string password = dataGridView1.Rows[index].Cells[2].Value.ToString();
                Thread.Sleep(thread * 1000);
                addLogToDataGridView1(index, 6, "Setting Chrome!");
                ChromeDriverService cService = ChromeDriverService.CreateDefaultService();
                cService.HideCommandPromptWindow = true;
                var options = new ChromeOptions();
                options.AddArgument("disable-infobars");
                options.AddArgument("--blink-settings=imagesEnabled=false");
                options.AddExcludedArgument("enable-automation");
                options.AddArgument("--window-size=600,400");
                options.AddArgument("--no-sandbox");
                options.AddArguments("--disable-notifications"); // to disable notification
                options.AddArguments("--disable-application-cache"); // to disable cache
                options.AddUserProfilePreference("profile.default_content_setting_values.notifications", 2);
                if (!string.IsNullOrEmpty(dataGridView1.Rows[index].Cells[4].Value?.ToString()))
                {
                    string[] proxy = dataGridView1.Rows[index].Cells[4].Value.ToString().Split('|');
                    if (dataGridView1.Rows[index].Cells[4].Value.ToString().Contains(':'))
                    {
                        proxy = dataGridView1.Rows[index].Cells[4].Value.ToString().Split(':');
                    }
                    if (proxy.Length > 2)
                    {
                        options.AddHttpProxy(proxy[0], int.Parse(proxy[1]), proxy[2], proxy[3]);
                    }
                    else
                    {
                        options.AddArgument("--proxy-server=" + dataGridView1.Rows[index].Cells[4].Value?.ToString());
                    }
                }

                ChromeDriver driver = new ChromeDriver(cService, options);
                if (driver != null)
                {
                    try
                    {
                        addLogToDataGridView1(index, 6, "Setting Chrome Thành Công");
                        driver.Manage().Cookies.DeleteAllCookies();
                        driver.Manage().Window.Position = new Point(posX, posY);
                        Thread.Sleep(1000);
                        driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(120));
                        addLogToDataGridView1(index, 6, "Go to Link: hotmail.com");
                        driver.Url = "https://login.live.com/";
                        _ = driver.Manage().Timeouts().ImplicitWait;
                        randomTime();
                        addLogToDataGridView1(index, 6, "Send Username");
                        driver.FindElement(By.Name("loginfmt")).SendKeys(username);
                        Thread.Sleep(2000);
                        addLogToDataGridView1(index, 6, "Click Next");
                        driver.FindElement(By.Id("idSIButton9")).Click();
                        randomTime();
                        IWebElement errorElement = null;
                        try
                        {
                            errorElement = driver.FindElement(By.Id("usernameError"));
                        }
                        catch { }
                        if (errorElement != null)
                        {
                            addLogToDataGridView1(index, 6, "Sign-in is blocked");
                            try
                            {
                                saveData.updateStatusByID(username, dataGridView1.Rows[index].Cells[6].Value.ToString());
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
                            continue;
                        }
                        addLogToDataGridView1(index, 6, "Send Password");
                        try
                        {
                            WebDriverWait driverWait1 = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                            Thread.Sleep(2000);
                            driverWait1.Until(ExpectedConditions.ElementExists(By.Name("passwd"))).SendKeys(password);
                        }
                        catch
                        {
                            addLogToDataGridView1(index, 6, "Không thể nhập Password");
                            try
                            {
                                saveData.updateStatusByID(username, dataGridView1.Rows[index].Cells[6].Value.ToString());
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
                            continue;
                        }
                        Thread.Sleep(2000);
                        addLogToDataGridView1(index, 6, "Click Login");
                        try
                        {
                            WebDriverWait driverWait1 = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                            Thread.Sleep(2000);
                            driverWait1.Until(ExpectedConditions.ElementExists(By.Id("idSIButton9"))).Click();
                        }
                        catch
                        {
                            try
                            {
                                driver.Navigate().Refresh();
                                addLogToDataGridView1(index, 6, "không thể click login");
                                randomTime();
                                WebDriverWait driverWait1 = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                                Thread.Sleep(2000);
                                driverWait1.Until(ExpectedConditions.ElementExists(By.Name("passwd"))).SendKeys(password);
                                Thread.Sleep(2000);
                                driver.FindElement(By.Id("idSIButton9")).Click();
                            }
                            catch { }

                        }
                        randomTime();
                        string url = driver.Url;
                        addLogToDataGridView1(index, 6, "Check Login");
                        while (!url.Contains("/?refd=account.microsoft.com") || !url.Contains("/?lang=en-US&refd=account.live.com"))
                        {
                            addLogToDataGridView1(index, 6, driver.Url);
                            hotmailAutomatic.checkUrlAfterLogin(url, driver, index, dataGridView1);
                            Thread.Sleep(5000);
                            if (url.Contains("/?refd=account.microsoft.com") || url.Contains("/?lang=en-US&refd=account.live.com"))
                            {
                                break;
                            }
                            else if (dataGridView1.Rows[index].Cells[6].Value.ToString().Equals("Sign-in is blocked") || dataGridView1.Rows[index].Cells[6].Value.ToString().Equals("Sai Pass") || dataGridView1.Rows[index].Cells[6].Value.ToString().Equals("Verify"))
                            {
                                try
                                {
                                    saveData.updateStatusByID(username, dataGridView1.Rows[index].Cells[6].Value.ToString());
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
                                break;
                            }
                            url = driver.Url;
                        }
                        if (dataGridView1.Rows[index].Cells[6].Value.ToString().Equals("Sign-in is blocked") || dataGridView1.Rows[index].Cells[6].Value.ToString().Equals("Sai Pass") || dataGridView1.Rows[index].Cells[6].Value.ToString().Equals("Verify"))
                        {
                            try
                            {
                                saveData.updateStatusByID(username, dataGridView1.Rows[index].Cells[6].Value.ToString());
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
                            continue;
                        }
                        Thread.Sleep(2000);
                        try
                        {
                            saveData.updateStatusByID(username, "Success");
                        }
                        catch { }
                        dataGridView1.Invoke(new Action(() =>
                        {
                            dataGridView1.Rows[index].Cells[3].Value = "Success";
                            dataGridView1.Rows[index].DefaultCellStyle.BackColor = Color.LightGreen;
                        }));
                        addLogToDataGridView1(index, 6, "Success");
                        driver.Navigate().GoToUrl("https://account.microsoft.com/billing/payments?fref=home.drawers.payment-options.cold-add-payment");
                        randomTime();
                        _ = driver.Manage().Timeouts().ImplicitWait;
                        Thread.Sleep(1000);
                        int addVisa = getQualityAdd(username);
                        while (addVisa < (int)numericUpDown12.Value)
                        {
                            int indexVisa = -1;
                            try
                            {
                                indexVisa = listCC.Dequeue();
                            }
                            catch { }
                            if (indexVisa < 0)
                            {
                                addLogToDataGridView1(index, 6, "Đã hết CC. Vui lòng add vào");
                                return;
                            }
                            IJavaScriptExecutor javascriptExecutor = (IJavaScriptExecutor)driver;
                            try
                            {
                                driver.FindElement(By.XPath("//i[@data-icon-name='Add']")).Click();
                            }
                            catch
                            {
                                driver.Navigate().Refresh();
                                Thread.Sleep(2000);
                                driver.FindElement(By.XPath("//i[@data-icon-name='Add']")).Click();
                            }

                            addLogToDataGridView1(index, 6, "Change United State");
                            //market-selector-dropdown-list199
                            WebDriverWait driverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                            driverWait.Until(ExpectedConditions.ElementExists(By.Id("market-selector-dropdown-option")));
                            IWebElement element1 = driver.FindElement(By.Id("market-selector-dropdown-option"));
                            javascriptExecutor.ExecuteScript("arguments[0].click();", element1);
                            Thread.Sleep(2000);
                            driverWait.Until(ExpectedConditions.ElementExists(By.Id("market-selector-dropdown-list199"))).Click();
                            Thread.Sleep(2000);
                            driverWait.Until(ExpectedConditions.ElementExists(By.Id("pidlddc-text-optionText_credit_card_visa_amex_mc_discover"))).Click();
                            randomTime();
                            bool fakeAddressBool = hotmailAutomatic.fakeAddress(driver, index, dataGridView1);
                            while (!fakeAddressBool)
                            {
                                fakeAddressBool = hotmailAutomatic.fakeAddress(driver, index, dataGridView1);
                            }
                            string[] visaArr = dataGridView2.Rows[indexVisa].Cells[0].Value.ToString().Split('|');
                            addLogToDataGridView1(index, 6, "Send Number");
                            driver.FindElement(By.Id("accountToken")).SendKeys(visaArr[0]);
                            Thread.Sleep(1000);
                            addLogToDataGridView1(index, 6, "Change Month");
                            driver.FindElement(By.Id("input_expiryMonth")).Click();
                            Thread.Sleep(1000);
                            driverWait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@data-automationid='splitbuttonprimary']")));
                            List<IWebElement> listMM = driver.FindElements(By.XPath("//span[@data-automationid='splitbuttonprimary']")).Cast<IWebElement>().ToList();
                            foreach (IWebElement element in listMM)
                            {
                                if (element.Text.ToString().Equals(visaArr[1]))
                                {

                                    javascriptExecutor.ExecuteScript("arguments[0].click();", element);
                                    break;
                                }
                            }
                            Thread.Sleep(1000);
                            addLogToDataGridView1(index, 6, "Change Year");
                            driver.FindElement(By.Id("input_expiryYear")).Click();
                            Thread.Sleep(1000);
                            driverWait.Until(ExpectedConditions.ElementExists(By.XPath("//span[@data-automationid='splitbuttonprimary']")));
                            List<IWebElement> listYY = driver.FindElements(By.XPath("//span[@data-automationid='splitbuttonprimary']")).Cast<IWebElement>().ToList();
                            string year = visaArr[2].Substring(2);
                            foreach (IWebElement element in listYY)
                            {
                                if (element.Text.ToString().Contains(year))
                                {
                                    IJavaScriptExecutor javascript = (IJavaScriptExecutor)driver;
                                    javascriptExecutor.ExecuteScript("arguments[0].click();", element);
                                    break;
                                }
                            }
                            Thread.Sleep(1000);
                            addLogToDataGridView1(index, 6, "Click Save");
                            driver.FindElement(By.Id("pidlddc-button-saveButton")).Click();
                            randomTime();
                            addLogToDataGridView1(index, 6, "Check Status");
                            try
                            {
                                randomTime();
                                WebDriverWait driverWait1 = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                                driverWait1.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[@data-automation-id='error-message']")));
                                string messager = driver.FindElement(By.XPath("//span[@data-automation-id='error-message']")).Text;
                                switch (messager)
                                {
                                    case "Check your security code. There appears to be an error in it.":
                                        saveData.updateStatusCC(dataGridView2.Rows[indexVisa].Cells[0].Value.ToString(), "Success");

                                        dataGridView2.Invoke(new Action(() =>
                                        {
                                            dataGridView2.Rows[indexVisa].Cells[1].Value = "Success";
                                            dataGridView2.Rows[indexVisa].DefaultCellStyle.BackColor = Color.LightGreen;
                                        }));
                                        try
                                        {


                                            addLogToDataGridView1(index, 6, "Save File");
                                            utils.WriteDebug(dataGridView2.Rows[indexVisa].Cells[0].Value.ToString());
                                        }
                                        catch { }
                                        addVisa++;
                                        saveData.updateQualityByID(username, addVisa.ToString());
                                        dataGridView1.Invoke(new Action(() =>
                                        {
                                            dataGridView1.Rows[index].Cells[5].Value = addVisa.ToString();
                                        }));
                                        break;
                                    case "Check the info you entered. It doesn't match the info for this card.":
                                        saveData.updateStatusCC(dataGridView2.Rows[indexVisa].Cells[0].Value.ToString(), "Failed");
                                        dataGridView2.Invoke(new Action(() =>
                                        {
                                            dataGridView2.Rows[indexVisa].Cells[1].Value = "Failed";
                                            dataGridView2.Rows[indexVisa].DefaultCellStyle.BackColor = Color.Red;
                                        }));
                                        addVisa++;
                                        saveData.updateQualityByID(username, addVisa.ToString());
                                        dataGridView1.Invoke(new Action(() =>
                                        {
                                            dataGridView1.Rows[index].Cells[5].Value = addVisa.ToString();
                                        }));
                                        break;
                                    case "Check the city in your address. There appears to be an error in it.":
                                        addLogToDataGridView1(index, 6, "Error City");
                                        listCC.Enqueue(indexVisa);
                                        break;
                                    case "Check your address. There appears to be an error in it.":
                                        addLogToDataGridView1(index, 6, "Error Address");
                                        listCC.Enqueue(indexVisa);
                                        break;
                                    case "Check the Zip or Postal code in your address. There appears to be an error in it.":
                                        addLogToDataGridView1(index, 6, "Error ZipCode");
                                        listCC.Enqueue(indexVisa);
                                        break;
                                    case "For security purposes, captcha verification is required.":
                                        addLogToDataGridView1(index, 6, "Dính captcha!");
                                        addVisa = 5;
                                        listCC.Enqueue(indexVisa);
                                        break;
                                    default:
                                        addLogToDataGridView1(index, 6, "Không phát hiện lỗi. Chạy lại");
                                        addVisa = 5;
                                        listCC.Enqueue(indexVisa);
                                        break;
                                }
                            }
                            catch
                            {
                                Console.WriteLine("Error");
                                try
                                {
                                    IWebElement webElement = driver.FindElement(By.XPath("//i[@data-icon-name='AlertSolid']"));
                                    if (webElement != null)
                                    {
                                        addLogToDataGridView1(index, 6, "Không thể add visa, Thoát account");
                                        addVisa = 5;
                                        listCC.Enqueue(indexVisa);
                                    }
                                }
                                catch
                                {

                                }
                            }
                            driver.FindElement(By.XPath("//i[@data-icon-name='Cancel']")).Click();
                            Thread.Sleep(1000);
                        }
                    }
                    catch (Exception err)
                    {
                        addLogToDataGridView1(index, 6, "Có lỗi thoát Thread!");
                        utils.WriteLogError(err.Message.ToString());
                        dataGridView1.Invoke(new Action(() =>
                        {
                            dataGridView1.Rows[index].DefaultCellStyle.BackColor = Color.White;
                        }));
                        try
                        {
                            driver.Close();
                            driver.Quit();
                            driver.Dispose();
                        }
                        catch { }

                    }
                    finally
                    {
                        try
                        {
                            driver.Close();
                            driver.Quit();
                            driver.Dispose();
                        }
                        catch { }
                    }

                }

            }
        }

        public void loginBarnesAndAddCC(int thread, int posX, int posY)
        {
            while (listRun.Count > 0)
            {
                if (!isRun)
                {
                    return;
                }
                int index = -1;
                try
                {
                    Thread.Sleep(500 * thread);
                    index = listRun.Dequeue();
                }
                catch
                {
                }
                if (index < 0)
                {
                    return;
                }
                dataGridView4.Invoke(new Action(() =>
                {
                    dataGridView4.Rows[index].DefaultCellStyle.BackColor = Color.Yellow;
                }));
                Thread.Sleep(thread * 1000);
                addLogToDataGridView4(index, 6, "Setting Chrome!");
                var options = new ChromeOptions();
                string userAgent = listUserAgent[Faker.RandomNumber.Next(0, listUserAgent.Count - 1)];
                options.AddArgument("--user-agent=" + userAgent);
                options.AddArgument("--blink-settings=imagesEnabled=false");
                options.AddArguments("--disable-notifications"); // to disable notification
                options.AddArguments("--disable-application-cache");
                options.AddArgument("--disable-features=PrivacySandboxSettings4");
                options.AddArgument("--disable-features=ChromeWhatsNewUI");
                if (!string.IsNullOrEmpty(dataGridView4.Rows[index].Cells[4].Value?.ToString()))
                {
                    string[] proxy = dataGridView4.Rows[index].Cells[4].Value.ToString().Split('|');
                    if (dataGridView4.Rows[index].Cells[4].Value.ToString().Contains(':'))
                    {
                        proxy = dataGridView4.Rows[index].Cells[4].Value.ToString().Split(':');
                    }
                    if (proxy.Length > 2)
                    {
                        options.AddHttpProxy(proxy[0], int.Parse(proxy[1]), proxy[2], proxy[3]);
                    }
                    else
                    {
                        options.AddArgument("--proxy-server=" + dataGridView4.Rows[index].Cells[4].Value?.ToString());
                    }
                }
                var driver = SeleniumUndetectedChromeDriver.UndetectedChromeDriver.Create(options: options, driverExecutablePath: Directory.GetCurrentDirectory() + "\\chromedriver.exe", hideCommandPromptWindow: true, suppressWelcome: false);
                string cookie = string.Empty;
                if (driver != null)
                {
                    try
                    {
                        addLogToDataGridView4(index, 6, "Setting Chrome Thành Công");
                        Thread.Sleep(1000);
                        driver.Manage().Window.Position = new Point(posX, posY);
                        Thread.Sleep(thread * 1000);
                        driver.Manage().Window.Size = new Size(600, 400);
                        driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(120));
                        addLogToDataGridView4(index, 6, "Get Cookie Page Mail");
                        driver.Navigate().GoToUrl("https://www.minuteinbox.com/");
                        _ = driver.Manage().Timeouts().ImplicitWait;
                        randomTime();
                        Dictionary<string, string> keyValues = utils.GetAllPageCookies(driver);
                        cookie = utils.ToDebugString(keyValues);
                        addLogToDataGridView4(index, 6, "Phục hồi Mail");
                        string username = dataGridView4.Rows[index].Cells[1].Value.ToString();
                        string mailPhucHoi = resRequest.returnEmail(cookie, username, dataGridView4.Rows[index].Cells[4].Value.ToString()).Result;
                        while (!mailPhucHoi.Equals("ok"))
                        {
                            addLogToDataGridView4(index, 6, "Phục hồi Mail lại");
                            cookie = mailPhucHoi;
                            mailPhucHoi = resRequest.returnEmail(cookie, username, dataGridView4.Rows[index].Cells[4].Value.ToString()).Result;
                        }
                        addLogToDataGridView4(index, 6, "Phục hồi Mail thành công");
                        Thread.Sleep(2000);
                        addLogToDataGridView4(index, 6, "Go to BarnesAndNoble");
                        driver.Navigate().GoToUrl("https://www.barnesandnoble.com/");
                        _ = driver.Manage().Timeouts().ImplicitWait;
                        randomTime();
                        addLogToDataGridView4(index, 6, "Login Account");
                        BarnesAutomatic barnesAutomatic = new BarnesAutomatic();

                        string password = dataGridView4.Rows[index].Cells[2].Value.ToString();
                        string resultAutomatic = barnesAutomatic.loginBarnes(driver, cookie, username, password, dataGridView4.Rows[index].Cells[4].Value.ToString());
                        if (!string.IsNullOrEmpty(resultAutomatic))
                        {
                            if (resultAutomatic.Equals("Login thành công"))
                            {
                                addLogToDataGridView4(index, 6, resultAutomatic);
                                addLogToDataGridView4(index, 3, "Success");
                                dataGridView4.Invoke(new Action(() =>
                                {
                                    dataGridView4.Rows[index].DefaultCellStyle.BackColor = Color.LightGreen;
                                }));
                                try
                                {
                                    saveData.updateStatusByUsernameBarnes(dataGridView4.Rows[index].Cells[1].Value.ToString(), "Success");
                                }
                                catch { }
                                Thread.Sleep(2000);
                            }
                        }
                        else
                        {
                            addLogToDataGridView4(index, 6, "Không thể đăng nhập hoặc có lỗi xảy ra");
                            dataGridView4.Invoke(new Action(() =>
                            {
                                dataGridView4.Rows[index].DefaultCellStyle.BackColor = Color.White;
                            }));
                            try
                            {
                                driver.Close();
                                driver.Quit();
                                driver.Dispose();
                            }
                            catch { }
                            continue;
                        }
                        driver.GoToUrl("https://www.barnesandnoble.com/");
                        //mua hang
                        try
                        {
                            addLogToDataGridView4(index, 6, "Kiểm tra giỏ hàng");
                            string numberBuy = driver.FindElement(By.XPath("//a[@id='navbarDropdown2']/span")).Text;
                            if (int.Parse(numberBuy) == 0)
                            {
                                bool buyBook = barnesAutomatic.buyBookAndCheckout(driver, index);
                                if (buyBook)
                                {
                                    addLogToDataGridView4(index, 6, "Mua hàng thành công!");
                                    Thread.Sleep(2000);
                                }
                                else
                                {
                                    addLogToDataGridView4(index, 6, "Có lỗi thoát Thread!");
                                    dataGridView4.Invoke(new Action(() =>
                                    {
                                        dataGridView4.Rows[index].DefaultCellStyle.BackColor = Color.White;
                                    }));
                                    try
                                    {
                                        driver.Close();
                                        driver.Quit();
                                        driver.Dispose();
                                    }
                                    catch { }
                                    continue;
                                }
                            }
                        }
                        catch (Exception err)
                        {
                            addLogToDataGridView4(index, 6, "Có lỗi khi mua hàng");
                            utils.WriteLogError(err.Message.ToString());
                            dataGridView4.Invoke(new Action(() =>
                            {
                                dataGridView4.Rows[index].DefaultCellStyle.BackColor = Color.White;
                            }));
                            try
                            {
                                driver.Close();
                                driver.Quit();
                                driver.Dispose();
                            }
                            catch { }
                            continue;
                        }

                        try
                        {
                            bool checkOut = barnesAutomatic.ViewShoppingCart(driver);
                            if (checkOut)
                            {
                                addLogToDataGridView4(index, 6, "View Shopping thành công");
                            }
                            else
                            {
                                addLogToDataGridView4(index, 6, "Có lỗi khi xem View Shopping Cart");
                                dataGridView4.Invoke(new Action(() =>
                                {
                                    dataGridView4.Rows[index].DefaultCellStyle.BackColor = Color.White;
                                }));
                                try
                                {
                                    driver.Close();
                                    driver.Quit();
                                    driver.Dispose();
                                }
                                catch { }
                                continue;
                            }
                        }
                        catch (Exception err)
                        {
                            addLogToDataGridView4(index, 6, "Có lỗi khi view shopping cart");
                            utils.WriteLogError(err.Message.ToString());
                            dataGridView4.Invoke(new Action(() =>
                            {
                                dataGridView4.Rows[index].DefaultCellStyle.BackColor = Color.White;
                            }));
                            try
                            {
                                driver.Close();
                                driver.Quit();
                                driver.Dispose();
                            }
                            catch { }
                            continue;
                        }

                        Thread.Sleep(2000);
                        //altPaymentErr
                        IWebElement checkError = null;
                        try
                        {
                            checkError = driver.FindElement(By.Id("errMissingInfo"));
                            if (checkError != null)
                            {
                                string textErr = checkError.FindElement(By.TagName("em")).Text;
                                if (textErr.Equals("Updates to your Shipping and Billing Information are required to complete this order."))
                                {
                                    addLogToDataGridView4(index, 6, "Add Shipping Address");
                                    bool addShipping = barnesAutomatic.addShippingAddress(driver, index);
                                    if (addShipping)
                                    {
                                        addLogToDataGridView4(index, 6, "Add Shipping Address Thành công");
                                    }
                                    else
                                    {
                                        addLogToDataGridView4(index, 6, "Add Shipping Address Thất bại");
                                        try
                                        {
                                            driver.Close();
                                            driver.Quit();
                                            driver.Dispose();
                                        }
                                        catch { }
                                        continue;
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                        try
                        {

                            int addVisa = getQualityAddBarnet(dataGridView4.Rows[index].Cells[1].Value.ToString());
                            _ = driver.Manage().Timeouts().ImplicitWait;
                            while (addVisa < (int)numericUpDown7.Value)
                            {
                                int indexVisa = -1;
                                try
                                {
                                    indexVisa = listCC.Dequeue();
                                }
                                catch { }
                                if (indexVisa < 0)
                                {
                                    addLogToDataGridView4(index, 6, "Đã hết CC. Vui lòng add vào");
                                    return;
                                }
                                string[] ccInformation = dataGridView3.Rows[indexVisa].Cells[0].Value.ToString().Split('|');
                                addLogToDataGridView4(index, 6, "Bắt đầu add CC");
                                bool startAddCC = barnesAutomatic.startAddVisaAndCheck(driver, index, ccInformation);
                                if (startAddCC)
                                {
                                    //check CC
                                    addLogToDataGridView4(index, 6, "Bắt đầu check");
                                    randomTime();
                                    _ = driver.Manage().Timeouts().ImplicitWait;
                                    string textErr = string.Empty;
                                    try
                                    {
                                        textErr = driver.FindElement(By.Id("altPaymentErr")).FindElement(By.TagName("em")).Text;
                                    }
                                    catch { }

                                    IJavaScriptExecutor javascript1 = (IJavaScriptExecutor)driver;
                                    while (string.IsNullOrEmpty(textErr))
                                    {
                                        IWebElement webElement = null;
                                        randomTime();
                                        try
                                        {
                                            webElement = driver.FindElement(By.Id("memberSubmitOrder"));
                                        }
                                        catch
                                        {
                                            webElement = driver.FindElement(By.Id("memberSubmitOrderCVVRequired"));
                                        }
                                        if (webElement != null)
                                        {
                                            try
                                            {
                                                javascript1.ExecuteScript("arguments[0].click();", webElement);
                                            }
                                            catch
                                            {
                                                Thread.Sleep(2000);
                                            }
                                            randomTime();
                                            try
                                            {
                                                WebDriverWait driverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                                                driverWait.Until(ExpectedConditions.ElementExists(By.Id("altPaymentErr")));
                                                textErr = driver.FindElement(By.Id("altPaymentErr")).FindElement(By.TagName("em")).Text;
                                                if (textErr != null)
                                                {
                                                    break;
                                                }
                                            }
                                            catch { }
                                            Thread.Sleep(2000);
                                            string text = string.Empty;
                                            try
                                            {
                                                text = driver.FindElement(By.Id("dialog-title")).Text;
                                                if (text != null)
                                                {
                                                    listCC.Enqueue(indexVisa);
                                                    textErr = "Không thể check visa";
                                                    break;
                                                }
                                            }
                                            catch
                                            {

                                            }
                                        }
                                    }
                                    Thread.Sleep(2000);
                                    if (textErr.Equals("Sorry! Your order cannot be processed due to an error with the Card Number entered. Please edit your card details and try again.") || textErr.Equals("The Security Code you entered is incorrect.\r\nPlease try again or contact Customer Service for assistance."))
                                    {
                                        try
                                        {
                                            saveData.updateStatusCC(dataGridView3.Rows[indexVisa].Cells[0].Value.ToString(), "Failed");
                                        }
                                        catch { }
                                        dataGridView3.Invoke(new Action(() =>
                                        {
                                            dataGridView3.Rows[indexVisa].Cells[1].Value = "Failed";
                                            dataGridView3.Rows[indexVisa].DefaultCellStyle.BackColor = Color.Red;
                                        }));
                                        addVisa++;
                                        saveData.updateQualityByUsernameBarnes(dataGridView4.Rows[index].Cells[1].Value.ToString(), addVisa.ToString());
                                        dataGridView4.Invoke(new Action(() =>
                                        {
                                            dataGridView4.Rows[index].Cells[5].Value = addVisa.ToString();
                                        }));
                                        Thread.Sleep(2000);
                                    }
                                    else if (textErr.Equals("Không thể check visa"))
                                    {
                                        try
                                        {
                                            saveData.updateStatusByUsernameBarnes(dataGridView4.Rows[index].Cells[1].Value.ToString(), "Failed");
                                        }
                                        catch { }
                                        break;
                                    }
                                    else if (textErr.Equals("We're sorry but the shipping zip code entered is invalid. Please enter a valid zip code."))
                                    {
                                        listCC.Enqueue(indexVisa);
                                        addLogToDataGridView4(index, 6, "Sai ZipCode");
                                        break;
                                    }
                                    else if (textErr.Equals("Sorry! Your order cannot be processed because the CVV entered is incorrect. Please update and try again."))
                                    {
                                        try
                                        {
                                            saveData.updateStatusCC(dataGridView3.Rows[indexVisa].Cells[0].Value.ToString(), "Success");
                                        }
                                        catch { }
                                        try
                                        {
                                            addLogToDataGridView4(index, 6, "Save File");
                                            utils.WriteDebugBarnes(dataGridView3.Rows[indexVisa].Cells[0].Value.ToString());
                                        }
                                        catch { }
                                        dataGridView3.Invoke(new Action(() =>
                                        {
                                            dataGridView3.Rows[indexVisa].Cells[1].Value = "Success";
                                            dataGridView3.Rows[indexVisa].DefaultCellStyle.BackColor = Color.LightGreen;
                                        }));
                                        addVisa++;
                                        saveData.updateQualityByUsernameBarnes(dataGridView4.Rows[index].Cells[1].Value.ToString(), addVisa.ToString());
                                        dataGridView3.Invoke(new Action(() =>
                                        {
                                            dataGridView4.Rows[index].Cells[5].Value = addVisa.ToString();
                                        }));
                                        Thread.Sleep(2000);
                                    }
                                }
                            }
                        }
                        catch { }

                    }
                    catch (Exception err)
                    {
                        resRequest.deleteEmail(cookie, dataGridView4.Rows[index].Cells[4].Value.ToString());
                        addLogToDataGridView4(index, 6, "Có lỗi không xác định, thoát Thread!");
                        utils.WriteLogError(err.Message.ToString());
                        dataGridView4.Invoke(new Action(() =>
                        {
                            dataGridView4.Rows[index].DefaultCellStyle.BackColor = Color.White;
                        }));
                        try
                        {
                            driver.Close();
                            driver.Quit();
                            driver.Dispose();
                        }
                        catch { }

                    }
                    finally
                    {
                        resRequest.deleteEmail(cookie, dataGridView4.Rows[index].Cells[4].Value.ToString());
                        try
                        {
                            driver.Close();
                            driver.Quit();
                            driver.Dispose();
                        }
                        catch { }
                    }

                }
            }
        }

        public int findIndexDataGridView(string email)
        {
            int index = -1;
            for (int i = 0; i < dataGridView5.Rows.Count; i++)
            {
                if (dataGridView5.Rows[i].Cells[0].Value.ToString().Equals(email))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        //signupBarnesAndAddCC
        public async void signupBarnesAndAddCC(int thread, int posX, int posY, bool signUpNoCheck)
        {
            while (listRun.Count < (int)numericUpDown15.Value)
            {
                if (!isRun)
                {
                    return;
                }
                if (listRun.Count > (int)numericUpDown15.Value)
                {
                    return;
                }
                Thread.Sleep(thread * 1000);
                string proxyItems = string.Empty;
                try
                {
                    proxyItems = listProxy.Dequeue();
                    try
                    {
                        saveData.deleteProxy(proxyItems);
                        Thread.Sleep(1000);
                        saveData.saveDataProxy(proxyItems);
                        listProxy.Enqueue(proxyItems);
                    }
                    catch { }
                }
                catch
                {
                }
                if (string.IsNullOrEmpty(proxyItems))
                {
                    return;
                }
                string fisrtName = utils.RemoveSpecialCharacters(Faker.Name.First());
                string lastName = utils.RemoveSpecialCharacters(Faker.Name.Last());
                string email = fisrtName + lastName + Faker.RandomNumber.Next(9999, 9999999);
                dataGridView5.Invoke(new Action(() =>
                {
                    dataGridView5.Rows.Add(email, "Newyork123@", proxyItems, "0", "Đang chạy!");
                }));
                int index = findIndexDataGridView(email);
                try
                {
                    listRun.Enqueue(index);
                }
                catch { }
                addLogToDataGridView5(index, 4, "Setting Chrome!");
                var options = new ChromeOptions();
                string userAgent = listUserAgent[Faker.RandomNumber.Next(0, listUserAgent.Count - 1)];
                options.AddArgument("--user-agent=" + userAgent);
                options.AddArgument("--blink-settings=imagesEnabled=false");
                options.AddArguments("--disable-notifications"); // to disable notification
                options.AddArguments("--disable-application-cache");
                options.AddArgument("--disable-features=PrivacySandboxSettings4");
                options.AddArgument("--disable-features=ChromeWhatsNewUI");
                string[] proxy = proxyItems.Split('|');
                if (proxyItems.Contains(':'))
                {
                    proxy = proxyItems.Split(':');
                }
                if (proxy.Length > 2)
                {
                    options.AddHttpProxy(proxy[0], int.Parse(proxy[1]), proxy[2], proxy[3]);
                }
                else
                {
                    options.AddArgument("--proxy-server=" + proxyItems);
                }
                var driver = SeleniumUndetectedChromeDriver.UndetectedChromeDriver.Create(options: options, driverExecutablePath: Directory.GetCurrentDirectory() + "\\chromedriver.exe", hideCommandPromptWindow: true, suppressWelcome: false);
                string cookie = string.Empty;
                if (driver != null)
                {
                    try
                    {
                        addLogToDataGridView5(index, 4, "Setting Chrome Thành Công");
                        //driver.Manage().Cookies.DeleteAllCookies();
                        Thread.Sleep(1000);
                        driver.Manage().Window.Position = new Point(posX, posY);
                        Thread.Sleep(thread * 1000);
                        driver.Manage().Window.Size = new Size(600, 400);
                        driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(120));
                        addLogToDataGridView5(index, 4, "Get Mail SignUp");
                        driver.Navigate().GoToUrl("https://www.minuteinbox.com/");
                        _ = driver.Manage().Timeouts().ImplicitWait;
                        randomTime();
                        email = driver.FindElement(By.XPath("//span[@id='email']")).Text;
                        Dictionary<string, string> keyValues = utils.GetAllPageCookies(driver);
                        cookie = utils.ToDebugString(keyValues);
                        Thread.Sleep(2000);
                        addLogToDataGridView5(index, 4, "Go to BarnesAndNoble");
                        driver.Navigate().GoToUrl("https://www.barnesandnoble.com/");
                        _ = driver.Manage().Timeouts().ImplicitWait;
                        randomTime();
                        addLogToDataGridView5(index, 4, "SignUp Account");
                        BarnesAutomatic barnesAutomatic = new BarnesAutomatic();
                        barnesAutomatic.signUpAccount(driver, email, "Newyork123@", fisrtName, lastName);
                        Thread.Sleep(2000);
                        List<Mail> mailInput = resRequest.getMailInbox(cookie, proxyItems).Result;
                        if (mailInput.Count != 1)
                        {
                            dataGridView5.Invoke(new Action(() =>
                            {
                                dataGridView5.Rows[index].DefaultCellStyle.BackColor = Color.LightGreen;
                            }));
                            try
                            {
                                saveData.saveAccountBarnes(email, "Newyork123@", "Success", proxyItems, "0", ""); ;
                            }
                            catch { }
                            addLogToDataGridView5(index, 4, "Save account thành công!");

                            Thread.Sleep(2000);
                        }
                        else
                        {
                            addLogToDataGridView5(index, 4, "Không thể đăng kí hoặc có lỗi xảy ra");
                            dataGridView5.Invoke(new Action(() =>
                            {
                                dataGridView5.Rows[index].DefaultCellStyle.BackColor = Color.Red;
                            }));
                            try
                            {
                                driver.Close();
                                driver.Quit();
                                driver.Dispose();
                            }
                            catch { }
                            continue;
                        }
                        if (signUpNoCheck)
                        {
                            continue;
                        }
                        driver.GoToUrl("https://www.barnesandnoble.com/");
                        //mua hang
                        try
                        {
                            addLogToDataGridView5(index, 4, "Kiểm tra giỏ hàng");
                            string numberBuy = driver.FindElement(By.XPath("//a[@id='navbarDropdown2']/span")).Text;
                            if (int.Parse(numberBuy) == 0)
                            {
                                bool buyBook = barnesAutomatic.buyBookAndCheckout(driver, index);
                                if (buyBook)
                                {
                                    addLogToDataGridView5(index, 4, "Mua hàng thành công!");
                                    Thread.Sleep(2000);
                                }
                                else
                                {
                                    addLogToDataGridView5(index, 4, "Có lỗi thoát Thread!");
                                    dataGridView5.Invoke(new Action(() =>
                                    {
                                        dataGridView5.Rows[index].DefaultCellStyle.BackColor = Color.White;
                                    }));
                                    try
                                    {
                                        driver.Close();
                                        driver.Quit();
                                        driver.Dispose();
                                    }
                                    catch { }
                                    continue;
                                }
                            }
                        }
                        catch (Exception err)
                        {
                            addLogToDataGridView5(index, 4, "Có lỗi khi mua hàng");
                            utils.WriteLogError(err.Message.ToString());
                            dataGridView5.Invoke(new Action(() =>
                            {
                                dataGridView5.Rows[index].DefaultCellStyle.BackColor = Color.White;
                            }));
                            try
                            {
                                driver.Close();
                                driver.Quit();
                                driver.Dispose();
                            }
                            catch { }
                            continue;
                        }

                        try
                        {
                            bool checkOut = barnesAutomatic.ViewShoppingCart(driver);
                            if (checkOut)
                            {
                                addLogToDataGridView5(index, 4, "View Shopping thành công");
                            }
                            else
                            {
                                addLogToDataGridView5(index, 4, "Có lỗi khi xem View Shopping Cart");
                                dataGridView5.Invoke(new Action(() =>
                                {
                                    dataGridView5.Rows[index].DefaultCellStyle.BackColor = Color.White;
                                }));
                                try
                                {
                                    driver.Close();
                                    driver.Quit();
                                    driver.Dispose();
                                }
                                catch { }
                                continue;
                            }
                        }
                        catch (Exception err)
                        {
                            addLogToDataGridView5(index, 4, "Có lỗi khi view shopping cart");
                            utils.WriteLogError(err.Message.ToString());
                            dataGridView5.Invoke(new Action(() =>
                            {
                                dataGridView5.Rows[index].DefaultCellStyle.BackColor = Color.White;
                            }));
                            try
                            {
                                driver.Close();
                                driver.Quit();
                                driver.Dispose();
                            }
                            catch { }
                            continue;
                        }

                        Thread.Sleep(2000);
                        //altPaymentErr
                        IWebElement checkError = null;
                        try
                        {
                            checkError = driver.FindElement(By.Id("errMissingInfo"));
                            if (checkError != null)
                            {
                                string textErr = checkError.FindElement(By.TagName("em")).Text;
                                if (textErr.Equals("Updates to your Shipping and Billing Information are required to complete this order."))
                                {
                                    addLogToDataGridView5(index, 4, "Add Shipping Address");
                                    bool addShipping = barnesAutomatic.addShippingAddress(driver, index);
                                    if (addShipping)
                                    {
                                        addLogToDataGridView5(index, 4, "Add Shipping Address Thành công");
                                    }
                                    else
                                    {
                                        addLogToDataGridView5(index, 4, "Add Shipping Address Thất bại");
                                        try
                                        {
                                            driver.Close();
                                            driver.Quit();
                                            driver.Dispose();
                                        }
                                        catch { }
                                        continue;
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                        try
                        {

                            int addVisa = getQualityAddBarnet(dataGridView5.Rows[index].Cells[0].Value.ToString());
                            _ = driver.Manage().Timeouts().ImplicitWait;
                            while (addVisa < (int)numericUpDown8.Value)
                            {
                                int indexVisa = -1;
                                try
                                {
                                    indexVisa = listCC.Dequeue();
                                }
                                catch { }
                                if (indexVisa < 0)
                                {
                                    addLogToDataGridView5(index, 4, "Đã hết CC. Vui lòng add vào");
                                    return;
                                }
                                string[] ccInformation = dataGridView6.Rows[indexVisa].Cells[0].Value.ToString().Split('|');
                                addLogToDataGridView5(index, 4, "Bắt đầu add CC");
                                bool startAddCC = barnesAutomatic.startAddVisaAndCheck(driver, index, ccInformation);
                                if (startAddCC)
                                {
                                    //check CC
                                    addLogToDataGridView5(index, 4, "Bắt đầu check");
                                    randomTime();
                                    _ = driver.Manage().Timeouts().ImplicitWait;
                                    string textErr = string.Empty;
                                    try
                                    {
                                        textErr = driver.FindElement(By.Id("altPaymentErr")).FindElement(By.TagName("em")).Text;
                                    }
                                    catch { }

                                    IJavaScriptExecutor javascript1 = (IJavaScriptExecutor)driver;
                                    while (string.IsNullOrEmpty(textErr))
                                    {
                                        IWebElement webElement = null;
                                        randomTime();
                                        try
                                        {
                                            webElement = driver.FindElement(By.Id("memberSubmitOrder"));
                                        }
                                        catch
                                        {
                                            webElement = driver.FindElement(By.Id("memberSubmitOrderCVVRequired"));
                                        }
                                        if (webElement != null)
                                        {
                                            try
                                            {
                                                javascript1.ExecuteScript("arguments[0].click();", webElement);
                                            }
                                            catch
                                            {
                                                Thread.Sleep(2000);
                                            }
                                            randomTime();
                                            try
                                            {
                                                WebDriverWait driverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                                                driverWait.Until(ExpectedConditions.ElementExists(By.Id("altPaymentErr")));
                                                textErr = driver.FindElement(By.Id("altPaymentErr")).FindElement(By.TagName("em")).Text;
                                                if (textErr != null)
                                                {
                                                    break;
                                                }
                                            }
                                            catch { }
                                            Thread.Sleep(2000);
                                            string text = string.Empty;
                                            try
                                            {
                                                text = driver.FindElement(By.Id("dialog-title")).Text;
                                                if (text != null)
                                                {
                                                    listCC.Enqueue(indexVisa);
                                                    textErr = "Không thể check visa";
                                                    break;
                                                }
                                            }
                                            catch
                                            {

                                            }

                                            try
                                            {
                                                webElement = driver.FindElement(By.Id("memberSubmitOrder"));
                                            }
                                            catch { }

                                        }
                                    }
                                    Thread.Sleep(2000);
                                    if (textErr.Equals("Sorry! Your order cannot be processed due to an error with the Card Number entered. Please edit your card details and try again.") || textErr.Equals("The Security Code you entered is incorrect.\r\nPlease try again or contact Customer Service for assistance."))
                                    {
                                        try
                                        {
                                            saveData.updateStatusCC(dataGridView6.Rows[indexVisa].Cells[0].Value.ToString(), "Failed");
                                        }
                                        catch { }
                                        dataGridView6.Invoke(new Action(() =>
                                        {
                                            dataGridView6.Rows[indexVisa].Cells[1].Value = "Failed";
                                            dataGridView6.Rows[indexVisa].DefaultCellStyle.BackColor = Color.Red;
                                        }));
                                        addVisa++;
                                        saveData.updateQualityByUsernameBarnes(dataGridView5.Rows[index].Cells[0].Value.ToString(), addVisa.ToString());
                                        dataGridView5.Invoke(new Action(() =>
                                        {
                                            dataGridView5.Rows[index].Cells[3].Value = addVisa.ToString();
                                        }));
                                        Thread.Sleep(2000);
                                    }
                                    else if (textErr.Equals("Không thể check visa"))
                                    {
                                        try
                                        {
                                            saveData.updateStatusByUsernameBarnes(dataGridView5.Rows[index].Cells[0].Value.ToString(), "Failed");
                                        }
                                        catch { }
                                        break;
                                    }
                                    else if (textErr.Equals("We're sorry but the shipping zip code entered is invalid. Please enter a valid zip code."))
                                    {
                                        listCC.Enqueue(indexVisa);
                                        addLogToDataGridView5(index, 4, "Sai ZipCode");
                                        break;
                                    }
                                    else if (textErr.Equals("Sorry! Your order cannot be processed because the CVV entered is incorrect. Please update and try again."))
                                    {
                                        try
                                        {
                                            saveData.updateStatusCC(dataGridView6.Rows[indexVisa].Cells[0].Value.ToString(), "Success");
                                        }
                                        catch { }
                                        try
                                        {
                                            addLogToDataGridView5(index, 4, "Save File");
                                            utils.WriteDebugBarnes(dataGridView6.Rows[indexVisa].Cells[0].Value.ToString());
                                        }
                                        catch { }
                                        dataGridView6.Invoke(new Action(() =>
                                        {
                                            dataGridView6.Rows[indexVisa].Cells[1].Value = "Success";
                                            dataGridView6.Rows[indexVisa].DefaultCellStyle.BackColor = Color.LightGreen;
                                        }));
                                        addVisa++;
                                        saveData.updateQualityByUsernameBarnes(dataGridView5.Rows[index].Cells[0].Value.ToString(), addVisa.ToString());
                                        dataGridView5.Invoke(new Action(() =>
                                        {
                                            dataGridView5.Rows[index].Cells[3].Value = addVisa.ToString();
                                        }));
                                        Thread.Sleep(2000);
                                    }
                                }
                            }

                        }
                        catch { }
                    }
                    catch (Exception err)
                    {
                        resRequest.deleteEmail(cookie, proxyItems);
                        addLogToDataGridView5(index, 4, "Có lỗi không xác định, thoát Thread!");
                        utils.WriteLogError(err.Message.ToString());
                        dataGridView5.Invoke(new Action(() =>
                        {
                            dataGridView5.Rows[index].DefaultCellStyle.BackColor = Color.White;
                        }));
                        try
                        {
                            driver.Close();
                            driver.Quit();
                            driver.Dispose();
                        }
                        catch { }

                    }
                    finally
                    {
                        resRequest.deleteEmail(cookie, proxyItems);
                        try
                        {
                            driver.Close();
                            driver.Quit();
                            driver.Dispose();
                        }
                        catch { }
                    }

                }
            }
        }

        public int getQualityAdd(string username)
        {
            int quality = -1;
            try
            {
                SQLiteDataReader sQLiteDataReader = saveData.getQuality(username);
                while (sQLiteDataReader.Read())
                {
                    quality = int.Parse(sQLiteDataReader.GetString(0));
                }
            }
            catch
            {
                Console.WriteLine("Error");
            }
            return quality;
        }

        public int getQualityAddBarnet(string username)
        {
            int quality = -1;
            try
            {
                SQLiteDataReader sQLiteDataReader = saveData.getQualityBarnes(username);
                while (sQLiteDataReader.Read())
                {
                    quality = int.Parse(sQLiteDataReader.GetString(0));
                }
            }
            catch
            {
                Console.WriteLine("Error");
            }
            return quality;
        }

        public void loadDataGridView()
        {
            try
            {
                dataGridView1.Rows.Clear();
                SQLiteDataReader sQLiteDataReader = saveData.loadDataAccount();
                int k = 1;
                while (sQLiteDataReader.Read())
                {
                    dataGridView1.Rows.Add(k, sQLiteDataReader.GetString(0), sQLiteDataReader.GetString(1), sQLiteDataReader.GetString(2), sQLiteDataReader.GetString(3), sQLiteDataReader.GetString(5), sQLiteDataReader.GetString(4));
                    if (!string.IsNullOrEmpty(sQLiteDataReader.GetString(2)))
                    {
                        if (sQLiteDataReader.GetString(2).Equals("Success"))
                        {
                            dataGridView1.Rows[k - 1].DefaultCellStyle.BackColor = Color.LightGreen;
                        }

                    }
                    k++;
                }

            }
            catch
            {
                Console.WriteLine("Error");
            }
        }


        public void loadDataProxy()
        {
            try
            {
                listBox1.Items.Clear();
                SQLiteDataReader sQLiteDataReader = saveData.loadDataProxy();
                while (sQLiteDataReader.Read())
                {
                    listBox1.Items.Add(sQLiteDataReader.GetString(1));
                }

            }
            catch
            {
                Console.WriteLine("Error");
            }
        }

        public void loadDataGridView4()
        {
            try
            {
                dataGridView4.Rows.Clear();
                SQLiteDataReader sQLiteDataReader = saveData.loadDataAccountBarnes();
                int k = 1;
                while (sQLiteDataReader.Read())
                {
                    dataGridView4.Rows.Add(k, sQLiteDataReader.GetString(0), sQLiteDataReader.GetString(1), sQLiteDataReader.GetString(2), sQLiteDataReader.GetString(3), sQLiteDataReader.GetString(5), sQLiteDataReader.GetString(4));
                    if (!string.IsNullOrEmpty(sQLiteDataReader.GetString(2)))
                    {
                        if (sQLiteDataReader.GetString(2).Equals("Success"))
                        {
                            dataGridView4.Rows[k - 1].DefaultCellStyle.BackColor = Color.LightGreen;
                        }

                    }
                    k++;
                }

            }
            catch
            {
                Console.WriteLine("Error");
            }
        }

        public void loadDataGridViewCC(DataGridView dataGridView)
        {
            try
            {
                dataGridView.Rows.Clear();
                SQLiteDataReader sQLiteDataReader = saveData.loadDataCC();
                int k = 0;
                while (sQLiteDataReader.Read())
                {
                    dataGridView.Rows.Add(sQLiteDataReader.GetString(1), sQLiteDataReader.GetString(2));
                    if (!string.IsNullOrEmpty(sQLiteDataReader.GetString(2)))
                    {
                        if (sQLiteDataReader.GetString(2).Equals("Success"))
                        {
                            dataGridView.Rows[k].DefaultCellStyle.BackColor = Color.LightGreen;
                        }

                    }
                    k++;
                }
            }
            catch
            {
                Console.WriteLine("Error");
            }
        }

        public void addCCToDatabase(DataGridView dataGridView)
        {
            try
            {
                string[] strings = File.ReadAllLines(utils.getFilePath());

                foreach (string vs in strings)
                {
                    saveData.saveCC(vs, "Unchecked");
                }
                loadDataGridViewCC(dataGridView);
            }
            catch { }
        }

        #endregion


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string[] accountList = File.ReadAllLines(utils.getFilePath());
                for (int i = 0; i < accountList.Length; i++)
                {
                    string[] account = { };
                    if (accountList[i].Contains('|'))
                    {
                        account = accountList[i].Split('|');

                    }
                    else if (accountList[i].Contains(":"))
                    {
                        account = accountList[i].Split(':');
                    }
                    try
                    {
                        saveData.saveAccount(account[0], account[1], "Unchecked", "", "0", "");
                    }
                    catch { }
                }
                loadDataGridView();
            }
            catch { }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có muốn xoá hết account Hotmail", "Thông báo", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                dataGridView1.Rows.Clear();
                saveData.deleteProfileByAccount(null);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            addCCToDatabase(dataGridView2);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có muốn xoá hết account", "Thông báo", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                saveData.deleteTableCC("");
                dataGridView2.Rows.Clear();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string[] proxy = File.ReadAllLines(utils.getFilePath());
                if (dataGridView1.Rows.Count == 0)
                {
                    return;
                }
                else
                {
                    List<string> proxyList = new List<string>();
                    if (proxy.Length < dataGridView1.Rows.Count)
                    {
                        int k = 0;
                        for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        {
                            proxyList.Add(proxy[k]);
                            k++;
                            if (k == proxy.Length)
                            {
                                k = 0;
                            }
                        }
                    }
                    else
                    {
                        proxyList = proxy.ToList();
                    }
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        if (string.IsNullOrEmpty(dataGridView1.Rows[i].Cells[4].Value?.ToString()))
                        {
                            saveData.updateProxyByID(dataGridView1.Rows[i].Cells[1].Value.ToString(), proxyList[i]);
                            dataGridView1.Rows[i].Cells[4].Value = proxyList[i];
                        }
                    }
                }
            }
            catch { }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có muốn thay hết các proxy!", "Thông báo", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    string[] proxy = File.ReadAllLines(utils.getFilePath());
                    if (dataGridView1.Rows.Count == 0)
                    {
                        return;
                    }
                    else
                    {
                        List<string> proxyList = new List<string>();
                        if (proxy.Length < dataGridView1.Rows.Count)
                        {
                            int k = 0;
                            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                            {
                                proxyList.Add(proxy[k]);
                                k++;
                                if (k == proxy.Length)
                                {
                                    k = 0;
                                }
                            }
                        }
                        else
                        {
                            proxyList = proxy.ToList();
                        }
                        for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        {
                            saveData.updateProxyByID(dataGridView1.Rows[i].Cells[1].Value.ToString(), proxyList[i]);
                            dataGridView1.Rows[i].Cells[4].Value = proxyList[i];
                        }
                    }
                }
                catch { }
                loadDataGridView();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có muốn xoá hết account", "Thông báo", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dataGridView1.Rows[i].Cells[4].Value?.ToString()))
                    {
                        saveData.updateProxyByID(dataGridView1.Rows[i].Cells[1].Value.ToString(), "");
                        dataGridView1.Rows[i].Cells[4].Value = "";
                    }
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0 || dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm tài khoản và cc", "Thông báo");
                return;
            }
            if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\ExportCC"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\ExportCC");
            }
            isRun = true;
            button7.Enabled = false;
            button8.Enabled = true;
            listRun = new Queue<int>();
            for (int z = (int)numericUpDown14.Value; z < dataGridView1.Rows.Count; z++)
            {
                if (dataGridView1.Rows[z].Cells[3].Value.ToString().Equals("Unchecked"))
                {
                    listRun.Enqueue(z);
                }
                else if (dataGridView1.Rows[z].Cells[3].Value.ToString().Equals("Success") && int.Parse(dataGridView1.Rows[z].Cells[5].Value.ToString()) < (int)numericUpDown12.Value)
                {
                    listRun.Enqueue(z);
                }
            }
            listCC = new Queue<int>();
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                if (dataGridView2.Rows[i].Cells[1].Value.ToString().Equals("Unchecked"))
                {
                    listCC.Enqueue(i);
                }
            }
            List<List<int>> listvitri = utils.chiaViTri((int)numericUpDown3.Value);
            Task.Run(() =>
            {
                List<Task> tasks = new List<Task>();
                for (int i = 0; i < (int)numericUpDown3.Value; i++)
                {
                    int thread = i + 1;
                    List<int> vitri = listvitri[i];
                    Task task = Task.Run(() =>
                    {
                        loginHotmailAndAddCC(thread, vitri[0], vitri[1]);
                    });
                    tasks.Add(task);
                }
                Task.WaitAll(tasks.ToArray());
                MessageBox.Show("Done", "Thông báo");
                this.Invoke(new Action(() =>
                {
                    button8.Enabled = false;
                    button7.Enabled = true;
                }));
            });
        }

        private void Dashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process[] processes = Process.GetProcessesByName("chromedriver");

            foreach (Process process in processes)
            {
                process.Kill();
            }
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            SaveData saveData = new SaveData();
            SQLiteDataReader activeFormResult = saveData.loadDataKey();
            ActiveKey activeKey = new ActiveKey();
            while (activeFormResult.Read())
            {
                string key = activeFormResult.GetString(1);
                string keyEcrypt = Settings.Default.keyEcrypt;
                SKGL.Validate validate = new SKGL.Validate();
                validate.secretPhase = keyEcrypt;
                validate.Key = key;
                string dayLeft = (validate.DaysLeft).ToString();
                saveData.updateDayLeftDataKey(Int32.Parse(dayLeft));
                label15.Text = "Creation Date: " + DateTime.Parse(activeFormResult.GetString(2)).ToString("dd/MM/yyyy");
                label16.Text = "Expire Date: " + DateTime.Parse(activeFormResult.GetString(3)).ToString("dd/MM/yyyy");
                label17.Text = "Day Left: " + dayLeft;
            }
            loadDataGridView();
            loadDataGridViewCC(dataGridView2);
            loadDataGridViewCC(dataGridView3);
            loadDataGridViewCC(dataGridView6);
            loadDataGridView4();
            loadDataProxy();
            button8.Enabled = false;
            button11.Enabled = false;
            button20.Enabled = false;
            Process[] processes = Process.GetProcessesByName("chromedriver");

            foreach (Process process in processes)
            {
                process.Kill();
            }
            DirectoryInfo d = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Plugins");
            try
            {
                FileInfo[] Files = d.GetFiles("*.zip"); //Getting Text files

                foreach (FileInfo file in Files)
                {
                    File.Delete(file.FullName);
                }
            }
            catch { }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            try
            {
                string[] accountList = File.ReadAllLines(utils.getFilePath());
                for (int i = 0; i < accountList.Length; i++)
                {
                    string[] account = { };
                    if (accountList[i].Contains('|'))
                    {
                        account = accountList[i].Split('|');

                    }
                    else if (accountList[i].Contains(":"))
                    {
                        account = accountList[i].Split(':');
                    }
                    try
                    {
                        saveData.saveAccountBarnes(account[0], account[1], "Unchecked", "", "0", "");
                    }
                    catch { }

                }
                loadDataGridView4();
            }
            catch { }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có muốn xoá hết account Barnes", "Thông báo", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                dataGridView4.Rows.Clear();
                saveData.deleteProfileByAccountBarnes(null);
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            addCCToDatabase(dataGridView3);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có muốn xoá hết CC", "Thông báo", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                saveData.deleteTableCC("");
                dataGridView3.Rows.Clear();
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            try
            {
                string[] proxy = File.ReadAllLines(utils.getFilePath());
                if (dataGridView4.Rows.Count == 0)
                {
                    return;
                }
                else
                {
                    List<string> proxyList = new List<string>();
                    if (proxy.Length < dataGridView4.Rows.Count)
                    {
                        int k = 0;
                        for (int i = 0; i < dataGridView4.Rows.Count; i++)
                        {
                            proxyList.Add(proxy[k]);
                            k++;
                            if (k == proxy.Length)
                            {
                                k = 0;
                            }
                        }
                    }
                    else
                    {
                        proxyList = proxy.ToList();
                    }
                    for (int i = 0; i < dataGridView4.Rows.Count; i++)
                    {
                        if (string.IsNullOrEmpty(dataGridView4.Rows[i].Cells[4].Value?.ToString()))
                        {
                            saveData.updateProxyByUsernameBarnes(dataGridView4.Rows[i].Cells[1].Value.ToString(), proxyList[i]);
                            dataGridView4.Rows[i].Cells[4].Value = proxyList[i];
                        }
                    }
                }
            }
            catch { }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có muốn thay hết các proxy cho account Barnes!", "Thông báo", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    string[] proxy = File.ReadAllLines(utils.getFilePath());
                    if (dataGridView4.Rows.Count == 0)
                    {
                        return;
                    }
                    else
                    {
                        List<string> proxyList = new List<string>();
                        if (proxy.Length < dataGridView4.Rows.Count)
                        {
                            int k = 0;
                            for (int i = 0; i < dataGridView4.Rows.Count; i++)
                            {
                                proxyList.Add(proxy[k]);
                                k++;
                                if (k == proxy.Length)
                                {
                                    k = 0;
                                }
                            }
                        }
                        else
                        {
                            proxyList = proxy.ToList();
                        }
                        for (int i = 0; i < dataGridView4.Rows.Count; i++)
                        {
                            saveData.updateProxyByUsernameBarnes(dataGridView4.Rows[i].Cells[1].Value.ToString(), proxyList[i]);
                            dataGridView4.Rows[i].Cells[4].Value = proxyList[i];
                        }
                    }
                }
                catch { }
                loadDataGridView4();
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có muốn xoá proxy account Barnes", "Thông báo", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                for (int i = 0; i < dataGridView4.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dataGridView4.Rows[i].Cells[4].Value?.ToString()))
                    {
                        saveData.updateProxyByUsernameBarnes(dataGridView4.Rows[i].Cells[1].Value.ToString(), "");
                        dataGridView4.Rows[i].Cells[4].Value = "";
                    }
                }
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            isRun = false;
            button11.Enabled = false;
            button12.Enabled = true;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (dataGridView4.Rows.Count == 0 || dataGridView3.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm tài khoản và cc", "Thông báo");
                return;
            }
            isRun = true;
            button12.Enabled = false;
            button11.Enabled = true;
            listRun = new Queue<int>();
            for (int z = (int)numericUpDown13.Value - 1; z < dataGridView4.Rows.Count; z++)
            {
                if (dataGridView4.Rows[z].Cells[3].Value.ToString().Equals("Unchecked"))
                {
                    listRun.Enqueue(z);
                }
                else if (dataGridView4.Rows[z].Cells[3].Value.ToString().Equals("Success") && int.Parse(dataGridView4.Rows[z].Cells[5].Value.ToString()) < (int)numericUpDown7.Value)
                {
                    listRun.Enqueue(z);
                }
            }
            listCC = new Queue<int>();
            for (int i = 0; i < dataGridView3.Rows.Count; i++)
            {
                if (dataGridView3.Rows[i].Cells[1].Value.ToString().Equals("Unchecked"))
                {
                    listCC.Enqueue(i);
                }
            }
            listUserAgent = new List<string>();
            string[] userAgent = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\useragent.txt");
            listUserAgent = userAgent.ToList();
            List<List<int>> listvitri = utils.chiaViTri((int)numericUpDown4.Value);
            Task.Run(() =>
            {
                List<Task> tasks = new List<Task>();
                for (int i = 0; i < (int)numericUpDown4.Value; i++)
                {
                    int thread = i + 1;
                    List<int> vitri = listvitri[i];
                    Task task = Task.Run(() =>
                    {
                        loginBarnesAndAddCC(thread, vitri[0], vitri[1]);
                    });
                    tasks.Add(task);
                }
                Task.WaitAll(tasks.ToArray());
                MessageBox.Show("Done", "Thông báo");
                this.Invoke(new Action(() =>
                {
                    button11.Enabled = false;
                    button12.Enabled = true;
                }));
                isRun = false;
            });
        }

        private void button8_Click(object sender, EventArgs e)
        {
            isRun = false;
            button8.Enabled = false;
            button7.Enabled = true;
        }

        private void button27_Click(object sender, EventArgs e)
        {
            try
            {
                string[] proxy = File.ReadAllLines(utils.getFilePath());
                for (int i = 0; i < proxy.Length; i++)
                {
                    saveData.saveDataProxy(proxy[i]);
                    listBox1.Invoke(new Action(() =>
                    {
                        listBox1.Items.Add(proxy[i]);
                    }));

                }
            }
            catch { }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có muốn thay hết các proxy!", "Thông báo", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    listBox1.Items.Clear();
                    saveData.deleteProxy();
                    string[] proxy = File.ReadAllLines(utils.getFilePath());
                    for (int i = 0; i < proxy.Length; i++)
                    {
                        saveData.saveDataProxy(proxy[i]);
                        listBox1.Invoke(new Action(() =>
                        {
                            listBox1.Items.Add(proxy[i]);
                        }));
                    }
                }
                catch { }
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có muốn xoá All proxy", "Thông báo", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    saveData.deleteProxy("");
                    listBox1.Items.Clear();
                }
                catch { }
            }
        }

        private void button24_Click(object sender, EventArgs e)
        {
            addCCToDatabase(dataGridView6);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có muốn xoá hết CC", "Thông báo", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                saveData.deleteTableCC("");
                dataGridView6.Rows.Clear();
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0 || dataGridView6.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm proxy và cc", "Thông báo");
                return;
            }
            isRun = true;
            button21.Enabled = false;
            button20.Enabled = true;
            bool signUpNoCheck = false;
            if (checkBox1.Checked)
            {
                signUpNoCheck = true;
            }
            listProxy = new Queue<string>();
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                listProxy.Enqueue(listBox1.Items[i].ToString());
            }
            listCC = new Queue<int>();
            for (int i = 0; i < dataGridView6.Rows.Count; i++)
            {
                if (dataGridView6.Rows[i].Cells[1].Value.ToString().Equals("Unchecked"))
                {
                    listCC.Enqueue(i);
                }
            }
            listRun = new Queue<int>();
            listUserAgent = new List<string>();
            string[] userAgent = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\useragent.txt");
            listUserAgent = userAgent.ToList();
            List<List<int>> listvitri = utils.chiaViTri((int)numericUpDown10.Value);
            Task.Run(() =>
            {
                List<Task> tasks = new List<Task>();
                for (int i = 0; i < (int)numericUpDown10.Value; i++)
                {
                    int thread = i + 1;
                    List<int> vitri = listvitri[i];
                    Task task = Task.Run(async () =>
                    {
                        signupBarnesAndAddCC(thread, vitri[0], vitri[1], signUpNoCheck);
                    });
                    tasks.Add(task);
                }
                Task.WaitAll(tasks.ToArray());
                MessageBox.Show("Done", "Thông báo");
                this.Invoke(new Action(() =>
                {
                    button21.Enabled = true;
                    button20.Enabled = false;
                }));
                isRun = false;
            });
        }

        private void button20_Click(object sender, EventArgs e)
        {
            isRun = false;
            button21.Enabled = true;
            button20.Enabled = false;
        }

        #region DataGridView
        public void deleteThisAccount_Click(object? sender, System.EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có muốn xoá this account", "Thông báo", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (tabControl1.SelectedIndex == 0)
                {
                    int selectedrowindex = dataGridView1.SelectedCells[1].RowIndex;
                    try
                    {
                        saveData.deleteProfileByAccount(dataGridView1.Rows[selectedrowindex].Cells[1].Value.ToString());

                    }
                    catch (Exception ex) { }
                    loadDataGridView();
                }
                else if (tabControl1.SelectedIndex == 1)
                {
                    int selectedrowindex = dataGridView4.SelectedCells[1].RowIndex;
                    try
                    {
                        saveData.deleteProfileByAccountBarnes(dataGridView4.Rows[selectedrowindex].Cells[1].Value.ToString());
                    }
                    catch (Exception ex) { }
                    loadDataGridView4();
                }

            }
        }

        public void deleteAllAccount_Click(object? sender, System.EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có muốn xoá All account", "Thông báo", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (tabControl1.SelectedIndex == 0)
                {
                    try
                    {
                        saveData.deleteProfileByAccount("");
                    }
                    catch (Exception ex) { }
                    dataGridView1.Rows.Clear();
                }
                else if (tabControl1.SelectedIndex == 1)
                {
                    try
                    {
                        saveData.deleteProfileByAccountBarnes("");
                    }
                    catch (Exception ex) { }
                    dataGridView4.Rows.Clear();
                }

            }
        }

        private void export_Click(Object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = @"C:\";
            saveFileDialog1.Title = "Save Text Files";
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "txt";
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                List<string> strings = new List<string>();
                if (tabControl1.SelectedIndex == 0)
                {
                    for (int i = 0; i < dataGridView2.Rows.Count; i++)
                    {
                        strings.Add(dataGridView2.Rows[i].Cells[0].Value.ToString());
                    }
                }
                else if (tabControl1.SelectedIndex == 1)
                {
                    for (int i = 0; i < dataGridView3.Rows.Count; i++)
                    {
                        strings.Add(dataGridView3.Rows[i].Cells[0].Value.ToString());
                    }
                }
                else if (tabControl1.SelectedIndex == 2)
                {
                    for (int i = 0; i < dataGridView6.Rows.Count; i++)
                    {
                        strings.Add(dataGridView6.Rows[i].Cells[0].Value.ToString());
                    }
                }
                File.WriteAllLines(saveFileDialog1.FileName, strings.ToArray());
            }
        }

        private void deleteThisAccount1_Click(Object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có muốn xoá this VS", "Thông báo", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (tabControl1.SelectedIndex == 0)
                {
                    int selectedrowindex = dataGridView2.SelectedCells[1].RowIndex;
                    try
                    {
                        saveData.deleteTableCC(dataGridView2.Rows[selectedrowindex].Cells[0].Value.ToString());
                    }
                    catch (Exception ex) { }
                    loadDataGridViewCC(dataGridView2);
                }
                else if (tabControl1.SelectedIndex == 1)
                {
                    int selectedrowindex = dataGridView3.SelectedCells[1].RowIndex;
                    try
                    {
                        saveData.deleteTableCC(dataGridView3.Rows[selectedrowindex].Cells[0].Value.ToString());
                    }
                    catch (Exception ex) { }
                    loadDataGridViewCC(dataGridView3);
                }
                else if (tabControl1.SelectedIndex == 2)
                {
                    int selectedrowindex = dataGridView6.SelectedCells[1].RowIndex;
                    try
                    {
                        saveData.deleteTableCC(dataGridView6.Rows[selectedrowindex].Cells[0].Value.ToString());
                    }
                    catch (Exception ex) { }
                    loadDataGridViewCC(dataGridView6);
                }

            }
        }

        private void deleteAllAccount1_Click(Object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có muốn xoá All VS", "Thông báo", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    saveData.deleteTableCC("");
                }
                catch (Exception ex) { }
                if (tabControl1.SelectedIndex == 0)
                {
                    dataGridView2.Rows.Clear();
                }
                else if (tabControl1.SelectedIndex == 1)
                {
                    dataGridView3.Rows.Clear();
                }
                else if (tabControl1.SelectedIndex == 2)
                {
                    dataGridView6.Rows.Clear();
                }

            }
        }

        private void deleteVSSuccess_Click(Object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có muốn xoá VS Success", "Thông báo", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (tabControl1.SelectedIndex == 0)
                {
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.Cells[1].Value.ToString().Equals("Success"))
                        {
                            try
                            {
                                saveData.deleteTableCC(row.Cells[0].Value.ToString());
                            }
                            catch (Exception ex) { }
                        }
                    }
                    loadDataGridViewCC(dataGridView2);
                }
                else if (tabControl1.SelectedIndex == 1)
                {
                    foreach (DataGridViewRow row in dataGridView3.Rows)
                    {
                        if (row.Cells[1].Value.ToString().Equals("Success"))
                        {
                            try
                            {
                                saveData.deleteTableCC(row.Cells[0].Value.ToString());
                            }
                            catch (Exception ex) { }
                        }
                    }
                    loadDataGridViewCC(dataGridView3);
                }
                else if (tabControl1.SelectedIndex == 2)
                {
                    foreach (DataGridViewRow row in dataGridView6.Rows)
                    {
                        if (row.Cells[1].Value.ToString().Equals("Success"))
                        {
                            try
                            {
                                saveData.deleteTableCC(row.Cells[0].Value.ToString());
                            }
                            catch (Exception ex) { }
                        }
                    }
                    loadDataGridViewCC(dataGridView6);
                }

            }
        }
        #endregion

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            var dataGrid = (DataGridView)sender;
            if (e.Button == MouseButtons.Right && e.RowIndex != -1)
            {
                var row = dataGrid.Rows[e.RowIndex];
                dataGrid.CurrentCell = row.Cells[e.ColumnIndex == -1 ? 1 : e.ColumnIndex];
                row.Selected = true;
                dataGrid.Focus();
            }
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    int selectedrowindex = dataGridView1.SelectedCells[1].RowIndex;
                    ContextMenuStrip m = new ContextMenuStrip();
                    m.Items.Add(new ToolStripMenuItem("Delete This Account", null, new EventHandler(deleteThisAccount_Click)));
                    m.Items.Add(new ToolStripMenuItem("Delete All Account", null, new EventHandler(deleteAllAccount_Click)));
                    m.Show(dataGridView1, new Point(e.X, e.Y));
                }
                catch { }

            }
        }

        private void dataGridView2_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            var dataGrid = (DataGridView)sender;
            if (e.Button == MouseButtons.Right && e.RowIndex != -1)
            {
                var row = dataGrid.Rows[e.RowIndex];
                dataGrid.CurrentCell = row.Cells[e.ColumnIndex == -1 ? 1 : e.ColumnIndex];
                row.Selected = true;
                dataGrid.Focus();
            }
        }

        private void dataGridView2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    int selectedrowindex = dataGridView2.SelectedCells[0].RowIndex;
                    ContextMenuStrip m = new ContextMenuStrip();

                    m.Items.Add(new ToolStripMenuItem("Export All VS Success", null, new EventHandler(export_Click)));
                    m.Items.Add(new ToolStripMenuItem("Delete This VS", null, new EventHandler(deleteThisAccount1_Click)));
                    m.Items.Add(new ToolStripMenuItem("Delete VS Susscess", null, new EventHandler(deleteVSSuccess_Click)));
                    m.Items.Add(new ToolStripMenuItem("Delete All VS", null, new EventHandler(deleteAllAccount1_Click)));
                    m.Show(dataGridView2, new Point(e.X, e.Y));
                }
                catch { }

            }
        }

        private void dataGridView4_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            var dataGrid = (DataGridView)sender;
            if (e.Button == MouseButtons.Right && e.RowIndex != -1)
            {
                var row = dataGrid.Rows[e.RowIndex];
                dataGrid.CurrentCell = row.Cells[e.ColumnIndex == -1 ? 1 : e.ColumnIndex];
                row.Selected = true;
                dataGrid.Focus();
            }
        }

        private void dataGridView4_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    int selectedrowindex = dataGridView4.SelectedCells[1].RowIndex;
                    ContextMenuStrip m = new ContextMenuStrip();
                    m.Items.Add(new ToolStripMenuItem("Delete This Account", null, new EventHandler(deleteThisAccount_Click)));
                    m.Items.Add(new ToolStripMenuItem("Delete All Account", null, new EventHandler(deleteAllAccount_Click)));
                    m.Show(dataGridView1, new Point(e.X, e.Y));
                }
                catch { }

            }
        }

        private void dataGridView3_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            var dataGrid = (DataGridView)sender;
            if (e.Button == MouseButtons.Right && e.RowIndex != -1)
            {
                var row = dataGrid.Rows[e.RowIndex];
                dataGrid.CurrentCell = row.Cells[e.ColumnIndex == -1 ? 1 : e.ColumnIndex];
                row.Selected = true;
                dataGrid.Focus();
            }
        }

        private void dataGridView3_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    int selectedrowindex = dataGridView2.SelectedCells[0].RowIndex;
                    ContextMenuStrip m = new ContextMenuStrip();

                    m.Items.Add(new ToolStripMenuItem("Export All VS Success", null, new EventHandler(export_Click)));
                    m.Items.Add(new ToolStripMenuItem("Delete This VS", null, new EventHandler(deleteThisAccount1_Click)));
                    m.Items.Add(new ToolStripMenuItem("Delete VS Susscess", null, new EventHandler(deleteVSSuccess_Click)));
                    m.Items.Add(new ToolStripMenuItem("Delete All VS", null, new EventHandler(deleteAllAccount1_Click)));
                    m.Show(dataGridView2, new Point(e.X, e.Y));
                }
                catch { }

            }
        }

        private void dataGridView6_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            var dataGrid = (DataGridView)sender;
            if (e.Button == MouseButtons.Right && e.RowIndex != -1)
            {
                var row = dataGrid.Rows[e.RowIndex];
                dataGrid.CurrentCell = row.Cells[e.ColumnIndex == -1 ? 1 : e.ColumnIndex];
                row.Selected = true;
                dataGrid.Focus();
            }
        }

        private void dataGridView6_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    int selectedrowindex = dataGridView2.SelectedCells[0].RowIndex;
                    ContextMenuStrip m = new ContextMenuStrip();

                    m.Items.Add(new ToolStripMenuItem("Export All VS Success", null, new EventHandler(export_Click)));
                    m.Items.Add(new ToolStripMenuItem("Delete This VS", null, new EventHandler(deleteThisAccount1_Click)));
                    m.Items.Add(new ToolStripMenuItem("Delete VS Susscess", null, new EventHandler(deleteVSSuccess_Click)));
                    m.Items.Add(new ToolStripMenuItem("Delete All VS", null, new EventHandler(deleteAllAccount1_Click)));
                    m.Show(dataGridView2, new Point(e.X, e.Y));
                }
                catch { }

            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                if (!isRun)
                {
                    loadDataGridView();
                    loadDataGridViewCC(dataGridView2);
                }
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                if (!isRun)
                {
                    loadDataGridViewCC(dataGridView3);
                    loadDataGridView4();
                }
            }
            else if (tabControl1.SelectedIndex == 2)
            {
                if (!isRun)
                {
                    loadDataGridViewCC(dataGridView6);
                    loadDataProxy();
                }
            }
        }

        private void numericUpDown14_ValueChanged(object sender, EventArgs e)
        {
            if ((int)numericUpDown14.Value > dataGridView1.Rows.Count)
            {
                numericUpDown14.Value = dataGridView1.Rows.Count;
            }
        }

        private void numericUpDown13_ValueChanged(object sender, EventArgs e)
        {
            if ((int)numericUpDown13.Value > dataGridView4.Rows.Count)
            {
                numericUpDown13.Value = dataGridView4.Rows.Count;
            }
        }
    }
}
