using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace fitness_club.Classes
{
    public static class LoadExerciseTypesClass
    {
        private const string connectionString = "Server=DESKTOP-K1I43VD;Database=master;TrustServerCertificate=True;Trusted_Connection=True";
        public static void LoadExerciseTypesIntoComboBox(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT DISTINCT [Type] FROM [Table_Exercises] WHERE [Type] IS NOT NULL AND [Type] <> ''";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string? type = reader["Type"]?.ToString();
                            if (!string.IsNullOrWhiteSpace(type))
                            {
                                comboBox.Items.Add(new ComboBoxItem { Content = type });
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

    }
}
