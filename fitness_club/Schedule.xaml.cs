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
    /// Interaction logic for Schedule.xaml
    /// </summary>
    public partial class Schedule : Window
    {
        private ObservableCollection<ScheduleClass> schedules = new ObservableCollection<ScheduleClass>();
        private const string connectionString = "Server=DESKTOP-K1I43VD;Database=master;TrustServerCertificate=True;Trusted_Connection=True";
        public Schedule()
        {
            InitializeComponent();
            LoadSchedule();
        }
        private void EditSchedule_Click(object sender, RoutedEventArgs e)
        {
            if (ScheduleListView.SelectedItem is ScheduleClass selected)
            {
                var window = new AddEditScheduleForm(selected);
                if (window.ShowDialog() == true)
                {
                    LoadSchedule();
                }
            }
        }
        private void LoadSchedule()
        {
            schedules.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                 SELECT 
                  s.Id,
                  s.CoachId,
                  s.Date,
                  s.Time,
                  c.[Last Name] AS CoachLastName,
                  c.[Name] AS CoachFirstName,
                  c.[Middle Name] AS CoachMiddleName,
                  c.[Fitness Services] AS FitnessServiceName
                  FROM Table_Schedule s 
                  LEFT JOIN Table_Coaches c ON s.CoachId = c.Id";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var coachFullName = $"{reader["CoachLastName"]} {reader["CoachFirstName"]} {reader["CoachMiddleName"]}".Trim();
                    schedules.Add(new ScheduleClass
                    {
                        Id = (int)reader["Id"],
                        CoachId = (int)reader["CoachId"],
                        Date = Convert.ToDateTime(reader["Date"]),
                        Time = (TimeSpan)reader["Time"],
                        CoachFullName = coachFullName,
                        FitnessServiceName = reader["FitnessServiceName"]?.ToString()
                    });
                }
            }
            ScheduleListView.ItemsSource = schedules;
        }

        private void ScheduleListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isItemSelected = ScheduleListView.SelectedItem is ScheduleClass;
            EditButton.IsEnabled = isItemSelected;
            DeleteButton.IsEnabled = isItemSelected;
        }

        private void AddSchedule_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddEditScheduleForm();
            if (window.ShowDialog() == true)
            {
                LoadSchedule();
            }
        }

        private void DeleteSchedule_Click(object sender, RoutedEventArgs e)
        {
            if (ScheduleListView.SelectedItem is ScheduleClass selected)
            {
                var result = MessageBox.Show($"Ви впевнені, що хочете видалити розклад тренера: {selected.CoachFullName}?", "Підтвердження", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM [Table_Schedule] WHERE Id = @Id";
                        SqlCommand cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@Id", selected.Id);
                        cmd.ExecuteNonQuery();
                    }
                    LoadSchedule();
                }
            }
        }
    }
}
