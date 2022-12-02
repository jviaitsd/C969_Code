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
    public partial class AppointmentsPage : Form
    {
        // Connection String
        string constrApp = ConfigurationManager.ConnectionStrings["clientdb"].ConnectionString;
        DateTime currentDate = new DateTime(); 

        public AppointmentsPage()
        {
            InitializeComponent();
            currentDate = DateTime.Now;
            AppCalendarMC.AddBoldedDate(currentDate);
            GetAppointments(@"SELECT appointmentId, 
                                     customerId, 
                                     userId, 
                                     type, 
                                     start, 
                                     end 
                                FROM appointment");
            FillCustomerIdCB();
        }

        // Initially Populate DGV
        private void GetAppointments(string getCust)
        {
            AppCalendarMC.RemoveAllBoldedDates();
            MySqlConnection conApp = new MySqlConnection(constrApp);
            conApp.Open();
            MySqlCommand cmdApp = new MySqlCommand(getCust, conApp);
            MySqlDataAdapter adpApp = new MySqlDataAdapter(cmdApp);
            DataTable dataTableApp = new DataTable();
            adpApp.Fill(dataTableApp);

            for (int i = 0; i < dataTableApp.Rows.Count; i++)
            {
                DateTime start = (DateTime)dataTableApp.Rows[i]["start"];
                DateTime end = (DateTime)dataTableApp.Rows[i]["end"];
                start.AddSeconds(-start.Second);
                end.AddSeconds(-end.Second);
                dataTableApp.Rows[i]["start"] = start.ToLocalTime();
                dataTableApp.Rows[i]["end"] = end.ToLocalTime();
            }

            AppCalendarMC.AddBoldedDate(currentDate);
            AppCalendarMC.UpdateBoldedDates();
            AppointmentsDGV.DataSource = dataTableApp;
            conApp.Close();
        }

        private void GetWeek()
        {
            AppCalendarMC.RemoveAllBoldedDates();
            DataTable dataTableWeek = new DataTable();
            string getWeek = @"SELECT appointmentId, 
                                      customerId, 
                                      userId, 
                                      type, 
                                      start, 
                                      end
                                    FROM appointment 
                                    WHERE start 
                                    BETWEEN @startWeek 
                                        AND @endWeek";

            int currentWeek = (int)currentDate.DayOfWeek;
            DateTime startWeek = currentDate.AddDays(- currentWeek);
            DateTime tempDateWeek = Convert.ToDateTime(startWeek);
            DateTime endWeek = currentDate.AddDays(7 - currentWeek);

            for (int i = 0; i < 7; i++)
            {
                AppCalendarMC.AddBoldedDate(tempDateWeek.AddDays(i));
            }

            MySqlConnection conWeek = new MySqlConnection(constrApp);
            conWeek.Open();
            MySqlCommand cmdWeek = new MySqlCommand(getWeek, conWeek);
            cmdWeek.Parameters.AddWithValue("@startWeek", startWeek);
            cmdWeek.Parameters.AddWithValue("@endWeek", endWeek);
            MySqlDataAdapter adpWeek = new MySqlDataAdapter(cmdWeek);
            adpWeek.Fill(dataTableWeek);

            for (int i = 0; i < dataTableWeek.Rows.Count; i++)
            {
                DateTime start = (DateTime)dataTableWeek.Rows[i]["start"];
                DateTime end = (DateTime)dataTableWeek.Rows[i]["end"];
                start.AddSeconds(-start.Second);
                end.AddSeconds(-end.Second);
                dataTableWeek.Rows[i]["start"] = start.ToLocalTime();
                dataTableWeek.Rows[i]["end"] = end.ToLocalTime();
            }

            AppCalendarMC.UpdateBoldedDates();
            AppointmentsDGV.DataSource = dataTableWeek;
            conWeek.Close();
        }

        private void GetMonth()
        {
            AppCalendarMC.RemoveAllBoldedDates();
            DataTable dataTableMonth = new DataTable();
            string getMonth = @"SELECT appointmentId, 
                                       customerId, 
                                       userId, 
                                       type, 
                                       start, 
                                       end
                                    FROM appointment 
                                    WHERE start 
                                        BETWEEN @startMonth AND @endMonth";

            int mth = currentDate.Month;
            int yr = currentDate.Year;
            int dys;
            DateTime startMonth = new DateTime(yr, mth, 1);
            DateTime tempDate = Convert.ToDateTime(startMonth);

            switch (mth)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                    dys = 31;
                    break;

                case 4:
                case 6:
                case 9:
                case 11:
                    dys = 30;
                    break;

                default:
                    dys = 29;
                    break;
            }

            for (int i = 0; i < dys; i++)
            {
                AppCalendarMC.AddBoldedDate(tempDate.AddDays(i));
            }

            DateTime currentMonth = DateTime.Today;
            DateTime endMonth = new DateTime(currentMonth.Year, currentMonth.Month, 1).AddMonths(1).AddDays(-1);
            MySqlConnection conMonth = new MySqlConnection(constrApp);
            conMonth.Open();
            MySqlCommand cmdMonth = new MySqlCommand(getMonth, conMonth);
            cmdMonth.Parameters.AddWithValue("@startMonth", startMonth);
            cmdMonth.Parameters.AddWithValue("@endMonth", endMonth);
            MySqlDataAdapter adpMonth = new MySqlDataAdapter(cmdMonth);
            adpMonth.Fill(dataTableMonth);

            for (int i = 0; i < dataTableMonth.Rows.Count; i++)
            {
                DateTime start = (DateTime)dataTableMonth.Rows[i]["start"];
                DateTime end = (DateTime)dataTableMonth.Rows[i]["end"];
                start.AddSeconds(-start.Second);
                end.AddSeconds(-end.Second);
                dataTableMonth.Rows[i]["start"] = start.ToLocalTime();
                dataTableMonth.Rows[i]["end"] = end.ToLocalTime();
            }

            AppCalendarMC.UpdateBoldedDates();
            AppointmentsDGV.DataSource = dataTableMonth;
            conMonth.Close();
        }

        // Appointment for selected Customer Id
        private void GetCustId()
        {
            AppCalendarMC.RemoveAllBoldedDates();
            DataTable dataTableCust = new DataTable();

            string getCust = @"SELECT appointmentId, 
                                      customerId, 
                                      userId, 
                                      type, 
                                      start, 
                                      end
                                    FROM appointment 
                                    WHERE customerId = @customerId";

            string custGet = AppCustIdCB.Text;
            MySqlConnection conCust = new MySqlConnection(constrApp);
            conCust.Open();
            MySqlCommand cmdCust = new MySqlCommand(getCust, conCust);
            cmdCust.Parameters.AddWithValue("@customerId", custGet);
            MySqlDataAdapter adpCust = new MySqlDataAdapter(cmdCust);
            adpCust.Fill(dataTableCust);

            for (int i = 0; i < dataTableCust.Rows.Count; i++)
            {
                DateTime start = (DateTime)dataTableCust.Rows[i]["start"];
                DateTime end = (DateTime)dataTableCust.Rows[i]["end"];
                start.AddSeconds(-start.Second);
                end.AddSeconds(-end.Second);
                dataTableCust.Rows[i]["start"] = start.ToLocalTime();
                dataTableCust.Rows[i]["end"] = end.ToLocalTime();
            }

            AppointmentsDGV.DataSource = dataTableCust;
            conCust.Close();
        }

        private void FillCustomerIdCB()
        {
            string fillCustId = @"SELECT customerId 
                                    FROM customer";

            MySqlConnection conCustId = new MySqlConnection(constrApp);
            conCustId.Open();
            MySqlCommand cmdCustId = new MySqlCommand(fillCustId, conCustId);
            MySqlDataReader rdrCustId;

            try
            {
                rdrCustId = cmdCustId.ExecuteReader();
                while (rdrCustId.Read())
                {
                    string custId = rdrCustId.GetString("customerId");
                    AppCustIdCB.Items.Add(custId);
                }
                conCustId.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error" + "\n" + ex.Message);
                return;
            }
        }

        private void AddAppBTN_Click(object sender, EventArgs e)
        {
            AddAppointment addApp = new AddAppointment();
            addApp.Show();
            this.Close();
        }

        // Sends highlighted row to Update Appointments Page and prefills text and combo boxes
        private void UpdateAppBTN_Click(object sender, EventArgs e)
        {
            // Checks line is Selected
            if (AppointmentsDGV.CurrentRow == null || !AppointmentsDGV.CurrentRow.Selected)
            {
                MessageBox.Show("Select an Appontment to Update");
                return;
            }

            DataTable dataTableUpdate = (DataTable)AppointmentsDGV.DataSource;
            int index = AppointmentsDGV.CurrentRow.Index;
            int appointmetId = (int)dataTableUpdate.Rows[index]["appointmentId"];
            int customerId = (int)dataTableUpdate.Rows[index]["customerId"];
            int userId = (int)dataTableUpdate.Rows[index]["userId"];
            string type = (string)dataTableUpdate.Rows[index]["type"];

            DateTime start = (DateTime)dataTableUpdate.Rows[index]["start"];
            DateTime end = (DateTime)dataTableUpdate.Rows[index]["end"];
            UpdateAppointment updApp = new UpdateAppointment(
                appointmetId, 
                customerId, 
                userId, 
                type, 
                start, 
                end);
            updApp.Show();
            this.Close();
        }

        private void CurWeekRB_CheckedChanged(object sender, EventArgs e)
        {
            GetWeek();
            AppointmentsDGV.ClearSelection();
        }

        private void CurMonthRB_CheckedChanged(object sender, EventArgs e)
        {
            GetMonth();
            AppointmentsDGV.ClearSelection();
        }

        private void AppSearchCustIdBTN_Click(object sender, EventArgs e)
        {
            GetCustId();
            AppointmentsDGV.ClearSelection();
        }

        private void AllAppRB_CheckedChanged(object sender, EventArgs e)
        {
            AppCalendarMC.RemoveAllBoldedDates();
            GetAppointments(@"SELECT appointmentId, 
                                     customerId, 
                                     userId, 
                                     type, 
                                     start, 
                                     end 
                                FROM appointment");
            AppointmentsDGV.ClearSelection();
        }

        private void BackBTN_Click(object sender, EventArgs e)
        {
            MainPage mainPage = new MainPage();
            mainPage.Show();
            this.Hide();
        }

        // Deleteds highlighted row in the database
        private void DeleteAppBTN_Click(object sender, EventArgs e)
        {
            // Checks if line is Selected
            if (AppointmentsDGV.CurrentRow == null || !AppointmentsDGV.CurrentRow.Selected)
            {
                MessageBox.Show("Select Appointment to Delete");
                return;
            }
            try
            {
                DataTable dataTableDeleteApp = (DataTable)AppointmentsDGV.DataSource;
                string deleteApp = @"DELETE FROM appointment
                                        WHERE appointmentId = @appointmentId";

                MySqlConnection conDeleteApp = new MySqlConnection(constrApp);
                conDeleteApp.Open();
                MySqlCommand cmdDeleteAppointment = new MySqlCommand(deleteApp, conDeleteApp);
                int indexAppointment = AppointmentsDGV.CurrentRow.Index;
                int appointmentId = (int)dataTableDeleteApp.Rows[indexAppointment]["appointmentId"];
                cmdDeleteAppointment.Parameters.AddWithValue("@appointmentId", appointmentId);
                cmdDeleteAppointment.ExecuteNonQuery();
                conDeleteApp.Close();
                GetAppointments(@"SELECT appointmentId, 
                                         customerId, 
                                         userId, 
                                         type, 
                                         start, 
                                         end 
                                    FROM appointment");

                MessageBox.Show("Appointment Deleted");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error:" + "\n" + ex.Message);
                return;
            }

            AppointmentsPage appPage = new AppointmentsPage();
            appPage.Show();
            this.Close();
        }

        private void ExitBTN_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
