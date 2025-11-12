using System.Windows;

namespace MunicipalityApp.Services
{
    // Concrete implementation of INavigationService. Behaves like the previous NavigationManager logic.
    public class NavigationService : INavigationService
    {
        private Window? _mainWindow;

        public void RegisterMainWindow(Window main)
        {
            _mainWindow = main;
        }

        public void ShowMainWindow()
        {
            if (_mainWindow == null)
            {
                _mainWindow = new MainWindow();
                _mainWindow.Show();
                _mainWindow.Activate();
                return;
            }

            if (!_mainWindow.IsVisible)
                _mainWindow.Show();

            _mainWindow.Activate();
        }

        public void NavigateTo(Window target)
        {
            if (_mainWindow != null)
            {
                _mainWindow.Hide();
            }

            target.Closed += (s, e) =>
            {
                if (_mainWindow != null)
                {
                    _mainWindow.Show();
                    _mainWindow.Activate();
                }
            };

            target.Show();
        }
    }
}
