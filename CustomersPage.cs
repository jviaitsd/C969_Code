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
    public partial class CustomersPage : Form
    {
        // Connection String
        string constrCust = ConfigurationManager.ConnectionStrings["clientdb"].ConnectionString;
        public CustomersPage()
        {
            InitializeComponent();
            // Loads the Customers Update Page
            string loadUpdCust = @"SELECT DISTINCT 
                                        customer.customerId, 
                                        customer.customerName, 
                                        customer.active,
                                        address.addressId, 
                                        address.address, 
                                        address.address2, 
                                        city.cityId, 
                                        city.city, 
                                        address.postalCode, 
                                        address.phone,
                                        country.countryId, 
                                        country.country  
                                    FROM customer
                                        INNER JOIN address 
                                            ON customer.addressId = address.addressId
                                        INNER JOIN city 
                                            ON address.cityId = city.cityId
                                        INNER JOIN country 
                                            ON city.countryId = country.countryId";
            GetCustomers(loadUpdCust);
        }

        // Initially Populate DGV
        private void GetCustomers(string getCust)
        {
            DataTable dataTableCust = new DataTable();
            MySqlConnection conCust = new MySqlConnection(constrCust);
            conCust.Open();
            MySqlCommand cmdCust = new MySqlCommand(getCust, conCust);
            MySqlDataAdapter adpCust = new MySqlDataAdapter(cmdCust);
            adpCust.Fill(dataTableCust);
            conCust.Close();
            CustomersDGV.DataSource = dataTableCust;
        }

        private void CustAddBTN_Click(object sender, EventArgs e)
        {
            AddCustomer addCust = new AddCustomer();
            addCust.Show();
            this.Close();
        }

        private void CustUpdateBTN_Click(object sender, EventArgs e)
        {
            // Checks line is Selected
            if (CustomersDGV.CurrentRow == null || !CustomersDGV.CurrentRow.Selected)
            {
                MessageBox.Show("Select Customer to Update");
                return;
            }
            DataTable dataTableUpdate = (DataTable)CustomersDGV.DataSource;
            int index = CustomersDGV.CurrentRow.Index;
            int customerId = (int)dataTableUpdate.Rows[index]["customerId"];
            string customerName = (string)dataTableUpdate.Rows[index]["customerName"];
            string address = (string)dataTableUpdate.Rows[index]["address"];
            string address2 = (string)dataTableUpdate.Rows[index]["address2"];
            string city = (string)dataTableUpdate.Rows[index]["city"];
            string postalCode = (string)dataTableUpdate.Rows[index]["postalCode"];
            string country = (string)dataTableUpdate.Rows[index]["country"];
            string phone = (string)dataTableUpdate.Rows[index]["phone"];
            int countryId = (int)dataTableUpdate.Rows[index]["countryId"];
            int cityId = (int)dataTableUpdate.Rows[index]["cityId"];
            int addressId = (int)dataTableUpdate.Rows[index]["addressId"];
            UpdateCustomer updCust = new UpdateCustomer(
                customerId, 
                customerName, 
                address, 
                address2, 
                city, 
                postalCode, 
                country, 
                phone, 
                countryId, 
                cityId, 
                addressId);
            updCust.Show();
            CustomersDGV.ClearSelection();
            this.Close();
        }

        // Deletes customer from database
        private void CustDeleteBTN_Click(object sender, EventArgs e)
        {
            // Checks if line is Selected
            if (CustomersDGV.CurrentRow == null || !CustomersDGV.CurrentRow.Selected)
            {
                MessageBox.Show("Select Customer to Delete");
                return;
            }
            try
            {
                DataTable dataTableDeleteCust = (DataTable)CustomersDGV.DataSource;
                string deleteCust = @"DELETE FROM customer 
                                        WHERE customerId = @customerId";
                MySqlConnection conDeleteCust = new MySqlConnection(constrCust);
                conDeleteCust.Open();
                MySqlCommand cmdDeleteCustomer = new MySqlCommand(deleteCust, conDeleteCust);
                int indexCustomer = CustomersDGV.CurrentRow.Index;
                int deleteCustomer = (int)dataTableDeleteCust.Rows[indexCustomer]["customerId"];
                cmdDeleteCustomer.Parameters.AddWithValue("@customerId", deleteCustomer);
                cmdDeleteCustomer.ExecuteNonQuery();
                MessageBox.Show("Customer Deleted");
                conDeleteCust.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error:" + "\n" + ex.Message);
                return;
            }
            CustomersPage custPage = new CustomersPage();
            custPage.Show();
            this.Close();
        }

        private void BackBTN_Click(object sender, EventArgs e)
        {
            MainPage mainPage = new MainPage();
            this.Hide();
            mainPage.Show();
        }

        private void CustExitBTN_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CustomersDGV_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            CustomersDGV.Columns["customerName"].Visible = false;
        }
    }
}
