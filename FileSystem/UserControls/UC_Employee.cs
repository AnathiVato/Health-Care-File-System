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
using Org.BouncyCastle.Asn1.Cmp;

namespace FileSystem.UserControls
{
    public partial class UC_Employee : UserControl
    {

        static string connString = "server=localhost;database=health_care_db;pooling=false; Integrated Security=SSPI;uid=root;pwd=maponya;";
        MySqlConnection conn = new MySqlConnection(connString);
        MySqlCommand command;
        MySqlDataAdapter adapter = new MySqlDataAdapter();


        public UC_Employee()
        {
            InitializeComponent();
        }

        private void UC_Employee_Load(object sender, EventArgs e)
        {
            lblRecordNumber.Text = dgvEmployees.Rows.Count.ToString();
            LoadProvince();
            LoadGridView();
            LoadGender();
            LoadRole();
            LoadStatus();
        }

        public void LoadRole()
        {

            try
            {
                DataTable dt = new DataTable();
                string query = "SELECT role_id, role_name FROM role";

                command = new MySqlCommand(query, conn);
                adapter.SelectCommand = command;

                adapter.Fill(dt);

                conn.Open();
                cmbRole.DataSource = dt;
                cmbRole.ValueMember = "role_id";
                cmbRole.DisplayMember = "role_name";

                conn.Close();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void LoadProvince()
        {
            try
            {
                DataTable dt = new DataTable();
                string query = "SELECT Province_id, Provname FROM province";

                command = new MySqlCommand(query, conn);
                adapter.SelectCommand = command;

                adapter.Fill(dt);

                conn.Open();
                cmbProvince.DataSource = dt;
                cmbProvince.ValueMember = "Province_id";
                cmbProvince.DisplayMember = "Provname";

                conn.Close();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void LoadCity(string provinceId)
        {


            try
            {

                string query = "SELECT City_id, Cityname FROM City INNER JOIN Province ON Province.Province_id = City.Province_id AND Province.Province_id = @ProvId";

                command = new MySqlCommand(query, conn);
                command.Parameters.AddWithValue("@ProvId", provinceId);

                DataTable dt = new DataTable();
                adapter.SelectCommand = command;

                //conn.Open();
                adapter.Fill(dt);
                //conn.Close();

                cmbCity.DataSource = dt;
                cmbCity.ValueMember = "City_id";
                cmbCity.DisplayMember = "Cityname";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void LoadFacility(string cityId)
        {


            try
            {

                string query = "SELECT facility_id, Name FROM facility INNER JOIN city ON city.City_id = facility.City_id AND facility.City_id = @CityId";

                command = new MySqlCommand(query, conn);
                command.Parameters.AddWithValue("@CityId", cityId);

                DataTable dt = new DataTable();
                adapter.SelectCommand = command;


                adapter.Fill(dt);


                cmbFacility.DataSource = dt;
                cmbFacility.ValueMember = "facility_id";
                cmbFacility.DisplayMember = "Name";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void LoadGridView()
        {
            try
            {
                DataTable dt = new DataTable();
                string query = "SELECT employee_id, employee.name, surname, email,password, gender, role_name, phone_number, facility.Name AS facility, status "
                    + "FROM employee INNER JOIN facility ON facility.facility_id = employee.facility_id"+
                    " INNER JOIN role ON role.role_id = employee.role_id";
                conn.Open();
                command = new MySqlCommand(query, conn);
                adapter.SelectCommand = command;

                dt.Clear();
                adapter.Fill(dt);

                dgvEmployees.DataSource = dt;
                conn.Close();
                             

                dgvEmployees.Columns["employee_id"].DisplayIndex = 0;
                dgvEmployees.Columns["Name"].DisplayIndex = 1;
                dgvEmployees.Columns["surname"].DisplayIndex = 2;
                dgvEmployees.Columns["email"].DisplayIndex = 3;
                dgvEmployees.Columns["password"].DisplayIndex = 4;
                dgvEmployees.Columns["gender"].DisplayIndex = 5;
                dgvEmployees.Columns["role_name"].DisplayIndex = 6;
                dgvEmployees.Columns["phone_number"].DisplayIndex =7;
                dgvEmployees.Columns["facility"].DisplayIndex = 8;
                dgvEmployees.Columns["status"].DisplayIndex = 9;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public void LoadGender()
        {
            cmbGender.Items.Add("Female");
            cmbGender.Items.Add("Male");
        }

        public void LoadStatus()
        {
            cmbStatus.Items.Add("Active");
            cmbStatus.Items.Add("In-Active");
        }

        private void cmbProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCity(cmbProvince.SelectedValue.ToString());
        }

        private void cmbCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadFacility(cmbCity.SelectedValue.ToString());
        }
        //** Password To Play Around Withi
        //9@5sW0rd
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtSurname.Text) 
                    || string.IsNullOrEmpty(txtNumber.Text)|| string.IsNullOrEmpty(txtPassword.Text) || string.IsNullOrEmpty(txtEmail.Text))
                {
                    error.SetError(txtName, "Cannot insert an empty field");
                    error.SetError(txtSurname, "Cannot insert an empty field");
                    error.SetError(txtPassword, "Cannot insert an empty field");
                    error.SetError(txtNumber, "Cannot insert an empty field");
                    error.SetError(txtEmail, "Cannot insert an empty field");
                }
                else
                {
                    conn.Open();
                    
                    DateTime date = DateTime.Now;
                    string query = "INSERT INTO employee (name, surname, email,gender, password,role_id, facility_id, phone_number, status)" +
                        "VALUES (@Name, @Surname,@Email,@gender, @password, @role_id, @facility_id, @phone_number, @status)";
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@Name", txtName.Text);
                    command.Parameters.AddWithValue("@Surname", txtSurname.Text);
                    command.Parameters.AddWithValue("@Email", txtEmail.Text);
                    command.Parameters.AddWithValue("@gender", cmbGender.Text);
                    command.Parameters.AddWithValue("@password", txtPassword.Text);
                    command.Parameters.AddWithValue("@role_id", int.Parse(cmbRole.SelectedValue.ToString()));
                    command.Parameters.AddWithValue("@facility_id", int.Parse(cmbFacility.SelectedValue.ToString()));
                    command.Parameters.AddWithValue("@phone_number", txtNumber.Text);
                    command.Parameters.AddWithValue("@status", cmbStatus.SelectedItem);


                    int i = command.ExecuteNonQuery();

                    if (i > 0)
                    {
                        MessageBox.Show("Record saved successfully");
                    }
                    conn.Close();
                    lblRecordNumber.Text = dgvEmployees.Rows.Count.ToString();
                    LoadGridView();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtSurname.Text)
                    || string.IsNullOrEmpty(txtNumber.Text) || string.IsNullOrEmpty(txtPassword.Text))
                {
                    error.SetError(txtName, "Cannot insert an empty field");
                    error.SetError(txtSurname, "Cannot insert an empty field");
                    error.SetError(txtPassword, "Cannot insert an empty field");
                    error.SetError(txtNumber, "Cannot insert an empty field");
                }
                else
                {
                    conn.Open();

                    string query = "UPDATE employee SET name =  @Name, surname =  @Surname, gender = @gender, password = @password, " +
                        " role_id = @role_id, facility_id = @facility_id, phone_number = @phone_number, status = @status, email = @email " +
                        "WHERE employee_id = @employeeId";
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@Name", txtName.Text);
                    command.Parameters.AddWithValue("@Surname", txtSurname.Text);
                    command.Parameters.AddWithValue("@gender", cmbGender.Text);
                    command.Parameters.AddWithValue("@password", txtPassword.Text);
                    command.Parameters.AddWithValue("@role_id", int.Parse(cmbRole.SelectedValue.ToString()));
                    command.Parameters.AddWithValue("@facility_id", int.Parse(cmbFacility.SelectedValue.ToString()));
                    command.Parameters.AddWithValue("@phone_number", txtNumber.Text);
                    command.Parameters.AddWithValue("@status", cmbStatus.SelectedItem);
                    command.Parameters.AddWithValue("@email", txtEmail.Text);


                    int cityId = Convert.ToInt32(dgvEmployees.SelectedRows[0].Cells["employee_id"].Value);
                    command.Parameters.AddWithValue("@employeeId", cityId);


                    int i = command.ExecuteNonQuery();

                    if (i > 0)
                    {
                        MessageBox.Show("Record updated successfully");
                    }
                    conn.Close();
                    LoadGridView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvEmployees_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dgvEmployee_SelectionChanged(object sender, EventArgs e)
        {
            // Check if a row is selected
            if (dgvEmployees.SelectedRows.Count > 0)
            {
                // Get the first selected row
                DataGridViewRow selectedRow = dgvEmployees.SelectedRows[0];
                
                txtName.Text = selectedRow.Cells[1].Value.ToString();
                txtSurname.Text = selectedRow.Cells[2].Value.ToString();
                txtNumber.Text = selectedRow.Cells[7].Value.ToString();
                txtEmail.Text = selectedRow.Cells[3].Value.ToString();                
                txtPassword.Text = selectedRow.Cells[4].Value.ToString();
                
            }
        }

        private void dgvEmployees_SelectionChanged(object sender, EventArgs e)
        {
            dgvEmployees.SelectionChanged += dgvEmployee_SelectionChanged;
        }
    }
}
