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
using System.Globalization;
using System.IO;




namespace C969_PerformanceAssessment
{
    public partial class LoginPage : Form
    {
        // Connection String
        string constrLogin = ConfigurationManager.ConnectionStrings["clientdb"].ConnectionString;
        public int counter = 1;

        public LoginPage()
        {
            InitializeComponent();
            // Uncomment for testing only Make sure it is COMMENTED OUT when done testing
            // CultureInfo.CurrentCulture = new CultureInfo("es-ES");
            if (CultureInfo.CurrentCulture.Name == "es-ES")
            {
                ChangeToSpanish();
            }
        }

        // 15 min Alert when user is logging in
        private void AppointmentAlert()
        {
            string appAlert = @"SELECT userId
                                    FROM user
                                    WHERE userName = @userName";

            MySqlConnection conAppAlert = new MySqlConnection(constrLogin);
            conAppAlert.Open();
            MySqlCommand cmdAppAlert = new MySqlCommand(appAlert, conAppAlert);
            cmdAppAlert.Parameters.AddWithValue("@userName", LoginUserNameTB.Text);
            Object obj = cmdAppAlert.ExecuteScalar();
            int userId = Convert.ToInt32(obj.ToString());
            DateTime alertStart = DateTime.Now.ToUniversalTime();
            DateTime alertEnd = DateTime.Now.AddMinutes(15).ToUniversalTime();
            string alertCurUser = @"SELECT *
                                        FROM appointment
                                        WHERE userid =  @userId
                                            AND start 
                                            BETWEEN @alertStart and @alertEnd";

            DataTable dataTableAlert = new DataTable();
            MySqlCommand cmdAlert = new MySqlCommand(alertCurUser, conAppAlert);
            cmdAlert.Parameters.AddWithValue("userId", userId);
            cmdAlert.Parameters.AddWithValue("@alertStart", alertStart);
            cmdAlert.Parameters.AddWithValue("@alertEnd", alertEnd);
            MySqlDataAdapter adpAlert = new MySqlDataAdapter(cmdAlert);
            adpAlert.Fill(dataTableAlert);

            if (dataTableAlert.Rows.Count > 0)
            {
                MessageBox.Show("You have an Appointment with in 15 minutes");
                return;
            }
            conAppAlert.Close();
        }

        // Changes all labels and buttons. 
        private void ChangeToSpanish()
        {
            LoginLB.Text = "Iniciar sesión";
            UserNameLB.Text = "Nombre de usuario";
            PasswordLB.Text = "Contraseña";
            LoginBTN.Text = "Iniciar sesión";
            LoginExitBTN.Text = "Salida";
            LanguageLB.Text = "La página de inicio de sesión está en español." + "\r\n" + "Login is in Spanish";
        }

        // Verifies User Name matches Password
        private void LoginBTN_Click(object sender, EventArgs e)
        {
            string login = @"SELECT userName, 
                                    password
                                FROM user
                                WHERE userName = @userName
                                    And password = @password";

            MySqlConnection conLogin = new MySqlConnection(constrLogin);
            conLogin.Open();
            MySqlCommand cmdLogin = new MySqlCommand(login, conLogin);
            cmdLogin.Parameters.AddWithValue("@userName", LoginUserNameTB.Text);
            cmdLogin.Parameters.AddWithValue("@password", LoginPasswordTB.Text);
            MySqlDataReader drLogin;

            try
            {
                //TODO Checks for directory
                string loginFolder = @"c:\C969temp";
                string loginFile = @"\login.txt";
                loginFile = loginFolder + loginFile;
                drLogin = cmdLogin.ExecuteReader();
                int count = 0;

                // Checks if folder exsists if it doesn't creates one
                if (!Directory.Exists(loginFolder))
                {
                    Directory.CreateDirectory(loginFolder);
                }
                if (string.IsNullOrWhiteSpace(LoginUserNameTB.Text))
                {
                    if (CultureInfo.CurrentCulture.Name == "es-ES") // Changes MesageBox to spanish.
                    {
                        MessageBox.Show("Introduzca el nombre de usuario");
                        return;
                    }
                    MessageBox.Show("Enter User Name");
                    return;
                }
                if (string.IsNullOrWhiteSpace(LoginPasswordTB.Text))
                {
                    if (CultureInfo.CurrentCulture.Name == "es-ES") // Changes MesageBox to spanish.
                    {
                        MessageBox.Show("Introduzca la contraseña");
                        return;
                    }
                    MessageBox.Show("Enter Password");
                    return;
                }
                while (drLogin.Read())
                {
                    count++;
                }
                // User Name and Password Match goes to Main Page
                if (count == 1)
                {
                    using (StreamWriter loginRecord = File.AppendText(loginFile))
                    {
                        string output = ("User: " + LoginUserNameTB.Text + " successful login: " + DateTime.Now.ToString("F"));
                        loginRecord.WriteLine(output);
                        loginRecord.Close();
                    }
                   MainPage mainPage = new MainPage();
                    this.Hide();
                    mainPage.Show();
                    AppointmentAlert();
                    return;
                }
                else
                {
                    //TODO Add Code for logging Unsuccessful
                    using (StreamWriter loginRecord = File.AppendText(loginFile))
                    {
                        string output = ("User: " + LoginUserNameTB.Text + " unsuccessful login: " + DateTime.Now.ToString("F"));
                        loginRecord.WriteLine(output);
                        loginRecord.Close();
                    }
                    if (CultureInfo.CurrentCulture.Name == "ES")
                    {
                        MessageBox.Show("Nombre de usuario o contraseña incorrectos");  // Changes MesageBox to spanish.
                    }
                    MessageBox.Show("Incorrect User Name or Password");
                }
                conLogin.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error" + "\n" + ex.Message);
                return;
            }
        }
        
        private void ExitBTN_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
