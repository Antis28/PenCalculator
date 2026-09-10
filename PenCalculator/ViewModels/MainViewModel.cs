using CV19Core.Infrastructure.Commands;
using CV19Core.ViewModels.Base;
using PenCalculator.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Markup;
using Newtonsoft.Json;
using PenCalculator.Infrastructure.Services;
using System.Runtime.Remoting.Contexts;

namespace PenCalculator.ViewModels
{
    [MarkupExtensionReturnType(typeof(MainViewModel))]
    internal class MainViewModel : ViewModel
    {
        private double _PayTotalVal;
        private double _PayTotal;
        private string _FileName;
        private string _TimeStep;

        public string FileName { get => _FileName; set => Set(ref _FileName, value); }

        public double PayTotal { get => _PayTotal; set => Set(ref _PayTotal, value); }
        public string TimeStep { get => _TimeStep; set => Set(ref _TimeStep, value); }


        #region PaidTotal : double - Выплачено всего
        ///<summary>Выплачено всего</summary>
        private double _PaidTotal;
        ///<summary>Выплачено всего</summary>
        public double PaidTotal { get => _PaidTotal; set => Set(ref _PaidTotal, value); }
        #endregion


        #region DifferencePaid : double - Разница в выплате
        ///<summary>Разница в выплате</summary>
        private double _DifferencePaid;
        ///<summary>Разница в выплате</summary>
        public double DifferencePaid { get => _DifferencePaid; set => Set(ref _DifferencePaid, value); }
        #endregion



        /// <summary>
        /// Положено к выплате
        /// </summary>
        public ObservableCollection<PaymentForPeriod> PaymentPurposes { get; }

        #region SelectedPaymentPurposes : Group - Выбранная группа
        ///<summary>Выбранная группа</summary>
        private PaymentForPeriod _SelectedPaymentPurposes;
        ///<summary>Выбранная группа</summary>
        public PaymentForPeriod SelectedPaymentPurposes { get => _SelectedPaymentPurposes; set => Set(ref _SelectedPaymentPurposes, value); }

        #endregion

        /// <summary>
        /// Выплачено
        /// </summary>
        public ObservableCollection<PaymentForPeriod> PaidOut { get; private set; }

        #region SelectedPaidOut : Выплачено текущая строка
        ///<summary>Выбранная группа</summary>
        private PaymentForPeriod _SelectedPaidOut;
        ///<summary>Выбранная группа</summary>
        public PaymentForPeriod SelectedPaidOut { get => _SelectedPaidOut; set => Set(ref _SelectedPaidOut, value); }

        #endregion


        #region CalculatePaymentCommand
        public ICommand CalculatePaymentCommand { get; }
        private bool CanCalculatePaymentCommandExecute(object p) => true;

        private void OnCalculatePaymentCommandExecuted(object p)
        {
            CalcPaymentTotal();
        }

        private double CalcPaymentTotal()
        {
            // сумма за период
            double payTotal = 0;

            foreach (var group in PaymentPurposes)
            {
                payTotal += group.PaySizeOnPeriod;
            }

            _PayTotalVal = payTotal;
            PayTotal = Math.Round(payTotal, 2);
            return PayTotal;
        }

        #endregion

        #region CalculatePaidCommand
        public ICommand CalculatePaidCommand { get; }
        private bool CanCalculatePaidCommandExecute(object p) => true;

        private void OnCalculatePaidCommandExecuted(object p)
        {
            CalcDifferentSumm();
        }

        private void CalcDifferentSumm()
        {
            // Положеная сумма за период
            CalcPaymentTotal();
            // Выплаченая сумма за период
            CalcPaidTotal();
            DifferencePaid = Math.Round(_PayTotalVal - _PaidTotal, 2);
        }

        private double CalcPaidTotal()
        {
            double paidTotal = 0;

            foreach (var group in PaidOut)
            {
                paidTotal += group.PaySizeOnPeriod;
            }

            PaidTotal = Math.Round(paidTotal, 2);
            return paidTotal;
        }

        #endregion


        #region AddPeriodCommand
        public ICommand AddPeriodCommand { get; }
        private bool CanAddPeriodCommandExecute(object p) => true;

