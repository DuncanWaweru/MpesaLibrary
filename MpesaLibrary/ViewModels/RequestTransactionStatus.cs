using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MpesaLibrary.ViewModels
{
   public class RequestTransactionStatus
    {
        public string Initiator { get; set; }
        public string SecurityCredential { get; set; }
        public string CommandID { get; set; }
        public string TransactionID { get; set; }
        public string OriginalConversationID { get; set; }
        public string PartyA { get; set; }
        public string IdentifierType { get; set; }
        public string ResultURL { get; set; }
        public string QueueTimeOutURL { get; set; }
        public string Remarks { get; set; }
        public string Occasion { get; set; }
    }
}
