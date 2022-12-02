using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C969_PerformanceAssessment
{
    public partial class AddAppointment : Form
    {
        // Connection String
        string constrAddApp = ConfigurationManager.ConnectionStrings["clientdb"].ConnectionString;

        public AddAppointment()
        {
            InitializeComponent();
            FillCustomerIdCB();
            FillUserIdCB();
            FillMeetingTypeCB();
            GetApp(@"SELECT appointmentId, 
                            customerId, 
                            userId, 
                            type, 
                            start, 
                            end 
                        FROM appointment");
            AddStartDTP.Value.AddSeconds(- AddStartDTP.Value.Second);
            AddEndDTP.Value.AddSeconds(-AddEndDTP.Value.Second);
        }

        private void GetApp(string getApp)
        {
            DataTable dataTableApp = new DataTable();
            MySqlConnection conApp = new MySqlConnection(constrAddApp);
            conApp.Open();
            MySqlCommand cmdApp = new MySqlCommand(getApp, conApp);
            MySqlDataAdapter adpApp = new MySqlDataAdapter(cmdApp);
            adpApp.Fill(dataTableApp);
            conApp.Close();
        }

        private void FillCustomerIdCB()
        {
            string fillCustId = @"SELECT customerId 
                                    FROM customer";

            MySqlConnection conCustId = new MySqlConnection(constrAddApp);
            conCustId.Open();
            MySqlCommand cmdCustId = new MySqlCommand(fillCustId, conCustId);
            MySqlDataReader rdrCustId;

            try
            {
                rdrCustId = cmdCustId.ExecuteReader();
                while (rdrCustId.Read())
                {
                    string custId = rdrCustId.GetString("customerId");
                    AddCustIdCB.Items.Add(custId);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error" + "\n" + ex.Message);
            }
        }

        private void FillMeetingTypeCB()
        {
            // Lambda Expression to sort string
            // Meeting Type Alphabetical - Sorted
            List<string> fillMeetingType = new List<string>
            {
                "Call",
                "Consult",
                "In-Person",
                "Zoom",
                "Scrum",
                "Training"
            };

            fillMeetingType = fillMeetingType.OrderBy(type => type).ToList();
            AddTypeCB.DataSource = fillMeetingType;
            AddTypeCB.SelectedItem = null;
        }

        private void FillUserIdCB()
        {
            string fillUserId = @"SELECT * 
                                    FROM user";

            MySqlConnection conUserId = new MySqlConnection(constrAddApp);
            conUserId.Open();
            MySqlCommand cmdUserId = new MySqlCommand(fillUserId, conUserId);
            MySqlDataReader userIdReader;

            try
            {
                userIdReader = cmdUserId.ExecuteReader();

                while (userIdReader.Read())
                {
                    string userId = userIdReader.GetString("userId");
                    AddUserIdCB.Items.Add(userId);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error" + "\n" + ex.Message);
            }
        }

        // Creates new Appointment
        private void InsertAppointment() 
        {
            string insertApp = @"INSERT INTO appointment 
                                VALUES (Null, 
                                        @customerId, 
                                        @userId, 
                                        'not needed', 
                                        'not needed', 
                                        'not needed', 
                                        'not needed', 
                                        @type,
                                        'not needed', 
                                        @start, 
                                        @end, 
                                        Now(), 
                                        'test', 
                                        Now(), 
                                        'test')";

            MySqlConnection conInsertApp = new MySqlConnection(constrAddApp);
            conInsertApp.Open();
            DateTime startApp = TimeZoneInfo.ConvertTimeToUtc(AddStartDTP.Value);
            DateTime endApp = TimeZoneInfo.ConvertTimeToUtc(AddEndDTP.Value);
            startApp.AddSeconds(-startApp.Second);
            endApp.AddSeconds(-endApp.Second);
            MySqlCommand cmdInsertApp = new MySqlCommand(insertApp, conInsertApp);
            cmdInsertApp.Parameters.AddWithValue("@customerId", AddCustIdCB.Text);
            cmdInsertApp.Parameters.AddWithValue("@userId", AddUserIdCB.Text);
            cmdInsertApp.Parameters.AddWithValue("@type", AddTypeCB.Text);
            cmdInsertApp.Parameters.AddWithValue("@start", startApp);
            cmdInsertApp.Parameters.AddWithValue("@end", endApp);

            try
            {
                cmdInsertApp.ExecuteNonQuery();
                conInsertApp.Close();
            }
            catch(MySqlException ex)
            {
                MessageBox.Show("Error:" + "\n" + ex.Message);
                return;
            }
        }

        // Inserts New Appointment into database
        private void AddAppAddBTN_Click(object sender, EventArgs e)
        {
            string overlapStart = @"SELECT userId,
                                           start
                                        FROM appointment
                                        WHERE userId = @userIdStart 
                                            AND start >= @startStart 
                                            And start < @endStart";
            string overlapEnd = @"SELECT userId,
                                         end
                                    FROM appointment
                                    WHERE userId = @userIdEnd 
                                        AND end > @startEnd
                                        AND end <= @endEnd";
            string overlapStartEnd = @"SELECT userId,
                                             start,
                                             end
                                        FROM appointment
                                        WHERE userId = @userIdStartEnd
                                            AND start <= @startStartEnd
                                            AND end >= @endStartEnd";

            DateTime startOverlap = TimeZoneInfo.ConvertTimeToUtc(AddStartDTP.Value);
            DateTime endOverlap = TimeZoneInfo.ConvertTimeToUtc(AddEndDTP.Value);
            startOverlap.AddSeconds(-startOverlap.Second);
            endOverlap.AddSeconds(-endOverlap.Second);
            MySqlConnection conAddApp = new MySqlConnection(constrAddApp);
            conAddApp.Open();

            MySqlCommand cmdStartOverlap = new MySqlCommand(overlapStart, conAddApp);
            cmdStartOverlap.Parameters.AddWithValue("@startStart", startOverlap);
            cmdStartOverlap.Parameters.AddWithValue("@endStart", endOverlap);
            cmdStartOverlap.Parameters.AddWithValue("@userIdStart", AddUserIdCB.Text);
            DataTable dataTableStart = new DataTable();
            MySqlDataAdapter adpStart = new MySqlDataAdapter(cmdStartOverlap);
            adpStart.Fill(dataTableStart);

            MySqlCommand cmdEndOverlap = new MySqlCommand(overlapEnd, conAddApp);
            cmdEndOverlap.Parameters.AddWithValue("@startEnd", startOverlap);
            cmdEndOverlap.Parameters.AddWithValue("@endEnd", endOverlap);
            cmdEndOverlap.Parameters.AddWithValue("@userIdEnd", AddUserIdCB.Text);
            DataTable dataTableEnd = new DataTable();
            MySqlDataAdapter adpEnd = new MySqlDataAdapter(cmdEndOverlap);
            adpEnd.Fill(dataTableEnd);

            MySqlCommand cmdStartEndOverlap = new MySqlCommand(overlapStartEnd, conAddApp);
            cmdStartEndOverlap.Parameters.AddWithValue("@startStartEnd", startOverlap);
            cmdStartEndOverlap.Parameters.AddWithValue("@endStartEnd", endOverlap);
            cmdStartEndOverlap.Parameters.AddWithValue("@userIdStartEnd", AddUserIdCB.Text);
            DataTable dataTableStartEnd = new DataTable();
            MySqlDataAdapter adpStartEnd = new MySqlDataAdapter(cmdStartEndOverlap);
            adpStartEnd.Fill(dataTableStartEnd);

            try
            {
                DateTime currentApp = DateTime.Now;
                DateTime startApp = AddStartDTP.Value;
                DateTime endApp = AddEndDTP.Value;
                startApp.AddSeconds(-startApp.Second);
                endApp.AddSeconds(-endApp.Second);
                TimeSpan startTime = new DateTime(
                    currentApp.Year,
                    currentApp.Month,
                    currentApp.Day, 9, 0, 0).TimeOfDay;
                TimeSpan endTime = new DateTime(
                    currentApp.Year,
                    currentApp.Month,
                    currentApp.Day, 17, 30, 0).TimeOfDay;

                if (string.IsNullOrWhiteSpace(AddCustIdCB.Text))
                {
                    MessageBox.Show("Select Customer ID");
                    return;
                }
                if (string.IsNullOrWhiteSpace(AddTypeCB.Text))
                {
                    MessageBox.Show("Select Appointment Type");
                    return;
                }
                if ((startApp.TimeOfDay < startTime) ||
                    (startApp.TimeOfDay > endTime) ||
                    (endApp.TimeOfDay < startTime) ||
                    (endApp.TimeOfDay > endTime))
                {
                    MessageBox.Show("Appointment is outside Business Hours, 9 am - 5:30 pm");
                    return;
                }
                if (startApp >= endApp)
                {
                    MessageBox.Show("Check Start and End times");
                    return;
                }
                if (dataTableStart.Rows.Count > 0)
                {
                    MessageBox.Show("Overlapping Appointment");
                    return;

                }
                if (dataTableEnd.Rows.Count > 0)
                {
                    MessageBox.Show("Overlapping Appointment");
                    return;

                }
                if (dataTableStartEnd.Rows.Count > 0)
                {
                    MessageBox.Show("Overlapping Appointment");
                    return;
                }
                if (string.IsNullOrWhiteSpace(AddUserIdCB.Text))
                {
                    MessageBox.Show("Need User Id");
                    return;
                }
                else
                {
                    InsertAppointment();
                    MessageBox.Show("New Appointment Added"); 
                    this.Close();
                    AppointmentsPage appPage = new AppointmentsPage();
                    appPage.Show();
                    conAddApp.Close();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error" + "\n" + ex.Message);
                return;
            }
        }

        // Cancel does not save new Appointment
        private void AddAppCancelBTN_Click(object sender, EventArgs e)
        {
            DataTable dataTableCancel = new DataTable();
            MessageBox.Show("Appointment Not Added");
            this.Close();
            dataTableCancel.Clear();
            AppointmentsPage appPage = new AppointmentsPage();
            appPage.Show();
        }
   }
}
