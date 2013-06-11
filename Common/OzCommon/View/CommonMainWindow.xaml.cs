using System.Windows;

namespace OzCommon.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CommonMainWindow : Window, IWindow
    {
        public CommonMainWindow()
        {
            InitializeComponent();
        }

        public void ShowWindow()
        {
            Activate();
            ShowInTaskbar = true;
            Show();
            WindowState = WindowState.Normal;
            Focus();
        }

        void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
