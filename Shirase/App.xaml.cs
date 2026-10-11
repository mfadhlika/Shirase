using H.NotifyIcon;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Shirase.Pushover;
using Shirase.Pushover.Models;
using Shirase.Services;
using Shirase.Views;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Shirase
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();

            m_window.Closed += (sender, args) =>
            {
                args.Handled = true;
                m_window.Hide();
            };

            Frame rootFrame = ((MainWindow)m_window).GetRootFrame();

            rootFrame.NavigationFailed += OnNavigationFailed;

            if (SessionService.Secret == "")
            {
                rootFrame.Navigate(typeof(LoginPage), args.Arguments);
            }
            else if (SessionService.DeviceId == "")
            {
                rootFrame.Navigate(typeof(DeviceRegistrationPage), args.Arguments);
            }
            else
            {
                rootFrame.Navigate(typeof(MessagePage), args.Arguments);
            }

            string secret = SessionService.Secret;
            string deviceId = SessionService.DeviceId;

            AppNotificationManager.Default.NotificationInvoked += OnNotificationInvoked;
            AppNotificationManager.Default.Register();

            var activatedArgs = AppInstance.GetCurrent().GetActivatedEventArgs();

            if (activatedArgs.Kind == ExtendedActivationKind.AppNotification)
            {
                // App was launched by clicking a notification
                var notificationArgs = (AppNotificationActivatedEventArgs)activatedArgs.Data;
                HandleNotification(notificationArgs);
            }
            else
            {
                // Normal launch
                m_window.Activate();
            }
        }

        private void OnNotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
        {
            HandleNotification(args);
        }

        private void HandleNotification(AppNotificationActivatedEventArgs args)
        {
            var action = args.Arguments.ContainsKey("action") ? args.Arguments["action"] : "(none)";
            var messageId = args.Arguments.ContainsKey("messageId") ? args.Arguments["messageId"] : "(none)";

            m_window!.DispatcherQueue.TryEnqueue(() =>
            {
                switch (action)
                {
                    case "BackgroundAction":
                        // Handle the action without showing the app window.
                        // If the window was never shown, exit the app.
                        if (!m_window.Visible)
                        {
                            Application.Current.Exit();
                        }
                        break;

                    default:
                        // Bring the app to the foreground and display the notification arguments.
                        m_window.Activate();
                        ((MainWindow)m_window).GetRootFrame().Navigate(typeof(MessagePage), args.Arguments);
                        break;
                }
            });
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        static Window? m_window;

        public static Window MainWindow
        {
            get {
                return m_window;
            }
        }

        static SessionService? sessionService;

        public static SessionService SessionService
        {
            get
            {
                sessionService ??= new SessionService();
                return sessionService;
            }
        }
    }
}
