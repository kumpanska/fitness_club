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
    /// Interaction logic for ClientForm.xaml
    /// </summary>
    public partial class ClientForm : Window
    {
        private ObservableCollection<Classes.ClientClass> allClients = new ObservableCollection<ClientClass>();
        private ObservableCollection<Classes.ClientClass> filteredClientsBySearch = new ObservableCollection<ClientClass>();
        private ObservableCollection<Classes.CoachClass> coaches = new ObservableCollection<CoachClass>();
        private const string connectionString = "Server=DESKTOP-K1I43VD;Database=master;TrustServerCertificate=True;Trusted_Connection=True";
        public ClientForm()
        {
            InitializeComponent();
            LoadCoaches();
            LoadClients();
        }
        private void LoadCoaches()
        {
            coaches.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT [Id], [Last Name], [Name],[Middle Name] FROM [Table_Coaches]";
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
                       
                    });
                }
                CoachComboBox.ItemsSource = coaches;
            }
        }
        private void LoadClients()
        {
            allClients.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT [Id], [Last Name], [Name],[Middle Name] FROM [Table_Clients]";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    allClients.Add(new ClientClass
                    {
                        Id = (int)reader["Id"],
                        LastName = reader["Last Name"] as string ?? "",
                        Name = reader["Name"] as string ?? "",
                        MiddleName = reader["Middle Name"] as string ?? "",
                    });
                }
                filteredClientsBySearch.Clear();
                foreach (ClientClass client in allClients)
                {
                    filteredClientsBySearch.Add(client);
                }
                ClientComboBox.ItemsSource = filteredClientsBySearch;
                ClientComboBox.DisplayMemberPath = "FullNameText";
                ClientComboBox.SelectedValuePath = "Id";
            }
        }
        private void CoachComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientComboBox.SelectedValue == null || CoachComboBox.SelectedValue == null)
            {
                return;
            }
            int clientId = (int)ClientComboBox.SelectedValue;
            int coachNewId = (int)CoachComboBox.SelectedValue;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT CoachId FROM Table_ClientCoach WHERE ClientId=@ClientId", connection);
                cmd.Parameters.AddWithValue("@ClientId", clientId);
                var result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                {
                    return;
                }
                var coachOldId = (int)cmd.ExecuteScalar();
                if (coachNewId != coachOldId)
                {
                    MessageBoxResult res= MessageBox.Show("Перейти до нового тренера?","Перехід до нового тренера", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes)
                    {
                        SqlCommand update = new SqlCommand("UPDATE Table_ClientCoach SET CoachId=@CoachNewId WHERE ClientId=@ClientId", connection);
                        update.Parameters.AddWithValue("@CoachNewId", coachNewId);
                        update.Parameters.AddWithValue("@ClientId", clientId);
                        update.ExecuteNonQuery();
                        SqlCommand deleteMark = new SqlCommand("DELETE FROM Table_Ratings WHERE CoachId=@CoachOldId AND ClientId=@ClientId", connection);
                        deleteMark.Parameters.AddWithValue("@CoachOldId", coachOldId);
                        deleteMark.Parameters.AddWithValue("@ClientId", clientId);
                        deleteMark.ExecuteNonQuery();
                        ExerciseListView.Items.Clear();
                        UpdateCoachRating(coachOldId);
                        UpdateCoachRating(coachNewId);
                    }
                    else { CoachComboBox.SelectedValue = coachOldId; }
                }
            }


        }

        private void ClientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientComboBox.SelectedValue is int clientId)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT CoachId FROM Table_ClientCoach WHERE ClientId=@ClientId", connection);
                    cmd.Parameters.AddWithValue("@ClientId", clientId);
                    var coachId = cmd.ExecuteScalar();
                    if (coachId != null)
                    { CoachComboBox.SelectedValue = (int)coachId; }
                    LoadExercisesForClient(clientId);
                }
            }

        }
        private void LoadExercisesForClient(int clientId)
        {
            ExerciseListView.Items.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(@"
            SELECT [Exercise Name], [Type], [Number Of Repetitions]
            FROM Table_ClientsExercises
            WHERE ClientId = @ClientId", connection);
                command.Parameters.AddWithValue("@ClientId", clientId);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ExerciseListView.Items.Add(new Classes.ExerciseClass
                        {
                            NameOfExercise = reader.GetString(0),
                            Type = reader.GetString(1),
                            Repetitions = reader.GetInt32(2)
                        }
                        );

                    }
                }

            }
        }
        private void UpdateCoachRating(int coachId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT AVG(CAST(Mark AS DECIMAL(5,2))) FROM Table_Ratings WHERE CoachId=@CoachId", connection);
                cmd.Parameters.AddWithValue("@CoachId", coachId);
                var average = cmd.ExecuteScalar();
                decimal avgValue = 0;
                if (average != null && average != DBNull.Value)
                {
                    avgValue = Convert.ToDecimal(average);
                }
                SqlCommand updateCmd = new SqlCommand("UPDATE Table_Coaches SET AverageMark = @Average WHERE Id = @CoachId", connection);
                updateCmd.Parameters.AddWithValue("@Average", avgValue);
                updateCmd.Parameters.AddWithValue("@CoachId", coachId);
                updateCmd.ExecuteNonQuery();
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientComboBox.SelectedValue == null || CoachComboBox.SelectedValue == null)
            {
                MessageBox.Show("Оберіть клієнта і тренера.");
                return;
            }

            if (!double.TryParse(RatingTextBox.Text, out double mark) || mark < 0 || mark > 5)
            {
                MessageBox.Show("Потрібно ввести оцінку від 0 до 5.");
                return;
            }
            int coachId = (int)CoachComboBox.SelectedValue;
            int clientId = (int)ClientComboBox.SelectedValue;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Table_Ratings WHERE CoachId = @CoachId AND ClientId=@ClientId", connection);
                checkCmd.Parameters.AddWithValue("@CoachId", coachId);
                checkCmd.Parameters.AddWithValue("@ClientId", clientId);
                int count = (int)checkCmd.ExecuteScalar(); 
                if (count > 0)
                {
                    SqlCommand updateCmd = new SqlCommand("UPDATE Table_Ratings SET Mark = @Mark WHERE CoachId = @CoachId AND ClientId=@ClientId", connection);
                    updateCmd.Parameters.AddWithValue("@CoachId", coachId);
                    updateCmd.Parameters.AddWithValue("@ClientId", clientId);
                    updateCmd.Parameters.AddWithValue("@Mark", mark);
                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    SqlCommand insertCmd = new SqlCommand("INSERT INTO Table_Ratings(CoachId,ClientId, Mark) VALUES (@CoachId,@ClientId ,@Mark)", connection);
                    insertCmd.Parameters.AddWithValue("@CoachId", coachId);
                    insertCmd.Parameters.AddWithValue("@ClientId", clientId);
                    insertCmd.Parameters.AddWithValue("@Mark", mark);
                    insertCmd.ExecuteNonQuery();
                }
            }

            UpdateCoachRating(coachId);
            MessageBox.Show("Оцінку збережено.");
            RatingTextBox.Clear();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void ClientSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = ClientSearch.Text.ToLower().Trim();
            filteredClientsBySearch.Clear();
            if (string.IsNullOrWhiteSpace(search))
            {
                foreach (ClientClass client in allClients)
                {
                    filteredClientsBySearch.Add(client);
                }
            }
            else
            {
                foreach (ClientClass client in allClients)
                {
                    if (client.LastName.ToLower().Contains(search))
                    {
                        filteredClientsBySearch.Add(client);
                    }
                }
            }
            ClientComboBox.ItemsSource = null;
            ClientComboBox.ItemsSource = filteredClientsBySearch;
            if (filteredClientsBySearch.Count == 1)
            {
                ClientComboBox.SelectedItem = filteredClientsBySearch[0];
            }
            else
            {
                ClientComboBox.SelectedItem = null;
                CoachComboBox.SelectedItem = null;
                ExerciseListView.Items.Clear();
            }
        }
    }
}
