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
using ResponseClient.Model;
using System.Net.Mime;
using System.IO;
using Microsoft.Web.WebView2.Wpf;
using MimeTypes;
using Microsoft.Win32;
using HeyRed.Mime;

namespace ResponseClient
{
    public partial class MainWindow : Window
    {
        string content_type = "";
        Stream content_binary = null;
        Image image = null;
        string content_text = "";
        string mimeExtension = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
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
                    mimeExtension = "*." + MimeTypesMap.GetExtension(content_type);
                    content_binary.Position = 0;
                    Byte[] buffer = new byte[content_binary.Length];
                    content_binary.Read(buffer, 0, buffer.Length);

                    image = new Image();
                    image.Source = ConvertByteArrayToBitmapImage(buffer);
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

        private void RawRadioButton_Check(object sender, RoutedEventArgs e)
        {
            TextBlock txt_block = new TextBlock();
            txt_block.TextWrapping = TextWrapping.Wrap;
            txt_block.Text = content_text;

            this.tab_body.IsEnabled = true;
            this.sview_body.Content = null;
            this.sview_body.Content = txt_block;
        }

        private void PrettyRadioButton_Check(object sender, RoutedEventArgs e)
        {
            if (content_type.StartsWith("text/html"))
            {
                WebView2 webView2 = new WebView2();
                webView2.Source = new Uri(txt_url.Text);

                this.tab_body.IsEnabled = true;
                this.sview_body.Content = null;
                this.sview_body.Content = webView2;
            }
            else if (content_type.StartsWith("image"))
            {
                this.tab_body.IsEnabled = true;
                this.sview_body.Content = null;
                this.sview_body.Content = image;
            }
        }

        public BitmapImage ConvertByteArrayToBitmapImage(Byte[] bytes)
        {
            Stream strem = new MemoryStream(bytes);
            strem.Seek(0, SeekOrigin.Begin);
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = strem;
            image.EndInit();
            return image;
        }

        public void Save(SaveFileDialog saveFileDialog)
        {
            string fileName = saveFileDialog.FileName;
            if (fileName != null)
            {
                if (content_type.StartsWith("text/html"))
                {
                    StreamWriter streamWriter = new StreamWriter(File.Create(saveFileDialog.FileName));
                    streamWriter.Write(content_text);
                    streamWriter.Dispose();
                }
                else if (content_type.StartsWith("image"))
                {
                    var pngBitmapEncoder = new PngBitmapEncoder();
                    pngBitmapEncoder.Frames.Add(BitmapFrame.Create((BitmapSource)image.Source));
                    using (FileStream stream = new FileStream(fileName, FileMode.Create))
                    {
                        pngBitmapEncoder.Save(stream);
                    }
                }
                else
                {
                    MessageBox.Show("File extension not expected. Please, try again.", "Warning",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save",
                Filter = (content_type.StartsWith("text/html")) ? "(*.html)|*.html" : "(" + mimeExtension + ")|" + mimeExtension,
                FilterIndex = 0
            };
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != null)
            {
                Save(saveFileDialog);
            }
        }
    }
}
