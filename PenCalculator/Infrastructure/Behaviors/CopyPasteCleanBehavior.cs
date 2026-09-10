using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PenCalculator.Infrastructure.Behaviors
{
    public static class CopyPasteCleanBehavior
    {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(CopyPasteCleanBehavior),
                new PropertyMetadata(false, OnIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject obj) => (bool)obj.GetValue(IsEnabledProperty);
        public static void SetIsEnabled(DependencyObject obj, bool value) => obj.SetValue(IsEnabledProperty, value);

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((bool)e.NewValue)
                {
                    DataObject.AddCopyingHandler(textBox, OnCopying);
                    DataObject.AddPastingHandler(textBox, OnPasting);
                }
                else
                {
                    DataObject.RemoveCopyingHandler(textBox, OnCopying);
                    DataObject.RemovePastingHandler(textBox, OnPasting);
                }
            }
        }

        // Очистка при копировании в буфер
        private static void OnCopying(object sender, DataObjectCopyingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = (string)e.DataObject.GetData(DataFormats.Text);
                string cleaned = new string(text.Where(c => !char.IsWhiteSpace(c)).ToArray());

                e.CancelCommand();
                Clipboard.SetText(cleaned);
            }
        }

        // Очистка при вставке из буфера в поле
        private static void OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(typeof(string)))
                return;

            string text = (string)e.DataObject.GetData(typeof(string));
            // Удаляем все пробельные символы
            string cleaned = new string(text.Where(c => !char.IsWhiteSpace(c)).ToArray());

            // Если после очистки пусто — не вставляем
            if (string.IsNullOrEmpty(cleaned))
            {
                e.CancelCommand();
                return;
            }

            // Отменяем стандартную вставку
            e.CancelCommand();

            // Вставляем очищенный текст вручную
            if (sender is TextBox tb)
            {
                // Сохраняем позицию курсора относительно конца вставки, если нужно
                int start = tb.SelectionStart;
                int length = tb.SelectionLength;

                tb.Text = tb.Text.Remove(start, length).Insert(start, cleaned);
                tb.SelectionStart = start + cleaned.Length;
                tb.Focus();
            }
        }
    }
}