        private void OnAddPeriodCommandExecuted(object p)
        {
            double paySize = 0;
            var last = PaymentPurposes.LastOrDefault();

            var newStartDate = NewStartDate(last);

            // кол. дней в последнем месяце
            var newEndDate = NewEndDate(newStartDate);


            PaymentPurposes.Add(new PaymentForPeriod()
            {
                StartDate = newStartDate,
                EndDate = newEndDate,
                PaySizeFull = paySize,
            });
            OnPropertyChanged(nameof(PaymentPurposes));
        }

        private static DateTime NewEndDate(DateTime newStartDate)
        {
            var newEndDate = newStartDate;
            int daysInMonthForEnd = DateTime.DaysInMonth(newEndDate.Year, newEndDate.Month);
            newEndDate = new DateTime(newEndDate.Year, newEndDate.Month, daysInMonthForEnd);
            return newEndDate;
        }

        private static DateTime NewStartDate(PaymentForPeriod last)
        {
            if (last == null)
                return DateTime.Today;

            return last.EndDate.AddDays(1);
        }

        #endregion
        #region RemovePeriodCommand
        public ICommand RemovePeriodCommand { get; }
        private bool CanRemovePeriodCommandExecute(object p) => PaymentPurposes.Count > 1;

        private void OnRemovePeriodCommandExecuted(object p)
        {
            if (SelectedPaymentPurposes == null)
                return;
            var len = PaymentPurposes.Count;
            if (len == 1)
            {
                return;
            }

            var id = PaymentPurposes.IndexOf(SelectedPaymentPurposes);
            PaymentPurposes.Remove(SelectedPaymentPurposes);
            SelectedPaymentPurposes = id - 1 > -1 ? PaymentPurposes[id - 1] : PaymentPurposes[0];
            OnPropertyChanged(nameof(PaymentPurposes));
        }

        #endregion
        #region MovePeriodUpCommand
        public ICommand MovePeriodUpCommand { get; }
        private bool CanMovePeriodUpCommandExecute(object p) => true;

        private void OnMovePeriodUpCommandExecuted(object p)
        {
            var id = PaymentPurposes.IndexOf(SelectedPaymentPurposes);
            if (id <= 0)
            {
                return;
            }

            PaymentPurposes.Move(id, id - 1);
            SelectedPaymentPurposes = PaymentPurposes[id - 1];

            OnPropertyChanged(nameof(PaymentPurposes));
        }

        #endregion
        #region MovePeriodDownCommand
        public ICommand MovePeriodDownCommand { get; }
        private bool CanMovePeriodDownCommandExecute(object p) => true;

        private void OnMovePeriodDownCommandExecuted(object p)
        {
            var id = PaymentPurposes.IndexOf(SelectedPaymentPurposes);
            if (id < 0 || id + 1 >= PaymentPurposes.Count)
            {
                return;
            }

            PaymentPurposes.Move(id, id + 1);
            SelectedPaymentPurposes = PaymentPurposes[id + 1];


            //PaymentPurposes




            //PaymentPurposes.Add(new PaymentForPeriod()
            //{
            //    StartDate = newStartDate,
            //    EndDate = newEndDate,
            //    PaySizeFull = paySize,
            //});
            OnPropertyChanged(nameof(PaymentPurposes));
        }

        #endregion

        #region AddPaidOutCommand
        public ICommand AddPaidOutCommand { get; }
        private bool CanAddPaidOutCommandExecute(object p) => true;

        private void OnAddPaidOutCommandExecuted(object p)
        {
            AddPaidOut();

            OnPropertyChanged(nameof(PaidOut));
        }

        private void AddPaidOut()
        {
            double paySize = 0;
            var last = PaidOut.LastOrDefault();

            var newStartDate = NewStartDate(last);

            // кол. дней в последнем месяце
            var newEndDate = NewEndDate(newStartDate);

            PaidOut.Add(new PaymentForPeriod()
            {
                StartDate = newStartDate,
                EndDate = newEndDate,
                PaySizeFull = paySize,
            });
        }

        #endregion
        #region RemovePaidCommand
        public ICommand RemovePaidCommand { get; }
        private bool CanRemovePaidCommandExecute(object p) => PaidOut.Count > 1;

