using CV19Core.Infrastructure.Commands;
using CV19Core.ViewModels.Base;
using NodaTime;
using PenCalculator.Infrastructure.Services;
using PenCalculator.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace PenCalculator.ViewModels
{
    [MarkupExtensionReturnType(typeof(MainViewModel))]
    internal class MainViewModel : ViewModel
    {
        private string _PayTotal;
        public string PayTotal { get => _PayTotal; set => Set(ref _PayTotal, value); }

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
        public ObservableCollection<double> PaidOut { get; }


        #region CalculateCommand
        public ICommand CalculateCommand { get; }
        private bool CanCalculateCommandExecute(object p) => true;

        private void OnCalculateCommandExecuted(object p)
        {
            // сумма за период
            double payTotal = 0;

            foreach (var group in PaymentPurposes)
            {
                payTotal += group.PaySizeOnPeriod;
            }

            PayTotal = StringFormat.FormatCulture( Math.Round(payTotal, 2));
        }

        #endregion

        #region AddPeriodCommand
        public ICommand AddPeriodCommand { get; }
        private bool CanAddPeriodCommandExecute(object p) => true;

        private void OnAddPeriodCommandExecuted(object p)
        {
            double paySize = 0;
            var last = PaymentPurposes.LastOrDefault();
            PaymentPurposes.Add(new PaymentForPeriod()
            {
                StartDate = last.StartDate.AddMonths(1),
                EndDate = last.EndDate.AddMonths(4),
                PaySizeFull = paySize,
            });
            OnPropertyChanged(nameof(PaymentPurposes));
        }

        #endregion

        #region RemovePeriodCommand
        public ICommand RemovePeriodCommand { get; }
        private bool CanRemovePeriodCommandExecute(object p) => true;

        private void OnRemovePeriodCommandExecuted(object p)
        {
            var id = PaymentPurposes.IndexOf(SelectedPaymentPurposes);

            if (id == 0)
            {
                return;
            }
            PaymentPurposes.Remove(SelectedPaymentPurposes);
            SelectedPaymentPurposes = id - 1 > -1 ? PaymentPurposes[id - 1] : PaymentPurposes[0];

        }

        #endregion

        #region AddPaidOutCommand
        public ICommand AddPaidOutCommand { get; }
        private bool CanAddPaidOutCommandExecute(object p) => true;

        private void OnAddPaidOutCommandExecuted(object p)
        {
            double paySize = 0;
            var last = PaymentPurposes.LastOrDefault();
            PaymentPurposes.Add(new PaymentForPeriod()
            {
                StartDate = last.StartDate.AddMonths(1),
                EndDate = last.EndDate.AddMonths(4),
                PaySizeFull = paySize,
            });
            OnPropertyChanged(nameof(PaymentPurposes));
        }

        #endregion
        

        public MainViewModel()
        {
            // Создаем дочернюю view-model и даём ей ссылку на главную модель.
            // CountriesStatisticsVM = new CountriesStatisticsViewModel(this);

            PaymentPurposes = new ObservableCollection<PaymentForPeriod>
            {
                new PaymentForPeriod
                {
                    ID = 1,
                    StartDate = DateTime.Parse("01.03.2023"),
                    EndDate = DateTime.Parse("31.03.2023"),
                    PaySizeFull = 10_000
                },

                new PaymentForPeriod
                {
                    ID = 2,
                    StartDate = DateTime.Parse("01.04.2023"),
                    EndDate = DateTime.Parse("31.05.2023"),
                    PaySizeFull = 10_000
                },

                new PaymentForPeriod
                {
                    ID = 3,
                    StartDate = DateTime.Parse("01.06.2023"),
                    EndDate = DateTime.Parse("31.12.2023"),
                    PaySizeFull = 10_000
                },

                //new PaymentForPeriod
                //{
                //    ID = 4,
                //    StartDate = DateTime.Parse("01.01.2024"),
                //    EndDate = DateTime.Parse("31.03.2024"),
                //    PaySizeFull = 10_000
                //},

                //new PaymentForPeriod
                //{
                //    ID = 5,
                //    StartDate = DateTime.Parse("01.04.2024"),
                //    EndDate = DateTime.Parse("31.07.2024"),
                //    PaySizeFull = 10_000
                //},

                //new PaymentForPeriod
                //{
                //    ID = 6,
                //    StartDate = DateTime.Parse("01.08.2024"),
                //    EndDate = DateTime.Parse("31.12.2024"),
                //    PaySizeFull = 10_000
                //},

                //new PaymentForPeriod
                //{
                //    StartDate = DateTime.Parse("01.01.2025"),
                //    EndDate = DateTime.Parse("28.02.2025"),
                //    PaySizeFull = 10_000
                //}

            };
            PaidOut = new ObservableCollection<double>
            {
                500,
                200,
            };

            CalculateCommand =
                new LambdaCommand(OnCalculateCommandExecuted, CanCalculateCommandExecute);
            AddPeriodCommand =
                new LambdaCommand(OnAddPeriodCommandExecuted, CanAddPeriodCommandExecute);
            RemovePeriodCommand =
                new LambdaCommand(OnRemovePeriodCommandExecuted, CanRemovePeriodCommandExecute);
        }
    }
}
