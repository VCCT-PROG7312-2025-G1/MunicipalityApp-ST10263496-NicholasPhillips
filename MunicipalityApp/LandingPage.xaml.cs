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

            fadeOut.Completed += async (s, _) =>
            {
                // Open Main Window
                var main = new MainWindow();
                
                // Ensure the window is shown before activating it
                main.Show();
                main.Activate();
                
                // Close splash screen
                this.Close();
            };

            this.BeginAnimation(OpacityProperty, fadeOut);
        }
    }
}
