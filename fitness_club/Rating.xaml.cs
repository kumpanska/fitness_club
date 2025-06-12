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
    /// Interaction logic for Ratings.xaml
    /// </summary>
    public partial class Rating : Window
    {
        ObservableCollection<Classes.RatingClass> ratings = new ObservableCollection<Classes.RatingClass>();
        private const string connectionString = "Server=DESKTOP-K1I43VD;Database=master;TrustServerCertificate=True;Trusted_Connection=True";
        public Rating()
        {
            InitializeComponent();
            LoadRating();
        }
        private void LoadRating()
        {
            ratings.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT c.Id AS CoachId, AVG(CAST(r.Mark As DECIMAL(5,2)))" +
                    " AS Mark,COUNT(r.Mark) AS RatingCount,c.[Last Name],c.[Name],c.[Middle Name]" +
                    " FROM [Table_Ratings] r JOIN [Table_Coaches] c ON r.CoachId=c.Id GROUP BY c.Id, c.[Last Name], c.[Name],c.[Middle Name]";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                List<Classes.RatingClass> list = new List<Classes.RatingClass>();
                while (reader.Read())
                {
                    list.Add(new Classes.RatingClass
                    {
                        RatingId = 0,
                        CoachId = (int)reader["CoachId"],
                        AverageMark = (float)Math.Round(Convert.ToSingle(reader["Mark"]),2),
                        LastName = reader["Last Name"] as string ?? "",
                        Name = reader["Name"] as string ?? "",
                        MiddleName = reader["Middle Name"] as string ?? ""
                    });
                }
                List<Classes.RatingClass> sortedValues = list.OrderByDescending(value => value.AverageMark).ToList();
                for(int i=0;i<sortedValues.Count;i++)
                {
                    sortedValues[i].Number = i + 1;
                    ratings.Add(sortedValues[i]);
                }
                RatingsListView.ItemsSource = ratings;
            }
        }
        private void DeleteCoachButton_Click(object sender, RoutedEventArgs e)
        {
            if (RatingsListView.SelectedItem is Classes.RatingClass selected)
            {
                MessageBoxResult result = MessageBox.Show($"Видалити тренера з рейтингу?", "Підтвердження", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM [Table_Ratings] WHERE CoachId=@CoachId";
                        SqlCommand cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@CoachId", selected.CoachId);
                        cmd.ExecuteNonQuery();
                    }
                    LoadRating();
                }
            }
        }

        private void RatingsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isItemSelected = RatingsListView.SelectedItem is Classes.RatingClass;
            DeleteButton.IsEnabled = isItemSelected;
        }
    }
}
