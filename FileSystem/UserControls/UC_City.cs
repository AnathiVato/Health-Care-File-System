using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace FileSystem.UserControls
{
    public partial class UC_City : UserControl
    {
        static string connString = "server=localhost;database=health_care_db;pooling=false; Integrated Security=SSPI;uid=root;pwd=maponya;";
        MySqlConnection conn = new MySqlConnection(connString);        
        MySqlCommand command;
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        
        public UC_City()
        {
            InitializeComponent();
        }

        private void UC_City_Load(object sender, EventArgs e)
        {

            LoadProvince();
            LoadGridView();
            int records = dgvCity.Rows.Count;
            lblRecordNumber.Text = records.ToString();
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
        private void LoadGridView()
        {
            try
            {
                DataTable dt = new DataTable();
                string query = "SELECT City_id, Cityname, Address, Provname FROM city INNER JOIN province ON city.Province_id = province.Province_id"; 
                conn.Open();
                command = new MySqlCommand(query, conn);                
                adapter.SelectCommand = command;

                dt.Clear();
                adapter.Fill(dt);

                dgvCity.DataSource = dt;
                conn.Close() ;
                
                //Headings
                dgvCity.Columns["City_id"].HeaderText = "City ID";
                dgvCity.Columns["Cityname"].HeaderText = "City Name";
                dgvCity.Columns["Address"].HeaderText = "Address";
                dgvCity.Columns["Provname"].HeaderText = "Province Name";

                dgvCity.Columns["City_id"].DisplayIndex = 0;
                dgvCity.Columns["Cityname"].DisplayIndex = 1;
                dgvCity.Columns["Address"].DisplayIndex = 2;
                dgvCity.Columns["Provname"].DisplayIndex = 3;
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
                if (string.IsNullOrEmpty(txtCityName.Text) || string.IsNullOrEmpty(txtAddress.Text))
                {
                    errorProvider.SetError(txtCityName, "Cannot insert an empty value");
                    errorProvider.SetError(txtAddress, "Cannot insert an empty value");
                }
                else
                {

                    conn.Open();

                    string query = "INSERT INTO city (Cityname, Address,Province_id)" +
                        "VALUES (@CityName, @Address,@ProvinceId)";
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@CityName", txtCityName.Text);
                    command.Parameters.AddWithValue("@Address", txtAddress.Text);
                    command.Parameters.AddWithValue("@ProvinceId", int.Parse(cmbProvince.SelectedValue.ToString()));

                    int i = command.ExecuteNonQuery();

                    if (i > 0)
                    {
                        MessageBox.Show("Record saved successfully");
                    }
                    conn.Close();
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
            if (string.IsNullOrEmpty(txtCityName.Text) || string.IsNullOrEmpty(txtAddress.Text))
            {
                errorProvider.SetError(txtCityName, "Cannot insert an empty value");
                errorProvider.SetError(txtAddress, "Cannot insert an empty value");
            }
            else
            {
                try
                {
                    conn.Open();

                    string query = "UPDATE City SET Cityname =  @CityName, Address = @Address, province_id = @ProvinceId WHERE city_id = @CityId";
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@CityName", txtCityName.Text);
                    command.Parameters.AddWithValue("@Address", txtAddress.Text);
                    command.Parameters.AddWithValue("@ProvinceId", int.Parse(cmbProvince.SelectedValue.ToString()));


                    int cityId = Convert.ToInt32(dgvCity.SelectedRows[0].Cells["City_id"].Value);
                    command.Parameters.AddWithValue("@CityId", cityId);


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

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string query = "SELECT City_id, Cityname, Address, Provname FROM city INNER JOIN province ON city.Province_id = province.Province_id AND Cityname LIKE '@Search%'";

                DataTable dt = new DataTable();
                conn.Open();
                command = new MySqlCommand(query, conn);
                adapter.SelectCommand = command;

                command.Parameters.AddWithValue("@Search", txtSearch.Text);

                
                adapter.Fill(dt);

                dgvCity.DataSource = dt;
                conn.Close();
                LoadGridView();
                dgvCity.Columns["City_id"].DisplayIndex = 0;
                dgvCity.Columns["Cityname"].DisplayIndex = 1;
                dgvCity.Columns["Address"].DisplayIndex = 2;
                dgvCity.Columns["Provname"].DisplayIndex = 3;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
