using fitness_club.Classes;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace fitness_club
{
    /// <summary>
    /// Interaction logic for EditClientInformationForm.xaml
    /// </summary>
    public partial class EditClientInformationForm : Window
    {
        private readonly int clientId;
        private const string connectionString = "Server=DESKTOP-K1I43VD;Database=master;TrustServerCertificate=True;Trusted_Connection=True";
        public EditClientInformationForm(ClientClass client)
        {
            InitializeComponent();
            clientId = client.Id;
            ClientNameText.Text = client.Name;
            ClientLastNameText.Text = client.LastName;
            ClientMiddleNameText.Text = client.MiddleName;
            ClientPhoneNumberText.Text = client.PhoneNumber;
            ClientEmailText.Text = client.Email;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            txtError.Visibility=Visibility.Collapsed;
            string name = ClientNameText.Text.Trim();
            string lastName = ClientLastNameText.Text.Trim();
            string middleName = ClientMiddleNameText.Text.Trim();
            string phone = ClientPhoneNumberText.Text.Trim();
            string email = ClientEmailText.Text.Trim();
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Заповніть усі поля.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            ClientClass client = new ClientClass { Name = name, LastName = lastName, MiddleName = middleName,PhoneNumber=phone,Email=email };
            if (!ValidateClientAttributes(client, out string errorMessage))
            {
                txtError.Text = errorMessage;
                txtError.Visibility = Visibility.Visible;
                return;
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"UPDATE [Table_Clients] SET [Name] = @Name, [Last Name] = @LastName, [Middle Name] = @MiddleName,
                                 [Phone Number] = @PhoneNumber,
                                 [Email] = @Email
                             WHERE [Id] = @Id";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@MiddleName", middleName);
                    cmd.Parameters.AddWithValue("@PhoneNumber", phone);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Id", clientId);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Інформацію про клієнта оновлено!", "Оновлення інформації", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при збереженні інформації: " + ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private bool ValidateClientAttributes(ClientClass client, out string errorMessage)
        {
            var context = new ValidationContext(client);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
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
