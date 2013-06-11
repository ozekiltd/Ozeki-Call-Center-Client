using System;
using System.Windows;

namespace OzCommon.View
{
    /// <summary>
    /// Interaction logic for WaitWindow.xaml
    /// </summary>
    public partial class WaitWindow : Window
    {

        public WaitWindow(string loadingMessage, Boolean isNonClosableWindow = false)
        {
            InitializeComponent();
            if (isNonClosableWindow)
                WindowStyle = WindowStyle.None;
            TBCK_LoadingMessage.Text = loadingMessage;
        }
    }
}
