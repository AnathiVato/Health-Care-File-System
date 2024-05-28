using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using MySql.Data;
using MySql.Data.MySqlClient;
using TheArtOfDevHtmlRenderer.Adapters;
using Mysqlx;




namespace FileSystem.UserControls
{
    public partial class UC_File : UserControl
    {

        static string connString = "server=localhost;database=health_care_db;pooling=false; Integrated Security=SSPI;uid=root;pwd=maponya;";
        MySqlConnection conn = new MySqlConnection(connString);
        MySqlCommand command;
        MySqlDataAdapter adapter = new MySqlDataAdapter();
              
        
        public UC_File()
        {
            InitializeComponent();
        }

        private void UC_File_Load(object sender, EventArgs e)
        {

            cmbFacility.Hide();
            cmbCity.Hide();
            cmbProvince.Hide();
            Province.Hide();
            lblProvince.Hide();
            lblFacility.Hide();


            lblRecordNumber.Text = dgvFiles.Rows.Count.ToString();
            lblDigits.Hide();
            LoadGridView();
            LoadProvince();
            LoadGender();
        }

        public void LoadGender()
        {
            cmbGender.Items.Add("Female");
            cmbGender.Items.Add("Male");
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(txtSearch.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                txtSearch.Text = txtSearch.Text.Remove(txtSearch.Text.Length - 1);
            }
        }

        private void txtIDNum_TextChanged(object sender, EventArgs e)
        {
            
            
            if (System.Text.RegularExpressions.Regex.IsMatch(txtIDNum.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                txtIDNum.Text = txtIDNum.Text.Remove(txtIDNum.Text.Length - 1);
            }
            if(txtIDNum.Text.Length < 13)
            {
                lblDigits.ForeColor = Color.Red;
                lblDigits.Text = "ID Number must be 13 digits";
                lblDigits.Visible = true;
            }else if(txtIDNum.Text.Length == 13)
            {
                lblDigits.ForeColor = Color.Green;
                lblDigits.Text = "Good job!";
                lblDigits.Visible = true;
            }
            else if(txtIDNum.Text.Length > 13)
            {
                MessageBox.Show("ID number cannot exceed 13 digits.");
                lblDigits.Visible = false;
            }          
            
            
            
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtName.Text, "^[a-zA-Z ]*$"))
            {
                
                MessageBox.Show("Name accepts alphabetical characters only.");

                
            }
        }

