using HttpApiClient.Model.API;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HttpApiClient
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox()
        {

        }

        private async void convert_ClickButton(object sender, RoutedEventArgs e)
        {
            Double exchangeRateMxn = 0.0;
            Double amount;
            if (!string.IsNullOrEmpty(this.txt_amount.Text))
            {
                amount = Double.Parse(this.txt_amount.Text);

                ServiceResponse serviceResponse = await ExchangeRateService.GetExchangeRateConversion();

                if (!serviceResponse.Error)
                {
                    if (serviceResponse.ExchangeRate != null && serviceResponse.ExchangeRate.Rates != null
                        && serviceResponse.ExchangeRate.Rates.TryGetValue("MXN", out exchangeRateMxn))
                    {
                        if (this.mxn_radioButton.IsChecked == true)
                        {
                            this.txt_conversion.Text = String.Format("{0:#,##0.00}", amount / exchangeRateMxn);
                        }
                        else
                        {
                            this.txt_conversion.Text = String.Format("{0:#,##0.00}", amount * exchangeRateMxn);
                        }
                        long updateTime = serviceResponse.ExchangeRate.Timestamp;
                        DateTimeOffset.Now.ToUnixTimeSeconds();
                        this.txt_timezone.Text = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(updateTime).ToString("dd/MM/yyyy HH:mm:ss");
                        this.txt_exchange.Text = String.Format("{0:#,##0.00}", exchangeRateMxn);
                    }
                    else
                    {
                        MessageBox.Show(serviceResponse.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show(serviceResponse.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Write valid amount to convert", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
