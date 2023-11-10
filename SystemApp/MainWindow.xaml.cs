using System;
using System.Windows;
using System.Windows.Interop;
using SystemApp.WinApi;

namespace SystemApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntPtr GetHandle() => new WindowInteropHelper(this).Handle;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbMessage.Text = "Warning!\r\nThe rental of this PC ends on 03/01/2024!";
            WinApiHelper.HideFromAltTab(GetHandle());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => e.Cancel = true;

        private void Window_SourceInitialized(object sender, EventArgs e) 
            => WinApiHelper.SetTransparentFormForDevices(GetHandle());
    }
}
