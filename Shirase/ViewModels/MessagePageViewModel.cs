using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Shirase.Common;
using Shirase.Pushover.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shirase.ViewModels
{
    public partial class MessagePageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<App> AppLatestMessages
        {
            get
            {
                var query = database.Query<Message>("messages")
                    .GroupBy("{ App: $.App, Icon: $.Icon }")
                    .Select("{ Key: @key, Latest: FIRST(SORT(* => @.Date, 'desc'))  }")
                    .ToEnumerable();
                ObservableCollection<App> list = [new App("Latest Messages", "pushover", 0, ""), .. query.Select(x => {
                    var key = x["Key"].AsDocument;
                    var latest = x["Latest"].AsDocument;
                    return new App(key["App"].AsString, key["Icon"].AsString, latest["Date"].AsInt32, latest["Content"].AsString);
                }).ToList()];
                return list;
            }
        }

        public ObservableCollection<Message> Messages
        {
            get
            {
                var query = database.Query<Message>("messages");
                if (SelectedApp.Name != "Latest Messages")
                {
                    query = query.Where(x => x.App == SelectedApp.Name);
                }

                return [.. query.OrderByDescending(m => m.Date).ToList()];
            }
        }

        private Message _selectedMessage;

        public Message SelectedMessage
        {
            get => _selectedMessage;
            set
            {
                if (_selectedMessage == value)
                {
                    return;
                }

                _selectedMessage = value;
                OnPropertyChanged(nameof(SelectedMessage));
            }
        }

        private App _selectedApp = new("Latest Messages", "", 0, "");

        public App SelectedApp
        {
            get => _selectedApp;
            set
            {
                if (_selectedApp == value)
                {
                    return;
                }

                _selectedApp = value;
                OnPropertyChanged(nameof(SelectedApp));
                OnPropertyChanged(nameof(Messages));
            }
        }

        private readonly LocalDatabase database = new();

        public MessagePageViewModel()
        {
        }

        private static async Task<List<Message>> LoadNewMessages(string secret, string deviceId)
        {
            var response = await Pushover.Client.MessageDownload(secret, deviceId);

            return response.Messages;
        }

        public async void SyncMessages(string secret, string deviceId)
        {
            try
            {
                var newMessages = await LoadNewMessages(secret, deviceId);
                if (newMessages?.Count > 0)
                {
                    database.InsertBulk("messages", newMessages);

                    var messageId = newMessages.Max((message) => message.Id);

                    var response = await Pushover.Client.MessageDelete(secret, deviceId, messageId);

                    OnPropertyChanged(nameof(Messages));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class App
    {
        public readonly string Name;
        public readonly string Icon;
        public readonly int Date;

        public readonly string LatestMessage;

        public App(string name, string icon, int date, string latestMessage)
        {
            Name = name;
            Icon = icon;
            Date = date;
            LatestMessage = latestMessage;
        }
    }
}