        private void OnRemovePaidCommandExecuted(object p)
        {
            if (SelectedPaidOut == null)
                return;
            var len = PaidOut.Count;
            if (len == 0)
            {
                return;
            }

            var id = PaidOut.IndexOf(SelectedPaidOut);
            PaidOut.Remove(SelectedPaidOut);
            SelectedPaidOut = id - 1 > -1 ? PaidOut[id - 1] : PaidOut[0];
            OnPropertyChanged(nameof(PaidOut));
        }

        #endregion

        #region ClearPeriodCommand
        public ICommand ClearPeriodCommand { get; }
        private bool CanClearPeriodCommandExecute(object p) => true;

        private void OnClearPeriodCommandExecuted(object p)
        {
            var len = PaymentPurposes.Count;
            if (len == 1)
            {
                return;
            }

            for (int i = PaymentPurposes.Count - 1; i > 0; i--)
            {
                PaymentPurposes.Remove(PaymentPurposes[i]);
            }

            SelectedPaymentPurposes = PaymentPurposes[0];

            OnPropertyChanged(nameof(PaymentPurposes));
        }

        #endregion
        #region ClearPeriodPaidCommand
        public ICommand ClearPeriodPaidCommand { get; }
        private bool CanClearPeriodPaidCommandExecute(object p) => true;

        private void OnClearPeriodPaidCommandExecuted(object p)
        {
            var len = PaidOut.Count;
            if (len <= 1)
                return;

            for (int i = len - 1; i > 0; i--)
            {
                PaidOut.Remove(PaidOut[i]);
            }

            SelectedPaidOut = PaidOut[0];
        }

        #endregion


        #region SaveToFileCommand
        public ICommand SaveToFileCommand { get; }
        private bool CanSaveToFileCommandExecute(object p) => true;

        private void OnSaveToFileCommandExecuted(object p)
        {
            // this.FileName = this.FileName.Replace(":", "");

            var df = new DataFile()
            {
                FileName = this.FileName,
                PaidOut = this.PaidOut,
                DifferencePaid = this.DifferencePaid,
                PaidTotal = this.PaidTotal,
                PaymentPurposes = this.PaymentPurposes
            };
            this.FileName = JsonService.Save(df);
            var now = DateTime.Now;
            TimeStep = now.ToString("HH:mm:ss");
        }

        #endregion




