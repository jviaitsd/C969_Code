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
    public partial class UpdateCustomer : Form
    {
        string constrUpdCust = ConfigurationManager.ConnectionStrings["clientdb"].ConnectionString;
        public UpdateCustomer()
        {
            InitializeComponent();
            GetCustData(@"SELLECT * 
                            From address, 
                                 city, 
                                 country, 
                                 customers, 
                                 user");
        }

        public UpdateCustomer(
            int customerId,
            string customerName,
            string address,
            string address2,
            string city,
            string postalCode,
            string country,
            string phone,
            int countryId,
            int cityId,
            int addressId)
        {
            InitializeComponent();
            UpdCustIdTB.Text = customerId.ToString();
            UpdCustNameTB.Text = customerName;
            UpdAddressTB.Text = address;
            UpdAddress2TB.Text = address2;
            UpdCityTB.Text = city;
            UpdPostalTB.Text = postalCode;
            UpdCountryTB.Text = country;
            UpdPhoneTB.Text = phone;
            UpdCountryIdTB.Text = countryId.ToString();
            UpdCityIdTB.Text = cityId.ToString();
            UpdAddressIdTB.Text = addressId.ToString();
        }

        private void GetCustData(string getCust)
        {
            DataTable dataTableCust = new DataTable();
            MySqlConnection conCust = new MySqlConnection(constrUpdCust);
            conCust.Open();
            MySqlCommand cmdCust = new MySqlCommand(getCust, conCust);
            MySqlDataAdapter adpCust = new MySqlDataAdapter(cmdCust);
            adpCust.Fill(dataTableCust);
            conCust.Close();
        }

        // Creates updated customer
        private void UpdateCust()
        {
            try
            {
                string updCountry = @"UPDATE country 
                                        SET country = @country 
                                        WHERE countryId = @countryId";
                string updCity = @"UPDATE city 
                                    SET city = @city 
                                    WHERE cityId = @cityId";
                string updAddress = @"UPDATE address 
                                        SET address = @address, 
                                            address2 = @address2, 
                                            postalCode = @postalCode, 
                                            phone = @phone  
                                        WHERE addressId = @addressId";
                string updCustName = @"UPDATE customer 
                                        SET customerName = @customerName, 
                                            active = @active 
                                        WHERE customerId = @customerId";

                MySqlConnection conUpdCust = new MySqlConnection(constrUpdCust);
                conUpdCust.Open();
                MySqlCommand cmdAddCountry = new MySqlCommand(updCountry, conUpdCust);
                cmdAddCountry.Parameters.AddWithValue("@countryId", UpdCountryIdTB.Text);
                cmdAddCountry.Parameters.AddWithValue("@country", UpdCountryTB.Text);
                cmdAddCountry.ExecuteNonQuery();

                MySqlCommand cmdUpdCity = new MySqlCommand(updCity, conUpdCust);
                cmdUpdCity.Parameters.AddWithValue("@city", UpdCityTB.Text);
                cmdUpdCity.Parameters.AddWithValue("@cityId", UpdCityIdTB.Text);
                cmdUpdCity.ExecuteNonQuery();

                MySqlCommand cmdAddAddress = new MySqlCommand(updAddress, conUpdCust);
                cmdAddAddress.Parameters.AddWithValue("@address", UpdAddressTB.Text);
                cmdAddAddress.Parameters.AddWithValue("@address2", UpdAddress2TB.Text);
                cmdAddAddress.Parameters.AddWithValue("@addressId", UpdAddressIdTB.Text);
                cmdAddAddress.Parameters.AddWithValue("@postalCode", UpdPostalTB.Text);
                cmdAddAddress.Parameters.AddWithValue("@phone", UpdPhoneTB.Text);
                cmdAddAddress.ExecuteNonQuery();

                MySqlCommand cmdAddCustName = new MySqlCommand(updCustName, conUpdCust);
                cmdAddCustName.Parameters.AddWithValue("@customerName", UpdCustNameTB.Text);
                cmdAddCustName.Parameters.AddWithValue("@CustomerId", UpdCustIdTB.Text);
                cmdAddCustName.Parameters.AddWithValue("@active", UpdActiveRB.Checked);
                cmdAddCustName.ExecuteNonQuery();
                conUpdCust.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error:" + "\n" + ex.Message);
                return;
            }
        }

        // Updates customer in database
        private void UpdateCustBTN_Click(object sender, EventArgs e)
        {
            DataTable dataTableUpdCust = new DataTable();
            try
            {
                if (string.IsNullOrWhiteSpace(UpdCustNameTB.Text))
                {
                    MessageBox.Show("Enter Customer's Name");
                    return;
                }
                if (string.IsNullOrWhiteSpace(UpdAddressTB.Text))
                {
                    MessageBox.Show("Enter Customer's Address");
                    return;
                }
                if (string.IsNullOrWhiteSpace(UpdCityTB.Text))
                {
                    MessageBox.Show("Enter Customer's City");
                    return;
                }
                if (string.IsNullOrWhiteSpace(UpdPostalTB.Text))
                {
                    MessageBox.Show("Enter Customer's Postal Code");
                    return;
                }
                if (string.IsNullOrWhiteSpace(UpdPhoneTB.Text))
                {
                    MessageBox.Show("Enter Customer's Phone Number");
                    return;
                }
                else
                {
                    UpdateCust();
                    MessageBox.Show("Customer Updated");
                    dataTableUpdCust.Clear();
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

        private void UpdCustCancelBTN_Click(object sender, EventArgs e)
        {
            DataTable dataTableUpdCancel = new DataTable();
            MessageBox.Show("Customer Not Updated");
            this.Close();
            dataTableUpdCancel.Clear();
            CustomersPage appPage = new CustomersPage();
            appPage.Show();
        }
    }
}
