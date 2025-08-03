using CV19Core.ViewModels.Base;
using PenCalculator.Models;
using System.Collections.Generic;

namespace PenCalculator.ViewModels
{
    /// <summary>
    /// Корневой элемент
    /// </summary>
    internal class DataFile
    {
        public string FileName { get; set; }
        
        public IEnumerable<PaymentForPeriod> PaymentPurposes { get; set; }
        public IEnumerable<PaymentForPeriod> PaidOut { get; set; }
        public double PaidTotal { get; set; }
        public double DifferencePaid { get; set; }
    }
}
