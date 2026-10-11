using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using H.NotifyIcon.Core;
using Microsoft.UI.Xaml.Controls;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Shirase.Views
{
    [ObservableObject]
    public sealed partial class TrayIconControl : UserControl
    {
        public TrayIconControl()
        {
            InitializeComponent();
        }

        [RelayCommand]
        public void ShowWindow()
        {
            var window = App.MainWindow;
            if (window == null)
            {
                return;
            }

            if (!window.Visible) { 
                window.Show(); 
            }
        }

        [RelayCommand]
        public void AppExit()
        {
            App.HandleClosedEvents = false;
            TrayIcon.Dispose();
            App.MainWindow?.Close();
        }
    }
}
