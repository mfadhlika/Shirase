using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Shirase.Views.Helpers
{
    public class MessageContent: DependencyObject
    {
        public static readonly DependencyProperty HtmlTextProperty =
        DependencyProperty.RegisterAttached(
            "HtmlText",
            typeof(string),
            typeof(MessageContent),
            new PropertyMetadata(null, OnHtmlTextChanged));

        public static string GetHtmlText(DependencyObject obj) =>
            (string)obj.GetValue(HtmlTextProperty);

        public static void SetHtmlText(DependencyObject obj, string value) =>
            obj.SetValue(HtmlTextProperty, value);

        private static void OnHtmlTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not RichTextBlock richTextBlock) return;

            richTextBlock.Blocks.Clear();

            if (e.NewValue is string rawText && !string.IsNullOrEmpty(rawText))
            {
                // Parse text/HTML into Paragraph blocks.
                // Replace this simple parsing with an HTML/Markdown parser as needed.
                var paragraph = new Paragraph();
                var run = new Run { Text = rawText };
                paragraph.Inlines.Add(run);

                richTextBlock.Blocks.Add(paragraph);

                Debug.WriteLine(e.NewValue);
            }
        }
    }
}
