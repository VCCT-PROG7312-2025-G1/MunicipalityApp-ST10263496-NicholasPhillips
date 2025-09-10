using MunicipalityApp.Data;
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
    /// <summary>
    /// Interaction logic for ViewIssueWindow.xaml
    /// </summary>
    public partial class ViewIssueWindow : Window
    {
        public ViewIssueWindow()
        {
            InitializeComponent();
            // Create a snapshot of the queue for binding to avoid modification during enumeration
            dgIssues.ItemsSource = IssueRepository.Issues.ToArray();
        }
    }
}
