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
using Mysqlx;
using Org.BouncyCastle.Asn1.Cmp;
using System.Xml.Linq;
using System.Collections;
using System.Security.Claims;

namespace FileSystem.UserControls
{
    public partial class UC_Patient : UserControl
    {

        static string connString = "server=localhost;database=health_care_db;pooling=false; Integrated Security=SSPI;uid=root;pwd=maponya;";
        MySqlConnection conn = new MySqlConnection(connString);
        MySqlCommand command;
        MySqlCommand secondCommand;
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        MySqlDataAdapter adapter2 = new MySqlDataAdapter();
        DataTable dt = frmLogin.dtInfo;

        public UC_Patient()
        {
            InitializeComponent();
        }

        private void UC_Patient_Load(object sender, EventArgs e)
        {
            LoadProvince();
            LoadGridView();
            LoadAdmitted();
            lblDischargeDate.Hide();
            pkdischargeDate.Hide();

            

        }
        public DataTable GetPatientId(string idNumber)
        {
            DataTable patientData = new DataTable();

            string query = "SELECT * FROM patients_details WHERE dob = @dob";

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@dob", idNumber);

                    // Execute the query and fill the DataTable
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(patientData);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return patientData;
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


                cmbFacilityName.DataSource = dt;
                cmbFacilityName.ValueMember = "facility_id";
                cmbFacilityName.DisplayMember = "Name";

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
                string query = "SELECT patient_id, name, surname,dob , phone_number FROM patients_details";
                conn.Open();
                command = new MySqlCommand(query, conn);
                adapter.SelectCommand = command;

                dt.Clear();
                adapter.Fill(dt);

                dgvPatients.DataSource = dt;
                conn.Close();


                dgvPatients.Columns["patient_id"].DisplayIndex = 0;
                dgvPatients.Columns["name"].DisplayIndex = 1;
                dgvPatients.Columns["surname"].DisplayIndex = 2;
                dgvPatients.Columns["dob"].DisplayIndex = 3;
                dgvPatients.Columns["phone_number"].DisplayIndex = 4;
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void LoadAdmitted()
        {
            cmbAdmitted.Items.Add("Admitted");
            cmbAdmitted.Items.Add("Not-Admitted");
        }
        
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(txtSearch.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");                
                txtSearch.Clear();
            }
            else
            {
                try
                {
                    

                    DataTable dt = new DataTable();
                    DataTable dt2 = new DataTable();
                    string query1 = "SELECT patient_id, name, surname, dob, phone_number FROM patients_details WHERE dob LIKE @Search";

                    string query2 = "SELECT visit_id, f.Name, visit_date, diagnosis, treatment, admitted, discharge_date, e.name, e.surname, last_edited_on" +
                                    " FROM patient_visit AS pv" +
                                    " LEFT JOIN employee AS e ON e.employee_id = pv.employee_id" +
                                    " LEFT JOIN facility AS f ON f.facility_id = pv.facility" +
                                    " LEFT JOIN patients_details AS pd ON pd.patient_id = pv.patient_id" +
                                    " WHERE dob LIKE @Search";

                    command = new MySqlCommand(query1, conn);
                    secondCommand = new MySqlCommand(query2, conn);
                    
                    command.Parameters.AddWithValue("@Search", "%" + txtSearch.Text + "%");
                    secondCommand.Parameters.AddWithValue("@Search", "%" + txtSearch.Text + "%");


                    adapter.SelectCommand = command;
                    adapter2.SelectCommand = secondCommand;
                    adapter.Fill(dt);
                    adapter2.Fill(dt2);

                    dgvPatients.DataSource = dt;
                    dgvRecords.DataSource = dt2;
                    

                    dgvPatients.Columns["patient_id"].DisplayIndex = 0;
                    dgvPatients.Columns["name"].DisplayIndex = 1;
                    dgvPatients.Columns["surname"].DisplayIndex = 2;
                    dgvPatients.Columns["dob"].DisplayIndex = 3;
                    dgvPatients.Columns["phone_number"].DisplayIndex = 4;
                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            
        }
        int employeeId;
        private void btnSave_Click(object sender, EventArgs e)
        {


            DataTable patientData = GetPatientId(txtSearch.Text);
            int patientId = Convert.ToInt32(patientData.Rows[0]["patient_id"]);
            //Formatting Date
            DateTime visitdate = visitDate.Value;
            DateTime dischargeDate = pkdischargeDate.Value;
            string dischargeFormatted = pkdischargeDate.Value.ToString("yyyy-MM-dd");
            string formattedVisit = visitDate.Value.ToString("yyyy-MM-dd");
            DateTime currentDate = DateTime.Now;
            employeeId = int.Parse(dt.Rows[0]["employee_id"].ToString());

            
            if (string.IsNullOrEmpty(txtDiagnosis.Text) || string.IsNullOrEmpty(txtTreatment.Text))
            {
                errorProvider1.SetError(txtDiagnosis, "Cannot insert an empty field");
                errorProvider1.SetError(txtTreatment, "Cannot insert an empty field");

            }
            else
            {

                if (cmbAdmitted.SelectedItem.ToString() == "Admitted")
                {
                    pkdischargeDate.Show();
                    lblDischargeDate.Show();
                    
                    conn.Open();

                    DateTime date = DateTime.Now;
                    string query = "INSERT INTO patient_visit (patient_id, facility, visit_date,admitted, treatment,diagnosis, discharge_date, last_edited_on, employee_id )" +
                                    "VALUES (@patient_id, @facility_id,@visit_date,@admitted,@treatment, @diagnosis, @discharge_date, @last_edited_on, @employee_id)";
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@patient_id", patientId);
                    command.Parameters.AddWithValue("@facility_id", int.Parse(cmbFacilityName.SelectedValue.ToString()));
                    command.Parameters.AddWithValue("@visit_date", formattedVisit);
                    command.Parameters.AddWithValue("@admitted", cmbAdmitted.SelectedItem);
                    command.Parameters.AddWithValue("@treatment", txtTreatment.Text);
                    command.Parameters.AddWithValue("@diagnosis", txtTreatment.Text);
                    command.Parameters.AddWithValue("@discharge_date", dischargeFormatted);
                    command.Parameters.AddWithValue("@last_edited_on", currentDate);
                    command.Parameters.AddWithValue("@employee_id", employeeId);



                    int i = command.ExecuteNonQuery();
                    conn.Close();
                    if (i > 0)
                    {
                        MessageBox.Show("Record saved successfully");
                    }
                    

                    LoadGridView();

                }
                else
                {
                    pkdischargeDate.Show();
                    lblDischargeDate.Show();

                    conn.Open();                   


                    string query = "INSERT INTO patient_visit (patient_id, facility, visit_date,admitted, treatment,diagnosis, last_edited_on, employee_id)" +
                                    "VALUES (@patient_id, @facility_id,@visit_date,@admitted,@treatment, @diagnosis, @last_edited_on, @employee_id)";
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@patient_id", patientId);
                    command.Parameters.AddWithValue("@facility_id", int.Parse(cmbFacilityName.SelectedValue.ToString()));
                    command.Parameters.AddWithValue("@visit_date", formattedVisit);
                    command.Parameters.AddWithValue("@admitted", cmbAdmitted.SelectedItem);
                    command.Parameters.AddWithValue("@treatment", txtTreatment.Text);
                    command.Parameters.AddWithValue("@diagnosis", txtTreatment.Text);
                    command.Parameters.AddWithValue("@last_edited_on", currentDate);
                    command.Parameters.AddWithValue("@employee_id", employeeId);



                    int i = command.ExecuteNonQuery();

                    if (i > 0)
                    {
                        MessageBox.Show("Record saved successfully");
                    }
                    conn.Close();

                    LoadGridView();
                }



            }

        }

        private void cmbCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadFacility(cmbCity.SelectedValue.ToString());
        }

        private void cmbProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCity(cmbProvince.SelectedValue.ToString());
        }

        private void cmbAdmitted_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAdmitted.SelectedItem.ToString() == "Admitted")
            {
                pkdischargeDate.Show();
                lblDischargeDate.Show();
            }
            else
            {
                pkdischargeDate.Hide();
                lblDischargeDate.Hide();
            }
        }
    }
}
