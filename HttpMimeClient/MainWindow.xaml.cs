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
using System.Windows.Navigation;
using System.Windows.Shapes;
using HttpMimeClient.Model;
using System.Net.Mime;
using System.IO;

namespace HttpMimeClient
{
    public partial class MainWindow : Window
    {

        string txt_body = "";

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
                this.lbl_statuscode.Content = "";
                string content_type = "";
                Stream content_binary = null;

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
                        this.raw_radioButton.IsEnabled = true;
                        this.pretty_radioButton.IsEnabled = true;
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
                    txt_body = await response.Content.ReadAsStringAsync();
                    content_binary = await response.Content.ReadAsStreamAsync();
                    content_type = response.Content.Headers.ContentType.ToString();
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

        private void raw_radioButtonChecked(object sender, RoutedEventArgs e)
        {
            TextBlock txt_block = new TextBlock();
            txt_block.TextWrapping = TextWrapping.Wrap;
            txt_block.Text = txt_body;

            this.body.IsEnabled = true;
            this.body.Content = txt_block;
        }
    }
}
