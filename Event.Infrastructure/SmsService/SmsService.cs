using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Event.Application.SmsService;
using Event.Application.SmsService.Entities;

namespace Event.Infrastructure.SmsService
{
    public class SmsService : ISmsService
    {
        private readonly BulkGateSMS _bulkGateSMS;
        private readonly HollaTagsSMS _hollaTagsSMS;
        public SmsService(IOptions<BulkGateSMS> bulkGateSMS, IOptions<HollaTagsSMS> hollaTagsSMS)
        {
            _bulkGateSMS = bulkGateSMS.Value;
            _hollaTagsSMS = hollaTagsSMS.Value;
        }

        /// <summary>
        /// Bulkgate SMS
        /// </summary>
        /// <param name="smsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> SendOtpToNumber(BulkgateDTO smsDTO)
        {
            smsDTO.application_id = _bulkGateSMS.application_id;
            smsDTO.application_token = _bulkGateSMS.application_token;
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_bulkGateSMS.Url),
                Headers =
                {
                    { "Host", _bulkGateSMS.Host },
                    {"Cache-Control","no-cache" },
                },
            };
            try
            {
                using (var response = await client.PostAsJsonAsync<BulkgateDTO>(request.RequestUri, smsDTO))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return "OTP request was not successful";
                    }
                }
            }
            catch (System.AggregateException ag)
            {
                throw new Exception(ag.Message);
            }
            catch (HttpRequestException ht)
            {
                throw new Exception(ht.Message);
            }
        }
        /// <summary>
        /// HollaTags SMS
        /// </summary>
        /// <param name="smsDTo"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> SendSmsHollaTags(HollaTagsDTO smsDTo)
        {
            smsDTo.user = _hollaTagsSMS.Username;
            smsDTo.pass = _hollaTagsSMS.Pass;
            smsDTo.callback_url = _hollaTagsSMS.CallbackUrl;
            smsDTo.from = _hollaTagsSMS.From;

            //Create a list of your parameters
            var postParams = new List<KeyValuePair<string, string>>(){
                new KeyValuePair<string, string>("user", smsDTo.user) ,
                new KeyValuePair<string, string>("pass", smsDTo.pass) ,
                new KeyValuePair<string, string>("from", smsDTo.from) ,
                new KeyValuePair<string, string>("to", smsDTo.to) ,
                new KeyValuePair<string, string>("msg", smsDTo.msg) ,
                new KeyValuePair<string, string>("type", "0") ,
                new KeyValuePair<string, string>("callback_url", smsDTo.callback_url) ,
                new KeyValuePair<string, string>("enable_msg_id", "TRUE")
            };

            IEnumerable<KeyValuePair<string, string>> ee = postParams;
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_hollaTagsSMS.Url),
                Headers =
                {
                    { "Cookie", _hollaTagsSMS.Cookie },
                },
                Content = new FormUrlEncodedContent(ee)
            };
            //      client.DefaultRequestHeaders
            //.Accept
            //.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            try
            {
                using (var response = await client.PostAsync(request.RequestUri, request.Content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return "OTP request was not successful";
                    }
                }
            }
            catch (System.AggregateException ag)
            {
                throw new Exception(ag.Message);
            }
            catch (HttpRequestException ht)
            {
                throw new Exception(ht.Message);
            }
        }
    }
}
