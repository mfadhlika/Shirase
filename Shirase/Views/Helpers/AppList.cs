using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shirase.Views.Helpers
{
    public static class AppList
    {
        public static ImageSource Icon(string iconName)
        {
            if(iconName == null)
            {
                iconName = "pushover";
            }

            return new BitmapImage(new Uri($"https://api.pushover.net/icons/{iconName}.png"));
        }
    }
}
