using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace MunicipalityApp
{
    public partial class LandingPage : Window
    {
        public LandingPage()
        {
            InitializeComponent();
            Loaded += LandingPage_Loaded;
        }

        private async void LandingPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Wait for the logo animation to complete (1 second)
            await Task.Delay(1000);

            // Fade out the splash screen
            var fadeOut = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            fadeOut.Completed += (s, _) =>
            {
                // Show or create the registered MainWindow via NavigationManager
                MunicipalityApp.Services.NavigationManager.ShowMainWindow();

                // Close splash screen
                this.Close();
            };

            this.BeginAnimation(OpacityProperty, fadeOut);
        }
    }
}
