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


            // Example messages in the meantime
            lstMessages.Items.Add("Water outage in Zone A resolved");
            lstMessages.Items.Add("Road maintenance scheduled for Main Street");
            lstMessages.Items.Add("Electricity outage expected tomorrow in Sector 3");
        }
    }
}
