using MySql.Data.MySqlClient;
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

namespace FileSystem.UserControls
{
    public partial class UC_Facility : UserControl
    {

        static string connString = "server=localhost;database=health_care_db;pooling=false; Integrated Security=SSPI;uid=root;pwd=maponya;";
        MySqlConnection conn = new MySqlConnection(connString);
        MySqlCommand command;
        MySqlDataAdapter adapter = new MySqlDataAdapter();

        public UC_Facility()
        {
            InitializeComponent();
        }
        public void LoadType()
        {
            cmbType.Items.Add("Clinic");
            cmbType.Items.Add("Hospital");
        }

        

        private void UC_Facility_Load(object sender, EventArgs e)
        {
            lblRecordNumber.Text = dgvFacilities.Rows.Count.ToString();
            LoadGridView();
            LoadProvince();
            LoadType();
        }        

       

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtName.Text, "^[a-zA-Z ]*$"))
            {

                MessageBox.Show("Name accepts alphabetical characters only.");

            }
        }
        private void LoadGridView()
        {
            try
            {
                DataTable dt = new DataTable();
                string query = "SELECT facility_id, Name, facility.Address, Type, Cityname FROM facility INNER JOIN city ON facility.City_id = city.City_id";
                conn.Open();
                command = new MySqlCommand(query, conn);
                adapter.SelectCommand = command;

                dt.Clear();
                adapter.Fill(dt);

                dgvFacilities.DataSource = dt;
                conn.Close();

                //Headings                

                dgvFacilities.Columns["facility_id"].DisplayIndex = 0;
                dgvFacilities.Columns["Name"].DisplayIndex = 1;
                dgvFacilities.Columns["Address"].DisplayIndex = 2;
                dgvFacilities.Columns["Type"].DisplayIndex = 3;
                dgvFacilities.Columns["Cityname"].DisplayIndex = 4;
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


        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtAddress.Text))
                {
                    error.SetError(txtName, "Cannot insert an empty value");
                    error.SetError(txtAddress, "Cannot insert an empty value");
                }
                else
                {

                    conn.Open();

                    string query = "INSERT INTO facility (Name, Address,Type,city_id)" +
                        "VALUES (@Name, @Address,@Type, @cityId)";
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@Name", txtName.Text);
                    command.Parameters.AddWithValue("@Address", txtAddress.Text);
                    command.Parameters.AddWithValue("@Type", cmbType.SelectedItem);
                    command.Parameters.AddWithValue("@cityId", int.Parse(cmbCity.SelectedValue.ToString()));

                    int i = command.ExecuteNonQuery();

                    if (i > 0)
                    {
                        MessageBox.Show("Record saved successfully");
                    }
                    conn.Close();
                    lblRecordNumber.Text = dgvFacilities.Rows.Count.ToString();
                    LoadGridView();

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void cmbCity_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbProvince_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCity(cmbProvince.SelectedValue.ToString());
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAddress.Text) )
            {
                error.SetError(txtAddress, "Cannot insert an empty value");
                
            }
            else
            {
                try
                {
                    conn.Open();

                    string query = "UPDATE facility SET Name =  @Name, Address =  @Address, Type = @Type, city_id = @city_id WHERE facility_id = @facilityId";
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@Name", txtName.Text);
                    command.Parameters.AddWithValue("@Address", txtAddress.Text);
                    command.Parameters.AddWithValue("@Type", cmbType.SelectedItem);
                    command.Parameters.AddWithValue("@city_id", int.Parse(cmbCity.SelectedValue.ToString()));


                    int cityId = Convert.ToInt32(dgvFacilities.SelectedRows[0].Cells["facility_id"].Value);
                    command.Parameters.AddWithValue("@facilityId", cityId);


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
    }
}
