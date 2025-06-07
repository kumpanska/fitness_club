using fitness_club.Classes;
using Microsoft.Data.SqlClient;
using MySqlX.XDevAPI;
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
    /// Interaction logic for EditCoachInformationForm.xaml
    /// </summary>
    public partial class EditCoachInformationForm : Window
    {
        private readonly int coachId;
        private const string connectionString = "Server=DESKTOP-K1I43VD;Database=master;TrustServerCertificate=True;Trusted_Connection=True";
        public EditCoachInformationForm(CoachClass coach)
        {
            InitializeComponent();
            LoadExerciseTypes();
            coachId = coach.Id;
            CoachNameText.Text = coach.Name;
            CoachLastNameText.Text = coach.LastName;
            CoachMiddleNameText.Text = coach.MiddleName;
            CoachPhoneNumberText.Text = coach.PhoneNumber;
            CoachEmailText.Text = coach.Email;
            foreach(ComboBoxItem item in ExerciseTypeCbo.Items)
            {
                if (item.Content.ToString() == coach.FitnessServices)
                {
                    ExerciseTypeCbo.SelectedItem = item;
                    break;
                }
            }
        }
        private void LoadExerciseTypes()
        {
            Classes.LoadExerciseTypesClass.LoadExerciseTypesIntoComboBox(ExerciseTypeCbo);
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            txtError.Visibility = Visibility.Collapsed;
            string name = CoachNameText.Text.Trim();
            string lastName = CoachLastNameText.Text.Trim();
            string middleName = CoachMiddleNameText.Text.Trim();
            string phone = CoachPhoneNumberText.Text.Trim();
            string email = CoachEmailText.Text.Trim();
            string? fitnessServices = (ExerciseTypeCbo.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(lastName) ||
       string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(fitnessServices))
            {
                MessageBox.Show("Заповніть усі поля.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            CoachClass coach = new CoachClass { Name = name, LastName = lastName, MiddleName = middleName, PhoneNumber = phone, Email = email,FitnessServices=fitnessServices};
            if (!ValidateCoachAttributes(coach, out string errorMessage))
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
                    string query = @"UPDATE [Table_Coaches] SET [Name] = @Name,[Last Name] = @LastName, [Middle Name] = @MiddleName,
                                 [Phone Number] = @PhoneNumber,
                                 [Email] = @Email,
                                 [Fitness Services] = @FitnessServices
                             WHERE [Id] = @Id";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@MiddleName", middleName);
                    cmd.Parameters.AddWithValue("@PhoneNumber", phone);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@FitnessServices", fitnessServices);
                    cmd.Parameters.AddWithValue("@Id", coachId);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Інформацію про тренера оновлено!", "Оновлення інформації", MessageBoxButton.OK, MessageBoxImage.Information);
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
        private bool ValidateCoachAttributes(CoachClass coach, out string errorMessage)
        {
            var context = new ValidationContext(coach);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            bool isValid = Validator.TryValidateObject(coach, context, results, validateAllProperties: true);
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
