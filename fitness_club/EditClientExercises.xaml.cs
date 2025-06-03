using fitness_club.Classes;
using Microsoft.Data.SqlClient;
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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace fitness_club
{
    /// <summary>
    /// Interaction logic for EditClientExercises.xaml
    /// </summary>
    public partial class EditClientExercises : Window
    {
        private readonly ExerciseClass exercise;
        public EditClientExercises(ExerciseClass exerciseCopy)
        {
            InitializeComponent();
            exercise = exerciseCopy;
            LoadExerciseTypes();
            ExerciseNameTextBox.Text = exercise.NameOfExercise;
            foreach(ComboBoxItem item in ExerciseTypeComboBox.Items)
            {
                if (item.Content.ToString() == exercise.Type)
                {
                    ExerciseTypeComboBox.SelectedItem = item;
                    break;
                
                }
            }
            RepetitionsTextBox.Text = exercise.Repetitions.ToString();
        }
        private void LoadExerciseTypes()
        {
            Classes.LoadExerciseTypesClass.LoadExerciseTypesIntoComboBox(ExerciseTypeComboBox);
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = ExerciseNameTextBox.Text.Trim();
            string? type = (ExerciseTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string repetitionsText = RepetitionsTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type) || string.IsNullOrEmpty(repetitionsText))
            {
                MessageBox.Show("Заповніть усі поля.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(repetitionsText, out int repetitions) || repetitions < 0)
            {
                MessageBox.Show("Кількість повторень вправи має бути додатним числом.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            exercise.NameOfExercise = name;
            exercise.Type = type;
            exercise.Repetitions = repetitions;
            DialogResult = true;
            Close();
        }
    }
}
