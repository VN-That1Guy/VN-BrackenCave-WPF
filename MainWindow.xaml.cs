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
using System.Windows.Threading;

namespace VN_BrackenCave_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    /*
     * Project Bracken
     * Vinh Nguyen, December 2, 2024
     * Credits
     * - Timer code and page navigation from the PROG 201 Navigation Demo
     */
    public partial class MainWindow : Window
    {
        public Game game = new Game();
        public MainWindow()
        {
            InitializeComponent();
            NameBox.Text = "Name Here";
            ProceedButton.IsEnabled = false;
        }

        private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NameBox.Text.Length > 0 && (NameBox.Text != "" || NameBox.Text != "Name Here"))
            {
                ProceedButton.IsEnabled = true;
            }
            else
                ProceedButton.IsEnabled = false;
        }

        private void NameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (NameBox.Text == "")
            {
                NameBox.Text = "Name Here";
                ProceedButton.IsEnabled = false;
            }    
        }


        private void ProceedButton_Click(object sender, RoutedEventArgs e)
        {
            if (NameBox.Text != "")
                game.Client.Name = NameBox.Text;
            NavigationFrame.Navigate(new GameMenu());
        }

        private void ProceedButton_Initialized(object sender, EventArgs e)
        {
            ProceedButton.IsEnabled = false;
        }

        private void NameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (NameBox.Text == "Name Here")
            {
                NameBox.Text = "";
            }
        }
    }
}