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
    /// Interaction logic for CoachForm.xaml
    /// </summary>
    public partial class CoachForm : Window
    {
        private const string connectionString = "Server=DESKTOP-K1I43VD;Database=master;TrustServerCertificate=True;Trusted_Connection=True";
        private Dictionary<int,ObservableCollection<ExerciseClass>> clientPlans=new();
        private ObservableCollection<ExerciseClass> currentclientPlan;
        private ObservableCollection<ExerciseClass> allExercises = new ObservableCollection<ExerciseClass>();
        private string coachFitnessServices = "";
        public CoachForm()
        {
            InitializeComponent();
            LoadCoaches();
            LoadAllExercises();
        }
        private void LoadCoaches()
        {
            List<Classes.CoachClass> coaches = new List<Classes.CoachClass>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(@"SELECT Id,[Last Name],Name,[Middle Name],[Fitness Services] FROM Table_Coaches", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    coaches.Add(new Classes.CoachClass
                    {
                        Id = (int)reader["Id"],
                        LastName = reader["Last Name"].ToString(),
                        Name = reader["Name"].ToString(),
                        MiddleName = reader["Middle Name"].ToString(),
                        FitnessServices = reader["Fitness Services"].ToString()
                    });

                }
            }
            CoachComboBox.ItemsSource = coaches;
            CoachComboBox.DisplayMemberPath = "FullName";
            CoachComboBox.SelectedValuePath = "Id";
        }
        private void LoadClientExercisePlan(int clientId)
        {
            if (!clientPlans.ContainsKey(clientId))
            {
                ObservableCollection<ExerciseClass> newPlan=new ObservableCollection<ExerciseClass>();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(@"SELECT * FROM Table_ClientsExercises WHERE ClientId=@ClientId", connection);

                    cmd.Parameters.AddWithValue("@ClientId", clientId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        newPlan.Add(new Classes.ExerciseClass
                        {
                            Id = (int)reader["Id"],
                            NameOfExercise = reader["Exercise Name"].ToString(),
                            Type = reader["Type"].ToString(),
                            Repetitions = (int)(reader["Number Of Repetitions"] == DBNull.Value ? 0 : reader["Number Of Repetitions"])
                        });

                    }
                }
                clientPlans[clientId] = newPlan;
            }
            currentclientPlan = clientPlans[clientId];
            ClientPlanList.ItemsSource = currentclientPlan;
            ClientPlanList.Items.Refresh();
        }
        private void LoadClientsByCoach(int coachId)
        {
            List<Classes.ClientClass> clients = new List<Classes.ClientClass>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(@"SELECT c.Id,c.[Last Name],c.Name,c.[Middle Name] FROM Table_Clients c JOIN Table_ClientCoach cc ON cc.ClientId=c.Id WHERE cc.CoachId=@CoachId",connection);
                cmd.Parameters.AddWithValue("@CoachId", coachId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    clients.Add(new Classes.ClientClass
                    {
                        Id = (int)reader["Id"],
                        LastName = reader["Last Name"].ToString(),
                        Name = reader["Name"].ToString(),
                        MiddleName = reader["Middle Name"].ToString()
                    });
                
                }
            }
            ClientComboBox.ItemsSource = clients;
            ClientComboBox.DisplayMemberPath = "FullName";
            ClientComboBox.SelectedValuePath = "Id";
        }
        private void LoadAllExercises() 
        {
            allExercises.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(@"SELECT * FROM Table_Exercises", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    allExercises.Add(new Classes.ExerciseClass
                    {
                        Id = (int)reader["Id"],
                        NameOfExercise = reader["Exercise Name"].ToString(),
                        Type = reader["Type"].ToString(),
                        Repetitions = (int)(reader["Number Of Repetitions"] == DBNull.Value ? 0 : reader["Number Of Repetitions"])
                    });

                }
            }
            AllExercisesListBox.ItemsSource = allExercises;
            AllExercisesListBox.DisplayMemberPath = "NameOfExercise";

        }
        private void CoachComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CoachComboBox.SelectedItem is CoachClass selectedCoach)
            {
                if (!string.IsNullOrWhiteSpace(selectedCoach.FitnessServices))
                {
                    coachFitnessServices = selectedCoach.FitnessServices;
                    FitnessServicesTextBox.Text = coachFitnessServices;
                    LoadClientsByCoach(selectedCoach.Id);
                    LoadExercisesByFitnessServices(coachFitnessServices);
                }
            }
        }
        private void LoadExercisesByFitnessServices(string fitnessServices)
        {
            allExercises.Clear();
            if (string.IsNullOrWhiteSpace(fitnessServices))
            {
                AllExercisesListBox.ItemsSource = allExercises;
                return;
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Table_Exercises WHERE Type = @Type", connection);
                cmd.Parameters.AddWithValue("@Type", fitnessServices);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    allExercises.Add(new ExerciseClass
                    {
                        Id = (int)reader["Id"],
                        NameOfExercise = reader["Exercise Name"].ToString(),
                        Type = reader["Type"].ToString(),
                        Repetitions = (int)(reader["Number Of Repetitions"] == DBNull.Value ? 0 : reader["Number Of Repetitions"])
                    });
                }
            }
            AllExercisesListBox.ItemsSource = allExercises;
            AllExercisesListBox.DisplayMemberPath = "NameOfExercise";
        }
        private void AddExercise_Click(object sender, RoutedEventArgs e)
        {
            if (AllExercisesListBox.SelectedItem is ExerciseClass selectedExercise && ClientComboBox.SelectedItem is ClientClass selectedClient)
            { 
                if (!clientPlans.ContainsKey(selectedClient.Id))
                {
                    clientPlans[selectedClient.Id] = new ObservableCollection<ExerciseClass>();
                }
                var existingExercise = clientPlans[selectedClient.Id].FirstOrDefault(ex => ex.Id == selectedExercise.Id);

                if (existingExercise != null)
                {
                    MessageBox.Show($"Вправа '{selectedExercise.NameOfExercise}' вже існує в комплексі цього клієнта. Видаліть дублікат вправи за допомогою кнопки 'Видалити вправу'",
                                  "Дублікат вправи",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                    return;
                }
                ExerciseClass exerciseCopy = new ExerciseClass
                {
                    Id = selectedExercise.Id,
                    NameOfExercise = selectedExercise.NameOfExercise,
                    Type = selectedExercise.Type,
                    Repetitions = selectedExercise.Repetitions
                };

                clientPlans[selectedClient.Id].Add(exerciseCopy);
                currentclientPlan = clientPlans[selectedClient.Id];
                ClientPlanList.ItemsSource = currentclientPlan;
                ClientPlanList.Items.Refresh();
            }
        }

        private void EditExercise_Click(object sender, RoutedEventArgs e)
        {
            if (ClientPlanList.SelectedItem is ExerciseClass selected)
            {
                ExerciseClass exerciseCopy = new ExerciseClass
                {
                    Id = selected.Id,
                    NameOfExercise = selected.NameOfExercise,
                    Type = selected.Type,
                    Repetitions = selected.Repetitions
                };
                EditClientExercises editExerciseForm = new EditClientExercises(exerciseCopy);
                if (editExerciseForm.ShowDialog() == true)
                {
                    int index = currentclientPlan.IndexOf(selected);
                    if (index >= 0)
                    {
                        currentclientPlan[index] = new ExerciseClass
                        {
                            Id = exerciseCopy.Id,
                            NameOfExercise = exerciseCopy.NameOfExercise,
                            Type = exerciseCopy.Type,
                            Repetitions = exerciseCopy.Repetitions
                        };
                        ClientPlanList.Items.Refresh();
                    }
                }
            }
        }

        private void DeleteExercise_Click(object sender, RoutedEventArgs e)
        {
            if (ClientPlanList.SelectedItem is ExerciseClass selected)
            {
                currentclientPlan.Remove(selected);
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if ((CoachComboBox.SelectedItem as CoachClass) != null && (ClientComboBox.SelectedItem as ClientClass) != null)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    bool anyChangesSaved = false;
                    foreach (var pair in clientPlans)
                    {
                        int clientId = pair.Key;
                        List<(string NameOfExercise,string Type, int Repetitions)> exercisesInDb = new List<(string,string,int)>();
                        SqlCommand selectCmd = new SqlCommand("SELECT [Exercise Name],[Type],[Number Of Repetitions] FROM Table_ClientsExercises WHERE ClientId = @ClientId", connection);
                        selectCmd.Parameters.AddWithValue("@ClientId", clientId);
                        using (SqlDataReader reader = selectCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                exercisesInDb.Add((reader["Exercise Name"].ToString(), reader["Type"].ToString(), Convert.ToInt32(reader["Number Of Repetitions"])));
                            }
                        }
                        var exercisesInMemory = pair.Value.Select(ex => (ex.NameOfExercise,ex.Type,ex.Repetitions)).ToList();

                        bool plansAreEqual = exercisesInDb.Count == exercisesInMemory.Count &&
                                             !exercisesInDb.Except(exercisesInMemory).Any() &&
                                             !exercisesInMemory.Except(exercisesInDb).Any();

                        if (!plansAreEqual)
                        {
                            SqlCommand deleteCmd = new SqlCommand("DELETE FROM Table_ClientsExercises WHERE ClientId = @ClientId", connection);
                            deleteCmd.Parameters.AddWithValue("@ClientId", clientId);
                            deleteCmd.ExecuteNonQuery();

                            foreach (var exercise in pair.Value)
                            {
                                SqlCommand insertCmd = new SqlCommand(@"INSERT INTO Table_ClientsExercises 
                        (ClientId, [Exercise Name], [Type], [Number Of Repetitions]) 
                        VALUES (@ClientId, @ExerciseName, @Type, @Repetitions)", connection);
                                insertCmd.Parameters.AddWithValue("@ClientId", clientId);
                                insertCmd.Parameters.AddWithValue("@ExerciseName", exercise.NameOfExercise);
                                insertCmd.Parameters.AddWithValue("@Type", exercise.Type);
                                insertCmd.Parameters.AddWithValue("@Repetitions", exercise.Repetitions);
                                insertCmd.ExecuteNonQuery();
                            }
                            anyChangesSaved = true;
                        }
                    }

                    if (anyChangesSaved)
                    {
                        MessageBox.Show("Комплекс вправ клієнта збережено.", "Збереження плану", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("План вправ клієнта вже збережено і не було змін.", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                if ((CoachComboBox.SelectedItem as CoachClass) == null && (ClientComboBox.SelectedItem as ClientClass) == null)
                {
                    MessageBox.Show("Оберіть тренера  та клієнта.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void ClientPlanList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditExerciseButton.IsEnabled = ClientPlanList.SelectedItem != null;
            DeleteExerciseButton.IsEnabled = ClientPlanList.SelectedItem != null;
        }

        private void ClientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientComboBox.SelectedItem is ClientClass selectedClient)
            {
                LoadClientExercisePlan(selectedClient.Id);
            }
        }
    }
}
