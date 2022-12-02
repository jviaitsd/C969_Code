using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C969_PerformanceAssessment
{
    public partial class AddCustomer : Form
    {
        // Connection String
        string constrAddCust = ConfigurationManager.ConnectionStrings["clientdb"].ConnectionString;

        public AddCustomer()
        {
            InitializeComponent();
            GetCustData(@"SELECT * 
                            FROM address, 
                                city, 
                                country, 
                                customer, 
                                user");
        }

        private void GetCustData(string getAddCust)
        {
            DataTable dataTableCust = new DataTable();
            MySqlConnection conCust = new MySqlConnection(constrAddCust);
            conCust.Open();
            MySqlCommand cmdCust = new MySqlCommand(getAddCust, conCust);
            MySqlDataAdapter adpAddCust = new MySqlDataAdapter(cmdCust);
            adpAddCust.Fill(dataTableCust);
            conCust.Close();
        }
       
        private void AddCust()
        {
            try 
            { 
                string addCountry = @"INSERT INTO country 
                                        VALUES (Null, 
                                                @country, 
                                                Now(), 
                                                'test', 
                                                Now(),
                                                'test')";
                string addCity = @"INSERT INTO city 
                                    VALUES (Null, 
                                            @city, 
                                            @countryId, 
                                            Now(), 
                                            'test', 
                                            Now(), 
                                            'test')";
                string addAddress = @"INSERT INTO address 
                                        VALUES (Null, 
                                                @address, 
                                                @address2, 
                                                @cityId, 
                                                @postalCode, 
                                                @phone, 
                                                Now(), 
                                                'test', 
                                                Now(), 
                                                'test')";
                string addCustName = @"INSERT INTO customer 
                                        VALUES (Null, 
                                                @customerName, 
                                                @addressId, 
                                                @active, 
                                                Now(), 
                                                'test', 
                                                Now(), 
                                                'test')";

                MySqlConnection conAddCust = new MySqlConnection(constrAddCust);
                conAddCust.Open();
                MySqlCommand cmdAddCountry = new MySqlCommand(addCountry, conAddCust);
                cmdAddCountry.Parameters.AddWithValue("@country", AddCountryTB.Text);
                cmdAddCountry.ExecuteNonQuery();
                int countryId = (int)cmdAddCountry.LastInsertedId;

                MySqlCommand cmdAddCity = new MySqlCommand(addCity, conAddCust);
                cmdAddCity.Parameters.AddWithValue("@city", AddCityTB.Text);
                cmdAddCity.Parameters.AddWithValue("@countryId", countryId);
                cmdAddCity.ExecuteNonQuery();
                int cityId = (int)cmdAddCity.LastInsertedId;

                MySqlCommand cmdAddAddress = new MySqlCommand(addAddress, conAddCust);
                cmdAddAddress.Parameters.AddWithValue("@address", AddAddressTB.Text);
                cmdAddAddress.Parameters.AddWithValue("@address2", AddAddress2TB.Text);
                cmdAddAddress.Parameters.AddWithValue("@cityId", cityId);
                cmdAddAddress.Parameters.AddWithValue("@postalCode", AddPostalTB.Text);
                cmdAddAddress.Parameters.AddWithValue("@phone", AddPhoneTB.Text);
                cmdAddAddress.ExecuteNonQuery();
                int addressId = (int)cmdAddAddress.LastInsertedId;

                MySqlCommand cmdAddCustName = new MySqlCommand(addCustName, conAddCust);
                cmdAddCustName.Parameters.AddWithValue("@customerName", AddCustNameTB.Text);
                cmdAddCustName.Parameters.AddWithValue("@addressId", addressId);
                cmdAddCustName.Parameters.AddWithValue("@active", AddActiveRB.Checked);
                cmdAddCustName.ExecuteNonQuery();
                conAddCust.Close();
            }

            catch (MySqlException ex)
            {
                MessageBox.Show("Error:" + "\n" + ex.Message);
                return;
            }
        }

        // Inserts new Customer into database
        private void AddCustBTN_Click(object sender, EventArgs e)
        {
            DataTable dataTableAdd = new DataTable();
            try
            {
                if (string.IsNullOrWhiteSpace(AddCustNameTB.Text))
                {
                    MessageBox.Show("Enter Customer's Name");
                    return;
                }
                if (string.IsNullOrWhiteSpace(AddAddressTB.Text))
                {
                    MessageBox.Show("Enter Customer's Address");
                    return;
                }
                if (string.IsNullOrWhiteSpace(AddCityTB.Text))
                {
                    MessageBox.Show("Enter Customer's City");
                    return;
                }
                if (string.IsNullOrWhiteSpace(AddPostalTB.Text))
                {
                    MessageBox.Show("Enter Customer's Postal Code");
                    return;
                }
                if (string.IsNullOrWhiteSpace(AddPhoneTB.Text))
                {
                    MessageBox.Show("Enter Customer's Phone Number");
                    return;
                }
                if (!AddActiveRB.Checked && !AddActiveRB.Checked)
                {
                    MessageBox.Show("Check Active");
                    return;
                }
                else
                {
                    AddCust();
                    MessageBox.Show("New Customer Added");
                    dataTableAdd.Clear();
                    this.Close();
                    CustomersPage custPage = new CustomersPage();
                    custPage.Show();
                                  }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error" + "\n" + ex.Message);
                return;
            }
        }

        // Cancel doesn't insert new customer into database
        private void AddCustCancelBTN_Click(object sender, EventArgs e)
        {
            DataTable dataTableCancel = new DataTable();
            MessageBox.Show("Customer Not Added");
            this.Close();
            dataTableCancel.Clear();
            CustomersPage custPage = new CustomersPage();
            custPage.Show();
        }
    }
}
