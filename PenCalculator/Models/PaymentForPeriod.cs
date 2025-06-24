using CV19Core.ViewModels.Base;
using NodaTime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PenCalculator.Infrastructure.Services;

namespace PenCalculator.Models
{
    internal class PaymentForPeriod : Model
    {
        public int ID { get; set; }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                if (_endDate < _startDate)
                {
                    // последний день последнего месяца
                    // кол. дней в последнем месяце
                    int daysInMonthForEnd = DateTime.DaysInMonth(_startDate.Year, _startDate.Month);
                    DateTime endDay = new DateTime(_startDate.Year, _startDate.Month, daysInMonthForEnd);

                    EndDate = endDay;
                    OnPropertyChanged(nameof(EndDate));
                }

                CalcPaySizeOnPeriod();
                OnPropertyChanged(nameof(PaySizeOnPeriod));

            }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                if (_endDate < _startDate)
                {
                    // первый день 1-го месяца
                    DateTime startDay = new DateTime(_endDate.Year, _endDate.Month, 1);

                    StartDate = startDay;
                    OnPropertyChanged(nameof(StartDate));
                }
                CalcPaySizeOnPeriod();
                OnPropertyChanged(nameof(PaySizeOnPeriod));
            }
        }

        #region PaySizeFull : double - Полная назначенная сумма
        ///<summary>Полная назначенная сумма</summary>
        private double _PaySizeFull;

        ///<summary>Полная назначенная сумма</summary>
        public double PaySizeFull
        {
            get => _PaySizeFull;
            set
            {
                Set(ref _PaySizeFull, value);
                CalcPaySizeOnPeriod();
                OnPropertyChanged(nameof(PaySizeOnPeriod));
            }
        }

        #endregion


        #region PaySizeOnPeriod : double - Сумма за период  
        ///<summary>Сумма за период </summary>
        private double _PaySizeOnPeriod;

        ///<summary>Сумма за период </summary>
        public double PaySizeOnPeriod
        {
            get => CalcPaySizeOnPeriod();
            set
            {
                Set(ref _PaySizeOnPeriod, value);
                PaySizeOnPeriodString = $"{_PaySizeOnPeriod:#,0}";
                OnPropertyChanged(nameof(PaySizeOnPeriodString));
            }
        }
        #endregion

        #region PaySizeOnPeriodString : string - Сумма за период  
        ///<summary>Сумма за период </summary>
        private string _PaySizeOnPeriodString;

        ///<summary>Сумма за период </summary>
        public string PaySizeOnPeriodString
        {
            get => _PaySizeOnPeriodString; set => Set(ref _PaySizeOnPeriodString, value);
        }
        #endregion


        #region PaySizeFullString : string - Весь период строкой
        ///<summary>Весь период строкой</summary>
        public string PaySizeFullString => StringFormat.FormatCulture(_PaySizeFull);

        #endregion


        #region DateLength : string - Длинна периода в месяцах
        ///<summary>Длинна периода в месяцах</summary>
        private string _DateLength;
        ///<summary>Длинна периода в месяцах</summary>
        public string DateLength { get => _DateLength; set => Set(ref _DateLength, value); }
        #endregion



        double CalcPaySizeOnPeriod()
        {
            if (_PaySizeOnPeriod > 0)
            {
                return _PaySizeOnPeriod;
            }
            var paySizeOnPeriod = 0.0;
            // первый день 1-го месяца
            DateTime startDay = new DateTime(StartDate.Year, StartDate.Month, 1);
            // кол. дней в первом месяце
            int daysInMonthForStart = DateTime.DaysInMonth(StartDate.Year, StartDate.Month);
            // последний день последнего месяца
            // кол. дней в последнем месяце
            int daysInMonthForEnd = DateTime.DaysInMonth(EndDate.Year, EndDate.Month);
            DateTime endDay = new DateTime(EndDate.Year, EndDate.Month, daysInMonthForEnd);

            LocalDate date1 = new LocalDate(StartDate.Year, StartDate.Month, StartDate.Day);
            LocalDate date2 = new LocalDate(EndDate.Year, EndDate.Month, EndDate.Day);
            var r = (date2 + Period.FromDays(1)) - date1;
            
            var d = r.Days;
            var y = r.Years;

            var m = r.Months+y*12;

            DateLength = m.ToString();

            // Для ячейки первого месяца
            if (StartDate != startDay)
            {
                var dayStart = date1.Day;// дней с начала месяца
                paySizeOnPeriod = ((double)PaySizeFull / daysInMonthForStart) * (daysInMonthForStart - dayStart + 1);
            }
            // Для ячейки последнего месяца
            if (EndDate != endDay)
            {
                paySizeOnPeriod = ((double)PaySizeFull / endDay.Day) * d;
            }


            paySizeOnPeriod += PaySizeFull * m;

            PaySizeOnPeriodString = StringFormat.FormatCulture(paySizeOnPeriod);

            OnPropertyChanged(nameof(PaySizeOnPeriodString));

            return paySizeOnPeriod;
        }


    }
}
