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
    public sealed partial class DeviceRegistrationPage : Page
    {
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        public DeviceRegistrationPage()
        {
            this.InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string deviceName = DeviceNameTextBox.Text;

            Task.Run(async () =>
            {
                await dispatcherQueue.EnqueueAsync(async () =>
                {
                    try
                    {
                        RegisterProgressBar.Visibility = Visibility.Visible;
                        ErrorTextBlock.Visibility = Visibility.Collapsed;

                        string secret = App.SessionService.Secret;

                        var response = await Pushover.Client.DeviceRegistration(secret, deviceName);

                        App.SessionService.DeviceId = response.Id;

                        Frame.Navigate(typeof(MessagePage));
                    }
                    catch (DeviceNameAlreadyTakenException)
                    {
                        RegisterProgressBar.ShowError = true;
                        ErrorTextBlock.Visibility = Visibility.Visible;
                        ErrorTextBlock.Text = "name has already been taken";
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        ErrorTextBlock.Visibility = Visibility.Visible;
                        ErrorTextBlock.Text = ex.Message;
                    }
                    finally
                    {
                        RegisterProgressBar.Visibility = Visibility.Collapsed;
                    }
                });
            });
        }
    }
}
