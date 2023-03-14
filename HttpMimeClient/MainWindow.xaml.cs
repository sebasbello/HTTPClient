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
using Microsoft.Web.WebView2.Wpf;
using MimeTypes;

namespace HttpMimeClient
{
    public partial class MainWindow : Window
    {
        string content_type = "";
        Stream content_binary = null;
        string content_text = "";
        string extension = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_url.Text))
            {
                this.raw_radioButton.IsChecked = false;
                this.pretty_radioButton.IsChecked = false;
                this.tab_control.SelectedIndex = 0;
                this.tab_body.IsEnabled = false;
                this.bdr_indicator.BorderBrush = null;
                this.txt_headers.Text = "";
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
                    content_text = await response.Content.ReadAsStringAsync();
                    content_binary = await response.Content.ReadAsStreamAsync();
                    content_type = response.Content.Headers.ContentType.ToString();
                    this.lbl_content_type.Content = content_type;

                    extension = MimeTypeMap.GetExtension(content_type.Split(';')[0]);
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
            txt_block.Text = content_text;

            this.tab_body.IsEnabled = true;
            this.sview_body.Content = null;
            this.sview_body.Content = txt_block;
        }

        private void pretty_radioButtonChecked(object sender, RoutedEventArgs e)
        {
            if (content_type.StartsWith("text/html"))
            {
                WebView2 webView2 = new WebView2();
                webView2.Source = new Uri(txt_url.Text);

                this.tab_body.IsEnabled = true;
                this.sview_body.Content = null;
                this.sview_body.Content = webView2;
            }
        }
    }
}
