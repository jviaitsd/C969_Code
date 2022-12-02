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
    public partial class MainPage : Form
    {
        public MainPage()
        {
            InitializeComponent();
        }

        // Takes user to Customers Page
        private void ToCustomersBTN_Click_1(object sender, EventArgs e)
        {
           CustomersPage custPage = new CustomersPage(); 
           this.Hide();
           custPage.Show();
        }

        // Takes user to Appointments Page
        private void ToAppointmentsBTN_Click_1(object sender, EventArgs e)
        {
            AppointmentsPage appPage = new AppointmentsPage();
            this.Hide();
            appPage.Show();
        }

        // Takes user to Reports Page
        private void ToReportsBTN_Click_1(object sender, EventArgs e)
        {
            ReportsPage reportsPage = new ReportsPage();
            this.Hide();
            reportsPage.Show();
        }

        private void ExitBTN_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BackBTN_Click(object sender, EventArgs e)
        {
            this.Close();
            LoginPage loginPage = new LoginPage();
            loginPage.Show();
        }
    }
}
