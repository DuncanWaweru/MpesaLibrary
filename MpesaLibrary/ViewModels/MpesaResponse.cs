using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MpesaLibrary.Mpesa
{
    /// <summary>
    /// create this class in your models 
    /// </summary>
    public class MpesaResponse
    {
        
        public double Amount  { get; set; }
        public string ReceiptNo { get; set; }
        public string Receipient { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime TransactionCompletedDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public double AccountBalance { get; set; }
        public double WorkingBalance { get; set; }
        public bool IsMatched { get; set; }
        public string  PayRollDataId { get; set; }

    }
}