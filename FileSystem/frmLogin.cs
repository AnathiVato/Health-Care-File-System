using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheArtOfDevHtmlRenderer.Adapters;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace FileSystem
{
    public partial class frmLogin : Form
    {

        static string connString = "server=localhost;database=health_care_db;pooling=false; Integrated Security=SSPI;uid=root;pwd=maponya;";
        MySqlConnection conn = new MySqlConnection(connString);
        MySqlCommand command;
        MySqlDataAdapter adapter = new MySqlDataAdapter();


        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

        }
        private DataTable GetLogin(string email, string password)
        {
            
            string query = "SELECT employee_id, Email, Password, name, surname,role_name FROM employee INNER JOIN role ON role.role_id = employee.role_id WHERE Email = @Email AND Password = @Password AND Status = 'Active'";

            command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@Password", password);

            DataTable dt = new DataTable();
            adapter.SelectCommand = command;


            adapter.Fill(dt);                    
            
            return dt;
        }
        string getRole;
        public static DataTable dtInfo;
        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            

            dtInfo = GetLogin(txtEmail.Text, txtPassword.Text);
            frmManage admin = new frmManage();
            frmDoctor doc = new frmDoctor();

            if(dtInfo.Rows.Count > 0)
            {
                getRole = dtInfo.Rows[0]["role_name"].ToString().Trim();

                if (getRole == "Administrator")
                {
                    admin.Show();
                    this.Hide();
                } else if (getRole == "Doctor")
                {
                    doc.Show();
                    this.Hide();
                }else if (getRole == "Nurse")
                {
                    doc.Show();
                    this.Hide();
                }
                else
                {
                    lblIncorrect.Text = "Incorrect email or password.";
                    txtEmail.Clear();
                    txtPassword.Clear();
                    
                }
                    
            }
        }
    }
}
