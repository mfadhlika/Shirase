using Microsoft.UI.Xaml.Data;
using System;
using System.Globalization;

namespace Shirase.Views.Helpers
{
    public static class Formatter
    {
        public static string ISODateFromTimestamp(int timestamp)
        {
            return DateTimeOffset
                .FromUnixTimeSeconds(timestamp)
                .ToLocalTime()
                .ToString("yyyy-MM-dd HH:mm");
        }
        public static string DateFromTimestamp(int timestamp)
        {
            if (timestamp == 0)
            {
                return "";
            }

            return DateTimeOffset
                .FromUnixTimeSeconds(timestamp)
                .ToLocalTime()
                .ToString("MMM, dd");
        }

        public static string NotificationTitle(string title, string app)
        {
            if (title?.Length > 0)
            {
                return title;
            }

            return app;
        }

        public static string ContentSubtitle(string app, int timestamp)
        {
            var dt = DateTimeOffset
                .FromUnixTimeSeconds(timestamp)
                .ToLocalTime();
            var date = dt.ToString("yyyy-MM-dd");
            var time = dt.ToString("HH:mm");
            return $"From {app} on {date} at {time}";
        }
    }
}