        private void txtSurname_TextChanged(object sender, EventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtSurname.Text, "^[a-zA-Z ]*$"))
            {

                MessageBox.Show("Surname accepts alphabetical characters only.");


            }
        }

        private void dgvFiles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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
                string query = "SELECT patient_id, name, surname,dob ,sex, last_updated, phone_number FROM patients_details";
                conn.Open();
                command = new MySqlCommand(query, conn);
                adapter.SelectCommand = command;

                dt.Clear();
                adapter.Fill(dt);

                dgvFiles.DataSource = dt;
                conn.Close();
                               

                dgvFiles.Columns["patient_id"].DisplayIndex = 0;
                dgvFiles.Columns["name"].DisplayIndex = 1;
                dgvFiles.Columns["surname"].DisplayIndex = 2;
                dgvFiles.Columns["dob"].DisplayIndex = 3;
                dgvFiles.Columns["phone_number"].DisplayIndex = 4;
                dgvFiles.Columns["last_updated"].DisplayIndex = 5;
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void btnSave_Click(object sender, EventArgs e)
        {

            string idNumber = txtIDNum.Text;
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;
            
            // Extract birth year, month, and day
            string birthYearStr = idNumber.Substring(0, 2);
            string birthMonthStr = idNumber.Substring(2, 2);
            string birthDayStr = idNumber.Substring(4, 2);

            //Extracting the last two digits of a year 
            string yearstring = currentYear.ToString();
            string twoYearDigs = yearstring.Substring(yearstring.Length - 2);
            int lastTwoDigs = int.Parse(twoYearDigs);

            //Converting months to integer so I can validate the months of february to not accept dates more than 29
            int birthDay = int.Parse(birthDayStr);
            int birthMonth = int.Parse(birthMonthStr);

            //Geting the 08 part at the end of the ID
            string idEnd = idNumber.Substring(idNumber.Length - 3, 2);

            // Validate birth year
           if (int.Parse(birthMonthStr) < 1 || int.Parse(birthMonthStr) > 12)// Validate birth month
            {
                
                errorMonth.SetError(txtIDNum, "Invalid month.");                
                return;
            }
            else if (birthDay < 1 || birthDay > 31)// Validate birth day
            {
                errorDay.SetError(txtIDNum, "Invalid day");                
                return;
            }
            else if (birthMonth == 2 && birthDay > 29) // February
            {
                
                errorDay.SetError(txtIDNum, "February only has 1 - 29 days.");                
                return;
            }
            else if (idEnd != "08")// Validate ID number format
            {
                MessageBox.Show("Invalid ID number format, correct format ends with 08.");
                return;
            }
            else
            {
                
                try
                {
                    if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtSurname.Text) || string.IsNullOrEmpty(txtIDNum.Text))
                    {
                        errorText.SetError(txtName, "Cannot insert an empty value");
                        errorText.SetError(txtSurname, "Cannot insert an empty value");
                        errorText.SetError(txtIDNum, "Cannot insert an empty value");
                    }
                    else
                    {

                        conn.Open();

                        DateTime date = DateTime.Now;
                        string query = "INSERT INTO patients_details (Name, Surname,dob, sex, last_updated,phone_number)" +
                            "VALUES (@Name, @Surname,@dob, @sex, @last_updated, @phone_number)";
                        command.CommandText = query;

                        command.Parameters.AddWithValue("@Name", txtName.Text);
                        command.Parameters.AddWithValue("@Surname", txtSurname.Text);
                        command.Parameters.AddWithValue("@dob", txtIDNum.Text);
                        command.Parameters.AddWithValue("@sex", cmbGender.SelectedItem);                        
                        command.Parameters.AddWithValue("@last_updated", date);
                        command.Parameters.AddWithValue("@phone_number", txtPhone.Text);

                        int i = command.ExecuteNonQuery();

                        if (i > 0)
                        {
                            MessageBox.Show("Record saved successfully");
                        }
                        conn.Close();
                        lblRecordNumber.Text = dgvFiles.Rows.Count.ToString();
                        LoadGridView();

                    }

                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    MessageBox.Show(message);
                }
            }

            


        }

        private void cmbFacility_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void cmbCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadFacility(cmbCity.SelectedValue.ToString());
        }

        private void cmbProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCity(cmbProvince.SelectedValue.ToString());
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string idNumber = txtIDNum.Text;
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;

            // Extract birth year, month, and day
            string birthYearStr = idNumber.Substring(0, 2);
            string birthMonthStr = idNumber.Substring(2, 2);
            string birthDayStr = idNumber.Substring(4, 2);

            //Extracting the last two digits of a year 
            string yearstring = currentYear.ToString();
            string twoYearDigs = yearstring.Substring(yearstring.Length - 2);
            int lastTwoDigs = int.Parse(twoYearDigs);

            //Converting months to integer so I can validate the months of february to not accept dates more than 29
            int birthDay = int.Parse(birthDayStr);
            int birthMonth = int.Parse(birthMonthStr);

            //Geting the 08 part at the end of the ID
            string idEnd = idNumber.Substring(idNumber.Length - 3, 2);

            // Validate birth year
            if (int.Parse(birthMonthStr) < 1 || int.Parse(birthMonthStr) > 12)// Validate birth month
            {

                errorMonth.SetError(txtIDNum, "Invalid month.");
                return;
            }
            else if (birthDay < 1 || birthDay > 31)// Validate birth day
            {
                errorDay.SetError(txtIDNum, "Invalid day");
                return;
            }
            else if (birthMonth == 2 && birthDay > 29) // February
            {

                errorDay.SetError(txtIDNum, "February only has 1 - 29 days.");
                return;
            }
            else if (idEnd != "08")// Validate ID number format
            {
                MessageBox.Show("Invalid ID number format, correct format ends with 08.");
                return;
            }
            else
            {
                try
                {
                    DateTime date = DateTime.Now;
                    conn.Open();

                    string query = "UPDATE patients_details SET name =  @Name, surname =  @Surname, dob = @dob,sex = @sex, last_updated = @last_updated, phone_number = @phone_number WHERE patient_id = @patientId";
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@Name", txtName.Text);
                    command.Parameters.AddWithValue("@Surname", txtSurname.Text);
                    command.Parameters.AddWithValue("@dob", txtIDNum.Text);
                    command.Parameters.AddWithValue("@sex", cmbGender.SelectedItem);
                    command.Parameters.AddWithValue("@last_updated", date);
                    command.Parameters.AddWithValue("@phone_number", txtPhone.Text);


                    int cityId = Convert.ToInt32(dgvFiles.SelectedRows[0].Cells["patient_id"].Value);
                    command.Parameters.AddWithValue("@patientId", cityId);


                    int i = command.ExecuteNonQuery();

                    if (i > 0)
                    {
                        MessageBox.Show("Record updated successfully");
                    }
                    conn.Close();
                    LoadGridView();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                
                MessageBox.Show("Please enter letters of the alphabet only.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                
                e.Handled = true;
            }
        }

        private void txtSurname_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar))
            {

                MessageBox.Show("Please enter letters of the alphabet only.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);


                e.Handled = true;
            }
        }
    }
}
