using System.Windows;

namespace MunicipalityApp.Services
{
    // A simple navigation manager that keeps a single MainWindow instance and
    // provides helper methods to show it and navigate to other windows.
    public static class NavigationManager
    {
        // Backing implementation so callers can continue to use the existing static API.
        private static readonly INavigationService _impl = new NavigationService();

        // Expose the concrete instance as well in case advanced callers want to use DI or mocking later.
        public static INavigationService Instance => _impl;

        public static void RegisterMainWindow(MainWindow main) => _impl.RegisterMainWindow(main);
        public static void ShowMainWindow() => _impl.ShowMainWindow();
        public static void NavigateTo(Window target) => _impl.NavigateTo(target);
    }
}
