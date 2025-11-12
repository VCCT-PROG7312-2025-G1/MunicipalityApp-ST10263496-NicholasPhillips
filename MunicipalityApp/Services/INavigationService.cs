using System.Windows;

namespace MunicipalityApp.Services
{
    // Abstraction for navigation so it can be mocked or replaced in tests.
    public interface INavigationService
    {
        void RegisterMainWindow(Window main);
        void ShowMainWindow();
        void NavigateTo(Window target);
    }
}