        public MainViewModel()
        {
            // Создаем дочернюю view-model и даём ей ссылку на главную модель.
            // CountriesStatisticsVM = new CountriesStatisticsViewModel(this);

            // Последний день текущего месяца
            DateTime now = DateTime.Now;
            DateTime lastDayLastMonth = new DateTime(now.Year, now.Month,
                                             DateTime.DaysInMonth(now.Year, now.Month));

            PaymentPurposes = new ObservableCollection<PaymentForPeriod>
            {
                new PaymentForPeriod
                {
                    ID = 1,
                    StartDate = DateTime.Parse("01.03.2023"),
                    EndDate = DateTime.Parse("31.12.2023"),
                    PaySizeFull = 10_000
                },

                new PaymentForPeriod
                {
                    ID = 2,
                    StartDate = DateTime.Parse("01.01.2024"),
                    EndDate = DateTime.Parse("31.12.2024"),
                    PaySizeFull = 10_000
                },

                new PaymentForPeriod
                {
                    ID = 3,
                    StartDate = DateTime.Parse("01.01.2025"),
                    EndDate = DateTime.Parse("31.12.2025"),
                    PaySizeFull = 10_000
                },

                new PaymentForPeriod
                {
                    ID = 4,
                    StartDate = DateTime.Parse("01.01.2026"),
                    EndDate = lastDayLastMonth,
                    PaySizeFull = 10_000
                },

                //new PaymentForPeriod
                //{
                //    ID = 5,
                //    StartDate = DateTime.Parse("01.01.2024"),
                //    EndDate = DateTime.Parse("31.03.2024"),
                //    PaySizeFull = 10_000
                //},

                //new PaymentForPeriod
                //{
                //    ID = 6,
                //    StartDate = DateTime.Parse("01.04.2024"),
                //    EndDate = DateTime.Parse("31.07.2024"),
                //    PaySizeFull = 10_000
                //},

                //new PaymentForPeriod
                //{
                //    ID = 7,
                //    StartDate = DateTime.Parse("01.08.2024"),
                //    EndDate = DateTime.Parse("31.12.2024"),
                //    PaySizeFull = 10_000
                //},

                //new PaymentForPeriod
                //{
                //    ID = 8,
                //    StartDate = DateTime.Parse("01.01.2025"),
                //    EndDate = DateTime.Parse("28.02.2025"),
                //    PaySizeFull = 10_000
                //}

            };
            PaidOut = new ObservableCollection<PaymentForPeriod>
            {
                new PaymentForPeriod(){IsAllVisible = false},
            };
            AddPaidOut();

            CalculatePaymentCommand =
                new LambdaCommand(OnCalculatePaymentCommandExecuted, CanCalculatePaymentCommandExecute);

            AddPeriodCommand =
                new LambdaCommand(OnAddPeriodCommandExecuted, CanAddPeriodCommandExecute);
            RemovePeriodCommand =
                new LambdaCommand(OnRemovePeriodCommandExecuted, CanRemovePeriodCommandExecute);
            MovePeriodDownCommand =
                new LambdaCommand(OnMovePeriodDownCommandExecuted, CanMovePeriodDownCommandExecute);
            MovePeriodUpCommand =
                new LambdaCommand(OnMovePeriodUpCommandExecuted, CanMovePeriodUpCommandExecute);


            AddPaidOutCommand =
                new LambdaCommand(OnAddPaidOutCommandExecuted, CanAddPaidOutCommandExecute);
            RemovePaidCommand =
                new LambdaCommand(OnRemovePaidCommandExecuted, CanRemovePaidCommandExecute);
            CalculatePaidCommand =
                new LambdaCommand(OnCalculatePaidCommandExecuted, CanCalculatePaidCommandExecute);
            SaveToFileCommand =
                new LambdaCommand(OnSaveToFileCommandExecuted, CanSaveToFileCommandExecute);
            ClearPeriodCommand =
                new LambdaCommand(OnClearPeriodCommandExecuted, CanClearPeriodCommandExecute);
            ClearPeriodPaidCommand =
                new LambdaCommand(OnClearPeriodPaidCommandExecuted, CanClearPeriodPaidCommandExecute);

            FileName =
                $"{DateTime.Now.Year}.{DateTime.Now.Month}.{DateTime.Now.Day}_{DateTime.Now.Hour}.{DateTime.Now.Minute}.{DateTime.Now.Second}";
            if (App.StartUpArg != null)
            {
                LoadFromFile(App.StartUpArg);
                App.StartUpArg = null;
            }
        }

        public void LoadFromFile(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
                throw new ArgumentNullException(nameof(file));

            string jsonText;
            try
            {
                jsonText = File.ReadAllText(file);
            }
            catch (FileNotFoundException)
            {
                // Файл не найден — можно показать диалог, залогировать и т.д.
                System.Windows.MessageBox.Show(
                    $"Файл не найден:\n{file}",
                    "Ошибка загрузки",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
                return;
            }
            catch (IOException ex)
            {
                System.Windows.MessageBox.Show(
                    $"Не удалось прочитать файл:\n{ex.Message}",
                    "Ошибка загрузки",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                return;
            }

            DataFile dataFile;
            try
            {
                dataFile = JsonConvert.DeserializeObject<DataFile>(jsonText);
            }
            catch (JsonException ex)
            {
                System.Windows.MessageBox.Show(
                    $"Файл повреждён или имеет неверный формат:\n{ex.Message}",
                    "Ошибка загрузки",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                return;
            }

            // Проверка целостности данных
            if (dataFile == null ||
                dataFile.PaidOut == null ||
                dataFile.PaymentPurposes == null)
            {
                System.Windows.MessageBox.Show(
                    "Файл не содержит необходимых данных.",
                    "Ошибка загрузки",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
                return;
            }

            PaidOut.Clear();
            foreach (var item in dataFile.PaidOut)
                PaidOut.Add(item);

            PaymentPurposes.Clear();
            foreach (var item in dataFile.PaymentPurposes)
                PaymentPurposes.Add(item);

            FileName = dataFile.FileName;

            CalcDifferentSumm();
        }
    }
}
