using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Navigation;
using ForecastClient.Model.API;
using ForecastClient.Model.Object;

namespace ForecastClient
{
    public partial class MainWindow : Window
    {
        ServiceResponse serviceReponseCurrent;
        Current current;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void CurrentButton_Click(object sender, RoutedEventArgs e)
        {
            string location = this.location_textBox.Text;

            if (!string.IsNullOrEmpty(location))
            {
                serviceReponseCurrent = await CurrentService.GetCurrentWeather(location.ToLower());
                current = serviceReponseCurrent.Current;

                if (!serviceReponseCurrent.Error)
                {
                    if (current != null)
                    {
                        ShowCurrentWindow(current);
                    }
                    else
                    {
                        MessageBox.Show(serviceReponseCurrent.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show(serviceReponseCurrent.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Select an exchange rate you wish to convert", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ForecastButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ShowCurrentWindow(Current current)
        {
            CurrentWindow currentWindow = new CurrentWindow();
            currentWindow.ConfigureWindow(current);
            currentWindow.Show();
            this.Close();
        }

        private void ShowForecastWindow(Current current)
        {
            ForecastWindow forecastWindow = new ForecastWindow();
            forecastWindow.ConfigureWindow(current);
            forecastWindow.Show();
            this.Close();
        }
    }
}
