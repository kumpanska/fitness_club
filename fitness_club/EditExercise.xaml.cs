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
using fitness_club.Classes;
using Microsoft.Data.SqlClient;

namespace fitness_club
{
    /// <summary>
    /// Interaction logic for EditExercise.xaml
    /// </summary>
    public partial class EditExercise : Window
    {
        private readonly int exerciseId;
        private const string connectionString = "Server=DESKTOP-K1I43VD;Database=master;TrustServerCertificate=True;Trusted_Connection=True";
        public EditExercise(ExerciseClass exercise)
        {
            InitializeComponent();
            LoadExerciseTypes();
            exerciseId = exercise.Id;
            ExerciseNameTextBox.Text = exercise.NameOfExercise;
            foreach (ComboBoxItem item in ExerciseTypeComboBox.Items)
            {
                if (item.Content.ToString() == exercise.Type)
                {
                    ExerciseTypeComboBox.SelectedItem = item;
                    break;
                }
            }
            RepetitionsTextBox.Text = exercise.Repetitions.ToString();
        }
        private void LoadExerciseTypes()
        {
            Classes.LoadExerciseTypesClass.LoadExerciseTypesIntoComboBox(ExerciseTypeComboBox);
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = ExerciseNameTextBox.Text.Trim();
            string? type = (ExerciseTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string repetitionsText = RepetitionsTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type) || string.IsNullOrEmpty(repetitionsText))
            {
                MessageBox.Show("Заповніть усі поля.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(repetitionsText, out int repetitions) || repetitions < 0)
            {
                MessageBox.Show("Кількість повторень вправи має бути додатним числом.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            ExerciseClass exercise = new ExerciseClass { NameOfExercise = name, Type = type, Repetitions = repetitions };
            if (!ValidateExerciseAttributes(exercise, out string errorMessage))
            {
                txtError.Text = errorMessage;
                return;
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE [Table_Exercises] SET [Exercise Name] = @Name, [Type] = @Type, [Number Of Repetitions] = @Repetitions WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Type", type);
                    cmd.Parameters.AddWithValue("@Repetitions", repetitions);
                    cmd.Parameters.AddWithValue("@Id", exerciseId);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Вправу успішно оновлено!", "Оновлення вправи", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при оновленні вправи: " + ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private bool ValidateExerciseAttributes(ExerciseClass exercise, out string errorMessage)
        {
            var context = new ValidationContext(exercise);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            bool isValid = Validator.TryValidateObject(exercise, context, results, validateAllProperties: true);
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
