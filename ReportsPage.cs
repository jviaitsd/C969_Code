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
    public partial class ReportsPage : Form
    {
        // Connection String
        string constrReport = ConfigurationManager.ConnectionStrings["clientdb"].ConnectionString;
        DateTime currentDate = new DateTime();

        public ReportsPage()
        {
            InitializeComponent();
            currentDate = DateTime.Now;
            FillUserCB();
            FillAppTypeCB();
            FillMonthsTypeCB();
            FillCountryCB();
        }

        private void FillUserCB()
        {
            string fillUser = @"SELECT userId 
                                    FROM user";

            MySqlConnection conUser = new MySqlConnection(constrReport);
            conUser.Open();
            MySqlCommand cmdUser = new MySqlCommand(fillUser, conUser);
            MySqlDataReader rdrUser;

            try
            {
                rdrUser = cmdUser.ExecuteReader();
                while (rdrUser.Read())
                {
                    string user = rdrUser.GetString("userId");
                    UserCB.Items.Add(user);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error" + "\n" + ex.Message);
                return;
            }
        }

        // Creates User/Appointments Report
        private void AppUserReportBTN_Click(object sender, EventArgs e)
        {
            DataTable dataTableUser = new DataTable();

            string getUser = @"SELECT appointmentId, 
                                      customerId,
                                      userId, 
                                      type, 
                                      start, 
                                      end
                                    FROM appointment 
                                    WHERE userId = @userId";

            MySqlConnection conUser = new MySqlConnection(constrReport);
            conUser.Open();
            MySqlCommand cmdUser = new MySqlCommand(getUser, conUser);
            cmdUser.Parameters.AddWithValue("@userId", UserCB.Text);


            if (string.IsNullOrWhiteSpace(UserCB.Text))
            {
                MessageBox.Show("Select User Id");
                return;
            }
            else
            {
                MySqlDataAdapter adpUser = new MySqlDataAdapter(cmdUser);
                adpUser.Fill(dataTableUser);

                for (int i = 0; i < dataTableUser.Rows.Count; i++)
                {
                    DateTime start = (DateTime)dataTableUser.Rows[i]["start"];
                    DateTime end = (DateTime)dataTableUser.Rows[i]["end"];
                    start.AddSeconds(-start.Second);
                    end.AddSeconds(-end.Second);
                    dataTableUser.Rows[i]["start"] = start.ToLocalTime();
                    dataTableUser.Rows[i]["end"] = end.ToLocalTime();
                }

                ReportsUserDGV.DataSource = dataTableUser;
                conUser.Close();
                ReportsUserDGV.ClearSelection();
            }
        }

        private void FillAppTypeCB()
        {
            string fillType = @"SELECT DISTINCT type
                                    FROM appointment";

            MySqlConnection conType = new MySqlConnection(constrReport);
            conType.Open();
            MySqlCommand cmdType = new MySqlCommand(fillType, conType);
            MySqlDataReader rdrType;

            try
            {
                rdrType = cmdType.ExecuteReader();
                while (rdrType.Read())
                {
                    string appType = rdrType.GetString("type");
                    AppTypeCB.Items.Add(appType);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error" + "\n" + ex.Message);
            }
        }

        private void FillMonthsTypeCB()
        {
            // Lambda Expresion to sort string
            // Months in order
            List<string> fillMonthsType = new List<string>
            {
                "January",
                "Febuary",
                "March",
                "April",
                "May",
                "June",
                "July",
                "August",
                "September",
                "October",
                "November",
                "December"
            };
            MonthTypeCB.DataSource = fillMonthsType;
            MonthTypeCB.SelectedItem = null;
        }

        // Creates Report Appointment Type
        private void AppTypeReportBTN_Click(object sender, EventArgs e)
        {
            DataTable dataTableAppType = new DataTable();
            string getAppType = @"SELECT COUNT(type)
                                    FROM appointment
                                    WHERE type = @type
                                        AND MONTHNAME(start) = @month";

            MySqlConnection conAppType = new MySqlConnection(constrReport);
            conAppType.Open();
            MySqlCommand cmdAppType = new MySqlCommand(getAppType, conAppType);
            cmdAppType.Parameters.AddWithValue("@type", AppTypeCB.Text);
            cmdAppType.Parameters.AddWithValue("@month", MonthTypeCB.Text);

            if (string.IsNullOrWhiteSpace(AppTypeCB.Text))
            {
                MessageBox.Show("Select Appointment Type");
                return;
            }
            if (string.IsNullOrWhiteSpace(MonthTypeCB.Text))
            {
                MessageBox.Show("Select Month");
                return;
            }
            else
            {
                MySqlDataAdapter adpAppType = new MySqlDataAdapter(cmdAppType);
                adpAppType.Fill(dataTableAppType);
                TypeReportDGV.DataSource = dataTableAppType;
                conAppType.Close();
                TypeReportDGV.ClearSelection();
            }
        }

        private void FillCountryCB()
        {
            string fillCountry = @"SELECT DISTINCT country
                                    FROM country";

            MySqlConnection conCountry = new MySqlConnection(constrReport);
            conCountry.Open();
            MySqlCommand cmdCountry = new MySqlCommand(fillCountry, conCountry);
            MySqlDataReader rdrCountry;

            try
            {
                rdrCountry = cmdCountry.ExecuteReader();

                while (rdrCountry.Read())
                {
                    string countryfill = rdrCountry.GetString("country").Trim();
                    CountryCB.Items.Add(countryfill);
                }
                ReportsCountryDGV.ClearSelection();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error" + "\n" + ex.Message);
                return;
            }
        }

        // Creates Report Country
        private void CountryBTN_Click(object sender, EventArgs e)
        {
            DataTable dataTableCustId = new DataTable();
            string getCustId = @"SELECT COUNT(country)
                                        FROM country
                                        WHERE country = @country";
            MySqlConnection conCustId = new MySqlConnection(constrReport);
            conCustId.Open();
            MySqlCommand cmdCustId = new MySqlCommand(getCustId, conCustId);
            cmdCustId.Parameters.AddWithValue("@country", CountryCB.Text);

            if (string.IsNullOrWhiteSpace(CountryCB.Text))
            {
                MessageBox.Show("Select Country");
                return;
            }
            else
            {
                MySqlDataAdapter adpCustId = new MySqlDataAdapter(cmdCustId);
                adpCustId.Fill(dataTableCustId);
                ReportsCountryDGV.DataSource = dataTableCustId;
                conCustId.Close();
                ReportsCountryDGV.ClearSelection();
            }
        }

        private void BackBTN_Click(object sender, EventArgs e)
        {
            MainPage MP = new MainPage();
            this.Hide();
            MP.Show();
        }

        private void ExitBTN_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
