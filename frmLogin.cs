using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RM
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUser.Text;
            string password = txtPass.Text;

            // Check for specific username and password first
            if (username == "admin" && password == "123")
            {
                this.Hide();
                frmMain frm = new frmMain();
                frm.Show();
            }
            else
            {
                guna2MessageDialog1.Show ( "Invalid username or password");
                return;
            }

        }
    }
}
