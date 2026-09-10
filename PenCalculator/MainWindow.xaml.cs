using PenCalculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PenCalculator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void SetDropCommand(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) { return; }

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var mwContext = (DataContext) as MainViewModel;

            mwContext.LoadFromFile(files[0]);
            // var allFilesViewModel = fileListBox.DataContext as MainViewModel;

            // Загрузка Drag&Drop
            //  allFilesViewModel?.LoadFilesInBase(files);
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var tb = sender as TextBox;
                if (tb != null)
                {
                    // Принудительно обновляем источник (запускает ConvertBack у конвертера)
                    var be = BindingOperations.GetBindingExpression(tb, TextBox.TextProperty);
                    be?.UpdateSource();

                    // Опционально: можно сразу перевести фокус на следующее поле
                    //var req = new TraversalRequest(FocusNavigationDirection.Next);
                    //tb.MoveFocus(req);

                    e.Handled = true; // чтобы Enter не делал перенос строки
                }
            }
        }
    }
}
