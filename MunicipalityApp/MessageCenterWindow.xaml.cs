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

namespace MunicipalityApp
{

    // Displays informational messages to the user in a simple list.
    // Intended as a central place for updates and alerts.

    public partial class MessageCenterWindow : Window
    {

        // Initializes the message center window and populates example messages.
        public MessageCenterWindow()
        {
            InitializeComponent();

            // Subscribe to notifications from the NotificationManager
            MunicipalityApp.Services.NotificationManager.MessagePublished += OnMessagePublished;

            // Optionally show a short welcome message
            lstMessages.Items.Add("Message center initialized.");
        }

        private void OnMessagePublished(string message)
        {
            // Ensure we update UI on the dispatcher
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(() => OnMessagePublished(message)));
                return;
            }

            lstMessages.Items.Insert(0, message);
            // Keep list length reasonable
            while (lstMessages.Items.Count > 200) lstMessages.Items.RemoveAt(lstMessages.Items.Count - 1);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            // Unsubscribe to avoid leaks
            try { MunicipalityApp.Services.NotificationManager.MessagePublished -= OnMessagePublished; } catch { }
        }
    }
}
