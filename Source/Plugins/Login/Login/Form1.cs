using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MySql.Data.MySqlClient;

namespace Login
{
    public partial class Form_Main : Form
    {
        [DllImport("shell32.dll")]
        public extern static IntPtr ShellExecute(IntPtr hwnd,
                                                 string lpOperation,
                                                 string lpFile,
                                                 string lpParameters,
                                                 string lpDirectory,
                                                 int nShowCmd
                                                );  

        private const String GIS_EXE = "HydroDesktop_1_6_dev.exe";
        private const String DEF_USER = "test";
        private const String DEF_PASS = "test";


        public Form_Main()
        {
            InitializeComponent();
        }

  


        private void btnExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (VerifyAccount(tbUser.Text, tbPass.Text))
            {
                ShellExecute(IntPtr.Zero, "open", GIS_EXE, "", "", 1);
                Environment.Exit(0);
            }
            else
            {
                MessageBox.Show("请输入正确的用户名和密码");
                return;
            }
        }

        private bool VerifyAccount(String strUser, String strPass)
        {
            //if (strUser == DEF_USER && strPass == DEF_PASS)
                //return true;

            //return false;
            return isLogin(strUser,strPass);
        }

        public static string connStr = "server=139.196.126.19;user id=donghua;password=DongHua2017;database=dhdata;port=3306;";
        public static MySqlConnection conn;
        private bool isLogin(string userName, string password)
        {
            if (userName == null || password == null || userName.Length > 20 || password.Length > 20)
            {
                return false;
            }
            try
            {
                conn = new MySqlConnection(connStr);
                string sqlStr = string.Format("select UserNum from users where UserName='{0}' and Password='{1}'", userName, password);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                MySqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    return true;
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }



            return false;
        }

        private void tbPass_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }
    }
}
