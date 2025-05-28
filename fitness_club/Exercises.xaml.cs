using fitness_club.Classes;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for Exercises.xaml
    /// </summary>
    public partial class Exercises : Window
    {
        private ObservableCollection<ExerciseClass> exercises = new ObservableCollection<ExerciseClass>();
        private const string connectionString = "Server=DESKTOP-K1I43VD;Database=master;TrustServerCertificate=True;Trusted_Connection=True";
        public Exercises()
        {
            InitializeComponent();
            LoadExercises();
        }
        private void LoadExercises()
        {
            exercises.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT [Id], [Exercise Name], [Type], [Number Of Repetitions] FROM [Table_Exercises]";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    exercises.Add(new ExerciseClass
                    {
                        Id = (int)reader["Id"],
                        NameOfExercise = reader["Exercise Name"].ToString(),
                        Type = reader["Type"].ToString(),
                        Repetitions = reader["Number Of Repetitions"] != DBNull.Value ? (int)reader["Number Of Repetitions"] : 0
                    });
                }
            }
            ExercisesListView.ItemsSource = exercises;
        }
        private void ExercisesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isItemSelected = ExercisesListView.SelectedItem is ExerciseClass;
            EditButton.IsEnabled = isItemSelected;
            DeleteButton.IsEnabled = isItemSelected;
        }

        private void AddExercise_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddExercise();
            if (window.ShowDialog() == true)
            {
                LoadExercises();
            }
        }

        private void EditExercise_Click(object sender, RoutedEventArgs e)
        {
            if(ExercisesListView.SelectedItem is ExerciseClass selected)
            {
                var window = new EditExercise(selected);
                if (window.ShowDialog() == true)
                {
                    LoadExercises();
                }
            }
        }

        private void DeleteExercise_Click(object sender, RoutedEventArgs e)
        {
            if (ExercisesListView.SelectedItem is ExerciseClass selected)
            {
                var result = MessageBox.Show($"Ви впевнені, що хочете видалити вправу: {selected.NameOfExercise}?","Підтвердження",MessageBoxButton.YesNo);
                if(result==MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM [Table_Exercises] WHERE Id = @Id";
                        SqlCommand cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@Id", selected.Id);
                        cmd.ExecuteNonQuery();
                    }
                    LoadExercises();
                }
            }
        }
    }
}
