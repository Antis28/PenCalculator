using CV19Core.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PenCalculator.Models
{
    internal class PaymentForPeriod : ViewModel
    {
        public int ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
      
        #region PaySizeFull : double - Полная назначенная сумма
        ///<summary>Полная назначенная сумма</summary>
        private double _PaySizeFull;
        ///<summary>Полная назначенная сумма</summary>
        public double PaySizeFull { get => _PaySizeFull; set => Set(ref _PaySizeFull, value); }
        #endregion

        
        #region PaySizeOnPeriod : double - Сумма за период  
        ///<summary>Сумма за период </summary>
        private double _PaySizeOnPeriod;
        ///<summary>Сумма за период </summary>
        public double PaySizeOnPeriod { get => _PaySizeOnPeriod; set => Set(ref _PaySizeOnPeriod, value); }
        #endregion


    }
}
