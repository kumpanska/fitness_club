using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace fitness_club
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void SandwichButton_Click(object sender, RoutedEventArgs e)
        {
            LeftPanelStack.HorizontalAlignment = HorizontalAlignment.Left;
            LeftPanelColumn.Width = new GridLength(1, GridUnitType.Star);
            RightPanelColumn.Width = new GridLength(1, GridUnitType.Star);
            RightPanel.Visibility = Visibility.Visible;
            SandwichButton.Visibility = Visibility.Hidden;
            BackButton.Visibility = Visibility.Visible;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            LeftPanelStack.HorizontalAlignment = HorizontalAlignment.Center;
            LeftPanelColumn.Width = new GridLength(1, GridUnitType.Star);
            RightPanelColumn.Width = new GridLength(0);
            RightPanel.Visibility = Visibility.Collapsed;
            SandwichButton.Visibility = Visibility.Visible;
            BackButton.Visibility = Visibility.Hidden;
        }

    }
}