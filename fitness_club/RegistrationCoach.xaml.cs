using fitness_club.Classes;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for RegistrationCoach.xaml
    /// </summary>
    public partial class RegistrationCoach : Window
    {
        private const string connectionString = "Server=DESKTOP-K1I43VD;Database=master;TrustServerCertificate=True;Trusted_Connection=True";
        public RegistrationCoach()
        {
            InitializeComponent();
            LoadDistinctExerciseTypes();
        }
        private void LoadDistinctExerciseTypes()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT DISTINCT [Type] FROM [dbo].[Table_Exercises]";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            ExerciseTypeCbo.Items.Clear();
                            while (reader.Read())
                            {
                                string? type = reader["Type"]?.ToString();
                                if (!string.IsNullOrWhiteSpace(type))
                                {
                                    ExerciseTypeCbo.Items.Add(type);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при завантаженні типів вправ: " + ex.Message);
            }
        }
        private void RegistrationCoach_Click(object sender, RoutedEventArgs e)
        {
            CoachClass coach = new CoachClass
            {
                Name = CoachNameText.Text.Trim(),
                MiddleName = CoachMiddleNameText.Text.Trim(),
                LastName = CoachLastNameText.Text.Trim(),
                PhoneNumber = CoachPhoneNumberText.Text.Trim(),
                Email = CoachEmailText.Text.Trim(),
                FitnessServices = ExerciseTypeCbo.SelectedItem as string ?? string.Empty
            };

            if (string.IsNullOrWhiteSpace(coach.Name) || string.IsNullOrWhiteSpace(coach.LastName) || string.IsNullOrWhiteSpace(coach.MiddleName) ||
                string.IsNullOrWhiteSpace(coach.PhoneNumber) || string.IsNullOrWhiteSpace(coach.Email) ||
                string.IsNullOrWhiteSpace(coach.FitnessServices))
            {
                MessageBox.Show("Заповніть всі поля.");
                return;
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                INSERT INTO Table_Coaches ([Name], [Last Name],[Middle Name], [Phone Number], [Email], [Fitness Services])
                VALUES (@Name, @LastName,@MiddleName, @PhoneNumber, @Email, @FitnessServices)";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", coach.Name);
                        cmd.Parameters.AddWithValue("@LastName", coach.LastName);
                        cmd.Parameters.AddWithValue("@MiddleName", coach.MiddleName);
                        cmd.Parameters.AddWithValue("@PhoneNumber", coach.PhoneNumber);
                        cmd.Parameters.AddWithValue("@Email", coach.Email);
                        cmd.Parameters.AddWithValue("@FitnessServices", coach.FitnessServices);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Тренер успішно зареєстрований!", "Зареєстровано тренера");
                CoachNameText.Clear();
                CoachLastNameText.Clear();
                CoachMiddleNameText.Clear();
                CoachPhoneNumberText.Clear();
                CoachEmailText.Clear();
                ExerciseTypeCbo.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка під час запису даних в базу даних: " + ex.Message, "Помилка");
            }
        }
    }
}
