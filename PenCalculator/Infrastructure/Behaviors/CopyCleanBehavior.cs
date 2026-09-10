using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace PenCalculator.Infrastructure.Behaviors
{
    public static class CopyCleanBehavior
    {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(CopyCleanBehavior),
                new PropertyMetadata(false, OnIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject obj) => (bool)obj.GetValue(IsEnabledProperty);
        public static void SetIsEnabled(DependencyObject obj, bool value) => obj.SetValue(IsEnabledProperty, value);

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((bool)e.NewValue)
                    DataObject.AddCopyingHandler(textBox, OnCopying);
                else
                    DataObject.RemoveCopyingHandler(textBox, OnCopying);
            }
        }

        private static void OnCopying(object sender, DataObjectCopyingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = (string)e.DataObject.GetData(DataFormats.Text);

                // Убираем ВСЕ пробельные символы (обычный пробел, \u00A0, \u202F и любые другие)
                string cleaned = new string(text.Where(c => !char.IsWhiteSpace(c)).ToArray());

                // Отменяем стандартное копирование и пишем в буфер сами
                e.CancelCommand();
                Clipboard.SetText(cleaned);
            }
        }

    }
}
