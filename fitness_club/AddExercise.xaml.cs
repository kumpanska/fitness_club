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
using Microsoft.Data.SqlClient;

namespace fitness_club
{
    /// <summary>
    /// Interaction logic for AddExercise.xaml
    /// </summary>
    public partial class AddExercise : Window
    {
        private const string connectionString = "Server=DESKTOP-K1I43VD;Database=master;TrustServerCertificate=True;Trusted_Connection=True";
        public AddExercise()
        {
            InitializeComponent();
            LoadExerciseTypes();
        }
        private void LoadExerciseTypes()
        {
            ExerciseTypeComboBox.Items.Clear();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT DISTINCT [Type] FROM [Table_Exercises] WHERE [Type] IS NOT NULL AND [Type] <> ''";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string? type = reader["Type"]?.ToString();
                                if (!string.IsNullOrWhiteSpace(type))
                                {
                                    ExerciseTypeComboBox.Items.Add(new ComboBoxItem() { Content = type });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при завантаженні типів вправ: " + ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string name = ExerciseNameTextBox.Text.Trim();
            string? type = null;
            if (ExerciseTypeComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content != null)
            {
                type = selectedItem.Content.ToString();
            }
            string repetitionsText = RepetitionsTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type) || string.IsNullOrEmpty(repetitionsText))
            {
                MessageBox.Show("Заповніть усі поля.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(repetitionsText, out int repetitions) || repetitions < 0)
            {
                MessageBox.Show("Кількість повторень має бути додатним числом.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO [Table_Exercises] ([Exercise Name], [Type], [Number Of Repetitions]) " +
                                  "VALUES (@Name, @Type, @Repetitions)";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Type", type);
                    cmd.Parameters.AddWithValue("@Repetitions", repetitions);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Вправу успішно додано!", "Додавання вправи", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при додаванні вправи: " + ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
