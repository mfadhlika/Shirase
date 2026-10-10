using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Shirase.Pushover;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Shirase.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        public LoginPage()
        {
            this.InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordTextBox.Password;
            string twofa = null;
            if (TwoFactorTextBox.Visibility == Visibility.Visible)
            {
                twofa = TwoFactorTextBox.Text;
            }

            Task.Run(async () =>
            {
                await dispatcherQueue.EnqueueAsync(async () =>
                {
                    try
                    {
                        LoginProgressBar.Visibility = Visibility.Visible;

                        var response = await Pushover.Client.UserLogin(email, password, twofa);

                        App.SessionService.Secret = response.Secret;

                        Frame.Navigate(typeof(DeviceRegistrationPage));
                    }
                    catch (TwoFactorRequiredException)
                    {
                        TwoFactorTextBox.Visibility = Visibility.Visible;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    finally
                    {
                        LoginProgressBar.Visibility = Visibility.Collapsed;
                    }
                });
            });
        }
    }
}
