using System.Windows;

namespace MunicipalityApp
{
    /// <summary>
    /// Interaction logic for AnnouncementsWindow.
    /// Displays municipal announcements to the user.
    /// </summary>
    public partial class AnnouncementsWindow : Window
    {
        /// <summary>
        /// Initializes the AnnouncementsWindow and its components.
        /// </summary>
        public AnnouncementsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Back button click event.
        /// Returns to the main window and closes the current window.
        /// </summary>
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}
