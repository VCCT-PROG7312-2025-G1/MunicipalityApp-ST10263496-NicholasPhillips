using System.Windows;

namespace MunicipalityApp
{
	/// <summary>
	/// Interaction logic for StatusReportWindow.xaml
	/// This window is kept for backward-compatibility but is marked as deprecated in the UI.
	/// </summary>
	public partial class StatusReportWindow : Window
	{
		public StatusReportWindow()
		{
			InitializeComponent();
		}

		private void btnClose_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
