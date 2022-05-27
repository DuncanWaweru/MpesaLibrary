using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MpesaLibrary.ViewModels
{
   public  class TransactionStatus
    {
        public string OriginatorConversationID { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
        public BalanceResponseViewModel BalanceViewModel { get; set; } = new BalanceResponseViewModel();
    }
}
