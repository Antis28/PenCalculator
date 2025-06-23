using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PenCalculator.Models
{
    internal class PaidOutM : Model
    {

        #region Payment : double - Выплачено
        ///<summary>Выплачено</summary>
        private double _Payment;
        ///<summary>Выплачено</summary>
        public double Payment { get => _Payment; set => Set(ref _Payment, value); }
        #endregion

        public PaidOutM(double val)
        {
            Payment = val;
        }
    }
}
