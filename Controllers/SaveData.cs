using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hotmailCheck.Controllers
{
    public class SaveData
    {
        SQLiteConnection _con = new SQLiteConnection();

        public SQLiteDataReader loadDataKey()
        {
            createConection();
            string sql = "select * from tbl_key";
            SQLiteCommand command = new SQLiteCommand(sql, _con);
            SQLiteDataReader reader = command.ExecuteReader();
            return reader;
        }

        public void updateActiveDataKey(int activeForm)
        {
            try
            {
                string strUpdate = string.Format("UPDATE tbl_key SET activeForm=\'" + activeForm + "\' WHERE id=\'" + 1 + "\'");
                createConection();
                SQLiteCommand command = new SQLiteCommand(strUpdate, _con);
                command.ExecuteNonQuery();
                closeConnection();
            }
            catch
            {
                Console.WriteLine("Err");
            }
        }

        public void updateDayLeftDataKey(int dayLeft)
        {
            try
            {
                string strUpdate = string.Format("UPDATE tbl_key SET dayLeft=\'" + dayLeft + "\' WHERE id=\'" + 1 + "\'");
                createConection();
                SQLiteCommand command = new SQLiteCommand(strUpdate, _con);
                command.ExecuteNonQuery();
                closeConnection();
            }
            catch
            {
                Console.WriteLine("Err");
            }
        }

        public void createConection()
        {
            var CurrentDirectory = Directory.GetCurrentDirectory();
            Directory.CreateDirectory(CurrentDirectory + @"\Data");
            string _strConnect = "Data Source=" + CurrentDirectory + "\\Data\\Database";
            _con = new SQLiteConnection(_strConnect);
            _con.Open();
        }

        public void closeConnection()
        {
            _con.Close();
        }

        public SQLiteDataReader loadDataAccount()
        {
            createConection();
            string sql = "select * from tbl_account";
            SQLiteCommand command = new SQLiteCommand(sql, _con);
            SQLiteDataReader reader = command.ExecuteReader();
            return reader;
        }

        public SQLiteDataReader getQuality(string username)
        {
            createConection();
            string sql = "select quality from tbl_account WHERE username=\'" + username + "\'";
            SQLiteCommand command = new SQLiteCommand(sql, _con);
            SQLiteDataReader reader = command.ExecuteReader();
            return reader;
        }

        public void updateStatusByID(string username, string status)
        {
            try
            {
                string strUpdate = string.Format("UPDATE tbl_account SET status=\'" + status + "\' WHERE username=\'" + username + "\'");
                createConection();
                SQLiteCommand command = new SQLiteCommand(strUpdate, _con);
                command.ExecuteNonQuery();
                closeConnection();
            }
            catch { }
        }

        public void updateProxyByID(string username, string proxy)
        {
            try
            {
                string strUpdate = string.Format("UPDATE tbl_account SET proxy=\'" + proxy + "\' WHERE username=\'" + username + "\'");
                createConection();
                SQLiteCommand command = new SQLiteCommand(strUpdate, _con);
                command.ExecuteNonQuery();
                closeConnection();
            }
            catch { }

        }

        public void updateQualityByID(string username, string quality)
        {
            try
            {
                string strUpdate = string.Format("UPDATE tbl_account SET quality=\'" + quality + "\' WHERE username=\'" + username + "\'");
                createConection();
                SQLiteCommand command = new SQLiteCommand(strUpdate, _con);
                command.ExecuteNonQuery();
                closeConnection();
            }
            catch { }

        }

        public void saveAccount(string username, string password, string status, string proxy, string quality, string log)
        {
            try
            {
                string strInsert = string.Format("INSERT INTO tbl_account(username, password, status,proxy,log,quality) VALUES(\'" + username + "\',\'" + password + "\',\'" + status + "\',\'" + proxy + "\',\'" + log + "\',\'" + quality + "\')");
                createConection();
                SQLiteCommand command = new SQLiteCommand(strInsert, _con);
                command.ExecuteNonQuery();
                closeConnection();
            }
            catch { }

        }

        public void deleteProfileByAccount(string username)
        {
            try
            {
                string strDelete = string.Format("DELETE FROM tbl_account");
                if (!string.IsNullOrEmpty(username))
                {
                    strDelete = string.Format("DELETE FROM tbl_account WHERE username=\'" + username + "\'");
                }
                createConection();
                SQLiteCommand command = new SQLiteCommand(strDelete, _con);
                command.ExecuteNonQuery();
                closeConnection();
            }
            catch (Exception ex) { }
        }

        public SQLiteDataReader loadDataCC()
        {
            createConection();
            string sql = "select * from tbl_cc";
            SQLiteCommand command = new SQLiteCommand(sql, _con);
            SQLiteDataReader reader = command.ExecuteReader();
            return reader;
        }

        public void saveCC(string vs, string status)
        {
            try
            {
                string strInsert = string.Format("INSERT INTO tbl_cc(vs,status) VALUES(\'" + vs + "\',\'" + status + "\')");
                createConection();
                SQLiteCommand command = new SQLiteCommand(strInsert, _con);
                command.ExecuteNonQuery();
                closeConnection();
            }
            catch { }
        }

        public void deleteTableCC(string vs)
        {
            try
            {
                string strDelete = string.Format("DELETE FROM tbl_cc");
                if (!string.IsNullOrEmpty(vs))
                {
                    strDelete = string.Format("DELETE FROM tbl_cc WHERE vs=\'" + vs + "\'");
                }
                createConection();
                SQLiteCommand command = new SQLiteCommand(strDelete, _con);
                command.ExecuteNonQuery();
                closeConnection();
            }
            catch (Exception ex) { }
        }

        public void updateStatusCC(string vs, string status)
        {
            try
            {
                string strUpdate = string.Format("UPDATE tbl_cc SET status=\'" + status + "\' WHERE vs=\'" + vs + "\'");
                createConection();
                SQLiteCommand command = new SQLiteCommand(strUpdate, _con);
                command.ExecuteNonQuery();
                closeConnection();
            }
            catch {
                Console.WriteLine("Err");
            }
        }

        public SQLiteDataReader loadDataAccountBarnes()
        {
            createConection();
            string sql = "select * from tbl_barnes";
            SQLiteCommand command = new SQLiteCommand(sql, _con);
            SQLiteDataReader reader = command.ExecuteReader();
            return reader;
        }

        public SQLiteDataReader getQualityBarnes(string username)
        {
            createConection();
            string sql = "select quality from tbl_barnes WHERE username=\'" + username + "\'";
            SQLiteCommand command = new SQLiteCommand(sql, _con);
            SQLiteDataReader reader = command.ExecuteReader();
            return reader;
        }

        public void updateStatusByUsernameBarnes(string username, string status)
        {
            try
            {
                string strUpdate = string.Format("UPDATE tbl_barnes SET status=\'" + status + "\' WHERE username=\'" + username + "\'");
                createConection();
                SQLiteCommand command = new SQLiteCommand(strUpdate, _con);
                command.ExecuteNonQuery();
                closeConnection();
            }
            catch { }
        }

        public void updateProxyByUsernameBarnes(string username, string proxy)
        {
            try
            {
                string strUpdate = string.Format("UPDATE tbl_barnes SET proxy=\'" + proxy + "\' WHERE username=\'" + username + "\'");
                createConection();
                SQLiteCommand command = new SQLiteCommand(strUpdate, _con);
                command.ExecuteNonQuery();
                closeConnection();
            }
            catch { }

        }

        public void updateQualityByUsernameBarnes(string username, string quality)
        {
            try
            {
                string strUpdate = string.Format("UPDATE tbl_barnes SET quality=\'" + quality + "\' WHERE username=\'" + username + "\'");
                createConection();
                SQLiteCommand command = new SQLiteCommand(strUpdate, _con);
                command.ExecuteNonQuery();
                closeConnection();
            }
            catch { }

        }

        public void saveAccountBarnes(string username, string password, string status, string proxy, string quality, string log)
        {
            try
            {
                string strInsert = string.Format("INSERT INTO tbl_barnes(username, password, status,proxy,log,quality) VALUES(\'" + username + "\',\'" + password + "\',\'" + status + "\',\'" + proxy + "\',\'" + log + "\',\'" + quality + "\')");
                createConection();
                SQLiteCommand command = new SQLiteCommand(strInsert, _con);
                command.ExecuteNonQuery();
                closeConnection();
            }
            catch { }

        }

        public void deleteProfileByAccountBarnes(string username)
        {
            try
            {
                string strDelete = string.Format("DELETE FROM tbl_barnes");
                if (!string.IsNullOrEmpty(username))
                {
                    strDelete = string.Format("DELETE FROM tbl_barnes WHERE username=\'" + username + "\'");
                }
                createConection();
                SQLiteCommand command = new SQLiteCommand(strDelete, _con);
                command.ExecuteNonQuery();
                closeConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public SQLiteDataReader loadDataProxy()
        {
            createConection();
            string sql = "select * from tbl_proxy";
            SQLiteCommand command = new SQLiteCommand(sql, _con);
            SQLiteDataReader reader = command.ExecuteReader();
            return reader;
        }

        public void saveDataProxy(string sock)
        {
            try
            {
                string strInsert = string.Format("INSERT INTO tbl_proxy(sock) VALUES(\'" + sock + "\')");
                createConection();
                SQLiteCommand command = new SQLiteCommand(strInsert, _con);
                command.ExecuteNonQuery();
            }
            catch { }

        }

        public void deleteProxy(string sock = null)
        {
            try
            {
                string strInsert = string.Format("DELETE FROM tbl_proxy");
                if (!string.IsNullOrEmpty(sock))
                {
                    strInsert = string.Format("DELETE FROM tbl_proxy where sock=\'" + sock + "\'");
                }
                createConection();
                SQLiteCommand command = new SQLiteCommand(strInsert, _con);
                command.ExecuteNonQuery();
            }
            catch
            {
                Console.WriteLine("Error");
            }
        }
    }
}
