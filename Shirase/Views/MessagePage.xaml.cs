using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Shirase.Pushover.Models;
using Shirase.Services;
using Shirase.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Shirase.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MessagePage : Page, IDisposable
    {
        public MessagePageViewModel ViewModel { get; } = new MessagePageViewModel();

        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private bool _shouldReconnect = false;

        private readonly IProgress<Event> progress;
        public MessagePage()
        {
            this.InitializeComponent();

            progress = new Progress<Event>(OnPushoverMessage);
            ConnectWebsocket();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is IDictionary<string, string> args)
            {
                if (args.TryGetValue("messageId", out string messageId))
                {
                    ViewModel.SelectedMessage = ViewModel.Messages.First(m => m.IdStr == messageId);
                }
                else if (args.TryGetValue("action", out string action))
                {
                    switch(action)
                    {
                        case "openUrl":
                            {
                                _ = Windows.System.Launcher.LaunchUriAsync(new Uri(args["url"]));
                                break;
                            }
                    }
                }
            }
        }

        private void ConnectWebsocket()
        {
            _ = Task.Run(() => Pushover.Client.ConnectWebsocket(App.SessionService.Secret, App.SessionService.DeviceId, progress, _cancellationTokenSource.Token));
        }

        private void ListenWebsocket()
        {
            _ = Task.Run(() => Pushover.Client.ListenForNotifications(progress, _cancellationTokenSource.Token));
        }

        private void OnPushoverMessage(Event ev)
        {
            Debug.WriteLine("Received: " + ev);
            switch (ev)
            {
                case Event.Connected:
                    {
                        Debug.WriteLine("Websocket connected");
                        ListenWebsocket();
                        break;
                    }
                case Event.Disconnected:
                    {
                        if (_shouldReconnect)
                        {
                            _shouldReconnect = false;
                            ConnectWebsocket();
                        }
                        break;
                    }
                case Event.Keepalive:
                    {
                        break;
                    }
                case Event.NewMessage:
                    {
                        ViewModel.SyncMessages(App.SessionService.Secret, App.SessionService.DeviceId);
                        break;
                    }
                case Event.ReloadRequest:
                    {
                        _shouldReconnect = true;
                        _cancellationTokenSource.Cancel();
                        break;
                    }
                case Event.PermanentError:
                    {
                        Frame.Navigate(typeof(LoginPage));
                        break;
                    }
                case Event.SessionClosed:
                    {
                        _shouldReconnect = false;
                        _cancellationTokenSource.Cancel();
                        break;
                    }
            }
        }

        public void Dispose()
        {
        }
    }
}
