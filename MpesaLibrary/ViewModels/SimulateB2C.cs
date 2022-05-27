using MpesaLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MpesaLibrary.Mpesa
{
    public class SimulateB2C
    {
      
            public string InitiatorName { get; set; }
            public string SecurityCredential { get; set; }
            public string CommandID { get; set; }
            public string Amount { get; set; }
            public string PartyA { get; set; }
            public string PartyB { get; set; }
            public string Remarks { get; set; }
            public string QueueTimeOutURL { get; set; }
            public string ResultURL { get; set; }
            public string Occasion { get; set; }
        
    }
    public class SimulateB2CParameters
    {
        public string MobileNo { get; set; }
        public string SecurityCredential { get; set; }
        public string MpesaB2CEndpoint { get; set; }
        public string MpesaInitiatorName { get; set; }
        public decimal Amount { get; set; }
        public string MpesaShortCode { get; set; }
        public string QueueTimeOutURL { get; set; }
        public string B2CResponseUrl { get; set; }
        public string Remarks { get; set; }
        public GenerateToken GenerateToken { get; set; }

    }

}