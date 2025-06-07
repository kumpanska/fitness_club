using fitness_club.Classes;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Protocols.Configuration;
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
    /// Interaction logic for EditScheduleForm.xaml
    /// </summary>
    public partial class AddEditScheduleForm : Window
    {
        private ScheduleClass schedule = new ScheduleClass();
        private const string connectionString = "Server=DESKTOP-K1I43VD;Database=master;TrustServerCertificate=True;Trusted_Connection=True";
        public AddEditScheduleForm()
        {
            InitializeComponent();
            this.Title = "Додавання нового розкладу";
            LoadCoaches();
        }
        public AddEditScheduleForm(ScheduleClass schedule)
        {
            InitializeComponent();
            this.Title = "Редагування розкладу";
            this.schedule = schedule;
            LoadCoaches();
            CoachComboBox.SelectedValue = schedule.CoachId;
            LoadFitnessServices(schedule.CoachId);
            FitnessServiceComboBox.SelectedValue = schedule.FitnessServicesId;
            LoadCurrentData();
        }
        private void LoadCurrentData()
        {
            if (schedule != null)
            {
                CoachComboBox.SelectedValue = schedule.CoachId;
                DatePicker.SelectedDate = schedule.Date;
                TimeBox.Text = schedule.Time.ToString(@"hh\:mm");
                FitnessServiceComboBox.SelectedValue = schedule.FitnessServicesId;
            }
        }
        private void LoadCoaches()
        {
            var coaches = new Dictionary<int, string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Id,[Last Name],[Name],[Middle Name] FROM  Table_Coaches";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = (int)reader["Id"];
                            string fullName = $"{reader["Last Name"]} {reader["Name"]} {reader["Middle Name"]}".Trim();
                            coaches[id]=fullName;
                        }
                    }
                }
            }
            CoachComboBox.ItemsSource = coaches.ToList();
            CoachComboBox.DisplayMemberPath = "Value";
            CoachComboBox.SelectedValuePath = "Key";
        }
        private void LoadFitnessServices(int coachid)
        {
            var services = new Dictionary<int,string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT MIN(Id) AS Id,Type FROM Table_Exercises GROUP BY Type";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                { 
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = (int)reader["Id"];
                            int typeIndex = reader.GetOrdinal("Type");
                            string type = reader.IsDBNull(typeIndex) ? "" : reader.GetString(typeIndex);
                            if (!string.IsNullOrWhiteSpace(type))
                            {
                                services[id]=type;
                            }
                           
                        }
                    }
                }
            }
            FitnessServiceComboBox.ItemsSource = services.ToList();
            FitnessServiceComboBox.DisplayMemberPath = "Value";
            FitnessServiceComboBox.SelectedValuePath = "Key";
        }
        private void CoachComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CoachComboBox.SelectedValue is int coachId)
            {
                LoadFitnessServices(coachId);
            }
        }
        private bool ValidateScheduleAttributes(ScheduleClass schedule, out string errorMessage)
        {
            var context = new ValidationContext(schedule);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            bool isValid = Validator.TryValidateObject(schedule, context, results, validateAllProperties: true);
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
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (CoachComboBox.SelectedValue == null || FitnessServiceComboBox.SelectedValue == null || !DatePicker.SelectedDate.HasValue)

            {
                MessageBox.Show("Заповніть всі поля.");
                return;
            }
            if (!TimeSpan.TryParse(TimeBox.Text, out TimeSpan parsedTime))
            {
                MessageBox.Show("Введіть коректний формат часу(HH:MM).");
                return;
            }
            ScheduleClass scheduleValidate = new ScheduleClass
            {
                Id = schedule.Id,
                CoachId = (int)CoachComboBox.SelectedValue,
                Date=DatePicker.SelectedDate.Value,
                Time=parsedTime,
                FitnessServicesId=(int)FitnessServiceComboBox.SelectedValue
            };
            if (!ValidateScheduleAttributes(scheduleValidate,out string errorMessage))
            {
                MessageBox.Show($"{errorMessage}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int coachId = (int)CoachComboBox.SelectedValue;
            int fitnessServiceId = (int)FitnessServiceComboBox.SelectedValue;
            DateTime date = DatePicker.SelectedDate.Value;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd;
                    if (schedule != null)
                    {
                        cmd = new SqlCommand("UPDATE Table_Schedule SET CoachId=@CoachId,Date=@Date,Time=@Time,FitnessServicesId=@FitnessServicesId WHERE Id=@Id", connection);
                        cmd.Parameters.AddWithValue("@Id", schedule?.Id);
                    }
                    else
                    {
                        cmd = new SqlCommand("INSERT INTO Table_Schedule(CoachId,Date,Time,FitnessServicesId) VALUES (@CoachId,@Date,@Time,@FitnessServicesId)", connection);

                    }
                    cmd.Parameters.AddWithValue("@CoachId", coachId);
                    cmd.Parameters.AddWithValue("@Date", date);
                    cmd.Parameters.AddWithValue("@Time", parsedTime);
                    cmd.Parameters.AddWithValue("@FitnessServicesId", fitnessServiceId);
                    cmd.ExecuteNonQuery();
                    DialogResult = true;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні: {ex.Message}","Помилка",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
