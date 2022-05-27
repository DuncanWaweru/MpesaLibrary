using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MpesaLibrary.pesa
{
    public class SendRegisterUrl
    {
        public string ShortCode { get; set; }
        public string ResponseType { get; set; }
        public string ConfirmationURL { get; set; }
        public string ValidationURL { get; set; }
    }
}