using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking.Sockets;

namespace Shirase.Tasks
{
    public sealed class RealTimeNotificationTask : IBackgroundTask
    {
        private BackgroundTaskDeferral defferal;
        private MessageWebSocket messageWebSocket;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("RealtimeNotificationTask started");

            defferal = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskInstance_Canceled;

            try
            {
                if (messageWebSocket == null)
                {
                    messageWebSocket = new MessageWebSocket();
                    messageWebSocket.Control.MessageType = SocketMessageType.Utf8;
                    messageWebSocket.MessageReceived += MessageWebSocket_MessageReceived;
                    messageWebSocket.Closed += MessageWebSocket_Closed;

                    Uri uri = new Uri("wss://client.pushover.net/push");
                    await messageWebSocket.ConnectAsync(uri);
                    Debug.WriteLine("Websocket connected");

                    var keepAliveInterval = 300;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void MessageWebSocket_Closed(IWebSocket sender, WebSocketClosedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void MessageWebSocket_MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            throw new System.NotImplementedException();
        }
    }
}
