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
    /// Interaction logic for Clients.xaml
    /// </summary>
    public partial class Clients : Window
    {
        private ObservableCollection<Classes.ClientClass> clients = new ObservableCollection<Classes.ClientClass>();
        private const string connectionString = "Server=DESKTOP-K1I43VD;Database=master;TrustServerCertificate=True;Trusted_Connection=True";
        public Clients()
        {
            InitializeComponent();
            LoadClients();
        }
        private void LoadClients()
        {
            clients.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT [Id], [Last Name], [Name],[Middle Name],[Phone Number],[Email] FROM [Table_Clients]";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    clients.Add(new ClientClass
                    {
                        Id = (int)reader["Id"],
                        LastName = reader["Last Name"] as string ?? ""  ,
                        Name = reader["Name"] as string ?? "",
                        MiddleName = reader["Middle Name"] as string ?? "",
                        PhoneNumber = reader["Phone Number"] as string ?? "",
                        Email = reader["Email"] as string ?? ""
                    });
                }
                ClientsListView.ItemsSource = clients;
                
            }
        }
        private void CoachesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isItemSelected = ClientsListView.SelectedItem is ClientClass;
            EditButton.IsEnabled = isItemSelected;
            DeleteButton.IsEnabled = isItemSelected;
        }

        private void DeleteClient_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsListView.SelectedItem is ClientClass selected)
            {
                var result = MessageBox.Show($"Видалити клієнта: {selected.FullName()}?", "Підтвердження", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM [Table_Clients] WHERE Id = @Id";
                        SqlCommand cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@Id", selected.Id);
                        cmd.ExecuteNonQuery();
                    }
                    LoadClients();
                }
            }
        }

        private void EditClient_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsListView.SelectedItem is ClientClass selected)
            {
                var window = new EditClientInformationForm(selected);
                if (window.ShowDialog() == true)
                {
                    LoadClients();
                }
            }
        }
    }
}
