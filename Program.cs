using FoxLearn.License;
using hotmailCheck.Controllers;
using hotmailCheck.Models;
using hotmailCheck.Properties;
using System.Data.SQLite;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace hotmailCheck
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            SaveData saveData = new SaveData();
            SQLiteDataReader activeFormResult = saveData.loadDataKey();
            ActiveKey activeKey = new ActiveKey();
            while (activeFormResult.Read())
            {
                activeKey.key = activeFormResult.GetString(1);
                activeKey.creationDate = activeFormResult.GetString(2);
                activeKey.expireDate = activeFormResult.GetString(3);
                activeKey.dayLeft = activeFormResult.GetInt32(4);
                activeKey.activeForm = activeFormResult.GetInt32(5) == 0 ? false : true;
            }
            if (activeKey.key != null)
            {
                string hash = Settings.Default.hash;
                string computer = ComputerInfo.GetComputerId();
                byte[] data = UTF8Encoding.UTF8.GetBytes(computer.Replace("-", ""));
                using (MD5CryptoServiceProvider mD5 = new MD5CryptoServiceProvider())
                {
                    byte[] keys = mD5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
                    using (TripleDESCryptoServiceProvider triple = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.None })
                    {
                        ICryptoTransform transform = triple.CreateDecryptor();
                        byte[] res = transform.TransformFinalBlock(data, 0, data.Length);
                        string keyEcrypt = Convert.ToBase64String(res, 0, res.Length);
                        Settings.Default.keyEcrypt = keyEcrypt;
                        Settings.Default.Save();
                        SKGL.Validate validate = new SKGL.Validate();
                        validate.secretPhase = keyEcrypt;
                        validate.Key = activeKey.key;
                        if (validate.IsValid)
                        {
                            if (activeKey.dayLeft <= 0 && activeKey.activeForm || !activeKey.activeForm && activeKey.dayLeft <= 0)
                            {
                                MessageBox.Show("Vui lòng liên hệ admin để gia hạn key!", "Thông báo");
                                saveData.updateActiveDataKey(0);
                            }
                            else if (!activeKey.activeForm && activeKey.dayLeft > 0)
                            {
                                
                                Form1 frm1 = new Form1();

                                Application.Run(frm1);
                                if (frm1.IsLoggedIn)
                                {
                                    Application.Run(new Dashboard());
                                }
                            }
                            else if (activeKey.activeForm && activeKey.dayLeft > 0)
                            {
                                Application.Run(new Dashboard());
                            }
                        }
                        else
                        {
                            MessageBox.Show("Vui lòng liên hệ admin để mua key!", "Thông báo");
                        }
                        
                    }
                }
            }
            
        }
    }
}