using Shirase.Pushover.Models;
using Shirase.Pushover.Responses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Shirase.Pushover
{
    public static class Client
    {
        private static readonly HttpClient _client = new();

        private static readonly ClientWebSocket _clientWS = new();

        public static async Task<UserLoginResponse> UserLogin(string email, string password, string twofa = null)
        {
            MultipartFormDataContent content = new MultipartFormDataContent()
            {
                { new StringContent(email), "email" },
                { new StringContent(password), "password" }
            };

            if (twofa != null)
            {
                content.Add(new StringContent(twofa), "twofa");
            }

            var response = await _client.PostAsync("https://api.pushover.net/1/users/login.json", content);
            switch (response.StatusCode)
            {
                case HttpStatusCode.PreconditionFailed:
                    {
                        var message = await response.Content.ReadFromJsonAsync<TwoFactorRequiredErrorResponse>();
                        throw new TwoFactorRequiredException(message.Totp);
                    }
                case HttpStatusCode.OK:
                    return await response.Content.ReadFromJsonAsync<UserLoginResponse>();
                default:
                    {
                        var message = await response.Content.ReadAsStringAsync();
                        throw new Exception(message);
                    }
            }
        }

        public static async Task<DeviceRegistrationResponse> DeviceRegistration(string secret, string name)
        {
            MultipartFormDataContent content = new()
            {
                { new StringContent(secret), "secret" },
                { new StringContent(name), "name" },
                { new StringContent("O"), "os" },
            };

            var response = await _client.PostAsync("https://api.pushover.net/1/devices.json", content);
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    {
                        var message = await response.Content.ReadFromJsonAsync<DeviceNameTakenErrorResponse>();
                        throw new DeviceNameAlreadyTakenException(message.Errors.Name.ToString());
                    }
                case HttpStatusCode.OK:
                    return await response.Content.ReadFromJsonAsync<DeviceRegistrationResponse>();
                default:
                    {
                        var message = await response.Content.ReadAsStringAsync();
                        throw new Exception(message);
                    }
            }
        }

        public static async Task<MessageDownloadResponse> MessageDownload(string secret, string deviceId)
        {
            var response = await _client.GetAsync($"https://api.pushover.net/1/messages.json?secret={secret}&device_id={deviceId}");

            return await response.Content.ReadFromJsonAsync<MessageDownloadResponse>();
        }

        public static async Task<BaseResponse> MessageDelete(string secret, string deviceId, long messageId)
        {
            MultipartFormDataContent content = new()
            {
                { new StringContent(secret), "secret" },
                { new StringContent($"{messageId}"), "message" },
            };

            var response = await _client.PostAsync($"https://api.pushover.net/1/devices/{deviceId}/update_highest_message.json", content);

            return await response.Content.ReadFromJsonAsync<BaseResponse>();
        }

        public static async Task ConnectWebsocket(string secret, string deviceId, IProgress<Event> progress, CancellationToken token)
        {
            await _clientWS.ConnectAsync(new Uri("wss://client.pushover.net/push"), token);
            await _clientWS.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"login:{deviceId}:{secret}\n")), WebSocketMessageType.Text, true, token);
            Debug.WriteLine("Websocket is " + _clientWS.State);
            progress.Report(Event.Connected);
        }

        public static async Task ListenForNotifications(IProgress<Event> progress, CancellationToken cancellationToken)
        {
            while (_clientWS.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var buffer = new byte[1];

                var result = await _clientWS.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                if (result.MessageType == WebSocketMessageType.Close) {
                    await _clientWS.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cancellationToken);
                    break;
                }

                var ev = (char) buffer[0] switch {
                    '#' => Event.Keepalive,
                    '!' => Event.NewMessage,
                    'R' => Event.ReloadRequest,
                    'E' => Event.PermanentError,
                    'A' => Event.SessionClosed,
                    _ => throw new Exception("Unrecognized payload")
                };

                progress.Report(ev);
            }

            progress.Report(Event.Disconnected);

            Debug.WriteLine("Websocket is " + _clientWS.State);
        }
    }
}
