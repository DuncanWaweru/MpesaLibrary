using MpesaLibrary.Mpesa;
using MpesaLibrary.Repositories.Interfaces;
using MpesaLibrary.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MpesaLibrary.Repositories
{
    public class MpesaTransactionRepository :IMpesaTransactionRepository
    {
        public TransactionStatus CheckB2CBalance(QueryMpesaBalance queryMpesaBalance)
        {
            try
            {
                TransactionStatus generateTokenResponse = GenerateSecurityToken(queryMpesaBalance.GenerateToken);
                if (generateTokenResponse.Status)
                {
                    var endpoint = queryMpesaBalance.QueryBalanceUrl;
                    HttpClient httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + generateTokenResponse.Message);


                    queryMpesaBalance.GenerateToken = null;
                    var str = JsonConvert.SerializeObject(queryMpesaBalance);
                    var response = httpClient.PostAsync(endpoint, new StringContent(str, null, "application/json")).Result;
                    var json = response.Content.ReadAsStringAsync().Result;
                    ///TODO: check format of the json response
                    return new TransactionStatus { Message = "Checking Balance", Status = true };
                }
                else
                {
                    return new TransactionStatus { Message = generateTokenResponse.Message, Status = false };
                }

            }
            catch (Exception ex)
            {
                return new TransactionStatus { Message = ex.Message, Status = false };
            }
           
                
        }
        public TransactionStatus B2CBalanceresponse(JToken jToken)
        {
            try
            {
              var b2CBalanceResponse=   jToken.ToObject<B2CBalanceResponse>();
                var resultParameters = b2CBalanceResponse.Result.ResultParameters.ResultParameter;
                if (resultParameters == null)
                {
                    return new TransactionStatus { Message = "Invalid response", Status = false };
                }
                else
                {
                    var actualBalance = resultParameters.Where(x => x.Key == "AccountBalance").FirstOrDefault().Value.ToString().Split('&')[1].Split('|')[2].Replace(" ", "");
                    var lastDate = ConvertStringToDate(resultParameters.Where(x => x.Key == "BOCompletedTime").FirstOrDefault().Value.ToString());
                    BalanceResponseViewModel responseViewModel = new BalanceResponseViewModel
                    {
                        AmountBalance= decimal.TryParse(actualBalance,out decimal amount)==true? amount: 0,
                        LastTransactionDate = lastDate
                    };
                    return new TransactionStatus { Message = "success", Status = true, BalanceViewModel = responseViewModel };
                }
                
            }
            catch (Exception ex)
            {
                return new TransactionStatus { Message = ex.Message, Status = false };
            }
         }

        public TransactionStatus InitiateB2C(SimulateB2CParameters parameters)
        {
          return  SendMoney(parameters);
        }
        private TransactionStatus GenerateSecurityToken(GenerateToken generateToken)
        {
            TransactionStatus tokenResponse = new TransactionStatus();
            try
            {
                byte[] auth = Encoding.UTF8.GetBytes(generateToken.MpesaKey + ":" + generateToken.MpesaSecret);
                String encoded = System.Convert.ToBase64String(auth);
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(generateToken.MpesaTokenEndpoint);
                request.Headers.Add("Authorization", "Basic " + encoded);
                request.ContentType = "application/json";
                request.Headers.Add("cache-control", "no-cache");
                request.Method = "GET";
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    // Get the stream associated with the response.
                    Stream receiveStream = response.GetResponseStream();

                    // Pipes the stream to a higher level stream reader with the required encoding format. 
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    var authtoken = readStream.ReadToEnd();
                    var jArray = JObject.Parse(authtoken);
                    tokenResponse.Message = jArray["access_token"].Value<string>();
                    tokenResponse.Status = true;
                    response.Close();
                    readStream.Close();

                    return tokenResponse;
                }
                catch (WebException ex)
                {
                    var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();

                    tokenResponse.Message = resp;
                    tokenResponse.Status = false;
                    return tokenResponse;
                }

            }
            catch (Exception es)
            {
                tokenResponse.Message = es.Message;
                tokenResponse.Status = false;
                return tokenResponse;
            }
        }
  
        private TransactionStatus SendMoney(SimulateB2CParameters parameters)
        {
            try
            {
                TransactionStatus generateTokenResponse = GenerateSecurityToken(parameters.GenerateToken);
                if (generateTokenResponse.Status)
                {
                    var endpoint = parameters.MpesaB2CEndpoint;
                    HttpClient httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + generateTokenResponse.Message);
                    var simulateB2C = new SimulateB2C()
                    {
                        InitiatorName = parameters.MpesaInitiatorName,
                        SecurityCredential = parameters.SecurityCredential,
                        CommandID = "PromotionPayment",
                        Amount = Convert.ToInt32(parameters.Amount).ToString(),
                        PartyA = parameters.MpesaShortCode,
                        PartyB = parameters.MobileNo.Replace("+", ""),
                        Remarks = parameters.Remarks,
                        QueueTimeOutURL = parameters.QueueTimeOutURL,
                        ResultURL = parameters.B2CResponseUrl,
                        Occasion = null

                    };
                    var str = JsonConvert.SerializeObject(simulateB2C);
                    var response = httpClient.PostAsync(endpoint, new StringContent(str, null, "application/json")).Result;
                    var json = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    ///TODO: check format of the json response
                    
                    return new TransactionStatus { Message = "Money Sent", Status = true,OriginatorConversationID=json["OriginatorConversationID"].ToString() };
                }
                else
                {
                    //an error occurred
                    return new TransactionStatus { Message = generateTokenResponse.Message, Status = false };
                }
            }
            catch (Exception ex)
            {
                return new TransactionStatus { Message = ex.Message, Status = false };
            }
            
           
        }
        public  TransactionStatus InitiateB2B(SimulateB2CParameters parameters)
        {
            try
            {
                TransactionStatus generateTokenResponse = GenerateSecurityToken(parameters.GenerateToken);
                if (generateTokenResponse.Status)
                {
                    var endpoint = parameters.MpesaB2CEndpoint;
                    HttpClient httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + generateTokenResponse.Message);
                    var simulateB2C = new SimulateB2C()
                    {
                        InitiatorName = parameters.MpesaInitiatorName,
                        SecurityCredential = parameters.SecurityCredential,
                        CommandID = "BusinessPayment",
                        Amount = Convert.ToInt32(parameters.Amount).ToString(),
                        PartyA = parameters.MpesaShortCode,
                        PartyB = parameters.MobileNo.Replace("+", ""),
                        Remarks = parameters.Remarks,
                        QueueTimeOutURL = parameters.QueueTimeOutURL,
                        ResultURL = parameters.B2CResponseUrl,
                        Occasion = null

                    };
                    var str = JsonConvert.SerializeObject(simulateB2C);
                    var response = httpClient.PostAsync("https://api.safaricom.co.ke/mpesa/b2b/v1/paymentrequest", new StringContent(str, null, "application/json")).Result;
                    var json = response.Content.ReadAsStringAsync().Result;
                    ///TODO: check format of the json response
                    return new TransactionStatus { Message = "Money Sent", Status = true };
                }
                else
                {
                    //an error occurred
                    return new TransactionStatus { Message = generateTokenResponse.Message, Status = false };
                }
            }
            catch (Exception ex)
            {
                return new TransactionStatus { Message = ex.Message, Status = false };
            }


        }
        // my statatic Methods
        private static DateTime ConvertStringToDate(string transTime)
        {
            try
            {
                int year = Convert.ToInt32(transTime.Substring(0, 4));
                int month = Convert.ToInt32(transTime.Substring(4, 2));
                int day = Convert.ToInt32(transTime.Substring(6, 2));
                int hour = Convert.ToInt32(transTime.Substring(8, 2));
                int minute = Convert.ToInt32(transTime.Substring(10, 2));
                int seconds = Convert.ToInt32(transTime.Substring(12, 2));
                return new DateTime(year, month, day, hour, minute, seconds);
            }
            catch (Exception)
            {
                return DateTime.Now;
            }

        }

        public MpesaResponse ProcessB2CMpesaResponse(JObject jObject,string payrollId)
        {
            
            InitiateB2CResponse result = jObject.ToObject<InitiateB2CResponse>();

            if (result.Result.ResultParameters != null)
            {
                var item = result.Result.ResultParameters.ResultParameter;
                var mpesaResponse = new MpesaResponse();
                mpesaResponse.Amount = Convert.ToDouble(item.Where(x => x.Key == "TransactionAmount").Select(x => x.Value).SingleOrDefault());
                mpesaResponse.ReceiptNo = item.Where(x => x.Key == "TransactionReceipt").Select(x => x.Value).SingleOrDefault().ToString();
                mpesaResponse.Receipient = item.Where(x => x.Key == "ReceiverPartyPublicName").Select(x => x.Value).SingleOrDefault().ToString();
                mpesaResponse.PhoneNumber = "+" + item.Where(x => x.Key == "ReceiverPartyPublicName").Select(x => x.Value).SingleOrDefault().ToString().Substring(0, 12);
                var dt = item.Where(x => x.Key == "TransactionCompletedDateTime").Select(x => x.Value).SingleOrDefault().ToString().Replace(".", "/");
                var datetime = DateTime.ParseExact(dt, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                mpesaResponse.TransactionCompletedDate = Convert.ToDateTime(datetime);
                mpesaResponse.CreatedOn = DateTime.Now;
                mpesaResponse.AccountBalance = Convert.ToDouble(item.Where(x => x.Key == "B2CUtilityAccountAvailableFunds").Select(x => x.Value).SingleOrDefault());
                mpesaResponse.WorkingBalance = Convert.ToDouble(item.Where(x => x.Key == "B2CWorkingAccountAvailableFunds").Select(x => x.Value).SingleOrDefault());
                mpesaResponse.IsMatched = false;
                mpesaResponse.PayRollDataId = payrollId;
                //notify that money has been sent
                return mpesaResponse;
            }
            else
            {
                return new MpesaResponse();
            }
        }

        public MpesaResponse ProcessB2CMpesaQueryResponse(JObject jObject, string payrollId)
        {

            InitiateB2CResponse result = jObject.ToObject<InitiateB2CResponse>();

            if (result.Result.ResultParameters != null)
            {
                var item = result.Result.ResultParameters.ResultParameter;
                var mpesaResponse = new MpesaResponse();
                mpesaResponse.Amount = Convert.ToDouble(item.Where(x => x.Key == "Amount").Select(x => x.Value).SingleOrDefault());
                mpesaResponse.ReceiptNo = item.Where(x => x.Key == "ReceiptNo").Select(x => x.Value).SingleOrDefault().ToString();
                mpesaResponse.Receipient = item.Where(x => x.Key == "CreditPartyName").Select(x => x.Value).SingleOrDefault().ToString();
                mpesaResponse.PhoneNumber = "+" + item.Where(x => x.Key == "CreditPartyName").Select(x => x.Value).SingleOrDefault().ToString().Substring(0, 12);
                var dt = item.Where(x => x.Key == "FinalisedTime").Select(x => x.Value).SingleOrDefault().ToString().Replace(".", "/");
                var datetime = new DateTime(
                    Convert.ToInt32(dt.Substring(0,4)), //year
                    Convert.ToInt32(dt.Substring(4, 2)), //month
                    Convert.ToInt32(dt.Substring(6, 2)), //date
                    Convert.ToInt32(dt.Substring(8, 2)), //hour
                    Convert.ToInt32(dt.Substring(10, 2)), //min
                    Convert.ToInt32(dt.Substring(12, 2)) //sec

                    );//DateTime.ParseExact(dt, "ddMMyyyyHHmmss", CultureInfo.InvariantCulture);
                mpesaResponse.TransactionCompletedDate = datetime;
                mpesaResponse.CreatedOn = DateTime.Now;
                mpesaResponse.AccountBalance = 0;//Convert.ToDouble(item.Where(x => x.Key == "B2CUtilityAccountAvailableFunds").Select(x => x.Value).SingleOrDefault());
                mpesaResponse.WorkingBalance = 0;// Convert.ToDouble(item.Where(x => x.Key == "B2CWorkingAccountAvailableFunds").Select(x => x.Value).SingleOrDefault());
                mpesaResponse.IsMatched = false;
                mpesaResponse.PayRollDataId = payrollId;
                //notify that money has been sent
                return mpesaResponse;
            }
            else
            {
                return new MpesaResponse();
            }
        }

        public string GenerateSecurityCredentials(string password)
        {
            try
            {
                var buildDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var filePath = buildDir + @"\ProductionCertificate.cer";

                byte[] bytes = Encoding.ASCII.GetBytes(password); //Write the unencrypted password into a byte array.

                PemReader pr = new PemReader((StreamReader)File.OpenText(filePath));
                X509Certificate certificate = (X509Certificate)pr.ReadObject();
                //PKCS1 v1.5 padding
                Pkcs1Encoding eng = new Pkcs1Encoding(new RsaEngine());
                eng.Init(true, certificate.GetPublicKey());

                int length = bytes.Length;
                int blockSize = eng.GetInputBlockSize();
                List<byte> cipherTextBytes = new List<byte>();
                for (int chunkPosition = 0; chunkPosition < length; chunkPosition += blockSize)
                {
                    int chunkSize = Math.Min(blockSize, length - chunkPosition);
                    cipherTextBytes.AddRange(eng.ProcessBlock(bytes, chunkPosition, chunkSize));
                }
                return Convert.ToBase64String(cipherTextBytes.ToArray());
            }
            catch (Exception es)
            {

                throw;
            }
        }

        public async Task<TransactionStatus> QueryTransactionStatus(GenerateToken generateToken,RequestTransactionStatus parameters)
        {
            try
            {
                await Task.Delay(3000);
                TransactionStatus generateTokenResponse = GenerateSecurityToken(generateToken);
                if (generateTokenResponse.Status)
                {
                    var endpoint = "https://api.safaricom.co.ke/mpesa/transactionstatus/v1/query";
                    HttpClient httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + generateTokenResponse.Message);
                  
                    var str = JsonConvert.SerializeObject(parameters);
                    var response = httpClient.PostAsync(endpoint, new StringContent(str, null, "application/json")).Result;
                    var json = await response.Content.ReadAsStringAsync();
                    ///TODO: check format of the json response
                    

                    return new TransactionStatus { Message = "Money Sent", Status = true };
                }
                else
                {
                    //an error occurred
                    return new TransactionStatus { Message = generateTokenResponse.Message, Status = false };
                }
            }
            catch (Exception ex)
            {
                return new TransactionStatus { Message = ex.Message, Status = false };
            }
        }

    }
}
