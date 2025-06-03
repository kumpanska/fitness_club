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
    /// Interaction logic for Coaches.xaml
    /// </summary>
    public partial class Coaches : Window
    {
        private ObservableCollection<Classes.CoachClass> coaches = new ObservableCollection<Classes.CoachClass>();
        private const string connectionString = "Server=DESKTOP-K1I43VD;Database=master;TrustServerCertificate=True;Trusted_Connection=True";
        public Coaches()
        {
            InitializeComponent();
            LoadCoaches();
        }
        private void LoadCoaches()
        {
            coaches.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT [Id], [Last Name], [Name],[Middle Name],[Phone Number],[Email], [Fitness Services] FROM [Table_Coaches]";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    coaches.Add(new CoachClass
                    {
                        Id = (int)reader["Id"],
                        LastName = reader["Last Name"] as string ?? "",
                        Name = reader["Name"] as string ?? "",
                        MiddleName = reader["Middle Name"] as string ?? "",
                        PhoneNumber = reader["Phone Number"] as string ?? "",
                        Email = reader["Email"] as string ?? "",
                        FitnessServices  = reader["Fitness Services"] as string ?? ""
                    });
                }
                CoachesListView.ItemsSource = coaches;
            }
        }
        private void EditCoach_Click(object sender, RoutedEventArgs e)
        {
            if (CoachesListView.SelectedItem is CoachClass selected)
            {
                var window = new EditCoachInformationForm(selected);
                if (window.ShowDialog() == true)
                {
                    LoadCoaches();
                }
            }
        }
        private void DeleteCoach_Click(object sender, RoutedEventArgs e)
        {
            if (CoachesListView.SelectedItem is CoachClass selected)
            {
                MessageBoxResult result = MessageBox.Show($"Видалити тренера: {selected.LastName} {selected.Name} {selected.MiddleName}?", "Підтвердження", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM [Table_Coaches] WHERE Id=@Id";
                        SqlCommand cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@Id", selected.Id);
                        cmd.ExecuteNonQuery();
                    }
                    LoadCoaches();
                }
            }
        }
        private void CoachesListView_SelectionChanged(object sender,SelectionChangedEventArgs e)
        {
            bool isItemSelected = CoachesListView.SelectedItem is CoachClass;
            EditButton.IsEnabled = isItemSelected;
            DeleteButton.IsEnabled = isItemSelected;
        }
    }
}
