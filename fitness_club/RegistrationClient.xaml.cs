using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using fitness_club.Classes;
using Microsoft.Data.SqlClient;

namespace fitness_club
{
    /// <summary>
    /// Interaction logic for RegistrationClient.xaml
    /// </summary>
    public partial class RegistrationClient : Window
    {
        private const string connectionString = "Server=DESKTOP-K1I43VD;Database=master;TrustServerCertificate=True;Trusted_Connection=True";
        public RegistrationClient()
        {
            InitializeComponent();
        }

        private void RegistrationClient_Click(object sender, RoutedEventArgs e)
        {
            txtError.Text = string.Empty;
            ClientClass client = new ClientClass
            {
                Name = ClientNameText.Text.Trim(),
                LastName = ClientLastNameText.Text.Trim(),
                MiddleName = ClientMiddleNameText.Text.Trim(),
                PhoneNumber = ClientPhoneNumberText.Text.Trim(),
                Email = ClientEmailText.Text.Trim()
            };
            if (string.IsNullOrWhiteSpace(client.Name) || string.IsNullOrWhiteSpace(client.LastName) || string.IsNullOrWhiteSpace(client.MiddleName) ||
               string.IsNullOrWhiteSpace(client.PhoneNumber) || string.IsNullOrWhiteSpace(client.Email))
            {
                MessageBox.Show("Заповніть всі поля.");
                return;
            }
            if (!ValidateClientAttributes(client, out string errorMessage))
            {
                txtError.Text = errorMessage;
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO Table_Clients ([Name], [Last Name],[Middle Name], [Phone Number], [Email])
                        VALUES (@Name, @LastName,@MiddleName,@PhoneNumber, @Email)";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", client.Name);
                        cmd.Parameters.AddWithValue("@LastName", client.LastName);
                        cmd.Parameters.AddWithValue("@MiddleName", client.MiddleName);
                        cmd.Parameters.AddWithValue("@PhoneNumber", client.PhoneNumber);
                        cmd.Parameters.AddWithValue("@Email", client.Email);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Клієнт успішно зареєстрований!", "Клієнта зареєстровано");
                ClientNameText.Clear();
                ClientLastNameText.Clear();
                ClientMiddleNameText.Clear();
                ClientPhoneNumberText.Clear();
                ClientEmailText.Clear();
                txtError.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка під час запису даних в базу даних: " + ex.Message, "Помилка");
            }
        }
        private bool ValidateClientAttributes(ClientClass client, out string errorMessage)
        {
            var context = new ValidationContext(client);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(client, context, results, validateAllProperties: true);
            if (!isValid)
            {
                errorMessage = string.Join(Environment.NewLine, results.Select(r => r.ErrorMessage));
            }
            else
            {
                errorMessage = string.Empty;
            }
            return isValid;
        }
    }
}
