using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CV19Core.Infrastructure.Commands.Base;
using Newtonsoft.Json;

namespace PenCalculator.Infrastructure.Services
{
    internal class SaveToJsonFile : Command
    {
        public override bool CanExecute(object parameter) => true;

        public override void Execute(object parameter)
        {
           
        }
    }
}
