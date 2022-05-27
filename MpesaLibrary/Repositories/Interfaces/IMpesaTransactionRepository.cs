using MpesaLibrary.Mpesa;
using MpesaLibrary.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MpesaLibrary.Repositories.Interfaces
{
        public interface IMpesaTransactionRepository
        {
        public TransactionStatus CheckB2CBalance(QueryMpesaBalance queryMpesaBalance);

        public TransactionStatus InitiateB2C(SimulateB2CParameters parameters);
        public TransactionStatus InitiateB2B(SimulateB2CParameters parameters);
        public MpesaResponse ProcessB2CMpesaResponse(JObject jObject, string payrollId);
        public MpesaResponse ProcessB2CMpesaQueryResponse(JObject jObject, string payrollId);
        public string GenerateSecurityCredentials(string password);

        public Task<TransactionStatus> QueryTransactionStatus(GenerateToken generateToken, RequestTransactionStatus parameters);
    }
}
