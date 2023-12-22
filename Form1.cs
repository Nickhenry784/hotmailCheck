using FoxLearn.License;
using hotmailCheck.Controllers;
using hotmailCheck.Models;
using hotmailCheck.Properties;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;

namespace hotmailCheck
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public bool IsLoggedIn { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                SaveData saveData = new SaveData();
                SQLiteDataReader activeFormResult = saveData.loadDataKey();
                ActiveKey activeKey = new ActiveKey();
                //LRYVQ-WOJMN-PAKJV-JLFJC
                while (activeFormResult.Read())
                {
                    activeKey.key = activeFormResult.GetString(1);
                    activeKey.creationDate = activeFormResult.GetString(2);
                    activeKey.expireDate = activeFormResult.GetString(3);
                    activeKey.dayLeft = activeFormResult.GetInt32(4);
                    activeKey.activeForm = activeFormResult.GetInt32(5) == 0 ? false : true;
                }
                if (!textBox1.Text.Equals(activeKey.key) && activeKey.dayLeft != 0)
                {
                    MessageBox.Show("Vui lòng kiểm tra key!", "Thông báo");
                    return;
                }
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
                        validate.Key = textBox1.Text;
                        this.Close();
                        saveData.updateActiveDataKey(1);
                        IsLoggedIn = true;
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error");
            }
        }
    }
}