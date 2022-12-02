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
    public partial class UpdateAppointment : Form
    {
        // Connection String
        string constrUpdApp = ConfigurationManager.ConnectionStrings["clientdb"].ConnectionString;

        public UpdateAppointment()
        {
            InitializeComponent();
            UpdateApp();
            GetAppointments(@"SELECT * 
                                FROM appointment");
            UpdStartDTP.Value.AddSeconds(-UpdStartDTP.Value.Second);
            UpdEndDTP.Value.AddSeconds(-UpdEndDTP.Value.Second);

        }

        public UpdateAppointment(
            int appointmentId, 
            int customerId, 
            int userId, 
            string type, 
            DateTime start, 
            DateTime end)
        {
            InitializeComponent();
            FillMeetingTypeCB();
            FillUpdUserIdCB();
            UpdAppIdTB.Text = appointmentId.ToString();
            UpdCustIdTB.Text = customerId.ToString();
            UpdUserIdCB.Text = userId.ToString();
            UpdTypeCB.Text = type;
            UpdStartDTP.Text = start.ToString();
            UpdEndDTP.Text = end.ToString();
        }

        private void GetAppointments(string getApp)
        {
            DataTable dataTableApp = new DataTable();
            MySqlConnection conApp = new MySqlConnection(constrUpdApp);
            MySqlCommand cmdApp = new MySqlCommand(getApp, conApp);
            conApp.Open();
            MySqlDataAdapter adpApp = new MySqlDataAdapter(cmdApp);
            adpApp.Fill(dataTableApp);
            conApp.Close();
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
            UpdTypeCB.DataSource = fillMeetingType;
        }

        private void FillUpdUserIdCB()
        {
            string fillUpdUserId = @"SELECT userId 
                                    FROM user";

            MySqlConnection conUserId = new MySqlConnection(constrUpdApp);
            conUserId.Open();
            MySqlCommand cmdUserId = new MySqlCommand(fillUpdUserId, conUserId);
            MySqlDataReader updCustReader;

            try
            {
                updCustReader = cmdUserId.ExecuteReader();

                while (updCustReader.Read())
                {
                    string updUserId = updCustReader.GetString("userId");
                    UpdUserIdCB.Items.Add(updUserId);
                }
                conUserId.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error" + "\n" + ex.Message);
                return;
            }
        }

        // Creates Updated Appointment
        private void UpdateApp()
        {
            string updateApp = @"UPDATE appointment 
                                    SET customerId = @customerId,
                                        userId = @userId, 
                                        type = @type, 
                                        start = @start, 
                                        end = @end, 
                                        lastUpdate = Now() 
                                    WHERE (appointmentId = @appointmentID)";

            DateTime startApp = TimeZoneInfo.ConvertTimeToUtc(UpdStartDTP.Value);
            DateTime endApp = TimeZoneInfo.ConvertTimeToUtc(UpdEndDTP.Value);
            MySqlConnection conUpdApp = new MySqlConnection(constrUpdApp);
            conUpdApp.Open();
            startApp.AddSeconds(-startApp.Second);
            endApp.AddSeconds(-endApp.Second);
            MySqlCommand cmdUpdApp = new MySqlCommand(updateApp, conUpdApp);
            cmdUpdApp.Parameters.AddWithValue("@appointmentId", UpdAppIdTB.Text);
            cmdUpdApp.Parameters.AddWithValue("@customerId", UpdCustIdTB.Text);
            cmdUpdApp.Parameters.AddWithValue("@userId", UpdUserIdCB.Text);
            cmdUpdApp.Parameters.AddWithValue("@type", UpdTypeCB.Text);
            cmdUpdApp.Parameters.AddWithValue("@start", startApp);
            cmdUpdApp.Parameters.AddWithValue("@end", endApp);

            try
            {
                cmdUpdApp.ExecuteNonQuery();
                conUpdApp.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error:" + "\n" + ex.Message);
                return;
            }
        }

        // Updates Appointment in database
        private void UpdateAppBTN_Click(object sender, EventArgs e)
        {
            string overlapStart = @"SELECT userId,
                                           appointmentId,
                                           start
                                        FROM appointment
                                        WHERE userId = @userIdStart
                                            AND start>= @startStart 
                                            And start < @endStart
                                            AND appointmentId != @appIdStart";

            string overlapEnd = @"SELECT userId,
                                         appointmentId,
                                         start
                                    FROM appointment
                                    WHERE userId = @userIdEnd 
                                        AND end > @startEnd
                                        AND end <= @endEnd
                                        AND appointmentId != @appIdEnd";
            string overlapStartEnd = @"SELECT userId,
                                             appointmentId,
                                             start,
                                             end
                                        FROM appointment
                                        WHERE userId = @userIdStartEnd
                                            AND start <= @startStartEnd
                                            AND end >= @endStartEnd
                                            AND appointmentId != @appIdStartEnd";

            DateTime startOverlap = TimeZoneInfo.ConvertTimeToUtc(UpdStartDTP.Value);
            DateTime endOverlap = TimeZoneInfo.ConvertTimeToUtc(UpdEndDTP.Value);
            startOverlap.AddSeconds(-startOverlap.Second);
            endOverlap.AddSeconds(-endOverlap.Second);
            MySqlConnection conUpdApp = new MySqlConnection(constrUpdApp);
            conUpdApp.Open();

            MySqlCommand cmdOverlapStart = new MySqlCommand(overlapStart, conUpdApp);
            cmdOverlapStart.Parameters.AddWithValue("@appIdstart", UpdAppIdTB.Text);
            cmdOverlapStart.Parameters.AddWithValue("@startStart", startOverlap);
            cmdOverlapStart.Parameters.AddWithValue("@endStart", endOverlap);
            cmdOverlapStart.Parameters.AddWithValue("@userIdStart", UpdUserIdCB.Text);
            DataTable dataTableStart = new DataTable();
            MySqlDataAdapter adpStart = new MySqlDataAdapter(cmdOverlapStart);
            adpStart.Fill(dataTableStart);

            MySqlCommand cmdOverladEnd = new MySqlCommand(overlapEnd, conUpdApp);
            cmdOverladEnd.Parameters.AddWithValue("@appIdEnd", UpdAppIdTB.Text);
            cmdOverladEnd.Parameters.AddWithValue("@startEnd", startOverlap);
            cmdOverladEnd.Parameters.AddWithValue("@endEnd", endOverlap);
            cmdOverladEnd.Parameters.AddWithValue("@userIdEnd", UpdUserIdCB.Text);
            DataTable dataTableEnd = new DataTable();
            MySqlDataAdapter adpEnd = new MySqlDataAdapter(cmdOverladEnd);
            adpEnd.Fill(dataTableEnd);

            MySqlCommand cmdStartEndOverlap = new MySqlCommand(overlapStartEnd, conUpdApp);
            cmdStartEndOverlap.Parameters.AddWithValue("@appIdStartEnd", UpdAppIdTB.Text);
            cmdStartEndOverlap.Parameters.AddWithValue("@startStartEnd", startOverlap);
            cmdStartEndOverlap.Parameters.AddWithValue("@endStartEnd", endOverlap);
            cmdStartEndOverlap.Parameters.AddWithValue("@userIdStartEnd", UpdUserIdCB.Text);
            DataTable dataTableStartEnd = new DataTable();
            MySqlDataAdapter adpStartEnd = new MySqlDataAdapter(cmdStartEndOverlap);
            adpStartEnd.Fill(dataTableStartEnd);

            try
            {
                DateTime currentApp = DateTime.Now;
                DateTime startApp = UpdStartDTP.Value;
                DateTime endApp = UpdEndDTP.Value;
                startOverlap.AddSeconds(-startOverlap.Second);
                endOverlap.AddSeconds(-endOverlap.Second);
                TimeSpan startTime = new DateTime(
                    currentApp.Year,
                    currentApp.Month,
                    currentApp.Day, 9, 0, 0).TimeOfDay;
                TimeSpan endTime = new DateTime(
                    currentApp.Year,
                    currentApp.Month,
                    currentApp.Day, 17, 30, 0).TimeOfDay;

                if (string.IsNullOrWhiteSpace(UpdTypeCB.Text))
                {
                    MessageBox.Show("Select Appointment Type");
                    return;
                }
                if (string.IsNullOrWhiteSpace(UpdUserIdCB.Text))
                {
                    MessageBox.Show("Select User Id");
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
                else
                {
                    UpdateApp();
                    MessageBox.Show("Appointment Updated");
                    this.Close();
                    AppointmentsPage appPage = new AppointmentsPage();
                    appPage.Show();
                    conUpdApp.Close();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error" + "\n" + ex.Message);
                return;
            }
        }

        // Cancel doesn't update Appointment
        private void UpdateAppCancelBTN_Click(object sender, EventArgs e)
        {
            DataTable dataTableUpdCancel = new DataTable();
            MessageBox.Show("Appointment Not Updated");
            this.Close();
            dataTableUpdCancel.Clear();
            AppointmentsPage appPage = new AppointmentsPage();
            appPage.Show();
        }
    }
}
