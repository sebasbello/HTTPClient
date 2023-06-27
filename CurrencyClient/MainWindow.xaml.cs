using CurrencyClient.Model.API;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CurrencyClient
{
    public partial class MainWindow : Window
    {
        ServiceResponse serviceResponseSimple;
        ServiceResponse serviceResponseInternational;
        Dictionary<String, Double> exchangeRates;
        Dictionary<String, String> currencies;

        public MainWindow()
        {
            InitializeComponent();
            SetComboBox();
        }

        private void ConvertSimple_ClickButton(object sender, RoutedEventArgs e)
        {
            Double exchangeRateMxn = 0.0;
            Double amount;

            if (!string.IsNullOrEmpty(this.txt_amount_simple.Text))
            {
                amount = Double.Parse(this.txt_amount_simple.Text);

                exchangeRates = serviceResponseSimple.ExchangeRate.Rates; 

                if (!serviceResponseSimple.Error)
                {
                    if (serviceResponseSimple.ExchangeRate != null && serviceResponseSimple.ExchangeRate.Rates != null
                        && serviceResponseSimple.ExchangeRate.Rates.TryGetValue("MXN", out exchangeRateMxn))
                    {
                        if (this.mxn_radioButton.IsChecked == true)
                        {
                            this.txt_conversion.Text = String.Format("{0:#,##0.00}", amount / exchangeRateMxn);
                        }
                        else
                        {
                            this.txt_conversion.Text = String.Format("{0:#,##0.00}", amount * exchangeRateMxn);
                        }
                        long updateTime = serviceResponseSimple.ExchangeRate.Timestamp;
                        DateTimeOffset.Now.ToUnixTimeSeconds();
                        this.txt_timezone_simple.Text = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(updateTime).ToString("dd/MM/yyyy HH:mm:ss");
                        this.txt_exchange_simple.Text = String.Format("{0:#,##0.00}", exchangeRateMxn);
                    }
                    else
                    {
                        MessageBox.Show(serviceResponseSimple.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show(serviceResponseSimple.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Write valid amount to convert", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ConvertInternational_ClickButton(object sender, RoutedEventArgs e)
        {
            String from_currency = "";
            String to_currency = "";
            Double from_exchangeRate = 0.0;
            Double to_exchangeRate = 0.0;
            Double exchangeRate_relation = 0.0;
            Double amount = 0.0;

            if (this.cbx_from.SelectedIndex != this.cbx_to.SelectedIndex && !string.IsNullOrEmpty(this.txt_amount_international.Text))
            {
                amount = Double.Parse(this.txt_amount_international.Text);
                from_currency = ((KeyValuePair<string, string>)this.cbx_from.SelectedValue).Key.ToString();
                to_currency = ((KeyValuePair<string, string>)this.cbx_to.SelectedValue).Key.ToString();

                if (!serviceResponseInternational.Error)
                {
                    if (serviceResponseInternational.Currencies != null 
                        && serviceResponseSimple.ExchangeRate.Rates.TryGetValue(from_currency, out from_exchangeRate)
                        && serviceResponseSimple.ExchangeRate.Rates.TryGetValue(to_currency, out to_exchangeRate))
                    {
                        exchangeRate_relation = from_exchangeRate / to_exchangeRate;
                        this.txt_result.Text = String.Format("{0:#,##0.00}", amount / exchangeRate_relation);

                        long updateTime = serviceResponseSimple.ExchangeRate.Timestamp;
                        DateTimeOffset.Now.ToUnixTimeSeconds();
                        this.txt_timezone_international.Text = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(updateTime).ToString("dd/MM/yyyy HH:mm:ss");
                        this.txt_exchange_international.Text = String.Format("{0:#,##0.00}", exchangeRate_relation);
                    }
                    else
                    {
                        MessageBox.Show(serviceResponseInternational.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show(serviceResponseInternational.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Select an exchange rate you wish to convert", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void SetComboBox()
        {
            serviceResponseInternational = await CurrencyService.GetCurrencies();
            serviceResponseSimple = await ExchangeRateService.GetExchangeRateConversion();

            if (!serviceResponseInternational.Error)
            {
                this.cbx_from.ItemsSource = serviceResponseInternational.Currencies;
                this.cbx_to.ItemsSource = serviceResponseInternational.Currencies;
                currencies = serviceResponseInternational.Currencies;

                this.cbx_from.SelectedIndex = 0;
                this.cbx_to.SelectedIndex = 1;
            }
            else
            {
                MessageBox.Show("Currencies not loaded properly", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Switch_ClickButton(object sender, RoutedEventArgs e)
        {
            int change = cbx_from.SelectedIndex;
            this.cbx_from.SelectedIndex = this.cbx_to.SelectedIndex;
            this.cbx_to.SelectedIndex = change;
        }
    }
}
