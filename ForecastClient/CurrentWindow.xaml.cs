using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
using ForecastClient.Model.Object;

namespace ForecastClient
{
    public partial class CurrentWindow : Window
    {
        private Current current;
        public CurrentWindow()
        {
            InitializeComponent();
        }

        public void ConfigureWindow(Current current)
        {
            this.current = current;
        }
    }
}
