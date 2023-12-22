using hotmailCheck.Controllers;
using hotmailCheck.Models;
using System.Data.SQLite;

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
            if (activeKey.dayLeft == 0 && activeKey.activeForm || !activeKey.activeForm && activeKey.dayLeft == 0)
            {
                MessageBox.Show("Vui lòng liên hệ admin để gia hạn key!", "Thông báo");
                saveData.updateActiveDataKey(0);
            }
            else if (!activeKey.activeForm && activeKey.dayLeft != 0)
            {
                ApplicationConfiguration.Initialize();
                Form1 frm1 = new Form1();

                Application.Run(frm1);
                if (frm1.IsLoggedIn)
                {
                    Application.Run(new Dashboard());
                }
            }
            else if(activeKey.activeForm && activeKey.dayLeft != 0)
            {
                ApplicationConfiguration.Initialize();
                Application.Run(new Dashboard());
            }
        }
    }
}