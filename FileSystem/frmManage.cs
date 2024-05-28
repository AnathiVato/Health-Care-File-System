using FileSystem.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSystem
{
    public partial class frmManage : Form
    {
        public frmManage()
        {
            InitializeComponent();
            UC_Employee emp = new UC_Employee();
            addUserControl(emp);
        }

        private void addUserControl(UserControl userControl)
        {
            userControl.Dock = DockStyle.Fill;
            panelContainer.Controls.Clear();
            panelContainer.Controls.Add(userControl);
            userControl.BringToFront();
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        
        private void btnUser_Click(object sender, EventArgs e)
        {
            UC_Employee emp = new UC_Employee();
            addUserControl(emp);
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            UC_File file = new UC_File();
           
            addUserControl(file);
        }

        private void btnFacility_Click(object sender, EventArgs e)
        {
            UC_Facility facility = new UC_Facility();

            addUserControl(facility);
        }

        private void frmManage_Load(object sender, EventArgs e)
        {
            lblUserDetails.Text =  dt.Rows[0]["surname"].ToString()+ " " + dt.Rows[0]["name"].ToString();
            lblRole.Text = "(" + dt.Rows[0]["role_name"].ToString() + ")";
        }

        DataTable dt = frmLogin.dtInfo;

        private void btnCity_Click(object sender, EventArgs e)
        {
            UC_City city = new UC_City();

            addUserControl(city);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            frmLogin login = new frmLogin();

            login.Show();
            this.Hide();
        }
    }
}
