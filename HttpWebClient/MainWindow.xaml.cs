using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
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
using HttpWebClient.Model;
using Client = HttpWebClient.Model.Client;

namespace HttpWebClient
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_url.Text))
            {
                this.bdr_indicator.BorderBrush = null;
                this.txt_headers.Text = "";
                this.txt_body.Text = "";
                this.lbl_statuscode.Content = "";
                try
                {
                    if (!txt_url.Text.ToLower().StartsWith("http") && !txt_url.Text.ToLower().StartsWith("https"))
                    {
                        txt_url.Text = "https://" + txt_url.Text;
                    }
                    this.bdr_indicator.BorderBrush = new SolidColorBrush(Colors.Blue);
                    this.lbl_statuscode.Content = "Processing request...";

                    HttpResponseMessage response = await Client.executeRequest(txt_url.Text, cbx_method.Text);
                    HttpStatusCode statusCode = response.StatusCode;
                    this.lbl_statuscode.Content = "" + (int)statusCode + " - " + statusCode.ToString();

                    if (response.IsSuccessStatusCode)
                    {
                        this.bdr_indicator.BorderBrush = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        this.bdr_indicator.BorderBrush = new SolidColorBrush(Colors.Red);
                    }

                    this.txt_headers.Text += "****** General/Response: ******\r\n\r\n";
                    this.txt_headers.Text += response.Headers.ToString();
                    this.txt_headers.Text += "\r\n";
                    this.txt_headers.Text += "****** Entity: ******\r\n\r\n";
                    this.txt_headers.Text += response.Content.Headers.ToString();
                    this.txt_body.Text = await response.Content.ReadAsStringAsync();
                    ShowHtmlContentType(response);
                }
                catch (Exception exception)
                {
                    this.bdr_indicator.BorderBrush = new SolidColorBrush(Colors.Red);
                    this.lbl_statuscode.Content = exception.Message;
                }
            }
            else
            {
                MessageBox.Show("Add URL for search.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ShowHtmlContentType(HttpResponseMessage response)
        {
            if (response.Content.Headers.ContentType.ToString().StartsWith("text/html"))
            {
                content.Visibility = Visibility.Visible;
                webView.Source = new Uri(txt_url.Text);
            }
        }
    }
}
