using System.Windows;

namespace MunicipalityApp.Services
{
    // A simple navigation manager that keeps a single MainWindow instance and
    // provides helper methods to show it and navigate to other windows.
    public static class NavigationManager
    {
        private static MainWindow? _mainWindow;

        public static void RegisterMainWindow(MainWindow main)
        {
            _mainWindow = main;
        }

        public static void ShowMainWindow()
        {
            if (_mainWindow == null)
            {
                // create and register a new main window if none exists
                _mainWindow = new MainWindow();
                _mainWindow.Show();
                _mainWindow.Activate();
                return;
            }

            if (!_mainWindow.IsVisible)
                _mainWindow.Show();

            _mainWindow.Activate();
        }

        // Navigate from the main window to another window. The main window will be hidden.
        // The target window will restore the main window when it closes.
        public static void NavigateTo(Window target)
        {
            if (_mainWindow != null)
            {
                _mainWindow.Hide();
            }

            // When target closes, ensure main is shown again.
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
