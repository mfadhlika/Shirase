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
using System.Threading.Tasks;

namespace Shirase.ViewModels
{
    public partial class MessagePageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Message> AppLatestMessages { get; set; } = new([new() {
            App = "Latest Messages"
        }]);

        public ObservableCollection<Message> Messages
        {
            get
            {
                var query = database.Query<Message>("messages");
                if(SelectedApp != "Latest Messages")
                {
                    query = query.Where(x => x.App == SelectedApp);
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

        private string _selectedApp = "Latest Messages";

        public string SelectedApp
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

                var allMessages = LoadSavedMessages();
                Messages.Clear();
                foreach (var item in allMessages)
                {
                    if (_selectedApp != "Latest Messages" && item.App != _selectedApp)
                    {
                        continue;
                    }

                    Messages.Add(item);
                }
            }
        }

        private readonly LocalDatabase database = new();

        public MessagePageViewModel() {
        }

        private static async Task<List<Message>> LoadNewMessages(string secret, string deviceId)
        {
            var response = await Pushover.Client.MessageDownload(secret, deviceId);

            return response.Messages;
        }

        private List<Message> LoadSavedMessages()
        {
            return [.. database.GetAll<Message>("messages").OrderByDescending(m => m.Date)];
        }

        public async void LoadMessages(string secret, string deviceId)
        {
            try
            {
                var newMessages = await LoadNewMessages(secret, deviceId);
                var oldMessages = LoadSavedMessages();

                if (newMessages?.Count > 0) {
                    database.InsertBulk("messages", newMessages);
                    
                    var messageId = newMessages.Max((message) => message.Id);

                    var response = await Pushover.Client.MessageDelete(secret, deviceId, messageId);

                    Debug.WriteLine(response.ToString());
                }

                var allMessages = LoadSavedMessages();
                foreach (var item in allMessages.GroupBy(m => m.App))
                {
                    AppLatestMessages.Add(item.First());
                }

                foreach (var item in allMessages)
                {
                    if(_selectedApp != "Latest Messages" && item.App != _selectedApp)
                    {
                        continue;
                    }

                    Messages.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
