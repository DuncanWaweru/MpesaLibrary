using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MpesaLibrary.ViewModels
{
    public class ResultParameter
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }

    public class ResultParameters
    {
        public List<ResultParameter> ResultParameter { get; set; }
    }

    public class ReferenceItem
    {
        public string Key { get; set; } = "";
        public string Value { get; set; } = "";
    }

    public class ReferenceData
    {
        public ReferenceItem ReferenceItem { get; set; }
    }

    public class Result
    {
        public int ResultType { get; set; }
        public int ResultCode { get; set; }
        public string ResultDesc { get; set; }
        public string OriginatorConversationID { get; set; }
        public string ConversationID { get; set; }
        public string TransactionID { get; set; }
        public ResultParameters ResultParameters { get; set; }
        public ReferenceData ReferenceData { get; set; }
    }

    public class InitiateB2CResponse
    {
        public Result Result { get; set; }
    }
}
