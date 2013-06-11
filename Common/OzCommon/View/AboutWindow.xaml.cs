using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace OzCommon.View
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow(string productName)
        {
            InitializeComponent();
            TBCK_ProductName.Text = productName;
        }

        private void AboutLinksRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }

        void OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
