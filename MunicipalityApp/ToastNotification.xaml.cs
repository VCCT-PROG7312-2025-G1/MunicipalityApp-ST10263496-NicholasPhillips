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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MunicipalityApp
{
    /// <summary>
    /// Window for displaying a temporary toast notification message.
    /// </summary>
    public partial class ToastNotification : Window
    {
        // Constructor: Initializes the toast notification with a message
        public ToastNotification(string message)
        {
            InitializeComponent();
            txtMessage.Text = message;

            // Position the toast at the bottom-right corner of the screen
            this.Left = SystemParameters.WorkArea.Width - this.Width - 10;
            this.Top = SystemParameters.WorkArea.Height - this.Height - 10;

            // Create a timer to automatically close the toast after 3 seconds
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                this.Close();
            };
            timer.Start();
            
        }
    }
}
