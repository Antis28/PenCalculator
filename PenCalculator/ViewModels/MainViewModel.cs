using CV19Core.Infrastructure.Commands;
using CV19Core.ViewModels.Base;
using NodaTime;
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
        private double _PayTotal;
        public double PayTotal { get => _PayTotal; set => Set(ref _PayTotal, value); }

        public ObservableCollection<PaymentForPeriod> Groups { get; }
        #region SelectedGroup : Group - Выбранная группа
        ///<summary>Выбранная группа</summary>
        private PaymentForPeriod _SelectedGroup;
        ///<summary>Выбранная группа</summary>
        public PaymentForPeriod SelectedGroup { get => _SelectedGroup; set => Set(ref _SelectedGroup, value); }



        #endregion


        #region CalculateCommand
        public ICommand CalculateCommand { get; }
        private bool CanCalculateCommandExecute(object p) => true;

        private void OnCalculateCommandExecuted(object p)
        {
            // сумма за период
            double payTotal = 0;

            foreach (var group in Groups)
            {
                payTotal += group.PaySizeOnPeriod;
            }

            PayTotal = Math.Round(payTotal, 2);
        }

        #endregion

        #region AddPeriodCommand
        public ICommand AddPeriodCommand { get; }
        private bool CanAddPeriodCommandExecute(object p) => true;

        private void OnAddPeriodCommandExecuted(object p)
        {
            double paySize = 0;
            var last = Groups.LastOrDefault();
            Groups.Add(new PaymentForPeriod()
            {
                StartDate = last.StartDate.AddMonths(1),
                EndDate = last.EndDate.AddMonths(4),
            });

        }

        #endregion

        #region RemovePeriodCommand
        public ICommand RemovePeriodCommand { get; }
        private bool CanRemovePeriodCommandExecute(object p) => true;

        private void OnRemovePeriodCommandExecuted(object p)
        {
            var id = Groups.IndexOf(SelectedGroup);

            if (id == 0)
            {
                return;
            }
            Groups.Remove(SelectedGroup);
            SelectedGroup = id - 1 > -1 ? Groups[id - 1] : Groups[0];

        }

        #endregion

        public MainViewModel()
        {
            // Создаем дочернюю view-model и даём ей ссылку на главную модель.
            // CountriesStatisticsVM = new CountriesStatisticsViewModel(this);

            Groups = new ObservableCollection<PaymentForPeriod>
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

            CalculateCommand =
                new LambdaCommand(OnCalculateCommandExecuted, CanCalculateCommandExecute);
            AddPeriodCommand =
                new LambdaCommand(OnAddPeriodCommandExecuted, CanAddPeriodCommandExecute);
            RemovePeriodCommand =
                new LambdaCommand(OnRemovePeriodCommandExecuted, CanRemovePeriodCommandExecute);
        }
    }
}
