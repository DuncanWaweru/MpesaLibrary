using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MpesaLibrary.ViewModels
{
   public class BalanceResponseViewModel
    {
        public decimal AmountBalance { get; set; }
        public DateTime LastTransactionDate { get; set; }
    }
}
